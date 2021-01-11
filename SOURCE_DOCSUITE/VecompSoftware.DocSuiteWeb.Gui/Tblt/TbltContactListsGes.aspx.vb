Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons

Partial Public Class TbltContactListsGes
    Inherits CommonBasePage

#Region " Fields "
    Private _currentContactListFacase As ContactListFacade
    Private _selectedContactList As ContactList
#End Region

#Region " Properties "

    Private ReadOnly Property ContactListId As Guid
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("ContactListId", Nothing)
        End Get
    End Property

    Private ReadOnly Property CurrentContactListFacade As ContactListFacade
        Get
            If _currentContactListFacase Is Nothing Then
                _currentContactListFacase = New ContactListFacade()
            End If
            Return _currentContactListFacase
        End Get
    End Property

    Private ReadOnly Property SelectedContactList As ContactList
        Get
            If _selectedContactList Is Nothing Then
                _selectedContactList = CurrentContactListFacade.GetById(ContactListId)
            End If
            Return _selectedContactList
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)

        If Not IsPostBack Then
            InitializePage()
        End If
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Try
            Select Case Action
                Case "Add"
                    Dim list As ContactList = New ContactList()
                    list.Name = txtName.Text
                    CurrentContactListFacade.Save(list)
                Case "Edit"
                    SelectedContactList.Name = txtName.Text
                    CurrentContactListFacade.Update(SelectedContactList)
                Case "Delete"
                    CurrentContactListFacade.Delete(SelectedContactList)
            End Select

            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", Action))

        Catch ex As DocSuiteException
            AjaxAlert(ex.Descrizione)
            FileLogger.Warn(LoggerName, String.Format("Errore durante l'operazione di '{0}' di liste di contatti.", Action), ex)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()
        MasterDocSuite.TitleVisible = False
        Select Case Action
            Case "Add"
                Title = "Liste di contatti - Aggiungi"
                pnlInserimento.Visible = True
                txtName.Focus()
            Case "Edit"
                Title = "Liste di contatti - Rinomina"
                txtName.Text = SelectedContactList.Name
                txtName.Focus()
            Case "Delete"
                Title = "Liste di contatti - Elimina"
                txtName.Text = SelectedContactList.Name
                txtName.Enabled = False

        End Select
    End Sub

#End Region

End Class