using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class CompilationUnitSyntaxExtensions
    {
        public static async Task<List<UsingDirectiveSyntax>> GetUsingClassAsync(this CompilationUnitSyntax classSyntax)
        {
            return await Task.Run(() => classSyntax.Usings.ToList());
        }

        public static async Task<List<NamespaceDeclarationSyntax>> GetNamespacesFromClassAsync(this CompilationUnitSyntax classSyntax)
        {
            return await Task.Run(() => classSyntax.Members.OfType<NamespaceDeclarationSyntax>().ToList());
        }
        public static async Task<string> GetClassNameAsync(this CompilationUnitSyntax classe)
        {
            return await Task.Run(() => classe.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single().Identifier.ToString());
        }

        /// <summary>
        /// CompilationUnit
        /// ->Namespace
        /// -->Class
        /// --->Property
        /// </summary>
        /// <param name="unitSyntax"></param>
        /// <returns></returns>
        public static async Task<List<PropertyDeclarationSyntax>> GetPropertiesAsync(this CompilationUnitSyntax unitSyntax)
        {
            return await Task.Run(() =>
            {
                return unitSyntax.Members
                .OfType<NamespaceDeclarationSyntax>()
                .SingleOrDefault().ChildNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .SingleOrDefault().ChildNodes()
                        .OfType<PropertyDeclarationSyntax>()
                        .ToList();
            });
        }
    }
}
