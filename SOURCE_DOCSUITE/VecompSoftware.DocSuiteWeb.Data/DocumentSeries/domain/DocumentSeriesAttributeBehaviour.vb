Imports System.ComponentModel

<Serializable()> _
Public Class DocumentSeriesAttributeBehaviour
    Inherits DomainObject(Of Int32)

    Public Overridable Property DocumentSeries As DocumentSeries
    Public Overridable Property AttributeName As String
    Public Overridable Property AttributeValue As String
    Public Overridable Property AttributeGroup As String
    Public Overridable Property KeepValue As Boolean
    Public Overridable Property Visible As Boolean
    Public Overridable Property ValueType As DocumentSeriesAttributeBehaviourValueType
    Public Overridable Property Action As DocumentSeriesAction


End Class

Public Enum DocumentSeriesAttributeBehaviourValueType
    ConstantValue = 0
    Calculated = 1
    Pattern = 2
End Enum

Public Enum DocumentSeriesAction

    <Description("Inserimento")>
    Insert = 0
    <Description("Modifica")>
    Edit = 1
    <Description("Visualizzazione")>
    View = 2
    <Description("Annullamento")>
    Delete = 3
    <Description("Inserimento")>
    FromResolution = 4
    <Description("Duplicazione")>
    Duplicate = 5
    <Description("Inserimento")>
    FromCollaboration = 6
    <Description("Inserimento")>
    FromResolutionKind = 7
    <Description("Modifica")>
    FromResolutionKindUpdate = 8
    <Description("Visualizza")>
    FromResolutionView = 9
    <Description("Inserimento")>
    FromProtocol = 10

End Enum
