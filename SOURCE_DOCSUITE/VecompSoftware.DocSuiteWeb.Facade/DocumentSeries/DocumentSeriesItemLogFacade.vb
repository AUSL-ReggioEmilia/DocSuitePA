Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models
Imports System.Linq
Imports VecompSoftware.Helpers.Signer.Security

<Serializable>
Public Class DocumentSeriesItemLogFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesItemLog, Integer, NHibernateDocumentSeriesItemLogDao)

#Region " Fields "
    Private _hashHelper As HashGenerator
#End Region

#Region " Properties "
    Private ReadOnly Property HashHelper As Helpers.Signer.Security.HashGenerator
        Get
            If _hashHelper Is Nothing Then
                _hashHelper = New Helpers.Signer.Security.HashGenerator()
            End If
            Return _hashHelper
        End Get
    End Property
#End Region

    Public Function GetByItem(item As DocumentSeriesItem) As IList(Of DocumentSeriesItemLog)
        Return _dao.GetByItem(item)
    End Function

    Public Function GetByItemAndLogType(item As DocumentSeriesItem, t As DocumentSeriesItemLogType) As IList(Of DocumentSeriesItemLog)
        Return _dao.GetByItemAndLogType(item, t)
    End Function

    Public Function AddLog(Id As Integer, logType As DocumentSeriesItemLogType, text As String, Optional needTransaction As Boolean = True) As DocumentSeriesItemLog
        Dim documentSeries As DocumentSeriesItem = Factory.DocumentSeriesItemFacade.GetById(Id)
        Return AddLog(documentSeries, logType, text, needTransaction)
    End Function


    Public Function AddLog(item As DocumentSeriesItem, logType As DocumentSeriesItemLogType, text As String, Optional needTransaction As Boolean = True) As DocumentSeriesItemLog

        Dim log As New DocumentSeriesItemLog()
        log.DocumentSeriesItem = item
        log.LogType = logType
        log.LogDescription = text
        log.LogDate = _dao.GetServerDate()
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName
        log.Program = DocSuiteContext.Program
        log.UniqueIdDocumentSeriesItem = item.UniqueId

        Save(log, _dbName, needTransaction)

        FileLogger.Debug(LoggerName, String.Format("DocumentSeries [{0}] {1}: {2}", item.Id, logType, text))

        Return log
    End Function

    Public Sub InsertSbSuccesfullSendLog(item As DocumentSeriesItem)
        AddLog(item, DocumentSeriesItemLogType.SC, "Inviato con successo il comando Attestazione di Conformità")
    End Sub

    Public Sub AddInsertedDocumentPrivacyLevelLog(item As DocumentSeriesItem, ByRef docs As IList(Of BiblosDocumentInfo), Optional ByRef chainType As String = Nothing)
        Dim docType As String = If(String.IsNullOrEmpty(chainType), "documento", chainType)

        For Each doc As BiblosDocumentInfo In docs
            If doc.Attributes.Any(Function(f) f.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                AddLog(item, DocumentSeriesItemLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", doc.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), docType, doc.Name, doc.DocumentId))
            End If
        Next
    End Sub

    Public Overrides Sub Save(ByRef obj As DocumentSeriesItemLog, ByVal dbName As String, Optional needTransaction As Boolean = True)
        obj.Hash = HashHelper.GenerateHash(String.Concat(obj.SystemUser, "|", obj.LogType, "|", obj.LogDescription, "|", obj.UniqueId, "|", obj.UniqueIdDocumentSeriesItem, "|", obj.LogDate.ToString("yyyyMMddHHmmss")))
        MyBase.Save(obj, _dbName, needTransaction)
    End Sub

    Public Overrides Sub Save(ByRef obj As DocumentSeriesItemLog)
        obj.Hash = HashHelper.GenerateHash(String.Concat(obj.SystemUser, "|", obj.LogType, "|", obj.LogDescription, "|", obj.UniqueId, "|", obj.UniqueIdDocumentSeriesItem, "|", obj.LogDate.ToString("yyyyMMddHHmmss")))
        MyBase.Save(obj)
    End Sub

    Public Overrides Sub SaveWithoutTransaction(ByRef obj As DocumentSeriesItemLog)
        obj.Hash = HashHelper.GenerateHash(String.Concat(obj.SystemUser, "|", obj.LogType, "|", obj.LogDescription, "|", obj.UniqueId, "|", obj.UniqueIdDocumentSeriesItem, "|", obj.LogDate.ToString("yyyyMMddHHmmss")))
        MyBase.SaveWithoutTransaction(obj)
    End Sub

End Class
