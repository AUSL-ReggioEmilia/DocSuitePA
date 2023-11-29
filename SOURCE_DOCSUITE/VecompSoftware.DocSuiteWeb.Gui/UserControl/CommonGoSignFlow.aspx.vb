Imports VecompSoftware.DocSuiteWeb.Data

Public Class CommonGoSignFlow
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property GoSignSessionIs As String
        Get
            Return GetKeyValue(Of String)("GoSignSessionId")
        End Get
    End Property

    Public ReadOnly Property InfocertProxySignUrl As String
        Get
            Return DocSuiteContext.Current.InfocertProxySignUrl
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class