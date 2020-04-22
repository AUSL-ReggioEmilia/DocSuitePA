Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web

Partial Class UtltVerify
    Inherits CommonBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        btnFullPathRubrica.Visible = DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled
        InitializeAjaxSettings()
    End Sub

    Private Sub btnFullPathRuoli_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFullPathRuoli.Click
        lblDes1.Visible = False
        lblReport1.Visible = False

        Dim s As String = Facade.RoleFacade.FullIncrementalUtility()
        If Not String.IsNullOrEmpty(s) Then
            lblDes.Text = "Settori modificati:"
            lblReport.Text = s
        Else
            lblReport.Text = "Nessuna Modifica"
        End If
    End Sub

    Private Sub btnFullPathRubrica_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFullPathRubrica.Click
        lblDes1.Visible = False
        lblReport1.Visible = False
        lblDes.Text = "Contatti modificati:"

        Dim contactsModified As IList(Of Contact) = Facade.ContactFacade.FullIncrementalUtility()
        If contactsModified.Count <= 0 Then
            lblReport.Text = "Nessun contatto modificato."
            Exit Sub
        End If

        Dim report As New StringBuilder
        For Each contact As Contact In contactsModified
            If report.Length > 0 Then
                report.Append(WebHelper.Br)
            End If
            report.AppendFormat("{0} ({1})", Replace(contact.Description, "|", " "), contact.Id)
        Next

        lblReport.Text = report.ToString()
    End Sub

    Private Sub btnFullPathCategoria_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnFullPathCategoria.Click
        lblDes1.Visible = True
        lblReport1.Visible = True

        Dim sErr As String = ""
        Dim s As String = ""
        If Facade.CategoryFacade.FullIncrementalUtility(sErr, s) Then
            lblDes.Text = "Classificazioni modificate FullPath:"
            If String.IsNullOrEmpty(s) Then
                s = "Nessuna Modifica"
            End If
            lblReport.Text = s
        Else
            If Not String.IsNullOrEmpty(sErr) Then
                Throw New DocSuiteException("Calcolo FullPath Classificatore", sErr, Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If
        End If

        Dim s1 As String = ""
        If Facade.CategoryFacade.FullCodeUtility(sErr, s1) Then
            lblDes1.Text = "Classificazioni modificate FullCode:"
            If s1 = "" Then s1 = "Nessuna Modifica"
            lblReport1.Text = s1
        Else
            If Not String.IsNullOrEmpty(sErr) Then
                Throw New DocSuiteException("Calcolo FullCode Classificatore", sErr, Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFullPathRuoli, pnlRisultati, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFullPathCategoria, pnlRisultati, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFullPathRubrica, pnlRisultati, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

#End Region

End Class