Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.Design

Public Class UpdateHistoryDesigner
    Inherits ControlDesigner

    Public Overloads Overrides Sub Initialize(ByVal component As IComponent)
        If Not (TypeOf component Is UpdateHistory) Then
            Throw New ArgumentOutOfRangeException("component")
        End If

        MyBase.Initialize(component)
    End Sub

    Public Overloads Overrides Function GetDesignTimeHtml() As String
        Return CreatePlaceHolderDesignTimeHtml()
    End Function
End Class
