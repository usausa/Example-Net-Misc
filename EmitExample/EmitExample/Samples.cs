namespace EmitExample
{
    using System.Reflection;

    public sealed class SampleMutableDataActivator1 : IActivator
    {
        public ConstructorInfo Source { get; }

        public SampleMutableDataActivator1(ConstructorInfo source)
        {
            Source = source;
        }

        public object Create(params object[] arguments)
        {
            return new MutableData();
        }
    }

    public sealed class SampleMutableDataActivator2 : IActivator
    {
        private readonly ConstructorInfo source;

        public ConstructorInfo Source
        {
            get { return source; }
        }

        public SampleMutableDataActivator2(ConstructorInfo source)
        {
            this.source = source;
        }

        public object Create(params object[] arguments)
        {
            return new MutableData();
        }
    }

    public sealed class SampleImutableDataActivator : IActivator
    {
        public ConstructorInfo Source { get; }

        public SampleImutableDataActivator(ConstructorInfo source)
        {
            Source = source;
        }

        public object Create(params object[] arguments)
        {
            return new ImutableData((int)arguments[0], (string)arguments[1]);
        }
    }

    public sealed class SampleMutableDataStringValueAccessor : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleMutableDataStringValueAccessor(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((MutableData)target).StringValue;
        }

        public void SetValue(object target, object value)
        {
            ((MutableData)target).StringValue = (string)value;
        }
    }

    public sealed class SampleMutableDataIntValueAccessor1 : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleMutableDataIntValueAccessor1(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((MutableData)target).IntValue;
        }

        public void SetValue(object target, object value)
        {
            ((MutableData)target).IntValue = (int?)value ?? 0;
        }
    }

    public sealed class SampleMutableDataIntValueAccessor2 : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleMutableDataIntValueAccessor2(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((MutableData)target).IntValue;
        }

        public void SetValue(object target, object value)
        {
            if (value == null)
            {
                ((MutableData)target).IntValue = 0;
            }
            else
            {
                ((MutableData)target).IntValue = (int)value;
            }
        }
    }

    public sealed class EnumPropertyDataAccessor : IAccessor
    {
        public PropertyInfo Source { get; }

        public EnumPropertyDataAccessor(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((EnumPropertyData)target).EnumValue;
        }

        public void SetValue(object target, object value)
        {
            if (value == null)
            {
                ((EnumPropertyData)target).EnumValue = default(MyEnum);
            }
            else
            {
                ((EnumPropertyData)target).EnumValue = (MyEnum)value;
            }
        }
    }

    public sealed class StrunctPropertyDataAccessor : IAccessor
    {
        public PropertyInfo Source { get; }

        public StrunctPropertyDataAccessor(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((MutableData)target).IntValue;
        }

        public void SetValue(object target, object value)
        {
            if (value == null)
            {
                ((StructPropertyData)target).StructValue = default(MyStruct);
            }
            else
            {
                ((StructPropertyData)target).StructValue = (MyStruct)value;
            }
        }
    }
}
