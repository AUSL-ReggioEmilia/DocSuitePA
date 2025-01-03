﻿Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltMetadataRepositoryDesigner
    Inherits CommonBasePage

    Public ReadOnly Property IdMetadataRepository As String
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("IdMetadtaRepository", Nothing)
        End Get
    End Property

    Public ReadOnly Property IsEditPage As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("IsEditPage", Nothing)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.UserConnectedBelongsTo(ProtocolEnv.MetadataRepositoryGroups)) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

    End Sub

End Class