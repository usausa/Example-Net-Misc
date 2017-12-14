namespace Example.Library
{
    using System;

    [AttributeUsage(AttributeTargets.Interface)]
    public class ApiAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Name { get; }

        public MethodAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }

        public ParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
