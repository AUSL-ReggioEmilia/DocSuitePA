
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class TbltTipoDocGes
    Inherits CommonBasePage

#Region " Fields "

    Private doctype As DocumentType
    Protected WithEvents vNumber As RegularExpressionValidator
    Protected WithEvents lblForm As Label

#End Region

#Region " Properties "

    Public ReadOnly Property ObjectInstance() As DocumentType
        Get
            If doctype Is Nothing Then
                If Request.QueryString("idObject") <> "" Then
                    doctype = Facade.DocumentTypeFacade.GetById(Integer.Parse(Request.QueryString("idObject")))
                Else
                    doctype = New DocumentType()
                End If
            End If

            Return doctype
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim isUsed As Boolean

        Select Case Action
            Case "Add"
                With ObjectInstance
                    .Id = Facade.DocumentTypeFacade.GetMaxId() + 1
                    .Code = txtCode.Text
                    .Description = txtObject.Text
                    .IsActive = 1
                    .RegistrationDate = DateTimeOffset.UtcNow
                    .RegistrationUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.DocumentTypeFacade.Save(ObjectInstance)

            Case "Rename"
                With ObjectInstance
                    .Code = txtNewCode.Text
                    .Description = txtNewObject.Text
                    .LastChangedDate = DateTimeOffset.UtcNow
                    .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.DocumentTypeFacade.Update(ObjectInstance)

            Case "Delete"
                isUsed = Facade.DocumentTypeFacade.Delete(ObjectInstance)


            Case "Recovery"
                With ObjectInstance
                    .IsActive = 1
                    .LastChangedDate = DateTimeOffset.UtcNow
                    .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.DocumentTypeFacade.Update(ObjectInstance)

        End Select

        MasterDocSuite.AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}','{1}','{2}','{3}','{4}');", Action, ObjectInstance.Description, ObjectInstance.Code, ObjectInstance.Id.ToString(), isUsed))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Select Case Action
            Case "Add"
                Title = "Tipologia spedizione - Aggiungi Descrizione"
                pnlAggiungi.Visible = True
                pnlRename.Visible = False
                InitializeTvw()
                txtCode.Focus()
            Case "Rename"
                Title = "Tipologia spedizione - Modifica Descrizione"
                pnlAggiungi.Visible = False
                txtOldObject.Text = ObjectInstance.Description
                txtOldCode.Text = ObjectInstance.Code
                txtNewObject.Text = ObjectInstance.Description
                txtNewCode.Text = ObjectInstance.Code
                pnlRename.Visible = True
                InitializeTvw()
                txtNewCode.Focus()
            Case "Delete"
                Title = "Tipologia spedizione - Elimina Descrizione"
                txtCode.Enabled = False
                txtObject.Enabled = False
                InitializeTvw()
            Case "Recovery"
                Title = "Tipologia spedizione - Recupera Descrizione"
                txtCode.Enabled = False
                txtObject.Enabled = False
                InitializeTvw()
        End Select
    End Sub

    Private Sub InitializeTvw()
        RadTreeView1.Nodes.Clear()

        Dim tn As New RadTreeNode
        If Not ObjectInstance Is Nothing Then
            If ObjectInstance.Description Is Nothing Then
                tn.Text = "Tipologia spedizione"
            Else
                tn.Text = ObjectInstance.Description
            End If
        End If
        tn.ImageUrl = "../Comm/images/Oggetto.gif"
        tn.Expanded = True

        RadTreeView1.Nodes.Add(tn)
    End Sub

#End Region

End Class