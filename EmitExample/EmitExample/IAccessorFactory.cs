namespace EmitExample
{
    using System.Reflection;

    public interface IAccessorFactory
    {
        IAccessor CreateAccessor(PropertyInfo pi);
    }
}
