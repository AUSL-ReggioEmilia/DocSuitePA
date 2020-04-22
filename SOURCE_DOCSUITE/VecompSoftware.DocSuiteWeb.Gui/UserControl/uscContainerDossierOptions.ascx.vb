Public Class uscContainerDossierOptions
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Public Const LOAD_FOLDERS As String = "loadFolders({0})"
#End Region

#Region " Properties "

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "
    Public Sub LoadFolders(idContainer As Integer)
        AjaxManager.ResponseScripts.Add(String.Format(LOAD_FOLDERS, idContainer))
    End Sub
#End Region

End Class