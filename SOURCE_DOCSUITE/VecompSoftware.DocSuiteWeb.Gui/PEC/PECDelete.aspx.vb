Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
Imports VecompSoftware.DocSuiteWeb.Entity.Conservations

Public Class PECDelete
    Inherits PECBasePage

#Region " Fields "

    Private _selectedMails As List(Of PECMail)
    Private _conservationFinder As ConservationFinder

#End Region

#Region " Properties "

    ''' <summary> Elenco degli id delle PEC selezionate. </summary>
    Private Property SelectedMails As List(Of PECMail)
        Get
            If _selectedMails Is Nothing Then
                _selectedMails = (From tmpId In hiddenPecId.Value.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries) Select Facade.PECMailFacade.GetById(Integer.Parse(tmpId))).ToList()
            End If
            Return _selectedMails
        End Get
        Set(value As List(Of PECMail))
            _selectedMails = value
            hiddenPecId.Value = String.Join("|"c, (From pec In SelectedMails Select pec.Id).ToArray())
        End Set
    End Property

    Private ReadOnly Property PECMailDeletable As List(Of PECMail)
        Get
            Dim mailsDeletable As List(Of PECMail) = New List(Of PECMail)()
            For Each mail As PECMail In SelectedMails
                Dim right As New PECMailRightsUtil(mail, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId)
                If right.IsDeletable Then
                    mailsDeletable.Add(mail)
                End If
            Next
            Return mailsDeletable
        End Get
    End Property

    Public ReadOnly Property ConservationFinder As ConservationFinder
        Get
            If _conservationFinder Is Nothing Then
                _conservationFinder = New ConservationFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _conservationFinder
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

    Protected Sub PecGrid_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles PecGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim mail As PECMail = DirectCast(e.Item.DataItem, PECMail)

        With DirectCast(e.Item.FindControl("uscPECInfo"), uscPECInfo)
            .PECMail = mail
            .DataBind()
        End With
        With DirectCast(e.Item.FindControl("imgDeletable"), Image)
            .ImageUrl = If(New PECMailRightsUtil(mail, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId).IsDeletable, ImagePath.SmallFlagGreen, ImagePath.SmallError)
        End With
    End Sub

    Private Sub cmdOk_Click(sender As Object, e As EventArgs) Handles cmdOk.Click
        If ProtocolEnv.PecRequiredDeleteMessage AndAlso String.IsNullOrEmpty(txtDeleteNotes.Text) Then
            AjaxAlert("È obbligatorio inserire il motivo della cancellazione.")
            Exit Sub
        End If

        If Not PECMailDeletable.Any() Then
            AjaxAlert("Nessun messaggio presente per la cancellazione.")
            Exit Sub
        End If

        For Each mail As PECMail In PECMailDeletable
            Dim conservation As Conservation = WebAPIImpersonatorFacade.ImpersonateFinder(New ConservationFinder(DocSuiteContext.Current.CurrentTenant),
                    Function(impersonationType, finder)
                        finder.UniqueId = mail.UniqueId
                        finder.EnablePaging = False
                        Return finder.DoSearch().Select(Function(x) x.Entity).FirstOrDefault()
                    End Function)
            If conservation IsNot Nothing AndAlso conservation.Status = ConservationStatus.Conservated Then
                Continue For
            End If

            Facade.PECMailFacade.Delete(mail)
            Facade.PECMailLogFacade.Deleted(mail, txtDeleteNotes.Text)
        Next

        Dim outgoing As Boolean = SelectedMails.First().Direction.Equals(PECMailDirection.Outgoing)
        If outgoing Then
            Response.Redirect(String.Format("PECOutgoingMails.aspx?Type=Pec&ProtocolBox={0}", ProtocolBoxEnabled))
            Return
        End If

        Response.Redirect(String.Format("PECIncomingMails.aspx?Type=Pec&ProtocolBox={0}", ProtocolBoxEnabled))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdOk, PecGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        Title = String.Format("{0} - Conferma cancellazione", PecLabel)
        If (PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is IHavePecMail) Then
            ' Quando la pagina arriva con un crosspostback
            Dim source As IHavePecMail = DirectCast(PreviousPage, IHavePecMail)
            SelectedMails = New List(Of PECMail)(source.PecMails())
        Else
            ' Quando la pagina viene aperta in modale
            MasterDocSuite.TitleVisible = False
            hiddenPecId.Value = Request.QueryString.Item("selectedMails")
        End If

        Dim showWarning As Boolean
        If SelectedMails.Except(PECMailDeletable).Any() Then
            showWarning = True
        End If

        PecGrid.DataSource = PECMailDeletable
        If Not PECMailDeletable.Any() Then
            ErrLabel.Text = "Nessun messaggio disponibile per la cancellazione."
            ErrLabel.Visible = True
            cmdOk.Enabled = False
        End If
        If showWarning Then
            AjaxManager.ResponseScripts.Add("openWarningWindow();")
        End If
    End Sub
#End Region

End Class