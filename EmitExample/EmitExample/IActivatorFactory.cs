namespace EmitExample
{
    using System.Reflection;

    public interface IActivatorFactory
    {
        IActivator CreateActivator(ConstructorInfo ci);
    }
}
