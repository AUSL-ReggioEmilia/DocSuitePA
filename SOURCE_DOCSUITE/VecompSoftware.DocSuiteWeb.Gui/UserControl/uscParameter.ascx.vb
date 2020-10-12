Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

Partial Public Class uscParameter
    Inherits DocSuite2008BaseControl

#Region "Fields"

    Private _cont As Container
    Private _parametersList As IList(Of XmlParameter)

#End Region

#Region " Properties "

    Private ReadOnly Property SelectedEnvironment() As EnvironmentDataCode
        Get
            Return DirectCast(System.Enum.Parse(GetType(EnvironmentDataCode), environmentsRadioButtonList.SelectedValue), EnvironmentDataCode)
        End Get
    End Property

    Public Property ViewMode As ParameterViewMode

    Private ReadOnly Property HasParameterMode As Boolean
        Get
            Return ViewMode = ParameterViewMode.ContainerProperties
        End Get
    End Property

    Public Property CurrentContainer As Container
        Get
            Return _cont
        End Get
        Set(ByVal value As Container)
            _cont = value
        End Set
    End Property

    Private ReadOnly Property ParametersList As IList(Of XmlParameter)
        Get
            If _parametersList Is Nothing Then
                _parametersList = LoadCurrentParameters()
            End If
            Return _parametersList
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    ''' <summary> Associa i dati alla griglia. </summary>
    ''' <remarks> Metodo necessario per permettere il grouping. </remarks>
    Protected Sub dgParameterEnv_NeedDataSource(source As Object, e As GridNeedDataSourceEventArgs) Handles dgParameterEnv.NeedDataSource
        dgParameterEnv.DataSource = ParametersList
    End Sub

    Protected Sub EnvironmentSelection(ByVal sender As Object, ByVal e As EventArgs) Handles environmentsRadioButtonList.SelectedIndexChanged
        LoadGroupsAndVersion()
    End Sub

    Protected Sub dgParameterEnv_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles dgParameterEnv.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim itemParameter As XmlParameter = DirectCast(e.Item.DataItem, XmlParameter)

        Dim btnEdit As RadButton = DirectCast(e.Item.FindControl("btnEdit"), RadButton)

        With DirectCast(e.Item.FindControl("lblKey"), Label)
            .Text = itemParameter.Key
            If String.IsNullOrEmpty(itemParameter.AppValue) Then
                .Font.Italic = True
            End If
        End With

        With DirectCast(e.Item.FindControl("lblValue"), Label)
            Dim value As String
            If (itemParameter.AppValue Is Nothing) OrElse (itemParameter.AppValue IsNot Nothing) AndAlso itemParameter.AppValue.Eq(itemParameter.Value) Then
                ' Valore di default
                value = itemParameter.Value
                .ToolTip = "Valore di default"
            Else
                ' Valore impostato nel database
                .Font.Bold = True
                value = itemParameter.AppValue
                .ToolTip = "Valore impostato nel database"
            End If
            .Text = StringHelper.Shortner(HttpUtility.HtmlEncode(value))
        End With

    End Sub

    Protected Sub RefreshClick(sender As Object, e As EventArgs) Handles refresh.Click
        BindParameters()
    End Sub

    Private Sub UtltParameter_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        BindParameters()
    End Sub

    Protected Sub showWrong_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles showWrong.Click
        Dim missingParameters As List(Of String) = Facade.ParameterEnvFacade.GetWrongParameters()

        If missingParameters.Count = 0 Then
            AjaxAlert("I parametri presenti nel db sono corretti.")
            Exit Sub
        End If

        FileLogger.Warn(LoggerName, String.Format("Parametri ridondanti: [{0}]. Considerarne l'eliminazione.", String.Join(","c, missingParameters)))

        rptParams.DataSource = missingParameters
        rptParams.DataBind()
        AjaxManager.ResponseScripts.Add("openModalMessage();")
    End Sub

    Protected Sub dgParameterEnv_Edit(ByVal sender As Object, ByVal e As GridCommandEventArgs) Handles dgParameterEnv.ItemCommand
        If Not e.CommandName.Eq("prova") Then
            Exit Sub
        End If

        Dim gridItem As GridDataItem = DirectCast(e.Item, GridDataItem)
        Dim paramKey As String = DirectCast(gridItem.FindControl("lblKey"), Label).Text
        Dim paramValue As String = DirectCast(gridItem.FindControl("lblValue"), Label).Text
        Dim param As XmlParameter = ParametersList.SingleOrDefault(Function(x) x.Key.Eq(paramKey))

        lblEditKey.Text = paramKey
        lblDefaultValue.Text = If(String.IsNullOrEmpty(param.Value), "Nessuno", param.Value)

        If DocSuiteContext.Current.IsCustomInstance Then
            ' solo se è abilitata la custom instance visualizzo il valore impostato nell'istanza principale
            Dim baseValue As String = Facade.ParameterEnvFacade.GetValue(SelectedEnvironment, paramKey, False)
            lblNote.Text = String.Format("Istanza attiva [{0}] con valore base [{1}]", DocSuiteContext.CustomInstanceName, If(baseValue Is Nothing, "non impostato", baseValue))
        Else
            lblNote.Text = "Nessuna istanza attiva"
        End If

        If HasParameterMode Then
            Dim actualKey As String = GetActualParameterKey(param.Key)
            Dim propValue As Object = Facade.ContainerPropertyFacade.GetProperty(actualKey, CurrentContainer, param.ParameterType.Value)
            If propValue Is Nothing Then
                txtEditValue.EmptyMessage = "Non impostato"
                txtEditValue.Content = ""
            Else
                txtEditValue.Content = propValue.ToString()
            End If
        Else
            Dim value As String = Facade.ParameterEnvFacade.GetValue(SelectedEnvironment, paramKey, DocSuiteContext.Current.IsCustomInstance)
            If value Is Nothing Then
                txtEditValue.EmptyMessage = "Non impostato"
                txtEditValue.Content = ""
            Else
                txtEditValue.Content = value
            End If
        End If


        AjaxManager.ResponseScripts.Add("openEditWindow();")

        FileLogger.Info(LoggerName, String.Format("Utente [{0}] Ambiente [{1}] chiave [{2}]:{3}Parametro visualizzato [{4}].",
                                                  DocSuiteContext.Current.User.FullUserName, SelectedEnvironment, paramKey, Environment.NewLine, txtEditValue.Content))
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        FileLogger.Info(LoggerName, String.Format("Utente [{0}] Ambiente [{1}] chiave [{2}]:{3}Parametro modificato in [{4}].",
                                                  DocSuiteContext.Current.User.FullUserName, SelectedEnvironment, lblEditKey.Text, Environment.NewLine, txtEditValue.Text))

        If HasParameterMode Then
            Dim actualKey As String = lblEditKey.Text

            Dim param As XmlParameter = ParametersList.SingleOrDefault(Function(x) x.Key.Eq(actualKey))
            If param Is Nothing Then
                Throw New DocSuiteException(String.Format("Nessun parametro trovato con il nome {0}", actualKey))
            End If
            If Not param.ParameterType.HasValue Then
                Throw New DocSuiteException("Il parametro selezionato non ha ParameterType valorizzato")
            End If
            Dim parameterType As ContainerPropertyType = param.ParameterType.Value

            If DocSuiteContext.Current.IsCustomInstance Then
                actualKey = String.Concat(DocSuiteContext.CustomInstanceName, lblEditKey.Text)
            End If

            Facade.ContainerPropertyFacade.InsertContainerProperty(actualKey, txtEditValue.Content, CurrentContainer, parameterType)
        Else
            ' imposto nel database
            Facade.ParameterEnvFacade.SetValue(SelectedEnvironment, lblEditKey.Text, txtEditValue.Content, DocSuiteContext.Current.IsCustomInstance)
            ' imposto in memoria
            Facade.ParameterEnvFacade.GetContext(SelectedEnvironment).SetParameter(lblEditKey.Text, txtEditValue.Content)
        End If

        BindParameters()
        AjaxManager.ResponseScripts.Add("closeEditWindow();")
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        FileLogger.Info(LoggerName, String.Format("Utente [{0}] Ambiente [{1}] chiave [{2}]:{3}Parametro cancellato [{4}].",
                                                  DocSuiteContext.Current.User.FullUserName, SelectedEnvironment, lblEditKey.Text, Environment.NewLine, txtEditValue.Text))
        If HasParameterMode Then
            Dim actualKey As String = lblEditKey.Text
            Dim param As XmlParameter = ParametersList.SingleOrDefault(Function(x) x.Key.Eq(actualKey))
            If param Is Nothing Then
                Throw New DocSuiteException(String.Format("Nessun parametro trovato con il nome {0}", actualKey))
            End If
            If Not param.ParameterType.HasValue Then
                Throw New DocSuiteException("Il parametro selezionato non ha ParameterType valorizzato")
            End If
            If DocSuiteContext.Current.IsCustomInstance Then
                actualKey = String.Concat(DocSuiteContext.CustomInstanceName, lblEditKey.Text)
            End If
            Facade.ContainerPropertyFacade.DeleteContainerProperty(actualKey, CurrentContainer)
        Else
            ' imposto nel database
            Facade.ParameterEnvFacade.DeleteValue(SelectedEnvironment, lblEditKey.Text, DocSuiteContext.Current.IsCustomInstance)
            ' imposto in memoria
            Facade.ParameterEnvFacade.GetContext(SelectedEnvironment).SetParameter(lblEditKey.Text, Nothing)
        End If


        BindParameters()
        AjaxManager.ResponseScripts.Add("closeEditWindow();")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()

        AddHandler AjaxManager.AjaxRequest, AddressOf UtltParameter_AjaxRequest

        AjaxManager.AjaxSettings.AddAjaxSetting(environmentsRadioButtonList, groups)
        AjaxManager.AjaxSettings.AddAjaxSetting(environmentsRadioButtonList, versions)
        AjaxManager.AjaxSettings.AddAjaxSetting(environmentsRadioButtonList, environmentsRadioButtonList)
        AjaxManager.AjaxSettings.AddAjaxSetting(environmentsRadioButtonList, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, parameterModal)
        AjaxManager.AjaxSettings.AddAjaxSetting(refresh, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(showWrong, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgParameterEnv, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnClose, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelete, dgParameterEnv)
        AjaxManager.AjaxSettings.AddAjaxSetting(parameterModal, parameterModal)
        AjaxManager.AjaxSettings.AddAjaxSetting(dgParameterEnv, parameterModal)

    End Sub

    ''' <summary>Visualizza messaggio di errore popup in javascript</summary>
    ''' <param name="message">Messaggio d'errore composto</param>
    ''' <param name="args">Array di <see>Object</see> contenente zero o più argomenti da formattare</param>
    Public Sub AjaxAlert(ByVal message As String, ByVal ParamArray args() As Object)
        AjaxAlert(String.Format(message, args), True)
    End Sub


    ''' <summary>Metodo che esegue l'alert</summary>
    ''' <param name="message">messaggio da mandare</param>
    ''' <param name="checkJavascript">Indica se filtrare il messaggio per evitare caratteri che invalidano il javascript</param>
    Private Sub AjaxAlert(ByVal message As String, ByVal checkJavascript As Boolean)
        If checkJavascript Then
            message = StringHelper.ReplaceAlert(message)
        End If

        If AjaxManager IsNot Nothing Then
            AjaxManager.Alert(message)
        End If
    End Sub


    ''' <summary> Inizializza la pagina. </summary>
    Private Sub Initialize()

        If DocSuiteContext.Current.IsCustomInstance Then
            lblInstance.Text = DocSuiteContext.CustomInstanceName
        Else
            lblInstance.Text = "Istanza di default"
        End If
        ' Controlla quali ambienti sono attivi e carico i radioButton richiesti
        If DocSuiteContext.Current.IsDocumentEnabled Then
            environmentsRadioButtonList.Items.Add(New ListItem("Pratiche", System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB)))
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            environmentsRadioButtonList.Items.Add(New ListItem("Protocolli", System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            environmentsRadioButtonList.Items.Add(New ListItem(Facade.TabMasterFacade.TreeViewCaption, System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB)))
        End If
        ' preseleziono il primo pulsante e carico per la prima volta
        Dim element As ListItem = environmentsRadioButtonList.Items.FindByValue(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        If element IsNot Nothing Then
            element.Selected = True
        Else
            environmentsRadioButtonList.SelectedIndex = 0
        End If

        LoadGroupsAndVersion()
        BindParameters()
    End Sub

    ''' <summary> Carica i parametri come da filtri specificati. </summary>
    ''' <remarks>Il nome dell'ambiente deve essere presente nel file di configurazione XML</remarks>
    Private Sub BindParameters()
        dgParameterEnv.DataSource = LoadCurrentParameters()
        dgParameterEnv.DataBind()
    End Sub

    ''' <summary> Ritorna i parametri corrispondenti al filtro attuale. </summary>
    Private Function LoadCurrentParameters() As List(Of XmlParameter)
        Dim paramList As New List(Of XmlParameter)

        Dim xmlEnv As XmlEnvironment = BaseEnvironment.GetXmlEnvironment(SelectedEnvironment)
        ' Filtro tutti i parametri legati all'ambiente selezionato
        Dim results As IEnumerable(Of XmlParameter) = xmlEnv.Parameters().Where(Function(f) f.Container.Equals(HasParameterMode))
        If Not String.IsNullOrEmpty(groups.SelectedValue) Then
            results = results.Where(Function(f) f.Group.Eq(groups.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(versions.SelectedValue) Then
            results = results.Where(Function(f) f.Version.Eq(versions.SelectedValue))
        End If
        If Not String.IsNullOrEmpty(customers.SelectedValue) Then
            results = results.Where(Function(f) f.Customer.Eq(customers.SelectedValue))
        End If
        If includeDescription.Checked OrElse includeKey.Checked OrElse includeNote.Checked Then
            results = results.Where(Function(param) (includeDescription.Checked AndAlso param.Description IsNot Nothing AndAlso param.Description.ContainsIgnoreCase(searchText.Text)) OrElse
                (includeKey.Checked AndAlso param.Key IsNot Nothing AndAlso param.Key.ContainsIgnoreCase(searchText.Text)) OrElse
                (includeNote.Checked AndAlso param.Note IsNot Nothing AndAlso param.Note.ContainsIgnoreCase(searchText.Text)))
        End If

        paramList = results.ToList()

        ' Per ogni parametro trovato imposto il corrispettivo attualmente usato in memoria
        For Each param As XmlParameter In paramList
            If HasParameterMode Then
                Dim actualKey As String = GetActualParameterKey(param.Key)
                Dim propValue As Object = Facade.ContainerPropertyFacade.GetProperty(actualKey, CurrentContainer, param.ParameterType.Value)
                If propValue Is Nothing Then
                    param.AppValue = param.Value
                Else
                    param.AppValue = propValue.ToString()
                End If
            Else
                param.AppValue = Facade.ParameterEnvFacade.GetContext(SelectedEnvironment).GetParameter(param.Key)
            End If

        Next
        Return paramList
    End Function

    Private Sub LoadGroupsAndVersion()
        Dim groupList, versionList, customerList As New List(Of String)
        ' Carico i parametri xml
        Dim xmlEnv As XmlEnvironment = BaseEnvironment.GetXmlEnvironment(SelectedEnvironment)
        For Each parameter As XmlParameter In xmlEnv.Parameters
            If parameter.Container.Equals(HasParameterMode) Then
                If Not String.IsNullOrEmpty(parameter.Group) AndAlso (Not groupList.Contains(parameter.Group)) Then
                    groupList.Add(parameter.Group)
                End If
                If Not String.IsNullOrEmpty(parameter.Version) AndAlso (Not versionList.Contains(parameter.Version)) Then
                    versionList.Add(parameter.Version)
                End If
                If Not String.IsNullOrEmpty(parameter.Customer) AndAlso (Not customerList.Contains(parameter.Customer)) Then
                    customerList.Add(parameter.Customer)
                End If
            End If
        Next
        ' Ordino
        groupList.Sort()
        versionList.Sort(New AlphanumComparator())
        customerList.Sort()

        groupList.Insert(0, "")
        versionList.Insert(0, "")
        customerList.Insert(0, "")

        groups.DataSource = groupList
        groups.DataBind()

        versions.DataSource = versionList
        versions.DataBind()

        customers.DataSource = customerList
        customers.DataBind()

        BindParameters()
    End Sub

    Private Function GetActualParameterKey(paramKeyName As String) As String
        Dim actualKey As String = String.Empty
        If DocSuiteContext.Current.IsCustomInstance Then
            actualKey = DocSuiteContext.CustomInstanceName
        End If
        actualKey = String.Concat(actualKey, paramKeyName)
        Return actualKey
    End Function
#End Region

End Class