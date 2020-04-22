using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.Service.Roslyn
{
    public static class PropertyDeclarationSyntaxExtensions
    {
        public static AccessorDeclarationSyntax get = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        public static AccessorDeclarationSyntax set = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        public static PropertyDeclarationSyntax CreateProperty(this PropertyDeclarationSyntax prop, Type type, string fieldName)
        {
            prop = SyntaxFactory.PropertyDeclaration(prop.PropertyType(type), fieldName)
                .PublicModifier()
                .WithAccessorList(SyntaxFactory.AccessorList(prop.PropertyAccess()));
            return prop;
        }

        public static PropertyDeclarationSyntax CreateNullableProperty(this PropertyDeclarationSyntax prop, Type type, string fieldName)
        {
            prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.NullableType(prop.PropertyType(type)), fieldName)
                .PublicModifier()
                .WithAccessorList(SyntaxFactory.AccessorList(prop.PropertyAccess()));
            return prop;
        }

        public static PropertyDeclarationSyntax CreateProperty(this PropertyDeclarationSyntax prop, string typeName, string fieldName)
        {
            prop = SyntaxFactory.PropertyDeclaration(prop.PropertyType(typeName), fieldName)
                .PublicModifier()
                .WithAccessorList(SyntaxFactory.AccessorList(prop.PropertyAccess()));
            return prop;
        }

        public static SyntaxList<AccessorDeclarationSyntax> PropertyAccess(this PropertyDeclarationSyntax prop)
        {
            return SyntaxFactory.List(new List<AccessorDeclarationSyntax>() { get, set });
        }

        public static TypeSyntax PropertyType(this PropertyDeclarationSyntax prop, Type type)
        {
            return SyntaxFactory.ParseTypeName(type.Name);
        }

        public static TypeSyntax PropertyType(this PropertyDeclarationSyntax prop, string typeName)
        {
            return SyntaxFactory.ParseTypeName(typeName);
        }

        public static PropertyDeclarationSyntax PublicModifier(this PropertyDeclarationSyntax prop)
        {
            return prop.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }

        public static PropertyDeclarationSyntax RemoveTokenFromProperty(this PropertyDeclarationSyntax prop, SyntaxKind kind)
        {
            int index = 0, indexFind = -1;
            prop.Modifiers.ToList().ForEach(token =>
            {
                if (token.Text.Equals(SyntaxFactory.Token(kind).Text))
                {
                    indexFind = index;
                }
                index++;
            });
            if (indexFind >= 0)
            {
                prop = prop.WithModifiers(prop.Modifiers.RemoveAt(indexFind));
            }

            return prop;
        }

        public static PropertyDeclarationSyntax AddTokenFromProperty(this PropertyDeclarationSyntax prop, SyntaxKind kind)
        {
            prop = prop.WithModifiers(prop.Modifiers.Add(SyntaxFactory.Token(kind)));
            return prop;
        }

        public static PropertyDeclarationSyntax AddDataAnnotation(this PropertyDeclarationSyntax prop, string annotationKey, string annotationParamName)
        {
            return prop.WithAttributeLists(
                  SyntaxFactory.SingletonList<AttributeListSyntax>(
                      SyntaxFactory.AttributeList(
                          SyntaxFactory.SeparatedList(new[]
                              {
                                SyntaxFactory.Attribute(SyntaxFactory.ParseName(annotationKey),
                                SyntaxFactory.AttributeArgumentList(
                                        SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                        SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(new[]
                                        {
                                            SyntaxFactory.AttributeArgument(SyntaxFactory.IdentifierName(annotationParamName))
                                        }),
                                        SyntaxFactory.Token(SyntaxKind.CloseParenToken)
                                    )
                                )
                              })
                          )
                      )
              );
        }
        public static PropertyDeclarationSyntax AddSimpleDataAnnotation(this PropertyDeclarationSyntax prop, string annotationKey)
        {
            return prop.WithAttributeLists(
                SyntaxFactory.SingletonList<AttributeListSyntax>(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SeparatedList(new[]
                            {
                                SyntaxFactory.Attribute(SyntaxFactory.ParseName(annotationKey))
                            })
                        )
                    )
            );
        }

    }
}
