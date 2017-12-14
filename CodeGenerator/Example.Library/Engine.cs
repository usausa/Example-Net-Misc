namespace Example.Library
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public class Engine
    {
        public ILogger Logger { get; set; }

        public object Call(MethodMetadata md, params object[] arguments)
        {
            var sb = new StringBuilder();

            sb.Append("{ ");
            sb.AppendFormat("Name: \"{0}\", ", md.Name);
            sb.Append("Parameters: { ");

            for (var i = 0; i < md.Parameters.Length; i++)
            {
                sb.AppendFormat("{0}: \"{1}\", ", md.Parameters[i].Name, arguments[i]);
            }

            if (md.Parameters.Length > 0)
            {
                sb.Length = sb.Length - 2;
            }

            sb.Append(" }");
            sb.Append(" }");

            Logger?.Log(sb.ToString());

            return md.ReturnType.GetTypeInfo().IsValueType ? Activator.CreateInstance(md.ReturnType) : null;
        }
    }
}
