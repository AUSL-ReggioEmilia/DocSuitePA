Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class PECMoveMails
    Inherits PECBasePage

#Region " Fields "

    Private _mailboxes As List(Of PECMailBox)

#End Region

#Region " Properties "

    Public ReadOnly Property Mailboxes As List(Of PECMailBox)
        Get
            If _mailboxes Is Nothing Then
                If ProtocolBoxEnabled Then
                    _mailboxes = Facade.PECMailboxFacade.GetHumanManageable().GetVisibleProtocolMailBoxes()
                Else
                    _mailboxes = Facade.PECMailboxFacade.GetHumanManageable().GetMoveMailBoxes(CurrentPecMailList)
                End If
            End If

            Return _mailboxes
        End Get
    End Property
    
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
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
    End Sub

    Protected Sub BtnMoveClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnMove.Click
        If ProtocolEnv.PecRequiredMoveMessage AndAlso String.IsNullOrEmpty(txtDescription.Text) Then
            AjaxAlert("E' obbligatorio inserire il motivo dello spostamento.")
            Exit Sub
        End If
        Dim pecMailBoxId As Short
        If Not Short.TryParse(ddlMailbox.SelectedValue, pecMailBoxId) Then
            AjaxAlert("E' obbligatorio selezionare la casella di destinazione.")
            Exit Sub
        End If
        ' Sposto gli elementi selezionati nella nuova casella di posta
        Facade.PECMailFacade.MoveToNewInBoxWithNotification(pecMailBoxId, CurrentPecMailList, txtDescription.Text)

        Me.RedirectOnConfirm()
    End Sub

#End Region

#Region " Methods "
    Private Function GetPECHandleQueryStringPart() As String
        If DocSuiteContext.Current.ProtocolEnv.PECUnhandleOnMove Then
            Return "&setHandler=True"
        End If
        Return "&setHandler=False"
    End Function


    Private Function GetProtocolBoxQueryStringPart() As String
        If Me.ProtocolBoxEnabled Then
            Return "&ProtocolBox=True"
        End If

        Return String.Empty
    End Function

    Private Sub RedirectToSummary()

        Dim queryString As String = String.Format("Type=Pec&PECId={0}{1}{2}", Me.CurrentPecMailId, Me.GetProtocolBoxQueryStringPart(), Me.GetPECHandleQueryStringPart())

        queryString = CommonShared.AppendSecurityCheck(queryString)
        Me.Response.Redirect("../PEC/PECSummary.aspx?" & queryString)

    End Sub

    Private Sub RedirectToInbox()
        Dim queryString As String = String.Format("Type=Pec", Me.GetProtocolBoxQueryStringPart())
        queryString = CommonShared.AppendSecurityCheck(queryString)
        Me.Response.Redirect("../PEC/PECIncomingMails.aspx?" & queryString)
    End Sub

    Private Sub RedirectOnConfirm()
        Select Case DocSuiteContext.Current.ProtocolEnv.PECMoveRedirectOnConfirm
            Case 1
                If Me.CurrentPecMailIdList.HasSingle() Then
                    Me.RedirectToSummary()
                Else
                    Me.RedirectToInbox()
                End If

            Case 2
                Me.RedirectToInbox()

            Case Else
                Me.Response.Redirect(Me.PreviousPageUrl)
        End Select
    End Sub

    Private Sub Initialize()
        Title = String.Format("{0} - Sposta", PecLabel)

        ddlMailbox.Items.Clear()
        ddlMailbox.Items.Add(String.Empty)
        Dim defIndex As Integer = 0
        For Each mailbox As PECMailBox In MailBoxes
            If Not Facade.PECMailboxFacade.IsRealPecMailBox(mailbox) Then
                Continue For
            End If

            ddlMailbox.Items.Add(New ListItem(Facade.PECMailboxFacade.MailBoxRecipientLabel(mailbox), mailbox.Id.ToString()))
            If mailbox.Id = ProtocolEnv.PECUnmanagedDefaultMove Then
                defIndex = ddlMailbox.Items.Count - 1
            End If
        Next
        ddlMailbox.SelectedIndex = defIndex

        PecGrid.DataSource = CurrentPecMailList
    End Sub

#End Region

End Class