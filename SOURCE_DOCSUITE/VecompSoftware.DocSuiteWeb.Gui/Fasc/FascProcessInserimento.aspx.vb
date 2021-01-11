Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class FascProcessInserimento
    Inherits FascBasePage

    Private _idDocumentUnit As Guid?

    Public ReadOnly Property IdDocumentUnit As Guid?
        Get
            If Not _idDocumentUnit.HasValue Then
                _idDocumentUnit = Request.QueryString.GetValueOrDefault(Of Guid?)("IdDocumentUnit", Nothing)
            End If
            Return _idDocumentUnit
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

End Class