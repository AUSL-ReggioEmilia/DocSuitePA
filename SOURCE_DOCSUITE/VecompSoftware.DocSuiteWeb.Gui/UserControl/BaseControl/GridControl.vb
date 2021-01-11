Public Class GridControl
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public Property RedirectOnParentPage() As Boolean
        Get
            If ViewState.Item("_redirect") Is Nothing Then
                Return False
            Else
                Return CBool(ViewState.Item("_redirect"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState.Item("_redirect") = value
        End Set
    End Property
#End Region

#Region "Private Functions"
    Protected Overridable Sub RedirectOnPage(ByVal page As String)
        If RedirectOnParentPage Then
            Dim script As String = "parent.location='" & page & "';"
            AjaxManager.ResponseScripts.Add(script)
        Else
            Response.RedirectLocation = "parent"
            Response.Redirect(page)
        End If
    End Sub
#End Region
    

End Class
