using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public static ClassDeclarationSyntax CreatePublicClass(this ClassDeclarationSyntax cls, string className)
        {
            cls = SyntaxFactory.ClassDeclaration(className).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            return cls;
        }
        public static ClassDeclarationSyntax AddMember(this ClassDeclarationSyntax cls, PropertyDeclarationSyntax member)
        {
            return cls.AddMembers(member);
        }

        public static ClassDeclarationSyntax AddInheritanceClass(this ClassDeclarationSyntax cls, TypeSyntax inheritanceType)
        {
            return cls.AddBaseListTypes(SyntaxFactory.SimpleBaseType(inheritanceType));
        }

        public static ClassDeclarationSyntax AddInheritanceClass(this ClassDeclarationSyntax cls, string inheritanceType)
        {
            return cls.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(inheritanceType)));
        }

        public static ClassDeclarationSyntax AddInheritanceGenericClass(this ClassDeclarationSyntax cls, string inheritanceType, string typeObject)
        {
            return cls.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier(inheritanceType),
                        SyntaxFactory.TypeArgumentList(
                                    SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                    SyntaxFactory.SeparatedList(new[]
                                    {
                                            SyntaxFactory.ParseTypeName(typeObject)
                                    }),
                                    SyntaxFactory.Token(SyntaxKind.GreaterThanToken)
                                    )
                            )
                        )
                    );
        }

        public static ClassDeclarationSyntax AddConstructor(this ClassDeclarationSyntax cls, string constructorClassName,
            List<string> parameterClasses, List<string> argumentClasses)
        {
            SeparatedSyntaxList<ParameterSyntax> parameterListSyntaxes = new SeparatedSyntaxList<ParameterSyntax>();
            SeparatedSyntaxList<ArgumentSyntax> argumentListSyntaxes = new SeparatedSyntaxList<ArgumentSyntax>();
            foreach (string item in parameterClasses)
            {
                parameterListSyntaxes = parameterListSyntaxes.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(item)));
            }

            foreach (string item in argumentClasses)
            {
                argumentListSyntaxes = argumentListSyntaxes.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(item)));
            }

            ConstructorDeclarationSyntax c = SyntaxFactory.ConstructorDeclaration(
                                                SyntaxFactory.SingletonList(default(AttributeListSyntax)),
                                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                                                SyntaxFactory.Identifier(constructorClassName),
                                                SyntaxFactory.ParameterList(SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                                    parameterListSyntaxes,
                                                    SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                                                ),
                                                SyntaxFactory.ConstructorInitializer(
                                                    SyntaxKind.BaseConstructorInitializer,
                                                    SyntaxFactory.Token(SyntaxKind.ColonToken),
                                                    SyntaxFactory.Token(SyntaxKind.BaseKeyword),
                                                    SyntaxFactory.ArgumentList(
                                                        SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                                        argumentListSyntaxes,
                                                        SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                                                    )
                                                ),
                                                SyntaxFactory.Block()
                                            );

            return cls.AddMembers(c);
        }
        public static ClassDeclarationSyntax AddInternalOverrideMethod(this ClassDeclarationSyntax cls, string methodName,
            StatementSyntax bodySyntax, ParameterSyntax[] parameterLists, TypeSyntax returnType)
        {
            MethodDeclarationSyntax methodDeclaration = SyntaxFactory
                .MethodDeclaration(returnType, methodName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                .AddParameterListParameters(parameterLists)
                .WithBody(SyntaxFactory.Block(bodySyntax));

            return cls.AddMembers(methodDeclaration);
        }
        public static async Task<ClassDeclarationSyntax> AddBaseEntityConstructorAsync(this ClassDeclarationSyntax cls, string constructorClassName, ICollection<KeyValuePair<string, string>> relationProperties)
        {
            ICollection<ExpressionStatementSyntax> bodyExpression = new Collection<ExpressionStatementSyntax>();
            ExpressionStatementSyntax expression = null;
            foreach (KeyValuePair<string, string> properties in relationProperties)
            {
                bodyExpression.Add(await expression.InitializeObjectInConstructor(properties.Key, "HashSet", properties.Value));
            }

            ConstructorDeclarationSyntax c = SyntaxFactory.ConstructorDeclaration(
                                                SyntaxFactory.SingletonList(default(AttributeListSyntax)),
                                                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                                                SyntaxFactory.Identifier(constructorClassName),
                                                SyntaxFactory.ParameterList(),
                                                default(ConstructorInitializerSyntax),
                                                //SyntaxFactory.ConstructorInitializer(SyntaxKind.None),
                                                SyntaxFactory.Block(bodyExpression)
                                            );
            return await Task.Run(() => cls.AddMembers(c));
        }
        public static ExpressionStatementSyntax AddRelationPropertiesBlock(string propertyName, string hashType, string typeRelation)
        {
            return SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.ExpressionStatement,
                        SyntaxFactory.IdentifierName(propertyName),
                        SyntaxFactory.Token(SyntaxKind.EqualsToken),
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.ParseTypeName(hashType),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(new[] { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(typeRelation)) }),
                                SyntaxFactory.Token(SyntaxKind.GreaterThanToken)
                            ),
                            SyntaxFactory.InitializerExpression(SyntaxKind.None)
                        )
                    )
                );
        }

        public static ClassDeclarationSyntax AddDataAnnotation(this ClassDeclarationSyntax cls, string annotationKey, string annotationValue, string parameter, string parameterValues)
        {
            return cls.WithAttributeLists(
                SyntaxFactory.SingletonList<AttributeListSyntax>(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SeparatedList(new[]
                            {
                            SyntaxFactory.Attribute(SyntaxFactory.ParseName(annotationKey),
                            SyntaxFactory.AttributeArgumentList(
                                    SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                    SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(new[]
                                    {
                                        SyntaxFactory.AttributeArgument(SyntaxFactory.IdentifierName(annotationValue)),
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(parameter), SyntaxFactory.Token(SyntaxKind.EqualsToken)),
                                            null,
                                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(parameterValues))
                                            )
                                    }),
                                    SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                                )
                            )
                            }
                        )
                    )
                )
            );
        }
    }
}