Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class UtltCheckDBMapping
    Inherits SuperAdminPage

    Private _rowsMapping As ArrayList

    Private Sub Initialize()
        If DocSuiteContext.Current.IsDocumentEnabled Then
            ddlMapping.Items.Add(New ListItem("Pratiche", "DocmDB"))
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            ddlMapping.Items.Add(New ListItem("Protocollo", "ProtDB"))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            ddlMapping.Items.Add(New ListItem("Atti Delibere", "ReslDB"))
        End If

    End Sub

    Private Sub MakeDs()
        _rowsMapping = New ArrayList()

        If ddlMapping.SelectedValue = "Tutti" Then
            For Each dbitem As ListItem In ddlMapping.Items
                If Not dbitem.Value.Equals("Tutti") Then CheckMapping(dbitem.Value)
            Next
        Else
            CheckMapping(ddlMapping.SelectedValue)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Initialize()
            MakeDs()

            rgMapping.DataSource = _rowsMapping
            rgMapping.DataBind()
        End If
    End Sub

    Protected Sub btnEsegui_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnEsegui.Click
        MakeDs()
        rgMapping.DataSource = _rowsMapping
        rgMapping.DataBind()
    End Sub

    Private Sub CheckMapping(ByVal p_dbName As String)
        Dim cm As CommonMapping = New CommonMapping()
        _rowsMapping.AddRange(cm.CheckMapping(p_dbName))
    End Sub

End Class