Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits

Public Class FullTextSearch
    Inherits CommonBasePage
#Region " Properties "
    Public ReadOnly Property IdTenant() As String
        Get
            Return CurrentTenant.UniqueId.ToString()
        End Get
    End Property
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            rgvDocumentLists.DataSource = New List(Of DocumentUnitModel)
        End If
    End Sub

End Class