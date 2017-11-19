namespace EmitExample
{
    using System;
    using System.Diagnostics;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CanActivateDefaultConstructor();
            CanActivateWithArgument();

            CanAccessClassProperty();
            CanAccessValueTypeProperty();
            CanAccessEnumProperty();
            CanAccessStructProperty();
        }

        private static void CanActivateDefaultConstructor()
        {
            var ctor = typeof(Data).GetConstructor(Type.EmptyTypes);
            var activator = EmitFactory.Default.CreateActivator(ctor);

            var obj = activator.Create();
            Debug.Assert(obj.GetType() == typeof(Data));
        }

        private static void CanActivateWithArgument()
        {
            var ctor = typeof(Data2).GetConstructor(new[] { typeof(int), typeof(string) });
            var activator = EmitFactory.Default.CreateActivator(ctor);

            var obj = (Data2)activator.Create(1, "abc");
            Debug.Assert(obj.IntValue == 1);
            Debug.Assert(obj.StringValue == "abc");
        }

        private static void CanAccessClassProperty()
        {
            var pi = typeof(Data).GetProperty(nameof(Data.StringValue));
            var accessor = EmitFactory.Default.CreateAccessor(pi);

            var data = new Data();

            accessor.SetValue(data, "abc");
            Debug.Assert((string)accessor.GetValue(data) == "abc");

            accessor.SetValue(data, null);
            Debug.Assert((string)accessor.GetValue(data) == null);
        }

        private static void CanAccessValueTypeProperty()
        {
            var pi = typeof(Data).GetProperty(nameof(Data.IntValue));
            var accessor = EmitFactory.Default.CreateAccessor(pi);

            var data = new Data();

            accessor.SetValue(data, 1);
            Debug.Assert((int)accessor.GetValue(data) == 1);

            accessor.SetValue(data, null);
            Debug.Assert((int)accessor.GetValue(data) == 0);
        }

        private static void CanAccessEnumProperty()
        {
            var pi = typeof(EnumPropertyData).GetProperty(nameof(EnumPropertyData.EnumValue));
            var accessor = EmitFactory.Default.CreateAccessor(pi);

            var data = new EnumPropertyData();

            accessor.SetValue(data, MyEnum.One);
            Debug.Assert((MyEnum)accessor.GetValue(data) == MyEnum.One);

            accessor.SetValue(data, null);
            Debug.Assert((MyEnum)accessor.GetValue(data) == default(MyEnum));
        }

        private static void CanAccessStructProperty()
        {
            var pi = typeof(StructPropertyData).GetProperty(nameof(StructPropertyData.StructValue));
            var accessor = EmitFactory.Default.CreateAccessor(pi);

            var data = new StructPropertyData();

            accessor.SetValue(data, new MyStruct { X = 1, Y = 2 });
            var structValue = (MyStruct)accessor.GetValue(data);
            Debug.Assert(structValue.X == 1);
            Debug.Assert(structValue.Y == 2);

            accessor.SetValue(data, null);
            structValue = (MyStruct)accessor.GetValue(data);
            Debug.Assert(structValue.X == 0);
            Debug.Assert(structValue.Y == 0);
        }
    }
}
