namespace EmitExample
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public class EmitFactory : IActivatorFactory, IAccessorFactory
    {
        private const string AssemblyName = "DynamicAssembly";

        private const string ModuleName = "DynamicModule";

        public static EmitFactory Default { get; } = new EmitFactory();

        private readonly object sync = new object();

        private readonly Dictionary<ConstructorInfo, IActivator> activatorCache = new Dictionary<ConstructorInfo, IActivator>();

        private readonly Dictionary<PropertyInfo, IAccessor> accessorCache = new Dictionary<PropertyInfo, IAccessor>();

        private AssemblyBuilder assemblyBuilder;

        private ModuleBuilder moduleBuilder;

        private ModuleBuilder ModuleBuilder
        {
            get
            {
                if (moduleBuilder == null)
                {
                    assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName(AssemblyName),
                        AssemblyBuilderAccess.Run);
                        //AssemblyBuilderAccess.RunAndSave);
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(
                        ModuleName);
                        //"test.dll");
                }
                return moduleBuilder;
            }
        }

        //--------------------------------------------------------------------------------
        // Activator
        //--------------------------------------------------------------------------------

        public IActivator CreateActivator(ConstructorInfo ci)
        {
            lock (sync)
            {
                if (!activatorCache.TryGetValue(ci, out var activator))
                {
                    activator = CreateActivatorInternal(ci);
                    activatorCache[ci] = activator;
                }

                return activator;
            }
        }

        private IActivator CreateActivatorInternal(ConstructorInfo ci)
        {
            var typeBuilder = ModuleBuilder.DefineType(
                $"{ci.DeclaringType.FullName}_DynamicActivator{Array.IndexOf(ci.DeclaringType.GetConstructors(), ci)}",
                TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

            typeBuilder.AddInterfaceImplementation(typeof(IActivator));

            // Field
            var sourceField = typeBuilder.DefineField(
                "source",
                typeof(ConstructorInfo),
                FieldAttributes.Private | FieldAttributes.InitOnly);

            // Property
            DefineActivatorPropertySource(typeBuilder, sourceField);

            // Constructor
            DefineActivatorConstructor(typeBuilder, sourceField);

            // Method
            DefineActivatorMethodCreate(typeBuilder, ci);

            var typeInfo = typeBuilder.CreateTypeInfo();

            //assemblyBuilder.Save("test.dll");

            return (IActivator)Activator.CreateInstance(typeInfo.AsType(), ci);
        }

        private static void DefineActivatorPropertySource(TypeBuilder typeBuilder, FieldBuilder sourceField)
        {
            var property = typeBuilder.DefineProperty(
                "Source",
                PropertyAttributes.None,
                typeof(ConstructorInfo),
                null);
            var method = typeBuilder.DefineMethod(
                "get_Source",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(ConstructorInfo),
                Type.EmptyTypes);
            property.SetGetMethod(method);

            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, sourceField);

            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void DefineActivatorConstructor(TypeBuilder typeBuilder, FieldBuilder sourceField)
        {
            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new[] { typeof(ConstructorInfo) });

            var ilGenerator = ctor.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, sourceField);

            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void DefineActivatorMethodCreate(TypeBuilder typeBuilder, ConstructorInfo ci)
        {
            var method = typeBuilder.DefineMethod(
                nameof(IActivator.Create),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(object),
                new[] { typeof(object[]) });
            typeBuilder.DefineMethodOverride(method, typeof(IActivator).GetMethod(nameof(IActivator.Create)));

            var ilGenerator = method.GetILGenerator();

            for (var i = 0; i < ci.GetParameters().Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.EmitLdcI4(i);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);
                ilGenerator.EmitTypeConversion(ci.GetParameters()[i].ParameterType);
            }

            ilGenerator.Emit(OpCodes.Newobj, ci);

            ilGenerator.Emit(OpCodes.Ret);
        }

        //--------------------------------------------------------------------------------
        // Accessor
        //--------------------------------------------------------------------------------

        public IAccessor CreateAccessor(PropertyInfo pi)
        {
            lock (sync)
            {
                if (!accessorCache.TryGetValue(pi, out var accessor))
                {
                    accessor = CreateAccessorInternal(pi);
                    accessorCache[pi] = accessor;
                }

                return accessor;
            }
        }

        private IAccessor CreateAccessorInternal(PropertyInfo pi)
        {
            var typeBuilder = ModuleBuilder.DefineType(
                $"{pi.DeclaringType.FullName}_{pi.Name}_DynamicAccsessor",
                TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

            typeBuilder.AddInterfaceImplementation(typeof(IAccessor));

            // Fields
            var sourceField = typeBuilder.DefineField(
                "source",
                typeof(PropertyInfo),
                FieldAttributes.Private | FieldAttributes.InitOnly);

            // Property
            DefineAccessorPropertySource(typeBuilder, sourceField);

            // Constructor
            DefineAccessorConstructor(typeBuilder, sourceField);

            // Method
            DefineAccessorMethodGetValue(typeBuilder, pi);
            DefineAccessorMethodSetValue(typeBuilder, pi);

            var type = typeBuilder.CreateType();

            //assemblyBuilder.Save("test.dll");

            return (IAccessor)Activator.CreateInstance(type, pi);
        }

        private static void DefineAccessorPropertySource(TypeBuilder typeBuilder, FieldBuilder sourceField)
        {
            var property = typeBuilder.DefineProperty(
                "Source",
                PropertyAttributes.None,
                typeof(PropertyInfo),
                null);
            var method = typeBuilder.DefineMethod(
                "get_Source",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(PropertyInfo),
                Type.EmptyTypes);
            property.SetGetMethod(method);

            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, sourceField);

            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void DefineAccessorConstructor(TypeBuilder typeBuilder, FieldBuilder sourceField)
        {
            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new[] { typeof(PropertyInfo) });

            var ilGenerator = ctor.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, sourceField);

            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void DefineAccessorMethodGetValue(TypeBuilder typeBuilder, PropertyInfo pi)
        {
            var method = typeBuilder.DefineMethod(
                nameof(IAccessor.GetValue),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(object),
                new[] { typeof(object) });
            typeBuilder.DefineMethodOverride(method, typeof(IAccessor).GetMethod(nameof(IAccessor.GetValue)));

            var ilGenerator = method.GetILGenerator();

            if (!pi.CanRead)
            {
                ilGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
                ilGenerator.Emit(OpCodes.Throw);
                return;
            }

            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, pi.DeclaringType);
            ilGenerator.Emit(OpCodes.Callvirt, pi.GetGetMethod());
            if (pi.PropertyType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, pi.PropertyType);
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void DefineAccessorMethodSetValue(TypeBuilder typeBuilder, PropertyInfo pi)
        {
            var method = typeBuilder.DefineMethod(
                nameof(IAccessor.SetValue),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final,
                typeof(void),
                new[] { typeof(object), typeof(object) });
            typeBuilder.DefineMethodOverride(method, typeof(IAccessor).GetMethod(nameof(IAccessor.SetValue)));

            var ilGenerator = method.GetILGenerator();

            if (!pi.CanWrite)
            {
                ilGenerator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
                ilGenerator.Emit(OpCodes.Throw);
                return;
            }

            if (pi.PropertyType.IsValueType)
            {
                var hasValue = ilGenerator.DefineLabel();

                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Brtrue_S, hasValue);

                // null
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, pi.DeclaringType);

                var type = pi.PropertyType.IsEnum ? pi.PropertyType.GetEnumUnderlyingType() : pi.PropertyType;
                if (LdcDictionary.TryGetValue(type, out var action))
                {
                    action(ilGenerator);
                }
                else
                {
                    var local = ilGenerator.DeclareLocal(pi.PropertyType);
                    ilGenerator.Emit(OpCodes.Ldloca_S, local);
                    ilGenerator.Emit(OpCodes.Initobj, pi.PropertyType);
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                }

                ilGenerator.Emit(OpCodes.Callvirt, pi.GetSetMethod());

                ilGenerator.Emit(OpCodes.Ret);

                // not null
                ilGenerator.MarkLabel(hasValue);

                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, pi.DeclaringType);

                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Unbox_Any, pi.PropertyType);

                ilGenerator.Emit(OpCodes.Callvirt, pi.GetSetMethod());

                ilGenerator.Emit(OpCodes.Ret);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, pi.DeclaringType);

                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Castclass, pi.PropertyType);

                ilGenerator.Emit(OpCodes.Callvirt, pi.GetSetMethod());

                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        private static readonly Dictionary<Type, Action<ILGenerator>> LdcDictionary = new Dictionary<Type, Action<ILGenerator>>
        {
            { typeof(bool), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(byte), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(char), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(short), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(int), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(sbyte), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(ushort), il => il.Emit(OpCodes.Ldc_I4_0) },
            { typeof(uint), il => il.Emit(OpCodes.Ldc_I4_0) },      // Simplicity
            { typeof(long), il => il.Emit(OpCodes.Ldc_I8, 0L) },
            { typeof(ulong), il => il.Emit(OpCodes.Ldc_I8, 0L) },   // Simplicity
            { typeof(float), il => il.Emit(OpCodes.Ldc_R4, 0f) },
            { typeof(double), il => il.Emit(OpCodes.Ldc_R8, 0d) },
            { typeof(IntPtr), il => il.Emit(OpCodes.Ldc_I4_0) },    // Simplicity
            { typeof(UIntPtr), il => il.Emit(OpCodes.Ldc_I4_0) },   // Simplicity
        };
    }
}
