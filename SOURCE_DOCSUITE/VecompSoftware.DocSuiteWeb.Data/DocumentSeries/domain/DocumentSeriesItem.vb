Imports System.Linq

<Serializable()> _
Public Class DocumentSeriesItem
    Inherits DomainObject(Of Int32)
    Implements IAuditable

    Public Overridable Property Year As Integer?
    Public Overridable Property Number As Integer?
    Public Overridable Property DocumentSeries As DocumentSeries
    Public Overridable Property DocumentSeriesSubsection As DocumentSeriesSubsection
    Public Overridable Property Location As Location
    Public Overridable Property LocationAnnexed As Location
    Public Overridable Property LocationUnpublishedAnnexed As Location
    Public Overridable Property IdMain As Guid
    Public Overridable Property IdAnnexed As Guid
    Public Overridable Property IdUnpublishedAnnexed As Guid
    Public Overridable Property PublishingDate As DateTime?
    Public Overridable Property RetireDate As DateTime?
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property Status As DocumentSeriesItemStatus
    Public Overridable Property Priority As Boolean?
    Public Overridable Property DematerialisationChainId As Guid?
    Public Overridable Property Subject As String
    Public Overridable Property HasMainDocument As Boolean
    Public Overridable Property ConstraintValue As String
    Public Overridable Property Category As Category
    Public Overridable Property SubCategory As Category
    Public Overridable Property DocumentSeriesItemRoles As IList(Of DocumentSeriesItemRole)
    Public Overridable Property Logs As IList(Of DocumentSeriesItemLog)
    Public Overridable Property DocumentSeriesItemLinks As IList(Of DocumentSeriesItemLink)
    Public Overridable Property Messages As IList(Of DocumentSeriesItemMessage)
    Public Overridable Property ProtocolDocumentSeriesItems As IList(Of ProtocolDocumentSeriesItem)

    Public Overridable Function MessagesCount() As Integer
        Return Messages.Sum(Function(protocolMessage) protocolMessage.Message.Emails.Count())
    End Function

    Public Overridable Function IsPublished() As Boolean
        If (Status <> DocumentSeriesItemStatus.Active) Then
            Return False
        End If
        If (Not DocumentSeries.PublicationEnabled.HasValue OrElse (DocumentSeries.PublicationEnabled.HasValue AndAlso Not DocumentSeries.PublicationEnabled.Value)) Then
            Return True
        End If

        Return PublishingDate.HasValue AndAlso PublishingDate.Value <= Date.Today AndAlso (Not RetireDate.HasValue OrElse RetireDate.Value > Date.Today)
    End Function

#Region " Constructor "

    Public Sub New()
        Messages = New List(Of DocumentSeriesItemMessage)
        DocumentSeriesItemRoles = New List(Of DocumentSeriesItemRole)
        DocumentSeriesItemLinks = New List(Of DocumentSeriesItemLink)
        Logs = New List(Of DocumentSeriesItemLog)
        UniqueId = Guid.NewGuid()
        ProtocolDocumentSeriesItems = New List(Of ProtocolDocumentSeriesItem)
    End Sub

#End Region

End Class