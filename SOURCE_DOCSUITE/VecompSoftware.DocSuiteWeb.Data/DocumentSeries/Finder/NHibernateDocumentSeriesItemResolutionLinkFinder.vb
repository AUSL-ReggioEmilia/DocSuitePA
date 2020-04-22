Imports NHibernate.SqlCommand
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.NHibernate

<Serializable(), DataObject()> _
Public Class NHibernateDocumentSeriesItemResolutionLinkFinder
    Inherits NHibernateBaseFinder(Of DocumentSeriesItemLink, DocumentSeriesItemLink)

#Region " Constructors "

    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
    End Sub
#End Region

#Region " Properties "

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property IdMainIn As List(Of Guid)
    Public Property IdAnnexedIn As List(Of Guid)

    Public Property Year As Integer?
    Public Property NumberFrom As Integer?
    Public Property NumberTo As Integer?

    Public Property IdArchive() As Integer?
    Public Property IdDocumentSeriesIn As List(Of Integer)

    Public Property RegistrationDateFrom As DateTimeOffset?
    Public Property RegistrationDateTo As DateTimeOffset?

    Public Property PublishingDateFrom As DateTime?
    Public Property PublishingDateTo As DateTime?

    Public Property RetireDateFrom As DateTime?
    Public Property RetireDateTo As DateTime?

    Public Property IsPublished As Boolean?
    Public Property IsRetired As Boolean?
    Public Property IsPriority As Boolean?

    Public Property SubjectContains As String
    Public Property SubjectStartsWith As String

    Public Property CategoryPath As String

    Public Property ItemStatusIn As List(Of DocumentSeriesItemStatus)

    Public Property IdSubsectionIn As List(Of Integer)
    Public Property IdOwnerRoleIn As List(Of Integer)

    Public Property LogDate As Range(Of DateTime)

#End Region

#Region " Methods "

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.CountDistinct("DSI_L.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of DocumentSeriesItemLink)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        Return criteria.List(Of DocumentSeriesItemLink)()
    End Function

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

    End Sub

    Protected Overrides Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean

        Return MyBase.AttachSortExpressions(criteria)
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of DocumentSeriesItemLink)("DSI_L")
        Return criteria
    End Function

#End Region

End Class
