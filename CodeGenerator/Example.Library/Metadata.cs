namespace Example.Library
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class MethodMetadata
    {
        public string Name { get; }

        public Type ReturnType { get; }

        public ParameterMetadata[] Parameters { get; }

        public MethodMetadata(string name, Type returnType, ParameterMetadata[] parameters)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
        }
    }

    public class ParameterMetadata
    {
        public string Name { get; }

        public Type ParameterType { get; }

        public ParameterMetadata(string name, Type parameterType)
        {
            Name = name;
            ParameterType = parameterType;
        }
    }

    public static class MetadataFactory
    {
        public static MethodMetadata CreateMethodMetadata(Type type, string name, Type[] parameters)
        {
            var mi = type.GetRuntimeMethod(name, parameters);
            var ma = mi.GetCustomAttribute<MethodAttribute>(true);
            return new MethodMetadata(
                ma?.Name ?? mi.Name,
                mi.ReturnType,
                mi.GetParameters().Select(CreateParameterMetadata).ToArray());
        }

        private static ParameterMetadata CreateParameterMetadata(ParameterInfo pi)
        {
            var pa = pi.GetCustomAttribute<ParameterAttribute>(true);
            return new ParameterMetadata(pa?.Name ?? pi.Name, pi.ParameterType);
        }
    }
}
