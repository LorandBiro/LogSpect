namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using LogSpect;
    using LogSpect.Logging;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;

    internal class MethodRewriter
    {
        private readonly ModuleDefinition module;

        private readonly TypeReference objectArray;
        private readonly TypeReference runtimeMethodHandle;
        private readonly TypeReference methodBase;
        private readonly TypeReference exception;
        private readonly MethodReference compilerGeneratedAttributeConstructor;

        private readonly TypeReference methodLogger;
        private readonly MethodReference methodLoggerLogLeave;
        private readonly MethodReference methodLoggerLogException;
        private readonly TypeReference methodLoggerFactory;
        private readonly TypeReference methodLoggerFactoryLocator;

        public MethodRewriter(ModuleDefinition module)
        {
            this.module = module;

            this.objectArray = new ArrayType(this.module.TypeSystem.Object);
            this.runtimeMethodHandle = this.module.Import(typeof(RuntimeMethodHandle));
            this.methodBase = this.module.Import(typeof(MethodBase));
            this.exception = this.module.Import(typeof(Exception));

            TypeReference compilerGeneratedAttributeTypeReference = this.module.Import(typeof(CompilerGeneratedAttribute));
            this.compilerGeneratedAttributeConstructor = new MethodReference(".ctor", this.module.TypeSystem.Void, compilerGeneratedAttributeTypeReference) { HasThis = true };

            this.methodLogger = this.module.Import(typeof(IMethodLogger));

            this.methodLoggerLogLeave = new MethodReference("LogLeave", this.module.TypeSystem.Void, this.methodLogger) { HasThis = true };
            this.methodLoggerLogLeave.Parameters.Add(new ParameterDefinition(new ArrayType(this.module.TypeSystem.Object)));
            this.methodLoggerLogLeave.Parameters.Add(new ParameterDefinition(this.module.TypeSystem.Object));

            this.methodLoggerLogException = new MethodReference("LogException", this.module.TypeSystem.Void, this.methodLogger) { HasThis = true };
            this.methodLoggerLogException.Parameters.Add(new ParameterDefinition(this.exception));

            this.methodLoggerFactory = this.module.Import(typeof(IMethodLoggerFactory));
            this.methodLoggerFactoryLocator = this.module.Import(typeof(MethodLoggerFactory));
        }

        public void Rewrite(MethodDefinition method)
        {
            FieldDefinition methodLoggerField = new FieldDefinition(
                string.Format("<>{0}_{1:x8}", method.Name, method.FullName.GetHashCode()),
                Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.Static,
                this.methodLogger);
            methodLoggerField.CustomAttributes.Add(new CustomAttribute(this.compilerGeneratedAttributeConstructor));
            method.DeclaringType.Fields.Add(methodLoggerField);

            VariableDefinition argsVariable = new VariableDefinition("<>args", this.objectArray);
            method.Body.Variables.Add(argsVariable);

            VariableDefinition returnValueVariable = null;
            if (method.MethodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                returnValueVariable = new VariableDefinition("<>returnValue", method.ReturnType);
                method.Body.Variables.Add(returnValueVariable);
            }

            VariableDefinition exceptionVariable = new VariableDefinition("<>exception", this.exception);
            method.Body.Variables.Add(exceptionVariable);

            this.RewriteBody(methodLoggerField, method, argsVariable, returnValueVariable, exceptionVariable);
        }

        private void RewriteBody(FieldDefinition methodLoggerField, MethodDefinition method, VariableDefinition argsVariable, VariableDefinition returnValueVariable, VariableDefinition exceptionVariable)
        {
            IEnumerable<Instruction> beforeTryInstructions = this.CreateBeforeTryInstructions(method, methodLoggerField, argsVariable);
            IList<Instruction> tryBodyInstructions = this.CreateTryBodyInstructions(method, methodLoggerField, returnValueVariable, argsVariable);
            IList<Instruction> catchBodyInstructions = this.CreateCatchBodyInstructions(methodLoggerField, exceptionVariable);
            IList<Instruction> afterTryInstructions = this.CreateAfterTryInstructions(returnValueVariable);

            method.Body.Instructions.Clear();
            method.Body.Instructions.AddRange(beforeTryInstructions);
            method.Body.Instructions.AddRange(tryBodyInstructions);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Leave, afterTryInstructions[0]));
            method.Body.Instructions.AddRange(catchBodyInstructions);
            method.Body.Instructions.AddRange(afterTryInstructions);

            // It's possible that there's no 'ret' instruction in the method because it always throws exception.
            // In this case there will be an unclosed exception handler and we need to fix it.
            foreach (ExceptionHandler exceptionHandler in method.Body.ExceptionHandlers)
            {
                if (exceptionHandler.HandlerEnd == null)
                {
                    exceptionHandler.HandlerEnd = catchBodyInstructions[0];
                }
            }

            method.Body.ExceptionHandlers.Add(
                new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType = this.exception,
                    TryStart = tryBodyInstructions[0],
                    TryEnd = catchBodyInstructions[0],
                    HandlerStart = catchBodyInstructions[0],
                    HandlerEnd = afterTryInstructions[0]
                });

            method.Body.OptimizeMacros();
            method.Body.InitLocals = true;
        }

        /// <summary>
        /// if (methodLogger == null)
        /// {
        ///     methodLogger = MethodLoggerFactory.Current.Create(methodof(Foo));
        /// }
        /// 
        /// object[] args = { p1, p2, p3, ..., pn };
        /// methodLogger.LogEnter(args);
        /// </summary>
        private IEnumerable<Instruction> CreateBeforeTryInstructions(MethodDefinition method, FieldDefinition methodLoggerField, VariableDefinition argsVariable)
        {
            List<Instruction> instructions = new List<Instruction>();

            if (method.Body.Instructions[0].OpCode == OpCodes.Nop)
            {
                // In debug builds the first instruction of a method is always 'nop' and it has a sequence point pointing to the opening bracket of the method.
                // We'll place that instruction with its sequence point to the beginning and will skip this 'nop' later when we copy the method body.
                instructions.Add(method.Body.Instructions[0]);
            }
            else
            {
                // This is a release build and this method doesn't start with 'nop', but our first instruction must have a sequence point or the debugger will
                // try to step into assembly code. We will create a new sequence point from the first real sequence point.
                Document document = method.Body.Instructions[0].SequencePoint.Document;
                int startLine = method.Body.Instructions[0].SequencePoint.StartLine;
                int startColumn = Math.Max(method.Body.Instructions[0].SequencePoint.StartColumn - 1, 0);
                int endColumn = method.Body.Instructions[0].SequencePoint.StartColumn;

                Instruction nop = Instruction.Create(OpCodes.Nop);
                nop.SequencePoint = new SequencePoint(document) { StartLine = startLine, EndLine = startLine, StartColumn = startColumn, EndColumn = endColumn };
                instructions.Add(nop);
            }

            IEnumerable<Instruction> initInstructions = this.CreateMethodLoggerInitializationInstructions(method, methodLoggerField);
            IList<Instruction> logEnterInstructions = this.CreateCallingLogEnterInstructions(method, methodLoggerField, argsVariable);

            // Checking whether the method logger field is null, initializing it if necessary.
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, methodLoggerField));
            instructions.Add(Instruction.Create(OpCodes.Brtrue, logEnterInstructions[0]));
            instructions.AddRange(initInstructions);
            instructions.AddRange(logEnterInstructions);

            return instructions;
        }

        /// <summary>
        /// methodLogger = MethodLoggerFactory.Current.Create(methodof(Foo));
        /// </summary>
        private IEnumerable<Instruction> CreateMethodLoggerInitializationInstructions(MethodDefinition method, FieldDefinition methodLoggerField)
        {
            List<Instruction> instructions = new List<Instruction>();

            // Getting the current IMethodLoggerFactory instance
            MethodReference getCurrentReference = new MethodReference("get_Current", this.methodLoggerFactory, this.methodLoggerFactoryLocator);
            instructions.Add(Instruction.Create(OpCodes.Call, getCurrentReference));

            // Getting the current MethodBase
            MethodReference getMethodFromHandleReference = new MethodReference("GetMethodFromHandle", this.methodBase, this.methodBase);
            getMethodFromHandleReference.Parameters.Add(new ParameterDefinition(this.runtimeMethodHandle));
            instructions.Add(Instruction.Create(OpCodes.Ldtoken, method));
            instructions.Add(Instruction.Create(OpCodes.Call, getMethodFromHandleReference));

            // Calling IMethodEventLogger.Create(MethodBase)
            MethodReference createReference = new MethodReference("Create", this.methodLogger, this.methodLoggerFactory) { HasThis = true };
            createReference.Parameters.Add(new ParameterDefinition(this.methodBase));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, createReference));
            instructions.Add(Instruction.Create(OpCodes.Stsfld, methodLoggerField));

            return instructions;
        }

        /// <summary>
        /// object[] args = { p1, p2, p3, ..., pn };
        /// methodLogger.LogEnter(args);
        /// </summary>
        private IList<Instruction> CreateCallingLogEnterInstructions(MethodDefinition method, FieldDefinition methodLoggerField, VariableDefinition argsVariable)
        {
            // Creating the 'args' array.
            List<Instruction> instructions = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_I4, method.Parameters.Count),
                Instruction.Create(OpCodes.Newarr, this.module.TypeSystem.Object),
                Instruction.Create(OpCodes.Stloc, argsVariable)
            };

            // Filling the 'args' array with the parameters.
            instructions.AddRange(this.CreateFillArgsInstructions(method, argsVariable, false));

            // Calling IMethodLogger.LogEnter(object[]) on the method logger field.
            MethodReference logEnterReference = new MethodReference("LogEnter", this.module.TypeSystem.Void, this.methodLogger) { HasThis = true };
            logEnterReference.Parameters.Add(new ParameterDefinition(new ArrayType(this.module.TypeSystem.Object)));
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, methodLoggerField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc, argsVariable));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, logEnterReference));

            return instructions;
        }

        private IList<Instruction> CreateTryBodyInstructions(MethodDefinition method, FieldDefinition autoLoggerField, VariableDefinition returnValueVariable, VariableDefinition argsVariable)
        {
            IList<Instruction> callingLogLeaveInstructions = this.CreateCallingLogLeaveInstructions(method, autoLoggerField, returnValueVariable, argsVariable);

            List<Instruction> instructions = new List<Instruction>();
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                Instruction instruction = method.Body.Instructions[i];
                if (i == 0 && instruction.OpCode == OpCodes.Nop)
                {
                    continue;
                }

                if (instruction.OpCode == OpCodes.Ret)
                {
                    if (returnValueVariable != null)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Stloc, returnValueVariable));
                    }

                    Instruction leave = Instruction.Create(OpCodes.Leave, callingLogLeaveInstructions[0]);
                    leave.SequencePoint = instruction.SequencePoint;
                    instructions.Add(leave);

                    foreach (Instruction instruction2 in method.Body.Instructions)
                    {
                        if (instruction2.Operand == instruction)
                        {
                            instruction2.Operand = leave;
                        }
                    }

                    continue;
                }

                instructions.Add(instruction);
            }

            instructions.AddRange(callingLogLeaveInstructions);
            return instructions;
        }

        private IList<Instruction> CreateCallingLogLeaveInstructions(MethodDefinition method, FieldDefinition autoLoggerField, VariableDefinition returnValueVariable, VariableDefinition argsVariable)
        {
            List<Instruction> instructions = new List<Instruction>();
            instructions.AddRange(this.CreateFillArgsInstructions(method, argsVariable, true));
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, autoLoggerField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc, argsVariable));

            // If we have a return value variable we have to pass it to the method.
            if (returnValueVariable != null)
            {
                // We have a return value so we have to pass it to the method.
                instructions.Add(Instruction.Create(OpCodes.Ldloc, returnValueVariable));
                if (returnValueVariable.VariableType.IsValueType)
                {
                    // The return value is a value type so we have to box it.
                    instructions.Add(Instruction.Create(OpCodes.Box, returnValueVariable.VariableType));
                }
            }
            else
            {
                // This is a void method so we pass null as a return value.
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
            }

            instructions.Add(Instruction.Create(OpCodes.Callvirt, this.methodLoggerLogLeave));

            return instructions;
        }

        /// <summary>
        /// methodLogger.LogException(exception);
        /// throw;
        /// </summary>
        private IList<Instruction> CreateCatchBodyInstructions(
            FieldDefinition autoLoggerField,
            VariableDefinition exceptionVariable)
        {
            return new[]
                       {
                           Instruction.Create(OpCodes.Stloc, exceptionVariable),
                           Instruction.Create(OpCodes.Ldsfld, autoLoggerField),
                           Instruction.Create(OpCodes.Ldloc, exceptionVariable),
                           Instruction.Create(OpCodes.Callvirt, this.methodLoggerLogException),
                           Instruction.Create(OpCodes.Rethrow)
                       };
        }

        /// <summary>
        /// return ret;
        /// </summary>
        private IList<Instruction> CreateAfterTryInstructions(VariableDefinition returnValueVariable)
        {
            return returnValueVariable != null
                       ? new[] { Instruction.Create(OpCodes.Ldloc, returnValueVariable), Instruction.Create(OpCodes.Ret) }
                       : new[] { Instruction.Create(OpCodes.Ret) };
        }

        /// <summary>
        /// Creates the instructions that fill the 'args' object array with the method's parameters so it can be passed to the logger.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="argsVariable"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private IEnumerable<Instruction> CreateFillArgsInstructions(MethodDefinition method, VariableDefinition argsVariable, bool output)
        {
            List<Instruction> instructions = new List<Instruction>();

            int arrayIndex = 0;
            foreach (ParameterDefinition parameter in method.Parameters)
            {
                if (!output && parameter.IsOut)
                {
                    continue;
                }

                if (output && !parameter.ParameterType.IsByReference)
                {
                    continue;
                }

                instructions.Add(Instruction.Create(OpCodes.Ldloc, argsVariable)); // Push the args array to the stack.
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, arrayIndex++)); // Push the index to the stack where we want to store the parameter value.
                instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter)); // Push the parameter value to the stack.
                if (parameter.ParameterType.IsByReference)
                {
                    TypeDefinition underlyingType = parameter.ParameterType.Resolve();
                    instructions.Add(RewriterHelper.CreateLoadIndirectlyInstruction(underlyingType, this.module));
                    if (underlyingType.IsValueType)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Box, this.module.Import(underlyingType)));
                    }
                }
                else
                {
                    if (parameter.ParameterType.IsValueType)
                    {
                        // The current parameter is a value type so we need to box it to store it in an object array.
                        instructions.Add(Instruction.Create(OpCodes.Box, parameter.ParameterType)); // Pops the value type from the stack and push the boxed value to the stack.
                    }
                }

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            return instructions;
        }
    }
}