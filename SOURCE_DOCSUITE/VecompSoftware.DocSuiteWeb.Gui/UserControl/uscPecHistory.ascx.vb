Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscPecHistory
    Inherits DocSuite2008BaseControl

#Region " Properties "

    Public Property PecHistory() As IList(Of PECMail)

#End Region

#Region " Events "

    Private Sub historyRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles historyRepeater.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim pec As PECMail = DirectCast(e.Item.DataItem, PECMail)

        With DirectCast(e.Item.FindControl("imgMailState"), Image)
            .AlternateText = pec.MailType
            .ImageUrl = GetStateMailIconPath(pec.MailType)
        End With

        With DirectCast(e.Item.FindControl("lblMailType"), Label)
            .Text = pec.MailType
        End With

        With DirectCast(e.Item.FindControl("lblMailDate"), Label)
            .Text = pec.MailDate.ToString()
        End With
    End Sub

#End Region

#Region " Methods "

    Public Sub BindData()
        If PecHistory IsNot Nothing AndAlso PecHistory.Count > 0 Then
            historyRepeater.DataSource = PecHistory
            historyRepeater.DataBind()
            exit sub
        End If

        historyRepeater.Visible = False
        lblMessage.Visible = True
    End Sub

    Private Shared Function GetStateMailIconPath(ByVal mailType As String) As String
        Select Case mailType
            Case "accettazione"
                Return "~/Comm/Images/pec-accettazione.gif"
            Case "avvenuta-consegna"
                Return "~/Comm/Images/pec-avvenuta-consegna.gif"
            Case "non-accettazione"
                Return "~/Comm/Images/pec-non-accettazione.gif"
            Case "preavviso-errore-consegna"
                Return "~/Comm/Images/pec-preavviso-errore-consegna.gif"
            Case "errore-consegna"
                Return "~/Comm/Images/pec-errore-consegna.gif"
            Case Else
                Return "~/Comm/Images/pec-errore-consegna.gif"
        End Select
    End Function

#End Region

End Class