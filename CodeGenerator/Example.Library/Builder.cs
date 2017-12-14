namespace Example.Library
{
    using System;

    public class Builder
    {
        private readonly Engine engine = new Engine();

        public Builder UseLogger(ILogger logger)
        {
            engine.Logger = logger;
            return this;
        }

        public T For<T>()
        {
            var type = typeof(T);
            var proxyName = type.Name + "Proxy";
            var proxyTypeName = type.AssemblyQualifiedName.Replace(typeof(T).Name, proxyName);
            var proxyType = Type.GetType(proxyTypeName);
            if (proxyType == null)
            {
                throw new InvalidOperationException($"{type.Name} is not api interface.");
            }

            return (T)Activator.CreateInstance(proxyType, engine);
        }
    }
}
