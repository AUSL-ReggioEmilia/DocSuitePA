using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class ExpressionStatementSyntaxExtensions
    {
        /// <summary>
        /// Espressione nella forma di:
        /// <code>Authorizations = new HashSet<UDS_T_BozzaDiQualita_Authorization>();</code>
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="propertyName"></param>
        /// <param name="hashType"></param>
        /// <param name="typeRelation"></param>
        /// <returns></returns>
        public static async Task<ExpressionStatementSyntax> InitializeObjectInConstructor(this ExpressionStatementSyntax expression, string propertyName, string hashType, string typeRelation)
        {
            return await Task.Run(() =>
            {
                expression = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.ParseExpression(string.Format("{0} = new {1}<{2}>()", propertyName, hashType, typeRelation))
                    );
                /*
                SyntaxFactory.ExpressionStatement(
                       SyntaxFactory.AssignmentExpression(
                           SyntaxKind.SimpleAssignmentExpression,
                           SyntaxFactory.IdentifierName(propertyName),
                           SyntaxFactory.Token(SyntaxKind.EqualsToken),
                           SyntaxFactory.ObjectCreationExpression(
                               SyntaxFactory.Token(SyntaxKind.NewKeyword),
                               SyntaxFactory.GenericName(
                                    SyntaxFactory.ParseToken(hashType),
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                        SyntaxFactory.SeparatedList<TypeSyntax>(new[] { SyntaxFactory.ParseTypeName(typeRelation) }),
                                        SyntaxFactory.Token(SyntaxKind.GreaterThanToken)
                                    )
                                ),
                                SyntaxFactory.ArgumentList(),



                               //SyntaxFactory.ParseTypeName(hashType),
                               //SyntaxFactory.ArgumentList(
                               //    SyntaxFactory.Token(SyntaxKind.LessThanToken),
                               //    SyntaxFactory.SeparatedList<ArgumentSyntax>(new[] { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(typeRelation)) }),
                               //    SyntaxFactory.Token(SyntaxKind.GreaterThanToken)
                               //),
                               SyntaxFactory.InitializerExpression(
                                   SyntaxKind.None,
                                   SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                   SyntaxFactory.SingletonSeparatedList(SyntaxFactory.ParseExpression("")),
                                   SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                                   )
                               )
                           )
                       );
                       */
                return expression;
            });

        }
    }
}
