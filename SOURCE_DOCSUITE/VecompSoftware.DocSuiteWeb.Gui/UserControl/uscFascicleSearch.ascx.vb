Public Class uscFascicleSearch
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageControl As Control
        Get
            Return pageContent
        End Get
    End Property

    Public Property MinHeight As String
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(MinHeight) Then
            finderContent.Style.Add("min-height", MinHeight)
        End If
    End Sub
#End Region

#Region " Methods "

#End Region

End Class