Imports System.Security.Principal
Imports System.Threading
Imports System.Web
Imports VecompSoftware.Services.Logging

''' <summary>
''' Classe con i dati relativi alla connessione utente
''' </summary>
''' <remarks>
''' Non ha senso in VecompSoftware.DocSuiteWeb.Data richiamare HttpContext.Current o WindowsIdentity.GetCurrent.
''' Trovare un modo per istanziare questa classe in Facade e passarla alla VecompSoftware.DocSuiteWeb.Data.
''' </remarks>
Public Class DocSuiteUser

#Region " Fields "

    Private _userName As String
    Private _fullUserName As String
    Private _domain As String

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub

    Public Sub New(domain As String)
        _domain = domain
    End Sub
#End Region

#Region " Properties "

    Public ReadOnly Property Identity() As IIdentity
        Get
            Try
                Return HttpContext.Current.User.Identity
            Catch ex As Exception
                Try
                    Return WindowsIdentity.GetCurrent()
                Catch ex1 As Exception
                    Return Thread.CurrentPrincipal.Identity
                End Try
            End Try
        End Get
    End Property

    Public ReadOnly Property FullUserName As String
        Get
            If String.IsNullOrEmpty(_fullUserName) Then
                Dim identityName As String = Identity.Name
                If String.IsNullOrEmpty(identityName) Then
                    FileLogger.Info(LogName.FileLog, String.Concat("Attenzione UserName vuoto : Identity ", Identity Is Nothing, " Identity.IsAuthenticated ", IIf(Identity Is Nothing, False, Identity.IsAuthenticated)))
                    Return String.Empty
                End If
                _fullUserName = identityName
            End If
            Return _fullUserName
        End Get

    End Property

    Public Property UserName As String
        Get
            If String.IsNullOrEmpty(_userName) Then
                Dim identityName As String = Identity.Name
                If String.IsNullOrEmpty(identityName) Then
                    FileLogger.Info(LogName.FileLog, String.Concat("Attenzione UserName vuoto : Identity ", Identity Is Nothing, " Identity.IsAuthenticated ", IIf(Identity Is Nothing, False, Identity.IsAuthenticated)))
                    Return String.Empty
                End If
                _userName = identityName
                If identityName.Contains("\") Then
                    _userName = CType(identityName.Split("\"c).GetValue(1), String)
                End If
            End If
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
        End Set
    End Property

    Public Property Domain As String
        Get
            If String.IsNullOrEmpty(_domain) Then
                Dim identityName As String = Identity.Name
                If String.IsNullOrEmpty(identityName) Then
                    Return String.Empty
                End If
                If identityName.Contains("\") Then
                    _domain = CType(identityName.Split("\"c).GetValue(0), String)
                End If
            End If
            Return _domain
        End Get
        Set(ByVal value As String)
            _domain = Domain
        End Set
    End Property

#End Region

End Class