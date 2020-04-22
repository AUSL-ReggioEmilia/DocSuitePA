Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

' TODO: Finire di eliminare questa peste bubbonica
<Obsolete("Non usare, passare a javascript in pagina piuttosto.")>
Public Class WindowBuilder

#Region "Fields"
    Private _ajaxManager As RadAjaxManager
    Private _windowManager As RadWindowManager
    Private _externalManager As Boolean = False
#End Region

#Region "Constructor"
    Public Sub New(ByRef ajaxManager As RadAjaxManager)
        _ajaxManager = ajaxManager
    End Sub
#End Region

#Region "Register WindowManager"
    ''' <summary>
    ''' Registra il RadWindowManager che si occuperà dell'apertura delle finestre
    ''' </summary>
    ''' <param name="windowManager">RadWindowManager da registrare</param>
    ''' <param name="externalManager">True, indica un manager esterno allo usercontrol/pagina (utilizzato per aprire finestre una dentro l'altra)</param>
    ''' <remarks></remarks>
    Public Sub RegisterWindowManager(ByRef windowManager As RadWindowManager, Optional ByVal externalManager As Boolean = False)
        _windowManager = windowManager
        _externalManager = externalManager
    End Sub
#End Region

#Region "Register Opener Control"
    ''' <summary>
    ''' Registra il controllo che si occupa di aprire una finestra
    ''' </summary>
    ''' <param name="control">Control da registrare</param>
    ''' <remarks></remarks>
    Public Sub RegisterOpenerElement(ByRef control As Control)
        _ajaxManager.AjaxSettings.AddAjaxSetting(control, _windowManager)
    End Sub
#End Region

#Region "Load Window"
    ''' <summary> Apre una finestra </summary>
    ''' <param name="windowId">ID della finestra da aprire</param>
    ''' <param name="navigateUrl">URL da caricare</param>
    Public Function LoadWindow(ByRef windowId As String, ByVal navigateUrl As String) As RadWindow
        Dim window As RadWindow = Nothing
        If GetWindowByID(windowId, window) Then
            window.NavigateUrl = navigateUrl
            window.VisibleOnPageLoad = True
            Return window
        End If
        Return Nothing
    End Function

    ''' <summary> Apre una finestra </summary>
    ''' <param name="windowId">ID della finestra da aprire</param>
    ''' <param name="navigateUrl">URL da caricare</param>
    ''' <param name="closeFunction">Nome della funzione javascript di OnClientClose</param>
    Public Function LoadWindow(ByRef windowId As String, ByVal navigateUrl As String, ByVal closeFunction As String) As RadWindow
        Dim window As RadWindow = LoadWindow(windowId, navigateUrl)
        If (window IsNot Nothing) Then
            window.OnClientClose = GetCloseFunction(closeFunction)
        End If
        Return window
    End Function

    ''' <summary> Apre una finestra </summary>
    ''' <param name="windowId">ID della finestra da aprire</param>
    ''' <param name="navigateUrl">URL da caricare</param>
    ''' <param name="closeFunction">Nome della funzione javascript di OnClientClose</param>
    ''' <param name="width">Larghezza</param>
    ''' <param name="height">Altezza</param>
    Public Function LoadWindow(ByRef windowId As String, ByVal navigateUrl As String, ByVal closeFunction As String, ByVal width As Unit, ByVal height As Unit) As RadWindow
        Dim window As RadWindow = LoadWindow(windowId, navigateUrl, closeFunction)
        If (window IsNot Nothing) Then
            window.Width = width
            window.Height = height
        End If
        Return window
    End Function

    ''' <summary> Apre una finestra </summary>
    ''' <param name="windowId">ID della finestra da aprire</param>
    ''' <param name="navigateUrl">URL da caricare</param>
    ''' <param name="width">Larghezza</param>
    ''' <param name="height">Altezza</param>
    Public Function LoadWindow(ByRef windowId As String, ByVal navigateUrl As String, ByVal width As Unit, ByVal height As Unit) As RadWindow
        Dim window As RadWindow = LoadWindow(windowId, navigateUrl)
        If (window IsNot Nothing) Then
            window.Width = width
            window.Height = height
        End If
        Return window
    End Function

#End Region

#Region "Create Window"
    Public Function CreateWindow(ByVal page As Page, ByVal windowID As String, ByVal windowTitle As String) As RadWindowManager
        Dim windowManager As RadWindowManager = New RadWindowManager()
        Dim window As RadWindow = New RadWindow()
        window.ID = windowID
        window.Title = windowTitle

        windowManager.ID = Guid.NewGuid().ToString()
        windowManager.Windows.Add(window)
        page.Controls.Add(windowManager)

        RegisterWindowManager(windowManager)

        Return windowManager
    End Function
#End Region

#Region "Utils"
    Protected Overridable Function GetWindowByID(ByVal windowID As String, ByRef oWindow As RadWindow) As Boolean
        For Each window As RadWindow In _windowManager.Windows
            If window.ID.Equals(windowID) Then
                oWindow = window
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Restituisce il nome della funzione OnClientClose verificando se si sta usando un RadWindowManager esterno. In questo caso
    ''' aggancia la stringa "top" in modo di salire di livello nella ricerca della funzione. (Es: "top.OnClientClose" o "OnClientClose")
    ''' </summary>
    ''' <param name="closeFunction">nome originale della funzione javascript</param>
    ''' <returns>Se RadWindowManager esterno: "top.OnClientClose" altrimenti "OnClientClose"</returns>
    Protected Function GetCloseFunction(ByVal closeFunction As String) As String
        If _externalManager Then
            Return "top." & closeFunction
        Else
            Return closeFunction
        End If
    End Function
#End Region


End Class
