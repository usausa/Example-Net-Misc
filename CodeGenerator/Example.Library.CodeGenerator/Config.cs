namespace Example.Library.CodeGenerator
{
    public class Config
    {
        public string[] TargetDirectories { get; set; }

        public string[] ExcludeDirectories { get; set; } = { "bin", "obj" };

        public string FilePattern { get; set; } = "*.cs";
    }
}
