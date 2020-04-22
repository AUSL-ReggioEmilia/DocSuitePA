Imports System.ComponentModel

<Serializable()> _
Public Class DocumentSeriesAttributeEnum
    Inherits DomainObject(Of Int32)

    Public Overridable Property DocumentSeries As DocumentSeries

    Public Overridable Property AttributeName As String

    Public Overridable Property EnumType As AttributeEnumTypes

    Public Overridable Property EnumValues As IList(Of DocumentSeriesAttributeEnumValue)

End Class

Public Enum AttributeEnumTypes
    Checkbox = 0
    Combo = 1
    Propose = 2
End Enum
