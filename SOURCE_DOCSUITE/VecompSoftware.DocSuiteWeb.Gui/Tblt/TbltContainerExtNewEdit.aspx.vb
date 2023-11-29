Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class TbltContainerExtNewEdit
    Inherits CommonBasePage

#Region " Fields "

    Private _containerExtension As ContainerExtension

#End Region

#Region " Properties "

    ''' <summary> Id del Container corrente. </summary>
    Public ReadOnly Property IdContainer() As Integer
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("idContainer", 0)
        End Get
    End Property

    ''' <summary> Il KeyType corrente. </summary>
    Public ReadOnly Property KeyType() As String
        Get
            Return Request.QueryString("keyType")
        End Get
    End Property

    ''' <summary> Vecchio nome. </summary>
    Public ReadOnly Property OldKey() As String
        Get
            Return Request.QueryString("oldKey")
        End Get
    End Property

    ''' <summary> Numero di documenti richiesti. </summary>
    Public ReadOnly Property OldNumber() As String
        Get
            Return Request.QueryString("oldNumber")
        End Get
    End Property

    ''' <summary> Incremental corrente. </summary>
    Public ReadOnly Property Incremental() As Short
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short)("incremental", 0)
        End Get
    End Property

    ''' <summary> Istanza della ContainerExtension corrente. </summary>
    Public Property ContainerExtensionInstance() As ContainerExtension
        Get
            If _containerExtension Is Nothing Then
                ' chiave di ricerca
                Dim ceck As New ContainerExtensionCompositeKey()
                ceck.idContainer = IdContainer
                ceck.KeyType = KeyType
                ceck.Incremental = Incremental
                If Incremental <> 0 Then
                    _containerExtension = Facade.ContainerExtensionFacade.GetById(ceck, False)
                Else
                    _containerExtension = New ContainerExtension()
                    _containerExtension.Id = ceck
                End If
            End If

            Return _containerExtension
        End Get
        Set(ByVal value As ContainerExtension)
            _containerExtension = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click

        Select Case Action
            Case "Add"
                Dim containerExtension As New ContainerExtension()
                containerExtension.KeyValue = StringHelper.EncodeJS(txtFolderName.Text & "|" & txtDocNumber.Text)

                ContainerExtensionInstance = containerExtension
            Case "Rename"
                ContainerExtensionInstance.KeyValue = StringHelper.EncodeJS(txtNewFolderName.Text & "|" & txtNewDocNumber.Text)
        End Select

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}','{1}');", Action, JsonConvert.SerializeObject(ContainerExtensionInstance)))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Select Case Action
            Case "Add"
                Title = "Pratiche - Inserimento Cartella"
                FolderAdd.Visible = True
                txtDocNumber.Text = "0"
                txtFolderName.Focus()
            Case "Rename"
                Title = "Pratiche - Rinomina Cartella"
                FolderRename.Visible = True
                txtOldFolderName.Text = OldKey
                txtNewFolderName.Text = OldKey
                txtOldDocNumber.Text = If(OldNumber = "", "0", OldNumber)
                txtNewDocNumber.Text = If(OldNumber = "", "0", OldNumber)
                txtNewFolderName.Focus()
        End Select
    End Sub

#End Region

End Class