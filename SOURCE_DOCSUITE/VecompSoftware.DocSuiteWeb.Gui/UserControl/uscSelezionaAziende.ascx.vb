Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Imports Telerik.Web.UI
Imports System.Text

Public Class uscSelezionaAziende
    Inherits DocSuite2008BaseControl

    Public Event Transferring(sender As Object, ByVal e As ContactEventArgs)
    Public Event NeedFinder(ByVal sender As Object, ByVal e As ContactEventArgs)
    Public Event BindAziende(ByVal sender As Object, ByVal e As ContactEventArgs)
    Public Event CompleteSelectionAziende(ByVal sender As Object, ByVal e As ContactEventArgs)
    Private Const FullSizeOpenWindowScript As String = "return {0}_OpenWindowOLDFullScreen('{1}','{2}',{0}{3});"

    Private _realTransferring As Boolean = True
#Region " Property"
    Private Property AziendeSource As List(Of Contact)
        Get
            If (Not ViewState("SourceAziende") Is Nothing) Then
                Return DirectCast(ViewState("SourceAziende"), List(Of Contact))
            End If
            Return Nothing
        End Get
        Set(value As List(Of Contact))
            ViewState("SourceAziende") = value
        End Set
    End Property

    Private Property AziendeTarget As List(Of Contact)
        Get
            If (Not ViewState("TargetAziende") Is Nothing) Then
                Return DirectCast(ViewState("TargetAziende"), List(Of Contact))
            End If
            Return Nothing
        End Get
        Set(value As List(Of Contact))
            ViewState("TargetAziende") = value
        End Set
    End Property

    Public Property SearchEnabled As Boolean
#End Region

#Region " Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        blockSearch.Visible = SearchEnabled
        btnAddContact.Visible = ProtocolEnv.AVCPAddSelContactEnabled
        If SearchEnabled Then
            btnAddCompanyContact.Visible = Not ProtocolEnv.AVCPAddSelContactEnabled
        End If
        InitializeAjax()

        If Not IsPostBack Then
            txtFilterTags.Text = String.Empty
            sourceAziende.Items.Clear()
        End If
    End Sub

    Public Sub sourceAziende_Transferred(ByVal sender As Object, ByVal e As RadListBoxTransferredEventArgs) Handles sourceAziende.Transferred
        If (AziendeTarget Is Nothing) Then
            AziendeTarget = New List(Of Contact)
        End If
        For Each element As RadListBoxItem In e.Items
            If e.DestinationListBox Is sender Then
                ' Delete
                If AziendeTarget.Where(Function(x) x.Id = element.Value).Count() > 0 Then
                    AziendeTarget.Remove(AziendeTarget.Where(Function(x) x.Id = AziendeSource.Where(Function(el) el.Id = element.Value).FirstOrDefault().Id).FirstOrDefault())
                End If
            Else
                ' Insert
                If AziendeTarget.Where(Function(x) x.Id = element.Value).Count() = 0 Then
                    AziendeTarget.Add(AziendeSource.Where(Function(el) el.Id = element.Value).FirstOrDefault())
                End If
            End If
        Next
        sourceAziende.Sort = RadListBoxSort.Ascending
        sourceAziende.SortItems()
        targetAziende.Sort = RadListBoxSort.Ascending
        targetAziende.SortItems()
    End Sub

    Public Sub btnSave_Click() Handles btnSave.Click
        Dim contactEvent As ContactEventArgs = New ContactEventArgs() With {.ContactSource = AziendeSource, .ContactTarget = AziendeTarget}
        RaiseEvent CompleteSelectionAziende(Me, contactEvent)
    End Sub

    Private Sub AddSelContactCode() Handles btnAddContact.Click
        Dim url As StringBuilder
        Dim callBack As String
        url = New StringBuilder("../UserControl/CommonSelContactRubrica.aspx?")
        url.AppendFormat("ParentID={0}", ID)
        If Not String.IsNullOrEmpty(BasePage.Type) Then
            url.AppendFormat("&Type={0}", BasePage.Type)
            url.AppendFormat("&AVCPBusinessContactEnabled={0}", True)
            url.AppendFormat("&AVCPConfermaSelezioneVisible={0}", False)
        End If
        url.Append("&ShowAll=True")

        callBack = "_CloseFunction"
        AjaxManager.ResponseScripts.Add(String.Format(FullSizeOpenWindowScript, ID, url.ToString(), windowSelContact.ClientID, callBack))
    End Sub

    Private Sub AddCompanyContactCode() Handles btnAddCompanyContact.Click
        Dim url As New StringBuilder("../UserControl/CommonContactGes.aspx?")
        url.AppendFormat("ParentID={0}", ID)
        If ProtocolEnv.AVCPIdBusinessContact > 0 Then
            url.AppendFormat("&idContact={0}", ProtocolEnv.AVCPIdBusinessContact)
        End If
        If Not String.IsNullOrEmpty(BasePage.Type) Then
            url.AppendFormat("&Type={0}", BasePage.Type)
        End If
        url.Append("&Action=Add")
        url.Append("&ActionType=A")
        url.AppendFormat("&AVCPAddSelContactEnabled={0}", ProtocolEnv.AVCPAddSelContactEnabled)
        Dim callBack As String = "_CloseFunction"
        AjaxManager.ResponseScripts.Add(String.Format(FullSizeOpenWindowScript, ID, url.ToString(), windowSelContact.ClientID, callBack))
    End Sub

    Private Sub btnSearch_Click() Handles btnSearch.Click
        If Not String.IsNullOrEmpty(txtFilterTags.Text) AndAlso _
            Not String.IsNullOrWhiteSpace(txtFilterTags.Text) Then
            Dim contactEvent As ContactEventArgs = New ContactEventArgs() With
                                                   {.ContactSource = AziendeSource,
                                                    .ContactTarget = AziendeTarget,
                                                    .Description = txtFilterTags.Text
                                                   }
            RaiseEvent NeedFinder(Me, contactEvent)
        End If
    End Sub

    Private Sub sourceAziende_Transferring(sender As Object, e As Telerik.Web.UI.RadListBoxTransferringEventArgs) Handles sourceAziende.Transferring
        If (_realTransferring) Then
            RaiseEvent Transferring(Me, Nothing)
        End If

    End Sub

    ''' <summary>
    ''' Metodo che riceve gli argomenti dalla finestra modale di contatti.
    ''' Negli argomenti è presente l'idContact.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub uscContattiGes_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|")
        If (arguments.Length = 3) AndAlso (AziendeTarget IsNot Nothing AndAlso AziendeSource IsNot Nothing) Then

            Dim idContact As Integer = Convert.ToInt32(arguments(0))
            If String.IsNullOrEmpty(arguments(0)) Then
                Exit Sub
            End If
            Dim contact As Contact = Facade.ContactFacade.GetById(idContact, False)
            If Not AziendeSource.Where(Function(x) x.Id = idContact).Any() Then
                AziendeSource.Add(contact)
            End If

            If Not AziendeTarget.Where(Function(x) x.Id = idContact).Any() Then
                AziendeTarget.Add(AziendeSource.Where(Function(el) el.Id = idContact).FirstOrDefault())
            End If
            LoadListBoxContent()

        End If
    End Sub

#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscContattiGes_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlAziende, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    ''' <summary>
    ''' Seleziono il contatto sotto il quale sono presenti tutti i contatti delle aziende di AVCP.
    ''' Popolo la lista di contatti da inserire nella pagina
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadAziende()
        Dim contactEvent As ContactEventArgs = New ContactEventArgs() With {.ContactSource = AziendeSource, .ContactTarget = AziendeTarget}
        RaiseEvent BindAziende(Me, contactEvent)
        AziendeSource = contactEvent.ContactSource
        AziendeTarget = contactEvent.ContactTarget
    End Sub

    Public Sub ForceLoadingSource(ByVal Source As ICollection(Of Contact), ByVal Target As ICollection(Of Contact))
        AziendeSource = Source
        AziendeTarget = Target
        _realTransferring = False
        LoadListBoxContent()
        _realTransferring = True
    End Sub

    Public Function GetAziendeTarget() As List(Of Contact)
        Return AziendeTarget
    End Function

    Private Sub LoadListBoxContent()
        If (Not AziendeSource Is Nothing AndAlso AziendeSource.Count > 0) Then
            If ProtocolEnv.ShowAVCPFiscalCodeCompany Then
                AziendeSource = AziendeSource.OrderBy(Function(x) x.FullDescriptionWithFiscalCode).ToList()
            Else
                AziendeSource = AziendeSource.OrderBy(Function(x) x.FullDescription).ToList()
            End If
            sourceAziende.DataSource = AziendeSource
            sourceAziende.DataValueField = "Id"
            If ProtocolEnv.ShowAVCPFiscalCodeCompany Then
                sourceAziende.DataTextField = "FullDescriptionWithFiscalCode"
            Else
                sourceAziende.DataTextField = "FullDescription"
            End If
            sourceAziende.DataBind()
            targetAziende.Items.Clear()
            ' sposto le aziende già selezionate come target
            If Not AziendeTarget Is Nothing AndAlso AziendeTarget.Count > 0 Then
                If ProtocolEnv.ShowAVCPFiscalCodeCompany Then
                    AziendeTarget = AziendeTarget.OrderBy(Function(x) x.FullDescriptionWithFiscalCode).ToList()
                Else
                    AziendeTarget = AziendeTarget.OrderBy(Function(x) x.FullDescription).ToList()
                End If
                For Each azienda As Contact In AziendeTarget
                    Dim rli As RadListBoxItem = sourceAziende.FindItemByValue(azienda.Id)
                    If (Not rli Is Nothing) Then
                        sourceAziende.Transfer(rli, sourceAziende, targetAziende)
                    End If
                Next
            End If
        End If

    End Sub

#End Region

End Class