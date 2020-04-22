Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class TbltMassimari
    Inherits CommonBasePage

#Region "Fields"
    Private _categoryId As Integer?
    Private Const CLOSE_WINDOW As String = "CloseWindow();"
#End Region

#Region "Properties"
    Public ReadOnly Property CategoryId As Integer?
        Get
            If Not _categoryId.HasValue Then
                If Request.QueryString("CategoryID") IsNot Nothing Then
                    _categoryId = Integer.Parse(Request.QueryString("CategoryID"))
                End If
            End If
            Return _categoryId
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub TbltMassimari_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim arr As String() = e.Argument.Split({"|"c}, StringSplitOptions.None)

        Dim operation As String = arr(0)
        Dim IdMassimarioScarto As Guid = Guid.Parse(arr(1))

        Select Case operation
            Case "AddMassimario"
                Dim category As Category = Facade.CategoryFacade.GetById(CategoryId.Value)
                category.IdMassimarioScarto = IdMassimarioScarto
                Facade.CategoryFacade.Update(category)
                AjaxManager.ResponseScripts.Add(CLOSE_WINDOW)
        End Select
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then

        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf TbltMassimari_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave)
    End Sub
#End Region

End Class