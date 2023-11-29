Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Class TbltOggettoGes
    Inherits CommonBasePage

#Region " Fields "

    Private _object As CommonObject = Nothing
    Private _dbName As String

#End Region

#Region "Properties"

    Private Property CurrentObject() As CommonObject
        Get
            If _object Is Nothing Then
                Dim objId As Integer = Request.QueryString.GetValueOrDefault(Of Integer)("idObject", -1)
                If objId <> -1 Then
                    _object = Facade.CommonObjectFacade.GetById(objId, False, DbName)
                End If
            End If

            Return _object
        End Get
        Set(value As CommonObject)
            _object = value
        End Set
    End Property

    Public ReadOnly Property DbName() As String
        Get
            If String.IsNullOrEmpty(_dbName) Then
                _dbName = Request.QueryString("DBName")
            End If
            Return _dbName
        End Get
    End Property

#End Region

#Region " Event "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        Select Case Action
            Case "Add"
                Dim obj As New CommonObject() With {.Description = txtObject.Text, .Code = txtCode.Text}
                Facade.CommonObjectFacade.Save(obj, DbName)
                CurrentObject = obj
            Case "Rename"
                CurrentObject.Description = txtNewObject.Text
                CurrentObject.Code = txtNewCode.Text
                Facade.CommonObjectFacade.Update(CurrentObject, DbName)
            Case "Delete"
                Facade.CommonObjectFacade.Delete(CurrentObject, DbName)
        End Select
        Dim script As String = String.Format("return CloseWindow('{0}','{1}','{2}','{3}');", Action, CurrentObject.Description, CurrentObject.Code, CurrentObject.Id.ToString())
        AjaxManager.ResponseScripts.Add(script)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()

        Dim node As New RadTreeNode()
        node.ImageUrl = "../Comm/images/Oggetto.gif"
        node.Text = If(CurrentObject Is Nothing, "Nuovo Oggetto", CurrentObject.Description)
        RadTreeViewSelectedObject.Nodes.Add(node)

        Select Case Action
            Case "Add"
                Me.Title = "Oggetto - Aggiungi Descrizione"
                pnlInserimento.Visible = True
                pnlRinomina.Visible = False
                txtCode.Focus()
            Case "Rename"
                Me.Title = "Oggetto - Modifica Descrizione"
                pnlInserimento.Visible = False
                pnlRinomina.Visible = True
                txtOldObject.Text = CurrentObject.Description
                txtOldObject.Enabled = False
                txtOldCode.Text = CurrentObject.Code
                txtOldCode.Enabled = False
                lblNewObject.Text = "Nuova Descrizione:"
                txtNewObject.Text = CurrentObject.Description
                txtNewCode.Text = CurrentObject.Code
                lblNewCode.Text = "Nuovo Codice:"
                txtNewCode.Focus()
            Case "Delete"
                Me.Title = "Oggetto - Elimina Descrizione"
                pnlInserimento.Visible = True
                pnlRinomina.Visible = False
                rfvObject.Enabled = False
                txtObject.Text = CurrentObject.Description
                txtCode.Text = CurrentObject.Code
                txtCode.Enabled = False
                txtObject.Enabled = False
        End Select
    End Sub

#End Region

End Class



