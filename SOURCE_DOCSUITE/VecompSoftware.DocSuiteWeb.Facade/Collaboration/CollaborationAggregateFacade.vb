Imports System.IO
Imports System.Web
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos

Public Class CollaborationAggregateFacade
    Inherits BaseProtocolFacade(Of CollaborationAggregate, Guid, NHibernateCollaborationAggregateDao)


#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary>
    ''' Ricerca tutte le collaborazioni figlie dato un idCollaborazionepadre
    ''' </summary>
    ''' <param name="collaborationFatherId">collaborationFatherId </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCollaborationAggregateById(ByVal collaborationFatherId As Integer) As IList(Of CollaborationAggregate)
        Return _dao.GetCollaborationAggregateById(collaborationFatherId)
    End Function




#End Region

End Class

