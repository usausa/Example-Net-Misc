namespace EmitExample
{
    using System.Reflection;

    public sealed class SampleDataActivator : IActivator
    {
        public ConstructorInfo Source { get; }

        public SampleDataActivator(ConstructorInfo source)
        {
            Source = source;
        }

        public object Create(params object[] arguments)
        {
            return new Data();
        }
    }

    public sealed class SampleDataActivator2 : IActivator
    {
        private readonly ConstructorInfo source;

        public ConstructorInfo Source
        {
            get { return source; }
        }

        public SampleDataActivator2(ConstructorInfo source)
        {
            this.source = source;
        }

        public object Create(params object[] arguments)
        {
            return new Data();
        }
    }

    public sealed class SampleData2Activator : IActivator
    {
        public ConstructorInfo Source { get; }

        public SampleData2Activator(ConstructorInfo source)
        {
            Source = source;
        }

        public object Create(params object[] arguments)
        {
            return new Data2((int)arguments[0], (string)arguments[1]);
        }
    }

    public sealed class SampleDataStringValueAccessor : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleDataStringValueAccessor(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((Data)target).StringValue;
        }

        public void SetValue(object target, object value)
        {
            ((Data)target).StringValue = (string)value;
        }
    }

    public sealed class SampleDataIntValueAccessor : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleDataIntValueAccessor(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((Data)target).IntValue;
        }

        public void SetValue(object target, object value)
        {
            ((Data)target).IntValue = (int?)value ?? 0;
        }
    }

    public sealed class SampleDataIntValueAccessor2 : IAccessor
    {
        public PropertyInfo Source { get; }

        public SampleDataIntValueAccessor2(PropertyInfo source)
        {
            Source = source;
        }

        public object GetValue(object target)
        {
            return ((Data)target).IntValue;
        }

        public void SetValue(object target, object value)
        {
            if (value == null)
            {
                ((Data)target).IntValue = 0;
            }
            else
            {
                ((Data)target).IntValue = (int)value;
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
            return ((Data)target).IntValue;
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
