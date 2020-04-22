Partial Public Class UtltWSImport
    Inherits SuperAdminPage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub

    Private Sub btnImporta_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImporta.Click

        Throw New NotImplementedException()
        'Dim sError As String = String.Empty
        'Dim anno As Short
        'Dim idcont As Integer = 0

        'Dim doc As New XmlDocument
        'Try
        '    Dim dati As String = File.OpenText(uscXml.Documents(0).FileInfo.FullName).ReadToEnd()

        '    Dim ws As New WSProt()
        '    Dim s As String = ws.Importa(CommonUtil.UserFullName, dati, anno)
        '    Dim xml As New XmlDocument
        '    xml.LoadXml(s)
        '    Dim list As XmlNodeList = xml.DocumentElement.SelectNodes("child::Errore[text()!='']")
        '    If list.Count = 0 Then
        '        lblResult.Text = "<b>Importazione effettuata correttamente.</b>"
        '    Else
        '        lblResult.Text = "<b>Errore:</b><br />&nbsp;&nbsp;&nbsp;&nbsp;" & list(0).InnerText.Replace(vbCrLf, "<br />&nbsp;&nbsp;&nbsp;&nbsp;")
        '    End If
        'Catch ex As Exception
        '    lblResult.Text = "<b>Errore:</b><br />&nbsp;&nbsp;&nbsp;&nbsp;" & ex.Message
        'End Try

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnImporta, lblResult, MasterDocSuite.AjaxLoadingPanelSearch)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnImporta, uscXml)
    End Sub

#End Region

End Class