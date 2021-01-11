Imports System.Net
Imports System.Net.Http
Imports System.Security.Principal
Imports System.Threading
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.WebAPIManager.Dao
Imports VecompSoftware.WebAPIManager.Finder

Public Class WebAPIImpersonatorFacade
#Region " Fields "

#End Region

#Region " Methods "

#Region " Common "
    Private Shared Function BuildShibbolethAuthenticationHandler() As Func(Of ICredential, HttpClientHandler)
        Return Function(credential)
                   Dim handler As HttpClientHandler = New HttpClientHandler() With {
                       .UseDefaultCredentials = True
                   }
                   handler.UseCookies = True
                   handler.CookieContainer = New CookieContainer()
                   Dim rawRequestCookies As String = HttpContext.Current.Request.ServerVariables("HTTP_COOKIE")
                   Dim cookies As String() = rawRequestCookies.Split(";"c)

                   If cookies.Length > 0 Then
                       Dim cookieName, cookieValue As String
                       Dim webApiUrl As Uri = New Uri(DocSuiteContext.Current.CurrentTenant.ODATAUrl)

                       For Each cookie As String In cookies
                           cookieName = cookie.Split("="c)(0).Trim()
                           cookieValue = cookie.Split("="c)(1).Trim()
                           handler.CookieContainer.Add(New Uri($"{webApiUrl.Scheme}://{webApiUrl.Authority}"), New Cookie(cookieName, cookieValue))
                       Next
                   End If

                   Return handler
               End Function
    End Function
#End Region

#Region " WebAPIHelper "
    Public Shared Function ImpersonateSendRequest(Of T)(webAPIHelper As IWebAPIHelper, content As T, customClientConfiguration As IHttpClientConfiguration, Optional originalClientConfiguration As IHttpClientConfiguration = Nothing) As Boolean
        If originalClientConfiguration Is Nothing Then
            originalClientConfiguration = customClientConfiguration
        End If
        Dim shibbolethEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled
        If shibbolethEnabled Then
            Dim func As Func(Of ICredential, HttpClientHandler) = BuildShibbolethAuthenticationHandler()
            Return webAPIHelper.SendRequest(Of T)(customClientConfiguration, originalClientConfiguration, content, Nothing, externalHandlerInitialize:=func)
        Else
            Dim wi As WindowsIdentity = CType(DocSuiteContext.Current.User.Identity, WindowsIdentity)
            Using wic As WindowsImpersonationContext = wi.Impersonate()
                If ExecutionContext.IsFlowSuppressed() Then
                    Return webAPIHelper.SendRequest(Of T)(customClientConfiguration, originalClientConfiguration, content, Nothing, Nothing)
                Else
                    Using ExecutionContext.SuppressFlow()
                        Return webAPIHelper.SendRequest(Of T)(customClientConfiguration, originalClientConfiguration, content, Nothing, Nothing)
                    End Using
                End If
            End Using
        End If
    End Function

    Public Shared Function ImpersonateRawRequest(Of T, TResult)(webAPIHelper As IWebAPIHelper, filter As String, customClientConfiguration As IHttpClientConfiguration, Optional originalClientConfiguration As IHttpClientConfiguration = Nothing) As TResult
        If originalClientConfiguration Is Nothing Then
            originalClientConfiguration = customClientConfiguration
        End If
        Dim shibbolethEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled
        If shibbolethEnabled Then
            Dim func As Func(Of ICredential, HttpClientHandler) = BuildShibbolethAuthenticationHandler()
            Return webAPIHelper.GetRawRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, filter, func)
        Else
            Dim wi As WindowsIdentity = CType(DocSuiteContext.Current.User.Identity, WindowsIdentity)
            Using wic As WindowsImpersonationContext = wi.Impersonate()
                If ExecutionContext.IsFlowSuppressed() Then
                    Return webAPIHelper.GetRawRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, filter)
                Else
                    Using ExecutionContext.SuppressFlow()
                        Return webAPIHelper.GetRawRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, filter)
                    End Using
                End If
            End Using
        End If
    End Function

    Public Shared Function ImpersonateRequest(Of T, TResult)(webAPIHelper As IWebAPIHelper, customClientConfiguration As IHttpClientConfiguration, Optional originalClientConfiguration As IHttpClientConfiguration = Nothing) As TResult
        If originalClientConfiguration Is Nothing Then
            originalClientConfiguration = customClientConfiguration
        End If
        Dim shibbolethEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled
        If shibbolethEnabled Then
            Dim func As Func(Of ICredential, HttpClientHandler) = BuildShibbolethAuthenticationHandler()
            Return webAPIHelper.GetRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, func)
        Else
            Dim wi As WindowsIdentity = CType(DocSuiteContext.Current.User.Identity, WindowsIdentity)
            Using wic As WindowsImpersonationContext = wi.Impersonate()
                If ExecutionContext.IsFlowSuppressed() Then
                    Return webAPIHelper.GetRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, Nothing)
                Else
                    Using ExecutionContext.SuppressFlow()
                        Return webAPIHelper.GetRequest(Of T, TResult)(customClientConfiguration, originalClientConfiguration, Nothing)
                    End Using
                End If
            End Using
        End If
    End Function
#End Region

#Region " Finder "
    Public Shared Sub ImpersonateFinder(Of TWebAPIFinder As IImpersonateWebAPIFinder)(ByVal finder As TWebAPIFinder, ByVal impersonatedAction As Action(Of ImpersonationType, TWebAPIFinder))
        ImpersonateFinder(finder, Function(impersonationType, f)
                                      impersonatedAction(impersonationType, finder)
                                      Return True
                                  End Function)
    End Sub

    Public Shared Function ImpersonateFinder(Of TWebAPIFinder As IImpersonateWebAPIFinder, TResult)(ByVal finder As TWebAPIFinder, ByVal impersonatedAction As Func(Of ImpersonationType, TWebAPIFinder, TResult)) As TResult
        Dim shibbolethEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled
        If shibbolethEnabled Then
            Dim func As Func(Of ICredential, HttpClientHandler) = BuildShibbolethAuthenticationHandler()
            finder.SetCustomAuthenticationInizializer(func)
            Return impersonatedAction(ImpersonationType.Shibboleth, finder)
        Else
            Dim wi As WindowsIdentity = CType(DocSuiteContext.Current.User.Identity, WindowsIdentity)
            Using wic As WindowsImpersonationContext = wi.Impersonate()
                If ExecutionContext.IsFlowSuppressed() Then
                    Return impersonatedAction(ImpersonationType.Windows, finder)
                Else
                    Using ExecutionContext.SuppressFlow()
                        Return impersonatedAction(ImpersonationType.Windows, finder)
                    End Using
                End If
            End Using
        End If
    End Function
#End Region

#Region " Dao "
    Public Shared Sub ImpersonateDao(Of TWebAPIDao As IWebAPIDao(Of T), T)(ByVal dao As TWebAPIDao, ByVal impersonatedAction As Action(Of ImpersonationType, TWebAPIDao))
        ImpersonateDao(Of TWebAPIDao, T, Boolean)(dao, Function(impersonationType, d)
                                                           impersonatedAction(impersonationType, dao)
                                                           Return True
                                                       End Function)
    End Sub

    Public Shared Function ImpersonateDao(Of TWebAPIDao As IWebAPIDao(Of T), T, TResult)(ByVal dao As TWebAPIDao, ByVal impersonatedAction As Func(Of ImpersonationType, TWebAPIDao, TResult)) As TResult
        Dim shibbolethEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled
        If shibbolethEnabled Then
            Dim func As Func(Of ICredential, HttpClientHandler) = BuildShibbolethAuthenticationHandler()
            dao.SetCustomAuthenticationInizializer(func)
            Return impersonatedAction(ImpersonationType.Shibboleth, dao)
        Else
            Dim wi As WindowsIdentity = CType(DocSuiteContext.Current.User.Identity, WindowsIdentity)
            Using wic As WindowsImpersonationContext = wi.Impersonate()
                If ExecutionContext.IsFlowSuppressed() Then
                    Return impersonatedAction(ImpersonationType.Windows, dao)
                Else
                    Using ExecutionContext.SuppressFlow()
                        Return impersonatedAction(ImpersonationType.Windows, dao)
                    End Using
                End If
            End Using
        End If
    End Function
#End Region

#End Region
End Class
