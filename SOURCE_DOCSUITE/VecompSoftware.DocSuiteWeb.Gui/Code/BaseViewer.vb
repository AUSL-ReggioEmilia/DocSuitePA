Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Gui.Viewers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public MustInherit Class BaseViewer
    Inherits CommonBasePage

    Protected MustOverride ReadOnly Property CurrentViewer As ViewerLight
    Protected MustOverride Function GetDataSource() As IList(Of DocumentInfo)
    Protected MustOverride ReadOnly Property Allowed() As Boolean

    Private Sub BindDataSource()
        CurrentViewer.DataSource = GetDataSource()
        CurrentViewer.DataBind()
    End Sub

    Protected Overridable Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            If Not Allowed Then
                Throw New DocSuiteException("Visualizzazione documenti", "Diritti di visualizzazione mancanti.")
            End If
            BindDataSource()
            MasterDocSuite.TitleVisible = False
        End If
    End Sub

End Class