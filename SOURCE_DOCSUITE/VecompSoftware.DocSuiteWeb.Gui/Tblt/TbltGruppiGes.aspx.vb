Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.NHibernate.Resolvers.ContractResolver
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class TbltGruppiGes
    Inherits CommonBasePage

#Region " Fields "

    Private _group As SecurityGroups

#End Region

#Region " Properties "

    Private ReadOnly Property IdGroup() As Integer
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("IdGroup", 0)
        End Get
    End Property

    Private Property GroupInstance() As SecurityGroups
        Get
            If _group Is Nothing Then
                _group = Facade.SecurityGroupsFacade.GetById(IdGroup)
            End If
            Return _group
        End Get
        Set(ByVal value As SecurityGroups)
            _group = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasSecurityGroupAdminRight OrElse CommonShared.HasSecurityGroupPowerUserRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)

        If Not IsPostBack Then
            InitializePage()
        End If
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Try
            'Verifica che il nome Settore sia sempre impostato
            Select Case Action
                Case "Add"
                    Dim group As SecurityGroups = New SecurityGroups()
                    group.GroupName = txtName.Text
                    group.HasAllUsers = cbxAllUsers.Checked
                    Facade.SecurityGroupsFacade.Save(group)
                    GroupInstance = group
                Case "Rename"
                    GroupInstance.GroupName = txtName.Text
                    GroupInstance.HasAllUsers = cbxAllUsers.Checked
                    Facade.SecurityGroupsFacade.UpdateOnly(GroupInstance)
                Case ("Delete")
                    Facade.SecurityGroupsFacade.Delete(GroupInstance)
            End Select

            GroupInstance.RegistrationUser = String.Empty
            GroupInstance.LastChangedUser = String.Empty
            AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}|{1}');", Action, GroupInstance.Id))

        Catch ex As DocSuiteException
            AjaxAlert(ex.Descrizione)
            FileLogger.Warn(LoggerName, "Errore cancellazione gruppo.", ex)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()
        InitializeTreeView()
        MasterDocSuite.TitleVisible = False
        trOldName.Visible = False
        Select Case Action
            Case "Add"
                Title = "Gruppi - Aggiungi"
                pnlInserimento.Visible = True
                txtName.Focus()
            Case "Rename"
                Title = "Gruppi - Rinomina"
                pnlInserimento.Visible = True
                trOldName.Visible = True
                cbxAllUsers.Checked = GroupInstance.HasAllUsers
                txtName.Text = GroupInstance.GroupName
                txtOldName.Text = GroupInstance.GroupName
                txtName.Focus()
            Case "Delete"
                Title = "Gruppi - Elimina"
                pnlInserimento.Visible = True
                trAllUsers.Visible = False
                trName.Visible = False
                trOldName.Visible = True
                cbxAllUsers.Checked = GroupInstance.HasAllUsers
                txtOldName.Text = GroupInstance.GroupName
                txtOldName.ReadOnly = True
        End Select
    End Sub

    Private Sub InitializeTreeView()
        Dim node As New RadTreeNode()
        If GroupInstance IsNot Nothing Then
            node.Text = GroupInstance.GroupName
        Else
            node.Text = "Gruppo"
        End If
        node.ImageUrl = "../Comm/images/server16.gif"
        RadTreeViewSelectedGroup.Nodes.Add(node)
    End Sub

#End Region

End Class