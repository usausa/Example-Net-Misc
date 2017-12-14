﻿namespace Example.Library.CodeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class Generator
    {
        private readonly HashSet<string> usings = new HashSet<string>(new[] { "System" });

        private readonly List<ClassInfo> classes = new List<ClassInfo>();

        public void AddSource(string text)
        {
            var tree = CSharpSyntaxTree.ParseText(text);
            var root = tree.GetRoot();

            foreach (var item in root
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(ud => ud.Name.ToString()))
            {
                usings.Add(item);
            }

            classes.AddRange(root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .SelectMany(nds => nds.DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Where(ids => ids.AttributeLists.SelectMany(als => als.Attributes).Any(IsApiAttribute))
                    .Select(interfaceNode => CreateClassInfo(nds, interfaceNode))));
        }

        private static bool IsApiAttribute(AttributeSyntax syntax)
        {
            var name = syntax.Name.ToString().Split('.').Last();
            return ((name == "Api") || (name == "ApiAttribute")) &&
                ((syntax.ArgumentList?.Arguments.Count ?? 0) == 0);
        }

        private static ClassInfo CreateClassInfo(NamespaceDeclarationSyntax nds, InterfaceDeclarationSyntax ids)
        {
            return new ClassInfo
            {
                Namespace = nds.Name.ToString(),
                Interface = ids.Identifier.Text,
                Methods = ids.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Select(CreateMethodInfo)
                    .ToArray()
            };
        }

        private static MethodInfo CreateMethodInfo(MethodDeclarationSyntax mds)
        {
            return new MethodInfo
            {
                Name = mds.Identifier.Text,
                ReturnType = mds.ReturnType.ToString(),
                ParameterTypes = String.Join(", ", mds.ParameterList.Parameters.Select(ps => String.Format("typeof({0})", ps.Type.ToString()))),
                ArgumentsWithTypes = String.Join( ",", mds.ParameterList.Parameters.Select(ps => String.Format("{0} {1}", ps.Type.ToString(), ps.Identifier.Text))),
                Arguments = String.Join(", ", mds.ParameterList.Parameters.Select(ps => ps.Identifier.Text))
            };
        }

        public string Generate()
        {
            var sb = new StringBuilder(8192);

            sb.Append("// <auto-generated />").AppendLine();

            foreach (var item in usings.OrderBy(x => x))
            {
                sb.Append("using ").Append(item).AppendLine(";");
            }

            sb.AppendLine();

            sb.Append("namespace ExampleGenerated").AppendLine();
            sb.Append("{").AppendLine();
            sb.Append("    [AttributeUsage (AttributeTargets.Class)]").AppendLine();
            sb.Append("    internal sealed class PreserveAttribute : Attribute").AppendLine();
            sb.Append("    {").AppendLine();
            sb.Append("    }").AppendLine();
            sb.Append("}").AppendLine();

            foreach (var @class in classes)
            {
                var className = @class.Interface + "Proxy";

                sb.AppendLine();

                sb.Append("namespace ").Append(@class.Namespace).AppendLine();
                sb.Append("{").AppendLine();
                sb.Append("    using global::ExampleGenerated;").AppendLine();
                sb.AppendLine();
                sb.Append("    [Preserve]").AppendLine();
                sb.Append("    public class ").Append(className).Append(" : ").Append(@class.Interface).AppendLine();
                sb.Append("    {").AppendLine();
                sb.Append("        private readonly global::Example.Library.Engine engine;").AppendLine();
                sb.AppendLine();
                sb.Append("        public ").Append(className).Append("(global::Example.Library.Engine engine)").AppendLine();
                sb.Append("        {").AppendLine();
                sb.Append("            this.engine = engine;").AppendLine();
                sb.Append("        }").AppendLine();

                for (var i = 0; i < @class.Methods.Length; i++)
                {
                    var method = @class.Methods[i];

                    sb.AppendLine();
                    sb.Append("        private readonly global::Example.Library.MethodMetadata methodMetadata")
                        .Append(i)
                        .Append(" = global::Example.Library.MetadataFactory.CreateMethodMetadata(typeof(")
                        .Append(@class.Interface)
                        .Append("), \"")
                        .Append(method.Name)
                        .Append("\", new Type[] { ")
                        .Append(method.ParameterTypes)
                        .Append(" });")
                        .AppendLine();
                    sb.AppendLine();

                    sb.Append("        public ")
                        .Append(method.ReturnType)
                        .Append(" ")
                        .Append(method.Name)
                        .Append("(")
                        .Append(method.ArgumentsWithTypes)
                        .Append(")")
                        .AppendLine();
                    sb.Append("        {").AppendLine();
                    sb.Append("            ");
                    if (method.ReturnType != "void")
                    {
                        sb.Append("return (")
                            .Append(method.ReturnType)
                            .Append(")");
                    }
                    sb.Append("engine.Call(this.methodMetadata")
                        .Append(i)
                        .Append(String.IsNullOrEmpty(method.Arguments) ? string.Empty : ", ")
                        .Append(method.Arguments)
                        .Append(");")
                        .AppendLine();
                    sb.Append("        }").AppendLine();
                }

                sb.Append("    }").AppendLine();
                sb.Append("}").AppendLine();
            }

            return sb.ToString();
        }
    }

    public class ClassInfo
    {
        public string Namespace { get; set; }

        public string Interface { get; set; }

        public MethodInfo[] Methods { get; set; }
    }

    public class MethodInfo
    {
        public string ReturnType { get; set; }

        public string Name { get; set; }

        public string ParameterTypes { get; set; }

        public string ArgumentsWithTypes { get; set; }

        public string Arguments { get; set; }
    }
}