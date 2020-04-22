Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class DocumentSeriesItemDTO(Of T)

    Public Overridable Property Id As Integer
    Public Overridable Property Year As Integer?
    Public Overridable Property Number As Integer?
    Public Overridable Property DocumentSeries As DocumentSeries
    Public Overridable Property DocumentSeriesSubsection As DocumentSeriesSubsection
    Public Overridable Property IdMain As Guid
    Public Overridable Property IdAnnexed As Guid
    Public Overridable Property IdUnpublishedAnnexed As Guid
    Public Overridable Property PublishingDate As DateTime?
    Public Overridable Property RetireDate As DateTime?
    Public Overridable Property Priority As Boolean?
    Public Overridable Property RegistrationUser As String
    Public Overridable Property RegistrationDate As DateTimeOffset?
    Public Overridable Property LastChangedUser As String
    Public Overridable Property LastChangedDate As DateTimeOffset?
    Public Overridable Property Status As DocumentSeriesItemStatus
    Public Overridable Property Category As Category
    Public Overridable Property SubCategory As Category
    Public Overridable Property Subject As String
    Public Overridable Property DocumentSeriesItemRoles As IList(Of DocumentSeriesItemRole)
    Public Overridable Property HasMainDocument As Boolean
    Public Overridable Property ConstraintValue As String

    Public Overridable Property MainDocuments As List(Of T)
    Public Overridable Property Annexed As List(Of T)

    Public Overridable Property UnPublishedAnnexed As List(Of T)
    Public Overridable Property Attributes As Dictionary(Of String, String)

    Public Overridable ReadOnly Property FirstMainDocument As T
        Get
            If Not MainDocuments.IsNullOrEmpty() Then
                Return MainDocuments.First()
            End If
            Return Nothing
        End Get
    End Property

    Public Overridable Property IdDocumentSeries As Integer
    Public Overridable Property IdDocumentSeriesSubsection As Integer?
    Public Overridable Property IdContainer As Integer
    Public Overridable Property ContainerName As String

    Public Overridable Property LocationServer As String
    Public Overridable Property LocationAnnexedServer As String
    Public Overridable Property LocationUnpublishedAnnexedServer As String

    Public Overridable Property IdLocation As Integer
    Public Overridable Property IdLocationAnnexed As Integer

    Public Overridable ReadOnly Property IdentifierString As String
        Get
            Return GetIdentifierString(Status, Year, Number, Id)
        End Get
    End Property

    Public Overridable Property DematerialisationChainId As Guid?

    Public Shared Function GetIdentifierString(status As DocumentSeriesItemStatus, year As Integer?, number As Integer?, id As Integer) As String
        Select Case Status
            Case DocumentSeriesItemStatus.Active
                Return String.Format("{0}/{1:0000000}", Year, Number)
            Case DocumentSeriesItemStatus.Canceled
                If Year.HasValue AndAlso Number.HasValue Then
                    Return String.Format("{0}/{1:0000000}", Year, Number)
                Else
                    Return String.Format("Annullato ({0})", Id)
                End If
            Case DocumentSeriesItemStatus.Draft
                Return String.Format("Bozza N. {0}", Id)
            Case Else
                Return Id.ToString()
        End Select
    End Function
End Class
