dotnet restore Example.Library.sln
dotnet build Example.Library.sln -c Release
dotnet publish Example.Library.CodeGenerator\Example.Library.CodeGenerator.csproj -c Release -o bin\Release\PublishOutput
nuget pack Example.Library.nuspec
