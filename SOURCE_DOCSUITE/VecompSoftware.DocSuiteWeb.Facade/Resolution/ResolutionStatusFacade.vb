Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports VecompSoftware.Helpers.ExtensionMethods

<DataObject()>
Public Class ResolutionStatusFacade
    Inherits BaseResolutionFacade(Of ResolutionStatus, Short, NHibernateResolutionStatusDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce un elenco di resolutionStatus il cui Id non è compreso nell'insieme passato. </summary>
    ''' <param name="exceptionList">Array di Id che non devono essere restituiti</param>
    Public Function GetByExceptionList(ByVal exceptionList As Short()) As IList(Of ResolutionStatus)
        Return _dao.GetByExceptionList(exceptionList)
    End Function

    ''' <summary> Restituisce un elenco di resolutionStatus solo attiva e ritirata. </summary>
    Public Function GetStatusNotExecutive() As IList(Of ResolutionStatus)
        Return _dao.GetByExceptionList(New Short() {
                                       ResolutionStatusId.Errato,
                                       ResolutionStatusId.Annullato,
                                       ResolutionStatusId.Revocato,
                                       ResolutionStatusId.Rettificato,
                                       ResolutionStatusId.Temporaneo,
                                       ResolutionStatusId.Sospeso
                                       })
    End Function

    ''' <summary> Restituisce l'elenco di resolutionStatus. </summary>
    Public Function GetStatusList() As IList(Of ResolutionStatus)
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") OrElse DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            Return _dao.GetByExceptionList(New Short() {ResolutionStatusId.Errato, ResolutionStatusId.Temporaneo, -6})
        End If
        Return _dao.GetByExceptionList(New Short() {ResolutionStatusId.Errato, ResolutionStatusId.Temporaneo})
    End Function

End Class