Imports Telerik.Web.UI
Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods.QueryStringEx

Public Class BaseControl
    Inherits Web.UI.UserControl

    ''' <summary> Estrae la chiave dalla querystring, dal viewstate o dalla pagina precedente. </summary>
    ''' <remarks> Oltre a leggerlo, lo salva in automatico nel viewstate in modo da renderlo disponibile nel cross post back. </remarks>
    Public Function GetKeyValue(Of T, TBasePage As CommonBasePage)(key As String) As T
        If ViewState(key) Is Nothing Then

            If HttpContext.Current.Request.QueryString(key) IsNot Nothing Then
                ViewState(key) = HttpContext.Current.Request.QueryString.GetValue(Of T)(key)
            Else
                If BasePage.PreviousPage IsNot Nothing AndAlso TypeOf BasePage.PreviousPage Is TBasePage Then
                    ViewState(key) = DirectCast(BasePage.PreviousPage, TBasePage).GetKeyValue(Of T, TBasePage)(key)
                End If
            End If
        End If

        Return DirectCast(ViewState(key), T)
    End Function

    ''' <summary> Restituisce la pagina di base in cui è contenuto il controllo </summary>
    Public ReadOnly Property BasePage As CommonBasePage
        Get
            Return DirectCast(Page, CommonBasePage)
        End Get
    End Property

    ''' <summary> Restituisce l'istanza corrente (singleton) del RadAjaxManager nella pagina. </summary>
    Protected ReadOnly Property AjaxManager As RadAjaxManager
        Get
            Return RadAjaxManager.GetCurrent(Page)
        End Get
    End Property

    ''' <summary> Consente accesso diretto alla PreviousPageUrl della PageBase collegata </summary>
    Protected Friend Property PreviousPageUrl As String
        Get
            If BasePage IsNot Nothing Then
                Return BasePage.PreviousPageUrl
            End If
            Return String.Empty
        End Get
        Set(value As String)
            If BasePage IsNot Nothing Then
                BasePage.PreviousPageUrl = value
            End If
        End Set
    End Property

End Class
