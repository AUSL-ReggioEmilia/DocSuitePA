using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class DocumentExtension
    {
        public static async Task<Document> GetDocumentAsync(this Document doc, Project prj, string className, Languages language = Languages.cs)
        {
            return await Task.Run(() => prj.Documents
                .Where(x => x.Name.Equals(string.Concat(className, ".", language.ToString()), StringComparison.InvariantCultureIgnoreCase))
                .Single());
        }

        public static async Task<CompilationUnitSyntax> GetCompilationUnitAsync(this Document doc)
        {
            SyntaxNode node = await doc.GetSyntaxRootAsync();
            return (CompilationUnitSyntax)node;
        }

        public static async Task<SyntaxNode> GetNodeAsync(this Document doc)
        {
            return await doc.GetSyntaxRootAsync();
        }
    }
}
