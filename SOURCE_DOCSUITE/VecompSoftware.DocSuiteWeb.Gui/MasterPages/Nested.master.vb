Imports System.Web

Public Class Nested
    Inherits MasterPage

#Region " Properties "

    Public ReadOnly Property BaseMaster() As Base
        Get
            Return CType(Master, Base)
        End Get
    End Property

    Public Property HistoryTitle() As String
        Get
            Return CType(ViewState("_historyTitle"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_historyTitle") = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return lblHeader.Text
        End Get
        Set(ByVal value As String)
            lblHeader.Text = value
        End Set
    End Property

    Public Property TitleVisible() As Boolean
        Get
            Return divTitolo.Visible
        End Get
        Set(ByVal value As Boolean)
            divTitolo.Visible = value
        End Set
    End Property

    Public Property TitleContainer As HtmlGenericControl
        Get
            Return divTitolo
        End Get
        Set(value As HtmlGenericControl)
            divTitolo = value
        End Set
    End Property

    Public ReadOnly Property Header() As ContentPlaceHolder
        Get
            Return cphHeader
        End Get
    End Property

    Public ReadOnly Property Content() As ContentPlaceHolder
        Get
            Return cphContent
        End Get
    End Property

    Public ReadOnly Property Footer() As ContentPlaceHolder
        Get
            Return cphFooter
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Title = Page.Title

        'Imposto etichetta History di default (titolo pagina)
        If Not String.IsNullOrEmpty(HistoryTitle) Then
            Page.Title = HistoryTitle
        End If

        ' Visualizzazione pannelli
        If cphContent.Controls.Count = 0 Then
            cphContent.Visible = False
        End If
        If cphFooter.Controls.Count = 0 Then
            cphFooter.Visible = False
        End If

        If Not IsPostBack Then
            BaseMaster.HtmlBody.Attributes.Add("onresize", "SetDivContent();")
        End If
        BaseMaster.AjaxManager.ResponseScripts.Add("SetDivContent();")

    End Sub

    Private Sub BtnExportClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnExport.Click
        Dim cookieExportGrid As HttpCookie = Request.Cookies("ExportGridID")
        Dim cookieExportType As HttpCookie = Request.Cookies("ExportGridType")
        Dim grid As BindGrid
        If cookieExportGrid IsNot Nothing Then
            grid = CType(Page.FindControl(cookieExportGrid.Value), BindGrid)
            If grid IsNot Nothing AndAlso cookieExportType IsNot Nothing Then
                Dim exportType As String = cookieExportType.Value
                Request.Cookies.Remove("ExportGridID")
                Request.Cookies.Remove("ExportGridType")
                Select Case exportType
                    Case "Excel"
                        grid.MasterTableView.ExportToExcel()
                    Case "Word"
                        grid.MasterTableView.ExportToExcel()
                    Case "Pdf"
                        grid.MasterTableView.ExportToExcel()
                End Select
            End If
        End If
    End Sub

#End Region

End Class