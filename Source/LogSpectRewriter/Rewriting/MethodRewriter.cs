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
    using FieldAttributes = Mono.Cecil.FieldAttributes;

    internal class MethodRewriter
    {
        private readonly ModuleDefinition module;

        private readonly TypeReferences typeReferences;

        private readonly MethodReferences methodReferences;

        public MethodRewriter(ModuleDefinition module)
        {
            this.module = module;
            this.typeReferences = new TypeReferences(this.module);
            this.methodReferences = new MethodReferences(this.typeReferences);
        }

        public void Rewrite(MethodDefinition method)
        {
            FieldDefinition methodLoggerField = new FieldDefinition(
                string.Format("<>{0}_{1:x8}", method.Name, method.FullName.GetHashCode()),
                FieldAttributes.Private | FieldAttributes.Static,
                this.typeReferences.IMethodLogger);
            methodLoggerField.CustomAttributes.Add(new CustomAttribute(this.methodReferences.CompilerGeneratedAttribute_Constructor));
            method.DeclaringType.Fields.Add(methodLoggerField);

            VariableDefinition argsVariable = new VariableDefinition("<>args", this.typeReferences.ObjectArray);
            method.Body.Variables.Add(argsVariable);

            VariableDefinition typeVariable = new VariableDefinition("<>type", this.typeReferences.Type);
            method.Body.Variables.Add(typeVariable);

            VariableDefinition returnValueVariable = null;
            if (method.MethodReturnType.ReturnType.MetadataType != MetadataType.Void)
            {
                returnValueVariable = new VariableDefinition("<>returnValue", method.ReturnType);
                method.Body.Variables.Add(returnValueVariable);
            }

            VariableDefinition exceptionVariable = new VariableDefinition("<>exception", this.typeReferences.Exception);
            method.Body.Variables.Add(exceptionVariable);

            this.RewriteBody(methodLoggerField, method, typeVariable, argsVariable, returnValueVariable, exceptionVariable);
        }

        private void RewriteBody(FieldDefinition methodLoggerField, MethodDefinition method, VariableDefinition typeVariable, VariableDefinition argsVariable, VariableDefinition returnValueVariable, VariableDefinition exceptionVariable)
        {
            IEnumerable<Instruction> beforeTryInstructions = this.CreateBeforeTryInstructions(method, methodLoggerField, typeVariable, argsVariable);
            IList<Instruction> tryBodyInstructions = this.CreateTryBodyInstructions(method, methodLoggerField, typeVariable, returnValueVariable, argsVariable);
            IList<Instruction> catchBodyInstructions = this.CreateCatchBodyInstructions(methodLoggerField, typeVariable, exceptionVariable);
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
                    CatchType = this.typeReferences.Exception,
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
        /// Type type = this.GetType();
        /// object[] args = { p1, p2, p3, ..., pn };
        /// methodLogger.LogEnter(args);
        /// </summary>
        private IEnumerable<Instruction> CreateBeforeTryInstructions(MethodDefinition method, FieldDefinition methodLoggerField, VariableDefinition typeVariable, VariableDefinition argsVariable)
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
            IList<Instruction> logEnterInstructions = this.CreateCallingLogEnterInstructions(method, methodLoggerField, typeVariable, argsVariable);

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
            instructions.Add(Instruction.Create(OpCodes.Call, this.methodReferences.MethodLoggerFactory_GetCurrent));

            // Getting the current MethodBase
            instructions.Add(Instruction.Create(OpCodes.Ldtoken, method));
            instructions.Add(Instruction.Create(OpCodes.Call, this.methodReferences.MethodBase_GetMethodFromHandle));

            // Calling IMethodEventLogger.Create(MethodBase)
            instructions.Add(Instruction.Create(OpCodes.Callvirt, this.methodReferences.IMethodLoggerFactory_Create));
            instructions.Add(Instruction.Create(OpCodes.Stsfld, methodLoggerField));

            return instructions;
        }

        /// <summary>
        /// Type type = this.GetType();
        /// object[] args = { p1, p2, p3, ..., pn };
        /// methodLogger.LogEnter(args);
        /// </summary>
        private IList<Instruction> CreateCallingLogEnterInstructions(MethodDefinition method, FieldDefinition methodLoggerField, VariableDefinition typeVariable, VariableDefinition argsVariable)
        {
            List<Instruction> instructions = new List<Instruction>();

            // Determining the executing class type
            if (method.HasThis)
            {
                // This isn't a static method so we call this.GetType().
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Call, this.methodReferences.Object_GetType));
            }
            else
            {
                // This is a static method, we have to use typeof(DeclaringType).
                instructions.Add(Instruction.Create(OpCodes.Ldtoken, method.DeclaringType));
                instructions.Add(Instruction.Create(OpCodes.Call, this.methodReferences.Type_GetTypeFromHandle));
            }

            instructions.Add(Instruction.Create(OpCodes.Stloc, typeVariable));

            // Creating the 'args' array.
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, method.Parameters.Count));
            instructions.Add(Instruction.Create(OpCodes.Newarr, this.typeReferences.Object));
            instructions.Add(Instruction.Create(OpCodes.Stloc, argsVariable));

            // Filling the 'args' array with the parameters.
            instructions.AddRange(this.CreateFillArgsInstructions(method, argsVariable, false));

            // Calling IMethodLogger.LogEnter(object[]) on the method logger field.
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, methodLoggerField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc, typeVariable));
            instructions.Add(Instruction.Create(OpCodes.Ldloc, argsVariable));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, this.methodReferences.IMethodLogger_LogEnter));

            return instructions;
        }

        private IList<Instruction> CreateTryBodyInstructions(MethodDefinition method, FieldDefinition autoLoggerField, VariableDefinition typeVariable, VariableDefinition returnValueVariable, VariableDefinition argsVariable)
        {
            IList<Instruction> callingLogLeaveInstructions = this.CreateCallingLogLeaveInstructions(method, autoLoggerField, typeVariable, returnValueVariable, argsVariable);

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

        private IList<Instruction> CreateCallingLogLeaveInstructions(MethodDefinition method, FieldDefinition autoLoggerField, VariableDefinition typeVariable, VariableDefinition returnValueVariable, VariableDefinition argsVariable)
        {
            List<Instruction> instructions = new List<Instruction>();
            instructions.AddRange(this.CreateFillArgsInstructions(method, argsVariable, true));
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, autoLoggerField));
            instructions.Add(Instruction.Create(OpCodes.Ldloc, typeVariable));
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

            instructions.Add(Instruction.Create(OpCodes.Callvirt, this.methodReferences.IMethodLogger_LogLeave));

            return instructions;
        }

        /// <summary>
        /// methodLogger.LogException(exception);
        /// throw;
        /// </summary>
        private IList<Instruction> CreateCatchBodyInstructions(
            FieldDefinition autoLoggerField,
            VariableDefinition typeVariable,
            VariableDefinition exceptionVariable)
        {
            return new[]
                       {
                           Instruction.Create(OpCodes.Stloc, exceptionVariable),
                           Instruction.Create(OpCodes.Ldsfld, autoLoggerField),
                           Instruction.Create(OpCodes.Ldloc, typeVariable),
                           Instruction.Create(OpCodes.Ldloc, exceptionVariable),
                           Instruction.Create(OpCodes.Callvirt, this.methodReferences.IMethodLogger_LogException),
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

        private class TypeReferences
        {
            public TypeReferences(ModuleDefinition module)
            {
                this.Void = module.TypeSystem.Void;
                this.Object = module.TypeSystem.Object;

                this.CompilerGeneratedAttribute = module.Import(typeof(CompilerGeneratedAttribute));
                this.Exception = module.Import(typeof(Exception));
                this.MethodBase = module.Import(typeof(MethodBase));
                this.IMethodLogger = module.Import(typeof(IMethodLogger));
                this.IMethodLoggerFactory = module.Import(typeof(IMethodLoggerFactory));
                this.MethodLoggerFactory = module.Import(typeof(MethodLoggerFactory));
                this.RuntimeMethodHandle = module.Import(typeof(RuntimeMethodHandle));
                this.RuntimeTypeHandle = module.Import(typeof(RuntimeTypeHandle));
                this.Type = module.Import(typeof(Type));

                this.ObjectArray = new ArrayType(this.Object);
            }

            // ReSharper disable InconsistentNaming
            public TypeReference Object { get; private set; }

            public TypeReference Void { get; private set; }

            public TypeReference CompilerGeneratedAttribute { get; private set; }

            public TypeReference Exception { get; private set; }

            public TypeReference MethodBase { get; private set; }

            public TypeReference IMethodLogger { get; private set; }

            public TypeReference IMethodLoggerFactory { get; private set; }

            public TypeReference MethodLoggerFactory { get; private set; }

            public TypeReference RuntimeMethodHandle { get; private set; }

            public TypeReference RuntimeTypeHandle { get; private set; }

            public TypeReference Type { get; private set; }

            public TypeReference ObjectArray { get; private set; }
            // ReSharper restore InconsistentNaming
        }

        private class MethodReferences
        {
            public MethodReferences(TypeReferences typeReferences)
            {
                this.CompilerGeneratedAttribute_Constructor = new MethodReference(".ctor", typeReferences.Void, typeReferences.CompilerGeneratedAttribute) { HasThis = true };

                this.IMethodLogger_LogEnter = new MethodReference("LogEnter", typeReferences.Void, typeReferences.IMethodLogger) { HasThis = true };
                this.IMethodLogger_LogEnter.Parameters.Add(new ParameterDefinition(typeReferences.Type));
                this.IMethodLogger_LogEnter.Parameters.Add(new ParameterDefinition(typeReferences.ObjectArray));

                this.IMethodLogger_LogException = new MethodReference("LogException", typeReferences.Void, typeReferences.IMethodLogger) { HasThis = true };
                this.IMethodLogger_LogException.Parameters.Add(new ParameterDefinition(typeReferences.Type));
                this.IMethodLogger_LogException.Parameters.Add(new ParameterDefinition(typeReferences.Exception));

                this.IMethodLogger_LogLeave = new MethodReference("LogLeave", typeReferences.Void, typeReferences.IMethodLogger) { HasThis = true };
                this.IMethodLogger_LogLeave.Parameters.Add(new ParameterDefinition(typeReferences.Type));
                this.IMethodLogger_LogLeave.Parameters.Add(new ParameterDefinition(new ArrayType(typeReferences.Object)));
                this.IMethodLogger_LogLeave.Parameters.Add(new ParameterDefinition(typeReferences.Object));

                this.IMethodLoggerFactory_Create = new MethodReference("Create", typeReferences.IMethodLogger, typeReferences.IMethodLoggerFactory) { HasThis = true };
                this.IMethodLoggerFactory_Create.Parameters.Add(new ParameterDefinition(typeReferences.MethodBase));

                this.MethodBase_GetMethodFromHandle = new MethodReference("GetMethodFromHandle", typeReferences.MethodBase, typeReferences.MethodBase);
                this.MethodBase_GetMethodFromHandle.Parameters.Add(new ParameterDefinition(typeReferences.RuntimeMethodHandle));

                this.MethodLoggerFactory_GetCurrent = new MethodReference("get_Current", typeReferences.IMethodLoggerFactory, typeReferences.MethodLoggerFactory);

                this.Object_GetType = new MethodReference("GetType", typeReferences.Type, typeReferences.Object) { HasThis = true };

                this.Type_GetTypeFromHandle = new MethodReference("GetTypeFromHandle", typeReferences.Type, typeReferences.Type);
                this.Type_GetTypeFromHandle.Parameters.Add(new ParameterDefinition(typeReferences.RuntimeTypeHandle));
            }

            // ReSharper disable InconsistentNaming
            public MethodReference CompilerGeneratedAttribute_Constructor { get; private set; }

            public MethodReference IMethodLogger_LogEnter { get; private set; }

            public MethodReference IMethodLogger_LogException { get; private set; }

            public MethodReference IMethodLogger_LogLeave { get; private set; }

            public MethodReference IMethodLoggerFactory_Create { get; private set; }

            public MethodReference MethodBase_GetMethodFromHandle { get; private set; }

            public MethodReference MethodLoggerFactory_GetCurrent { get; private set; }

            public MethodReference Object_GetType { get; private set; }

            public MethodReference Type_GetTypeFromHandle { get; private set; }
            // ReSharper restore InconsistentNaming
        }
    }
}