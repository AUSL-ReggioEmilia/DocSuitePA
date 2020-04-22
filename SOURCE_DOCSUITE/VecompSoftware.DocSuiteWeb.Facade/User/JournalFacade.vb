Imports System
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class JournalFacade

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Recupera tutte le informazioni dai LOG Pratiche per visualizzare le informazioni di diario di un utente
    ''' </summary>
    ''' <param name="pDateFrom">Data da cui reperire le informazioni di log</param>
    ''' <param name="pDateTo">Data fino a cui reperire le informazioni di log</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserDocumentDiary(ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As ICollection(Of UserDiary)
        Dim dao As New NHibernateDocumentDao("DocmDB") ' sessionFactoryName mancante, da verificare. - FG
        Return dao.GetUserDocumentDiary(pDateFrom, pDateTo)
    End Function

    ''' <summary> Recupera tutte le informazioni dai LOG Protocolli per visualizzare le informazioni di diario di un utente </summary>
    ''' <param name="pDateFrom">Data da cui reperire le informazioni di log</param>
    ''' <param name="pDateTo">Data fino a cui reperire le informazioni di log</param>
    Public Function GetUserProtocolDiary(ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As ICollection(Of UserDiary)
        Dim protocolDao As New NHibernateProtocolDao("ProtDB") ' sessionFactoryName mancante, da verificare. - FG
        Return protocolDao.GetUserProtocolDiary(pDateFrom, pDateTo)
    End Function


    ''' <summary>
    ''' Recupera tutte le informazioni dai LOG Atti per visualizzare le informazioni di diario di un utente
    ''' </summary>
    ''' <param name="pDateFrom">Data da cui reperire le informazioni di log</param>
    ''' <param name="pDateTo">Data fino a cui reperire le informazioni di log</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserResolutionDiary(ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime) As ICollection(Of UserDiary)
        Dim dao As New NHibernateResolutionDao("ReslDB") ' sessionFactoryName mancante, da verificare. - FG
        Return dao.GetUserResolutionDiary(pDateFrom, pDateTo)
    End Function

    ''' <summary> Unisce le informazioni da tutti i LOG per visualizzare il diario completo di un utente. </summary>
    ''' <param name="from">Data da cui reperire le informazioni di log</param>
    ''' <param name="to">Data fino a cui reperire le informazioni di log</param>
    Public Function GetUserCommonDiary(ByVal [from] As DateTime, ByVal [to] As DateTime) As ICollection(Of UserDiary)
        Dim documentDao As New NHibernateDocumentDao("DocmDB") ' sessionFactoryName mancante, da verificare. - FG
        Dim protocolDao As New NHibernateProtocolDao("ProtDB") ' sessionFactoryName mancante, da verificare. - FG
        Dim resolutionDao As New NHibernateResolutionDao("ReslDB") ' sessionFactoryName mancante, da verificare. - FG

        Dim UserDiaries As New List(Of UserDiary)

        If DocSuiteContext.Current.IsDocumentEnabled Then
            UserDiaries.AddRange(documentDao.GetUserDocumentDiary([from], [to]))
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            UserDiaries.AddRange(protocolDao.GetUserProtocolDiary([from], [to]))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            UserDiaries.AddRange(resolutionDao.GetUserResolutionDiary([from], [to]))
        End If

        Return UserDiaries
    End Function

End Class