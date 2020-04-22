Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelContactRubrica
    Inherits CommBasePage

    Protected ReadOnly Property EnableFlagGroupChild() As Boolean
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("EnableFlagGroupChild")) Then
                Return CType(Request.QueryString("EnableFlagGroupChild"), Boolean)
            End If
            Return False
        End Get
    End Property

    Protected ReadOnly Property CategoryContactsProcedureType() As String
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("CategoryContactsProcedureType")) Then
                Return Request.QueryString("CategoryContactsProcedureType")
            End If
            Return String.Empty
        End Get
    End Property

    Protected ReadOnly Property SearchInCategoryContacts() As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("SearchInCategoryContacts")) Then
                Return CType(Request.QueryString("SearchInCategoryContacts"), Integer)
            End If
            Return Nothing
        End Get
    End Property

    Protected ReadOnly Property SearchInRoleContacts() As Integer?
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("SearchInRoleContacts")) Then
                Return CType(Request.QueryString("SearchInRoleContacts"), Integer)
            End If
            Return Nothing
        End Get
    End Property

    Protected ReadOnly Property RoleContactsProcedureType() As String
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("RoleContactsProcedureType")) Then
                Return Request.QueryString("RoleContactsProcedureType")
            End If
            Return String.Empty
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not String.IsNullOrEmpty(Request.QueryString("MultiSelect")) Then
            UscContatti1.MultiSelect = True
            If EnableFlagGroupChild() Then
                UscContatti1.EnableFlagGroupChild = True
            End If
        End If
        Dim exclContact As List(Of Integer) = Request.QueryString.GetValueOrDefault(Of List(Of Integer))("ExcludeContact", Nothing)
        UscContatti1.ExcludeContact = If(exclContact.IsNullOrEmpty(), Nothing, exclContact)
        UscContatti1.ExcludeRoleRoot = Request.QueryString.GetValueOrDefault(Of Boolean?)("ExcludeRoleRoot", False)

        If Not String.IsNullOrEmpty(Request.QueryString("OnlyDetail")) Then
            UscContatti1.EditMode = False
            UscContatti1.SearchMode = False
            UscContatti1.ShowAddressBook = False
            UscContatti1.ShowDetails = True
            UscContatti1.MultiSelect = False
        End If
        UscContatti1.ShowAll = Not String.IsNullOrEmpty(Request("ShowAll")) AndAlso Request("ShowAll").Eq("true")
        If Not String.IsNullOrEmpty(Request.QueryString("AVCPBusinessContactEnabled")) Then
            UscContatti1.AVCPBusinessContactEnabled = Boolean.Parse(Request.QueryString("AVCPBusinessContactEnabled"))
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("FascicleContactEnabled")) Then
            UscContatti1.FascicleContactEnabled = Boolean.Parse(Request.QueryString("FascicleContactEnabled"))
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("AVCPConfermaSelezioneVisible")) Then
            UscContatti1.ConfermaSelezioneVisible = Boolean.Parse(Request.QueryString("AVCPConfermaSelezioneVisible"))
        End If

        If Not String.IsNullOrEmpty(CategoryContactsProcedureType) Then
            UscContatti1.CategoryContactsProcedureType = CategoryContactsProcedureType
        End If

        If SearchInCategoryContacts.HasValue() Then
            UscContatti1.SearchInCategoryContacts = SearchInCategoryContacts.Value
        End If

        If SearchInRoleContacts.HasValue() Then
            UscContatti1.SearchInRoleContacts = SearchInRoleContacts.Value
        End If

        If Not String.IsNullOrEmpty(RoleContactsProcedureType) Then
            UscContatti1.RoleContactsProcedureType = RoleContactsProcedureType
        End If
    End Sub

End Class