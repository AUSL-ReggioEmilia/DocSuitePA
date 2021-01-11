
Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class WebPublicationFacade
    Inherits BaseProtocolFacade(Of WebPublication, Int32, NHibernateWebPublicationDao)

    ''' <summary> Torna tutte le WebPublication relative all'Atto </summary>
    Public Function GetByResolution(resolution As Resolution) As IList(Of WebPublication)
        Return _dao.GetByResolution(resolution)
    End Function

    ''' <summary> Torna tutte le WebPublication relative all'Atto, escludendo gli stati richiesti </summary>
    Public Function GetByResolution(resolution As Resolution, statusToIgnore As List(Of Integer)) As IList(Of WebPublication)
        Return _dao.GetByResolution(resolution, statusToIgnore)
    End Function

    ''' <summary> Controlla l'esistenza di una webPublication, escludendo gli stati richiesti </summary>
    Public Function Exists(resolution As Resolution, statusToIgnore As List(Of Integer)) As Boolean
        Return _dao.Exists(resolution, statusToIgnore)
    End Function

    ''' <summary> Ritorna l'ultimo numero di pubblicazione revocato relativamente all'atto </summary>
    Function GetLastRevokedNumber(resolution As Resolution) As String
        Return _dao.GetLastRevokedNumber(resolution)
    End Function

    Public Function GetPublishedResolutionsID(startDate As DateTime, endDate As DateTime) As IList(Of Integer)
        Return _dao.GetPublishedResolutionsID(startDate, endDate)
    End Function

    Public Function GetFirstPublishedResolutionID(startDate As DateTime, endDate As DateTime) As Integer
        Return _dao.GetFirstPublishedResolutionID(startDate, endDate)
    End Function

    Public Function GetLastPublishedResolutionID(startDate As DateTime, endDate As DateTime) As Integer
        Return _dao.GetLastPublishedResolutionID(startDate, endDate)
    End Function

    Public Function HasResolutionPublications(resolution As Resolution) As Boolean
        Return _dao.HasResolutionPublications(resolution)
    End Function

    Public Function HasResolutionPrivacyPublications(resolution As Resolution) As Boolean
        Return _dao.HasResolutionPrivacyPublications(resolution)
    End Function

End Class
