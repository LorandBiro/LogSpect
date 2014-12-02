namespace LogSpectRewriter.Rewriting
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using Mono.Collections.Generic;

    internal static class RewriterHelper
    {
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static bool IsEquivalentTo(this TypeReference typeReference, Type type)
        {
            return typeReference.Namespace == type.Namespace && typeReference.Name == type.Name;
        }

        /// <summary>
        /// Creates a 'load indirectly' instruction optimized for the specified type.
        /// Mono.Cecil.Rocks implements optimizations for some instructions ('ldc', 'ldarg', 'ldloc', 'stloc', ...) but 'ldind' is not supported.
        /// </summary>
        public static Instruction CreateLoadIndirectlyInstruction(TypeDefinition type, ModuleDefinition module)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    return CreateLoadIndirectlyInstruction(type.GetEnumUnderlyingType().Resolve(), module);
                }

                if (type == module.TypeSystem.SByte.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_I1);
                }

                if (type == module.TypeSystem.Int16.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_I2);
                }

                if (type == module.TypeSystem.Int32.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_I4);
                }

                if (type == module.TypeSystem.Int64.Resolve() || type == module.TypeSystem.UInt64.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_I8);
                }

                if (type == module.TypeSystem.Byte.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_U1);
                }

                if (type == module.TypeSystem.UInt16.Resolve() || type == module.TypeSystem.Char.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_U2);
                }

                if (type == module.TypeSystem.UInt32.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_U4);
                }

                if (type == module.TypeSystem.Single.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_R4);
                }

                if (type == module.TypeSystem.Double.Resolve())
                {
                    return Instruction.Create(OpCodes.Ldind_R8);
                }

                return Instruction.Create(OpCodes.Ldobj, module.Import(type));
            }

            return Instruction.Create(OpCodes.Ldind_Ref);
        }
    }
}
