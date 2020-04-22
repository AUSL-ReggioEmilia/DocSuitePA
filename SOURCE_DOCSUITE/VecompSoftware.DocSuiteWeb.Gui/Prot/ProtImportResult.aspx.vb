Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class ProtImportResult
    Inherits ProtBasePage

    Dim ImportFile As String = String.Empty

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        ImportFile = Request.QueryString("ImportFile")
        'stampa
        Dim s As String = String.Empty
        s = "document.all('" & tblButton.ClientID & "').style.display='none';" & _
            "var hgt=document.all('" & PageDiv.ClientID & "').style.height;" & _
            "document.all('" & PageDiv.ClientID & "').style.height='100%';" & _
            "window.print();" & _
            "document.all('" & tblButton.ClientID & "').style.display='inline';" & _
            "document.all('" & PageDiv.ClientID & "').style.height=hgt;" & _
            "return false;"
        btnStampa.Attributes.Add("onclick", s)
        If Not Me.IsPostBack Then
            ddlResult_SelectedIndexChanged(sender, e)
            Initialize(Nothing)
        End If
    End Sub

    Private Sub Initialize(ByVal sContainer As String)
        If ImportFile.ToUpper = "EXCEL" Then
            Me.DG.Columns(1).Visible = False
            Me.DG.Columns(2).Visible = False
        Else
            Me.DG.Columns(3).Visible = False
            Me.DG.Columns(4).Visible = False
        End If
    End Sub

    Private Sub ExportToExcel(ByVal Control As Control, ByRef MyPage As Page, Optional ByVal FileName As String = "")
        'export to excel
        MyPage.Response.Clear()
        MyPage.Response.Buffer = True
        MyPage.Response.ContentType = "application/vnd.ms-excel"
        If FileName <> "" Then
            MyPage.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", FileName))
        End If
        MyPage.Response.Charset = ""
        MyPage.EnableViewState = False
        Dim oStringWriter As New StringWriter
        Dim oHtmlTextWriter As New HtmlTextWriter(oStringWriter)
        Control.RenderControl(oHtmlTextWriter)
        MyPage.Response.Write(oStringWriter.ToString())
        MyPage.Response.End()
    End Sub

    Private Sub btnExcel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExcel.Click
        DG.Columns.Item(0).Visible = False
        ExportToExcel(DG, Me, "ImportazioneProtocollo.xls")
    End Sub

    Private Sub ddlResult_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlResult.SelectedIndexChanged
        Select Case (ddlResult.SelectedValue)

            Case "Protocollati"
                If Not (CommImportLettera.ResultTable Is Nothing) Then
                    DG.DataSource = CommImportLettera.ResultTable
                    DG.DataBind()
                End If
                Title = "Protocollo Import - Elenco File Importati (" & CommImportLettera.ResultTable.Rows.Count & ")"

            Case "Errori"
                If Not (CommImportLettera.ErrorsTable Is Nothing) Then
                    DG.DataSource = CommImportLettera.ErrorsTable
                    DG.DataBind()
                End If
                Title = "Protocollo Import - Elenco File Importati (" & CommImportLettera.ErrorsTable.Rows.Count & ")"

            Case "Tutti"
                Dim dt As DataTable = Nothing

                If Not (CommImportLettera.ResultTable Is Nothing) Then
                    dt = CommImportLettera.ResultTable.Copy()
                End If

                If Not (CommImportLettera.ErrorsTable Is Nothing) Then
                    For Each row As DataRow In CommImportLettera.ErrorsTable.Rows
                        dt.Rows.Add(row.ItemArray())
                    Next row
                End If

                DG.DataSource = dt
                DG.DataBind()
                If dt IsNot Nothing Then
                    Title = "Protocollo Import - Elenco File Importati (" & dt.Rows.Count & ")"
                Else
                    Title = "Protocollo Import - Nessun File Importato"
                End If
        End Select
    End Sub

    Private Sub DG_ItemCommand(ByVal source As Object, ByVal e As DataGridCommandEventArgs) Handles DG.ItemCommand
        Dim protocol As String = e.CommandArgument.ToString()
        If protocol <> "-1" Then
            Dim year As Short = Nothing
            Dim number As Integer = Nothing
            ProtocolFacade.ProtNumber(protocol, year, number)
            Dim params As String = String.Format("Year={0}&Number={1}", year, number)
            Response.Redirect("../Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(params))
        End If
    End Sub

    Private Sub DG_ItemBound(ByVal source As System.Object, ByVal e As DataGridItemEventArgs) Handles DG.ItemDataBound
        Dim protocol As String = DataBinder.Eval(e.Item.DataItem, "RESULT")
        If protocol <> "-1" Then
            e.Item.ToolTip = "Protocollo n. " & protocol
        End If
    End Sub

End Class
