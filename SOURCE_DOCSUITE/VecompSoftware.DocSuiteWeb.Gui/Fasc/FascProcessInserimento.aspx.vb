Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata

Public Class FascProcessInserimento
    Inherits FascBasePage

    Private Const FASCICLE_INSERT_CALLBACK As String = "fascInserimento.insertCallback('{0}');"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscContact.FilterByParentId = ProtocolEnv.FascicleContactId
        AddHandler AjaxManager.AjaxRequest, AddressOf FascProcessInserimento_AjaxRequest
    End Sub

    Private Sub uscCategory_CategoryChange(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded, uscCategory.CategoryRemoved

    End Sub

    Public Function GetDynamicValues() As MetadataModel
        Return uscDynamicMetadata.GetControlValues()
    End Function

    Protected Sub FascProcessInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
            If ajaxModel Is Nothing Then
                Return
            End If
            Select Case ajaxModel.ActionName
                Case "Insert"
                    Dim metadataModel As MetadataModel = Nothing
                    If ProtocolEnv.MetadataRepositoryEnabled Then
                        metadataModel = GetDynamicValues()
                    End If

                    AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_INSERT_CALLBACK, If(metadataModel IsNot Nothing, JsonConvert.SerializeObject(metadataModel).Replace("'", "\'"), Nothing)))
                    Exit Select
            End Select
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

End Class