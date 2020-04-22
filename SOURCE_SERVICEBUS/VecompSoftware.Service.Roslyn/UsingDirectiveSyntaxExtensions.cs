using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class UsingDirectiveSyntaxExtensions
    {
        public static async Task<UsingDirectiveSyntax> CreateUsingAsync(this UsingDirectiveSyntax usingDirective, string baseUsing, string folderUsing)
        {
            return await Task.Run(() =>
            {
                usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(baseUsing), SyntaxFactory.IdentifierName(folderUsing)).NormalizeWhitespace());
                return usingDirective;
            });
        }
    }
}
