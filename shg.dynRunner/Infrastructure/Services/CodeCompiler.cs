using FluentResults;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using shg.dynRunner.Application.Models;
using System.Collections;
using System.Reflection;

namespace shg.dynRunner.Infrastructure.Services
{
    internal class CodeCompiler
    {
        public static Result<byte[]> CompileSourceCode(string sourceCode, string identifier, List<Type> additionalTypes)
        {
            return CompileCodeFromString(sourceCode, identifier, additionalTypes);
        }

        private static Result<byte[]> CompileCodeFromString(string sourceCode, string? identifier = null, List<Type>? additionalTypes = null)
        {
            using var peStream = new MemoryStream();
            var result = GenerateCode(sourceCode, identifier, additionalTypes)
                                    .Emit(peStream);
            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                return Result.Fail(failures.Select(x => $"{x.Id} - {x.GetMessage()}"));
            }
            else
            {
                peStream.Seek(0, SeekOrigin.Begin);
                return Result.Ok(peStream.ToArray());
            }
        }

        private static CSharpCompilation GenerateCode(string sourceCode, 
            string? identifier = null,
            List<Type>? additionalTypes = null)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp11);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Task).Assembly.Location)
            };

            // the additional types comes before loading references
            if (additionalTypes is not null)
                additionalTypes.ForEach(x => references.Add(MetadataReference.CreateFromFile(x.Assembly.Location)));


            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            var dllName = identifier ?? "Default.dll";
            var compilation = CSharpCompilation.Create(dllName,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
            var analyseClasses = AnalyseClasses(compilation);
            return compilation;
        }

        public static Result<List<DynClass>> AnalyseClasses(CSharpCompilation compilation) //byte[] compiledCode)
        {
            //MetadataReference metadataReference = MetadataReference.CreateFromImage(compiledCode);
            //Compilation compilation = CSharpCompilation.Create("MyCompilation", references: new[] { metadataReference });
            INamespaceSymbol globalNamespace = compilation.GlobalNamespace;
            IEnumerable<INamedTypeSymbol> classes = globalNamespace.GetNamespaceMembers().SelectMany(x => x.GetTypeMembers());

            var classList = new List<DynClass>();
            foreach (INamedTypeSymbol classSymbol in classes)
            {
                IEnumerable<IMethodSymbol> publicMethods = classSymbol.GetMembers().OfType<IMethodSymbol>().Where(m => m.DeclaredAccessibility == Accessibility.Public);

                classList.Add(new DynClass
                {
                    ClassName = classSymbol.Name,
                    Methods = publicMethods.Select(x => new DynMethod
                    {
                        Name = x.Name,
                        ReturnType = x.ReturnsVoid ? null : x.ReturnType.GetType(),
                        Parameters = x.Parameters.Select(y => new DynParameter
                        {
                            Name = y.Name,
                            Type = y.Type.GetType(),

                        }).ToList()
                    }).ToList()
                });
            }

            return Result.Ok(classList);
        }
    }
}
