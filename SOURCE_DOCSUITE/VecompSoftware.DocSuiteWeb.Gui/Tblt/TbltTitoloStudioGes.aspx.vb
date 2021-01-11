Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class TbltTitoloStudioGes
    Inherits CommonBasePage

#Region " Fields "

    Private _contacttitle As ContactTitle

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentContactTitle() As ContactTitle
        Get
            If _contacttitle Is Nothing Then
                Dim idContactTitle As Integer
                If Integer.TryParse(Request.QueryString("idObject"), idContactTitle) Then
                    _contacttitle = Facade.ContactTitleFacade.GetById(idContactTitle)
                Else
                    _contacttitle = New ContactTitle()
                End If
            End If

            Return _contacttitle
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Select Case Action
            Case "Add"

                With CurrentContactTitle
                    .Id = Facade.ContactTitleFacade.GetMaxId() + 1
                    .Code = txtCode.Text
                    .Description = txtObject.Text
                    .IsActive = 1
                    .RegistrationDate = DateTimeOffset.UtcNow
                    .RegistrationUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.ContactTitleFacade.Save(CurrentContactTitle)

            Case "Rename"

                With CurrentContactTitle
                    .Code = txtNewCode.Text
                    .Description = txtNewObject.Text
                    .LastChangedDate = DateTimeOffset.UtcNow
                    .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.ContactTitleFacade.Update(CurrentContactTitle)

            Case "Delete"
                Facade.ContactTitleFacade.Delete(CurrentContactTitle)

            Case "Recovery"
                With CurrentContactTitle
                    .Code = txtCode.Text
                    .Description = txtObject.Text
                    .IsActive = 1
                    .LastChangedDate = DateTimeOffset.UtcNow
                    .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                End With

                Facade.ContactTitleFacade.Update(CurrentContactTitle)
        End Select

        AjaxManager.ResponseScripts.Add("CloseWindow();")

    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Select Case Action
            Case "Add"
                Title = "Titolo Studio - Aggiungi Descrizione"
                tblAdd.Visible = True
                tblRename.Visible = False
                txtCode.Focus()
            Case "Rename"
                Title = "Titolo Studio - Modifica Descrizione"
                tblAdd.Visible = False
                txtOldObject.Text = CurrentContactTitle.Description
                txtOldCode.Text = CurrentContactTitle.Code
                txtNewObject.Text = CurrentContactTitle.Description
                txtNewCode.Text = CurrentContactTitle.Code
                tblRename.Visible = True
                txtNewCode.Focus()
            Case "Delete"
                Title = "Titolo Studio - Elimina Descrizione"
                txtCode.Enabled = False
                txtObject.Enabled = False
            Case "Recovery"
                Title = "Titolo Studio - Recupera Descrizione"
                txtCode.Text = CurrentContactTitle.Code
                txtObject.Text = CurrentContactTitle.Description
                txtCode.Enabled = False
                txtObject.Enabled = False
        End Select

        Dim tn As RadTreeNode
        RadTreeView1.Nodes.Clear()
        tn = New RadTreeNode
        If CurrentContactTitle.Description <> String.Empty Then
            tn.Text = CurrentContactTitle.Description
        Else
            tn.Text = "Titolo Studio"
        End If
        tn.ImageUrl = "../Comm/images/Oggetto.gif"
        tn.Expanded = True
        RadTreeView1.Nodes.Add(tn)
    End Sub

#End Region

End Class