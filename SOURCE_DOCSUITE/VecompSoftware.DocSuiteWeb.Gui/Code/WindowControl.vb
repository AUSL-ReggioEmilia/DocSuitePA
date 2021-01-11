Imports Telerik.Web.UI

<Obsolete("Problemi risolti in altri modi da altre parti: ELIMINARE")>
Public Class WindowControl
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private _extWindowManager As RadWindowManager
#End Region

#Region "Properties"

    Public Property ExternalWindowManager() As RadWindowManager
        Get
            Return _extWindowManager
        End Get
        Set(ByVal value As RadWindowManager)
            _extWindowManager = value
        End Set
    End Property

    Public Property UseParentWindow() As Boolean
        Get
            If ViewState("_parentWindow") IsNot Nothing Then
                Return ViewState("_parentWindow")
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("_parentWindow") = value
        End Set
    End Property
#End Region

#Region "Protected Functions"
    Protected Sub RegisterWindowManager(ByRef windowManager As RadWindowManager)
        If ExternalWindowManager Is Nothing Then
            WindowBuilder.RegisterWindowManager(windowManager)
        Else
            WindowBuilder.RegisterWindowManager(ExternalWindowManager, True)
        End If
    End Sub
#End Region
End Class
