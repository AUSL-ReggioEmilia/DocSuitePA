Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class TbltServiceCategoryGes
    Inherits CommonBasePage

#Region " Fields "

    Private _serviceCategory As ServiceCategory

#End Region

#Region " Properties "

    Public ReadOnly Property IdServiceCategory() As Integer
        Get
            Return Request.QueryString.GetValueOrDefault("idServiceCategory", -1)
        End Get
    End Property

    Public ReadOnly Property ServiceCategoryInstance() As ServiceCategory
        Get
            If _serviceCategory Is Nothing Then
                Dim serviceCategoryFacade As New ServiceCategoryFacade()
                If IdServiceCategory <> -1 Then
                    _serviceCategory = serviceCategoryFacade.GetById(IdServiceCategory, False, DbName)
                Else
                    _serviceCategory = New ServiceCategory()
                End If
            End If

            Return _serviceCategory
        End Get
    End Property

    Public ReadOnly Property DbName() As String
        Get
            Select Case Request.QueryString("DBName")
                Case "DocmDB"
                    Return "DocmDB"
                Case "ReslDB"
                    Return "ReslDB"
                Case Else
                    Return "ProtDB"
            End Select
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        MasterDocSuite.TitleVisible = False
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Dim serviceCategoryFacade As New ServiceCategoryFacade()

        Select Case Action
            Case "Add"
                ServiceCategoryInstance.Description = txtServiceCategory.Text
                ServiceCategoryInstance.Code = txtCode.Text
                serviceCategoryFacade.Save(ServiceCategoryInstance, DBName)
            Case "Rename"
                ServiceCategoryInstance.Description = txtNewServiceCategory.Text
                ServiceCategoryInstance.Code = txtNewCode.Text
                serviceCategoryFacade.Update(ServiceCategoryInstance, DBName)
            Case "Delete"
                serviceCategoryFacade.Delete(ServiceCategoryInstance, DBName)
        End Select
        AjaxManager.ResponseScripts.Add("return CloseWindow('" & Action & "','" & ServiceCategoryInstance.Description & "','" & ServiceCategoryInstance.Code & "','" & ServiceCategoryInstance.Id.ToString() & "');")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()

        InitializeTreeView()

        Select Case Action
            Case "Add"
                Me.Title = "Categoria Servizio - Aggiungi Descrizione"
                pnlInserimento.Visible = True
                rfvServiceCategory.Enabled = True
                pnlRinomina.Visible = False
                txtCode.Focus()
            Case "Rename"
                Me.Title = "Categoria Servizio - Modifica Descrizione"
                pnlInserimento.Visible = False
                rfvServiceCategory.Enabled = False
                pnlRinomina.Visible = True
                txtOldServiceCategory.Text = ServiceCategoryInstance.Description
                txtOldServiceCategory.Enabled = False
                txtOldCode.Text = ServiceCategoryInstance.Code
                txtOldCode.Enabled = False
                lblNewServiceCategory.Text = "Nuova Descrizione:"
                txtNewServiceCategory.Text = ServiceCategoryInstance.Description
                txtNewCode.Text = ServiceCategoryInstance.Code
                lblNewCode.Text = "Nuovo Codice:"
                txtNewCode.Focus()
            Case "Delete"
                Me.Title = "Categoria Servizio - Elimina Descrizione"
                pnlInserimento.Visible = True
                rfvServiceCategory.Enabled = False
                pnlRinomina.Visible = False
                txtServiceCategory.Text = ServiceCategoryInstance.Description
                txtCode.Text = ServiceCategoryInstance.Code
                txtCode.Enabled = False
                txtServiceCategory.Enabled = False
        End Select
    End Sub

    Private Sub InitializeTreeView()
        Dim node As New RadTreeNode()
        If Not String.IsNullOrEmpty(ServiceCategoryInstance.Code) Then
            node.ImageUrl = "../Comm/images/Oggetto.gif"
            node.Text = ServiceCategoryInstance.Description
        Else
            node.ImageUrl = "../Comm/images/Oggetto.gif"
            node.Text = "Categoria Servizio"
        End If
        RadTreeViewSelectedServiceCategory.Nodes.Add(node)
    End Sub

#End Region

End Class



