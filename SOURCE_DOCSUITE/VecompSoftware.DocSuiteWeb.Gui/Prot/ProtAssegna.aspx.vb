Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ProtAssegna
    Inherits ProtBasePage

#Region " Fields "

    Private _selectedProtocolKeys As List(Of Guid) = Nothing
    Private _protocolKeys As List(Of Guid) = Nothing
    Private _listProtocol As List(Of Protocol) = Nothing


#End Region

#Region " Properties "
    Private ReadOnly Property SelectedProtocolsKeys As List(Of Guid)
        Get
            If _selectedProtocolKeys.IsNullOrEmpty() Then
                If ViewState("selectedKeys") Is Nothing Then
                    _selectedProtocolKeys = HttpContext.Current.Request.QueryString.GetValue(Of List(Of Guid))("selectedKeys")
                    ViewState("selectedKeys") = _selectedProtocolKeys
                Else
                    _selectedProtocolKeys = CType(ViewState("selectedKeys"), List(Of Guid))
                End If
            End If
            Return _selectedProtocolKeys
        End Get
    End Property
    Private ReadOnly Property ProtocolsKeys As List(Of Guid)
        Get
            If _protocolKeys.IsNullOrEmpty() Then
                If ViewState("keys") Is Nothing Then
                    _protocolKeys = HttpContext.Current.Request.QueryString.GetValue(Of List(Of Guid))("keys")
                    ViewState("keys") = _protocolKeys
                Else
                    _protocolKeys = CType(ViewState("keys"), List(Of Guid))
                End If
            End If
            Return _protocolKeys
        End Get
    End Property

    Public ReadOnly Property ProtocolsList As List(Of Protocol)
        Get
            If Not SelectedProtocolsKeys.IsNullOrEmpty() AndAlso _listProtocol Is Nothing Then
                _listProtocol = SelectedProtocolsKeys.Select(Function(k) FacadeFactory.Instance.ProtocolFacade.GetById(k)).ToList()
            End If
            If (_listProtocol Is Nothing) Then
                _listProtocol = New List(Of Protocol)()
            End If
            Return _listProtocol
        End Get
    End Property
    Public ReadOnly Property ProtocolStatusConfirm As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.ProtocolStatusConfirmRequest)
        End Get
    End Property
    Private Overloads ReadOnly Property CurrentContainerControl As ContainerControl
        Get
            Return New ContainerControl(cmbProtocolContenitori, rcbContainer)
        End Get
    End Property
#End Region
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            ' Iniazializza gli elementi di pagina
            Initialize()
        End If
        InitializeAjax()
    End Sub

#Region " Events "

    ''' <summary> Inizializza gli oggetti della pagina </summary>
    Private Sub Initialize()
        For Each prot As Protocol In ProtocolsList
            Dim node As RadTreeNode = New RadTreeNode With {
                .Text = String.Format("{0} del {1:dd/MM/yyyy} ({2})", prot.FullNumber, prot.RegistrationDate.ToLocalTime(), prot.ProtocolObject)
            }
            node.Font.Bold = True
            node.ImageUrl = "~/Comm/Images/DocSuite/Protocollo16.png"
            rtvProtocol.Nodes.Add(node)
        Next
        btnConfermaAssegna.Enabled = ProtocolsList.Count > 0
        If Not btnConfermaAssegna.Enabled Then
            btnConfermaAssegna.ToolTip = "E necessario selezionare almeno un protocollo prima di proseguire"
        End If
        'status        
        cmbProtocolStatus.DataSource = Facade.ProtocolStatusFacade.GetByDescription("In lavorazione")
        cmbProtocolStatus.DataBind()
        PopulateContainer()
    End Sub
    Private Sub PopulateContainer()
        Dim availableContainers As List(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True).ToList()

        If availableContainers IsNot Nothing AndAlso availableContainers.Count > 0 Then
            ' Visualizzo il risultato della ricerca
            CurrentContainerControl.ClearItems()
            CurrentContainerControl.AddItem(String.Empty, String.Empty)
            For Each container As Container In availableContainers
                CurrentContainerControl.AddItem(container.Name, container.Id.ToString())
            Next
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaAssegna, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub BtnConfermaAssegnaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaAssegna.Click
        If Not ProtocolsList.Any() Then
            Exit Sub
        End If

        If uscSubject.GetContactText.IsNullOrEmpty Then
            AjaxAlert("Campo Assegnatario obbligatorio")
            Exit Sub
        End If

        Dim idContainer As Integer
        For Each prot As Protocol In ProtocolsList
            Dim protocolRights As ProtocolRights = New ProtocolRights(prot)
            If protocolRights.IsEditable Then
                Dim changedContainer As Boolean = False
                prot.Subject = uscSubject.GetContactText()
                prot.Status = Facade.ProtocolStatusFacade.GetById(cmbProtocolStatus.SelectedValue)
                If Not String.IsNullOrEmpty(CurrentContainerControl.SelectedValue) AndAlso Integer.TryParse(CurrentContainerControl.SelectedValue, idContainer) AndAlso prot.Container.Id <> idContainer Then
                    prot.Container = Facade.ContainerFacade.GetById(idContainer)
                    changedContainer = True
                End If

                Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Assegn/Propon. (old): {prot.Subject}")
                Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Assegn/Propon. (new): {uscSubject.GetContactText()}")
                If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
                    Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Stato del protocollo (old): {prot.Status.Id}")
                    Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Stato del protocollo (new): {cmbProtocolStatus.SelectedValue}")
                End If
                If changedContainer Then
                    Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Contenitore del protocollo (old): {prot.Container.Name}")
                    Facade.ProtocolLogFacade.Insert(prot, ProtocolLogEvent.PM, $"Contenitore del protocollo (new): {CurrentContainerControl.SelectedText}")
                End If
                Facade.ProtocolFacade.Update(prot)
            End If
        Next
        Dim result As List(Of Guid) = ProtocolsKeys.Except(SelectedProtocolsKeys).ToList()
        Dim redirectUrl As String = "~/viewers/ProtocolViewer.aspx?multiple=true&keys={0}"
        If result.Any() Then
            Dim serialized As String = JsonConvert.SerializeObject(ProtocolsKeys.Except(SelectedProtocolsKeys))
            Dim encoded As String = HttpUtility.UrlEncode(serialized)
            redirectUrl = String.Format(redirectUrl, encoded)
        Else
            redirectUrl = String.Concat("~/User/UserScrivaniaD.aspx?", CommonShared.AppendSecurityCheck("Type=Prot&Title=Assegnato&Action=PV"))
        End If

        Response.Redirect(redirectUrl)
    End Sub
#End Region

#Region " Methods "


#End Region

End Class

