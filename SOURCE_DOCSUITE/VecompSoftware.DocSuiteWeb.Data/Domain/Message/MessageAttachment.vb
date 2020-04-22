Imports VecompSoftware.DocSuiteWeb.Data

<Serializable()>
Public Class MessageAttachment
    Inherits AuditableDomainObject(Of Int32)

#Region " Properties "

    Public Overridable Property Message As DSWMessage

    Public Overridable Property Server As String

    Public Overridable Property Archive As String

    Public Overridable Property ChainId As Integer

    Public Overridable Property DocumentEnum As Integer?

    Public Overridable Property Extension As String

#End Region

#Region " Constructors "

    Public Sub New()

    End Sub

    Public Sub New(server As String, archive As String, chainId As Integer, documentEnum As Integer?, extension As String)
        Me.Server = server
        Me.Archive = archive
        Me.ChainId = chainId
        Me.DocumentEnum = documentEnum
        Me.Extension = extension
    End Sub

#End Region

End Class
