using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.Service.Roslyn
{
    public class ClassContainer
    {
        public ClassContainer()
        {
            Usings = new Collection<UsingDirectiveSyntax>();
            Properties = new Collection<PropertyDeclarationSyntax>();
        }

        public Project WorkingProject { get; set; }
        public CompilationUnitSyntax UnitSyntax { get; set; }
        public ClassDeclarationSyntax ClassSyntax { get; set; }
        public Document Document { get; set; }
        public NamespaceDeclarationSyntax Namespace { get; set; }
        public ICollection<UsingDirectiveSyntax> Usings { get; set; }
        public ICollection<PropertyDeclarationSyntax> Properties { get; set; }
    }
}
