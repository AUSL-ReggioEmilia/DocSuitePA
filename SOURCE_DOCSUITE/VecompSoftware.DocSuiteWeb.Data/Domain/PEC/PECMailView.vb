<Serializable()>
Public Class PECMailView
    Inherits DomainObject(Of Int32)

#Region " Fields "
#End Region

#Region " Properties "
    Public Overridable Property Title() As String
    Public Overridable Property Description() As String
    Public Overridable Property FoldersToHide() As String
    Public Overridable Property FilesToHide() As String
    Public Overridable Property ExtensionsToHide() As String
    Public Overridable Property HideExtensions() As Boolean
    Public Overridable Property FlatAttachments() As Boolean
    Public Overridable Property DocumentoPrincipaleLabel() As String
    Public Overridable Property CorpoDelMessaggioLabel() As String
    Public Overridable Property AllegatiLabel() As String
    Public Overridable Property AllegatiTecniciLabel() As String
    Public Overridable Property RicevuteLabel() As String
    Public Overridable Property RootNodeName() As String
    Public Overridable Property ExclusivePageType() As String

    Public Overridable Property Roles() As IList(Of Role)

    ''' <summary>Collegamento tra viste e settori</summary>
    ''' <remarks>Aggiunto per poter accedere alla proprietà decorata <see>PECMailViewDefault.IdDefaultView</see></remarks>
    Public Overridable Property MailViewDefaults() As IList(Of PECMailViewDefault)
#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

End Class