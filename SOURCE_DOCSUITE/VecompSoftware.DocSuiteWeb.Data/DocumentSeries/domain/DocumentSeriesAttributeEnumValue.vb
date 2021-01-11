Imports System.ComponentModel

<Serializable()> _
Public Class DocumentSeriesAttributeEnumValue
    Inherits DomainObject(Of Int32)

    Public Overridable Property Attribute As DocumentSeriesAttributeEnum

    Public Overridable Property AttributeValue As Integer

    Public Overridable Property Description As String

End Class
