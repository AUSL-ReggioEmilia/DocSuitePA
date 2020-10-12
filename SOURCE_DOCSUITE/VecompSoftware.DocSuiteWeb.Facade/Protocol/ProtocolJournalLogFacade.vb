Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolJournalLogFacade
    Inherits BaseProtocolFacade(Of ProtocolJournalLog, Short, NHibernateProtocolJournalLogDao)

    Public Sub New()
        MyBase.New()
    End Sub


    ''' <summary>
    ''' Recupera la data dell'ultimo registro giornaliero di protocollo creato.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLastLogDate(currentTenantAOOId As Guid) As DateTime?
        Return _dao.GetLastLogDate(currentTenantAOOId)
    End Function

    ''' <summary>
    ''' Elimina l'associazione protocolli/registro giornaliero per una specifica data di registrazione.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearProtocolJournalReferencesByDate(registrationDate As DateTimeOffset, currentTenantAOOId As Guid)
        _dao.ClearProtocolJournalReferencesByDate(registrationDate, currentTenantAOOId)
    End Sub

    ''' <summary>
    ''' Recupera una distinta delle date dei registri giornalieri di protocollo rimasti incompleti.
    ''' </summary>
    ''' <param name="lowerDateLimit">Data di limite inferiore</param>
    ''' <param name="upperDateLimit">Data di limite superiore</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUnfinishedLogDates(lowerDateLimit As Date?, upperDateLimit As Date?, currentTenantAOOId As Guid) As IList(Of DateTime)
        Return _dao.GetUnfinishedLogDates(lowerDateLimit, upperDateLimit, currentTenantAOOId)
    End Function

    ''' <summary>
    ''' Ritorna l'elenco dei registri di protocollo incompleti nel periodo specificato.
    ''' </summary>
    ''' <param name="lowerLimit">Data di limite inferiore</param>
    ''' <param name="upperLimit">Data di limite superiore</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBrokenJournals(lowerLimit As Date?, upperLimit As Date?, currentTenantAOOId As Guid) As IList(Of ProtocolJournalLog)
        Return _dao.GetBrokenJournals(lowerLimit, upperLimit, currentTenantAOOId)
    End Function

End Class
