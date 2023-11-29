Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web

Public Class ErrorPage
    Inherits Page

    Private Enum ErrorType
        Info
        [Error]
    End Enum

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()
        Dim ex As Exception = Server.GetLastError()
        If ex Is Nothing Then
            ex = CType(Session("BasePageError"), Exception)
        ElseIf TypeOf ex Is HttpUnhandledException AndAlso ex.InnerException IsNot Nothing Then
            ex = ex.InnerException
        End If

        If ex Is Nothing Then
            ShowError(ErrorType.Error, "Errore Generico", "Contattare l'assistenza.", Nothing)
        End If

        Select Case ex.GetType()
            Case GetType(DocSuiteException)
                Dim dEx As DocSuiteException = DirectCast(ex, DocSuiteException)
                ' Gestione redirect se presente
                If dEx.RedirectUriAddress IsNot Nothing Then Page.ClientScript.RegisterStartupScript(Page.GetType(), "PageRedirect", String.Format("DoRedirect({0},""{1}"");", dEx.RedirectUriLatency * 1000, dEx.RedirectUriAddress), True)
                ShowError(ErrorType.Error, dEx.Titolo, dEx.Descrizione, dEx)

            Case GetType(ExpiredSessionStateException)
                ' In caso di Sessione Scaduta
                ShowError(ErrorType.Info, "Sessione Scaduta", "Per proseguire fare click sul bottone 'Aggiorna' nella Barra degli Strumenti", ex)

            Case GetType(InformationException)
                ' In caso di semplice comunicazione all'utente
                Dim dEx As DocSuiteException = DirectCast(ex, DocSuiteException)
                ' Gestione redirect se presente
                If dEx.RedirectUriAddress IsNot Nothing Then Page.ClientScript.RegisterStartupScript(Page.GetType(), "PageRedirect", String.Format("DoRedirect({0},""{1}"");", dEx.RedirectUriLatency * 1000, dEx.RedirectUriAddress), True)
                ShowError(ErrorType.Info, dEx.Titolo, dEx.Descrizione, Nothing)

            Case Else
                ShowError(ErrorType.Error, "Errore Generico", ex.Message, ex)

        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub ShowError(type As ErrorType, titleText As String, description As String, exception As Exception)
        Select Case type
            Case ErrorType.Info
                info.Text = "Informazione"
                image.ImageUrl = ImagePath.BigInfo
            Case ErrorType.Error
                info.Text = "Attenzione"
                image.ImageUrl = ImagePath.BigError
        End Select
        titolo.Text = titleText
        descrizione.Text = description
        ' Visualizzo ulteriori informazioni se sono super admin o se sono in debug
        Dim showStack As Boolean
#If DEBUG Then
        showStack = True
#Else
        If (HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("VecompSoftware.SuperAdminCheck") IsNot Nothing) Then
            showStack = DirectCast(HttpContext.Current.Session("VecompSoftware.SuperAdminCheck"), Boolean)
        End If
#End If
        If showStack AndAlso exception IsNot Nothing Then
            [console].Visible = True
            Dim message As New StringBuilder()
            Dim level As String = ""
            Dim tmpEx As Exception = exception
            While tmpEx IsNot Nothing
                ' Formattazione fatta in fretta e furia
                If message.Length <> 0 Then
                    message.Appendformat("{0}#########{0}", "<br/>")
                End If

                message.AppendFormat("{0}Eccezione [{4}]{1}{0}Message:{1}{0}{2}{1}{0}StackTrace:{1}{0}{3}", Server.HtmlEncode(level), "<br/>", Server.HtmlEncode(tmpEx.Message), Server.HtmlEncode(tmpEx.StackTrace), tmpEx.GetType.Name)
                If tmpEx.Data.Count > 0 Then
                    For Each key As Object In tmpEx.Data.Keys
                        message.Append("<br/>")
                        message.Append(Server.HtmlEncode(String.Format("{0}Data [{1}]: {2}", level, key, tmpEx.Data(key))))
                    Next
                End If

                tmpEx = tmpEx.InnerException
                level += "|"
            End While
            consoleMessage.Text = message.ToString()
        End If

    End Sub

    Protected Sub SetResponseNoCache()
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)
    End Sub

#End Region

End Class