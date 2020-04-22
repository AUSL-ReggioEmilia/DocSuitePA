Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata

Public Class FascPeriodInserimento
    Inherits FascBasePage

#Region " Fields "

    Private Const FASCICLE_PERIOD_INSERT_CALLBACK As String = "fascPeriodInserimento.insertCallback('{0}');"

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscFascicleInsert.IsPeriodic = True
        uscFascicleInsert.PageContentDiv = PageContentDiv
        InitializeAjax()
    End Sub

    Protected Sub FascPeriodInserimento_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
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
                        metadataModel = uscFascicleInsert.GetDynamicValues()
                    End If

                    AjaxManager.ResponseScripts.Add(String.Format(FASCICLE_PERIOD_INSERT_CALLBACK,
                                                                  If(metadataModel IsNot Nothing, JsonConvert.SerializeObject(metadataModel), Nothing)))
                    Exit Select
            End Select
        Catch
            Exit Sub
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf FascPeriodInserimento_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

#End Region


End Class