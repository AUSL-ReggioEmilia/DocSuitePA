Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers.Analytics.Models.AdaptiveSearches
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.Helpers.Analytics

<ComponentModel.DataObject()>
Public Class UserLogFacade
    Inherits BaseProtocolFacade(Of UserLog, Integer, NHibernateUserLogDao)

#Region " Fields "
    Public Const PROTOCOL_ADAPTIVE_SEARCH_KEY As String = "Protocol"
    Private _adaptiveSearchAnalysis As AdaptiveSearchAnalysis
#End Region

#Region " Properties "

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByUser(user As String, domain As String) As UserLog
        If user.Contains("\") AndAlso user.StartsWith(domain, StringComparison.InvariantCultureIgnoreCase) Then
            user = user.Split("\"c).Last()
        End If
        If String.IsNullOrEmpty(domain) Then
            domain = CommonShared.UserDomain
        End If
        Return _dao.GetByUser(user, domain)
    End Function

    Public Function EmailOfUser(userName As String, userLogEnabled As Boolean) As String
        If String.IsNullOrEmpty(userName) Then
            Return String.Empty
        End If

        Dim values As String() = userName.Split("\"c)
        Dim domain As String = CommonShared.UserDomain
        If values.Length > 1 Then
            domain = values.First()
            userName = values.Last()
        End If
        Return EmailOfUser(userName, domain, userLogEnabled)
    End Function

    Public Function EmailOfUser(userName As String, domain As String, userLogEnabled As Boolean) As String
        If userLogEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.ForceUserLogEmail Then
            FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("EmailOfUser UserLog [{0}]", userName))
            Dim userLog As UserLog = GetByUser(userName, domain)
            If userLog IsNot Nothing AndAlso Not String.IsNullOrEmpty(userLog.UserMail) Then
                Return userLog.UserMail
            End If
        End If

        FileLogger.Debug(LogName.DirectoryServiceLog, String.Format("EmailOfUser LDAP [{0}, {1}]", userName, domain))
        Dim email As String = CommonAD.LdapUserEmail(userName, domain)
        If Not String.IsNullOrEmpty(email) Then
            Return email
        End If

        If Not userLogEnabled OrElse DocSuiteContext.Current.ProtocolEnv.ForceUserLogEmail OrElse Not DocSuiteContext.Current.ProtocolEnv.UserLogEmail Then
            Return String.Empty
        End If

        Dim ul As UserLog = GetByUser(userName, domain)
        If ul IsNot Nothing AndAlso Not String.IsNullOrEmpty(ul.UserMail) Then
            Return ul.UserMail
        End If

        Return String.Empty
    End Function

    Public Function MobilePhoneOfUser(userName As String, domain As String) As String
        Dim user As UserLog = GetByUser(userName, domain)
        If user Is Nothing Then
            Return String.Empty
        End If

        Return user.MobilePhone
    End Function

    Public Function GetProtocolAdaptiveSearchControls(currentUser As UserLog) As AdaptiveSearchModel
        Dim adaptiveSearchModel As AdaptiveSearchModel = DocSuiteContext.Current.ProtocolEnv.ProtocolDefaultAdaptiveSearch
        If currentUser Is Nothing Then
            Return adaptiveSearchModel
        End If

        Try
            Dim currentDefaultAdaptiveSearchControls As IDictionary(Of String, ICollection(Of String)) = GetDefaultAdaptiveSearchControls(currentUser)
            If currentDefaultAdaptiveSearchControls IsNot Nothing AndAlso currentDefaultAdaptiveSearchControls.ContainsKey(PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                Dim tmpModel As UserSearchModel = New UserSearchModel() With {.SearchControls = currentDefaultAdaptiveSearchControls(PROTOCOL_ADAPTIVE_SEARCH_KEY).ToDictionary(Function(d) d, Function(d) String.Empty)}
                adaptiveSearchModel = TransformToAdaptiveSearchModel(tmpModel)
                For Each defaultControl As KeyValuePair(Of String, String) In DocSuiteContext.Current.ProtocolEnv.ProtocolDefaultAdaptiveSearch.SearchControls
                    If adaptiveSearchModel.SearchControls.ContainsKey(defaultControl.Key) Then
                        adaptiveSearchModel.SearchControls(defaultControl.Key) = defaultControl.Value
                    End If
                Next
            End If

            If Not String.IsNullOrEmpty(currentUser.AdaptiveSearchEvaluated) Then
                Dim evaluatedSearchModels As IDictionary(Of String, UserSearchModel) = JsonConvert.DeserializeObject(Of IDictionary(Of String, UserSearchModel))(currentUser.AdaptiveSearchEvaluated)
                If evaluatedSearchModels.ContainsKey(PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                    Dim adaptiveModel As AdaptiveSearchModel = TransformToAdaptiveSearchModel(evaluatedSearchModels(PROTOCOL_ADAPTIVE_SEARCH_KEY))
                    For Each control As KeyValuePair(Of String, String) In adaptiveModel.SearchControls
                        adaptiveSearchModel.SearchControls.AddSafe(control.Key, control.Value)
                    Next
                End If
            End If
            Return adaptiveSearchModel
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("GetProtocolAdaptiveSearchControls -> {0}", ex.Message), ex)
            Return DocSuiteContext.Current.ProtocolEnv.ProtocolDefaultAdaptiveSearch
        End Try
    End Function

    Public Function GetProtocolUserSearchControls(currentUser As UserLog) As UserSearchModel
        Dim protocolAdaptiveControls As AdaptiveSearchModel = GetProtocolAdaptiveSearchControls(currentUser)
        Return TransformToUserSearchModel(protocolAdaptiveControls)
    End Function

    Public Function GetDefaultAdaptiveSearchControls(currentUser As UserLog) As IDictionary(Of String, ICollection(Of String))
        If currentUser Is Nothing Then
            Return New Dictionary(Of String, ICollection(Of String))()
        End If

        If Not String.IsNullOrEmpty(currentUser.DefaultAdaptiveSearchControls) Then
            Dim model As IDictionary(Of String, ICollection(Of String)) = JsonConvert.DeserializeObject(Of IDictionary(Of String, ICollection(Of String)))(currentUser.DefaultAdaptiveSearchControls)
            Return model
        End If
        Return New Dictionary(Of String, ICollection(Of String))()
    End Function

    'TODO: Questi metodi devono essere eseguti da un servizio esterno in maniera asincrona
    Public Sub EvaluateProtocolSearchStatistics(currentUser As UserLog, affinity As Integer)
        If currentUser Is Nothing Then
            Exit Sub
        End If

        If String.IsNullOrEmpty(currentUser.AdaptiveSearchStatistics) Then
            FileLogger.Debug(LoggerName, String.Format("EvaluateSearchStatistics -> Nessuna statistica trovata per l'utente {0}", DocSuiteContext.Current.User.FullUserName))
            Exit Sub
        End If

        Dim searchStatisticsModel As IDictionary(Of String, ICollection(Of UserSearchStatisticModel)) = JsonConvert.DeserializeObject(Of IDictionary(Of String, ICollection(Of UserSearchStatisticModel)))(currentUser.AdaptiveSearchStatistics)
        If searchStatisticsModel.ContainsKey(PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
            Dim evaluatedModel As UserSearchModel = AdaptiveSearchAnalysis.AnalyzeStatistic(searchStatisticsModel(PROTOCOL_ADAPTIVE_SEARCH_KEY), affinity)
            Dim evaluatedSearchModels As IDictionary(Of String, UserSearchModel) = New Dictionary(Of String, UserSearchModel)

            If Not String.IsNullOrEmpty(currentUser.AdaptiveSearchEvaluated) Then
                evaluatedSearchModels = JsonConvert.DeserializeObject(Of IDictionary(Of String, UserSearchModel))(currentUser.AdaptiveSearchEvaluated)
                If evaluatedSearchModels.ContainsKey(PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                    Dim protocolEvaluatedModel As UserSearchModel = evaluatedSearchModels(PROTOCOL_ADAPTIVE_SEARCH_KEY)
                    If DateDiff(DateInterval.Day, protocolEvaluatedModel.CreatedDate, Date.UtcNow) < 7 Then
                        FileLogger.Debug(LoggerName, String.Format("EvaluateSearchStatistics -> Non sono ancora passati 7 giorni dall'ultima valutazione per l'utente {0}", DocSuiteContext.Current.User.FullUserName))
                        Exit Sub
                    End If
                End If
            End If
            evaluatedSearchModels.AddSafe(PROTOCOL_ADAPTIVE_SEARCH_KEY, evaluatedModel)
            'Sbianco le statistiche
            searchStatisticsModel.AddSafe(PROTOCOL_ADAPTIVE_SEARCH_KEY, New List(Of UserSearchStatisticModel))
            currentUser.AdaptiveSearchEvaluated = JsonConvert.SerializeObject(evaluatedSearchModels)
            currentUser.AdaptiveSearchStatistics = JsonConvert.SerializeObject(searchStatisticsModel)
            Update(currentUser)
        End If
    End Sub

    'TODO: Questi metodi devono essere eseguti da un servizio esterno in maniera asincrona
    Public Sub UpdateProtocolSearchStatistics(toUpdate As IDictionary(Of String, String), currentUser As UserLog)
        Dim currentStatisticModels As ICollection(Of UserSearchStatisticModel) = New List(Of UserSearchStatisticModel)()
        Dim models As IDictionary(Of String, ICollection(Of UserSearchStatisticModel)) = New Dictionary(Of String, ICollection(Of UserSearchStatisticModel))
        If Not String.IsNullOrEmpty(currentUser.AdaptiveSearchStatistics) Then
            models = JsonConvert.DeserializeObject(Of IDictionary(Of String, ICollection(Of UserSearchStatisticModel)))(currentUser.AdaptiveSearchStatistics)
            If models.ContainsKey(PROTOCOL_ADAPTIVE_SEARCH_KEY) Then
                currentStatisticModels = models(PROTOCOL_ADAPTIVE_SEARCH_KEY)
            End If
        End If

        Dim updatedStatisticModels As ICollection(Of UserSearchStatisticModel) = AdaptiveSearchAnalysis.UpdateStatistic(currentStatisticModels, toUpdate)
        models.AddSafe(PROTOCOL_ADAPTIVE_SEARCH_KEY, updatedStatisticModels)
        currentUser.AdaptiveSearchStatistics = JsonConvert.SerializeObject(models)
        Update(currentUser)
    End Sub

    Public Function TransformToAdaptiveSearchModel(userSearchModel As UserSearchModel) As AdaptiveSearchModel
        If userSearchModel Is Nothing Then
            Throw New ArgumentException("Impossibile trasformare UserSearchModel se il modello non è inizializzato")
        End If

        Dim controls As ICollection(Of AdaptiveSearchMappingControl) = DocSuiteContext.Current.ProtocolDefaultAdaptiveSearchConfigurations
        Dim model As AdaptiveSearchModel = New AdaptiveSearchModel()
        model.SearchControls = userSearchModel.SearchControls.Where(Function(x) controls.Any(Function(xx) xx.Id = x.Key)).Select(Function(s) New With {
            Key .Key = controls.First(Function(xx) xx.Id = s.Key).Name,
            Key .Value = s.Value
        }).ToDictionary(Function(d) d.Key, Function(d) d.Value)
        Return model
    End Function

    Public Function TransformToUserSearchModel(adaptiveSearchModel As AdaptiveSearchModel) As UserSearchModel
        If adaptiveSearchModel Is Nothing Then
            Throw New ArgumentException("Impossibile trasformare AdaptiveSearchModel se il modello non è inizializzato")
        End If

        Dim controls As ICollection(Of AdaptiveSearchMappingControl) = DocSuiteContext.Current.ProtocolDefaultAdaptiveSearchConfigurations
        Dim model As UserSearchModel = New UserSearchModel()
        model.CreatedDate = Date.UtcNow.Date
        model.SearchControls = adaptiveSearchModel.SearchControls.Where(Function(x) controls.Any(Function(xx) xx.Name = x.Key)).[Select](Function(s) New With {
            Key .Key = controls.First(Function(ss) ss.Name = s.Key).Id,
            Key .Value = s.Value
        }).ToDictionary(Function(d) d.Key, Function(d) d.Value)
        Return model
    End Function
#End Region

End Class