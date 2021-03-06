﻿namespace EmitExample
{
    using System.Reflection;

    public interface IAccessor
    {
        PropertyInfo Source { get; }

        object GetValue(object target);

        void SetValue(object target, object value);
    }
}
