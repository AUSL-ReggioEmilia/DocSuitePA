Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class CommonSelCategoryRest
    Inherits CommBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property Manager As String
        Get
            Return Request.QueryString.GetValueOrDefault("Manager", String.Empty)
        End Get
    End Property

    Public ReadOnly Property Secretary As String
        Get
            Return Request.QueryString.GetValueOrDefault("Secretary", String.Empty)
        End Get
    End Property

    Protected ReadOnly Property FascicleTypeToPage As String
        Get
            If FascicleType.HasValue Then
                Return DirectCast(FascicleType.Value, Short).ToString()
            End If
            Return "null"
        End Get
    End Property

    Public ReadOnly Property FascicleType As FascicleType?
        Get
            Return Request.QueryString.GetValueOrDefault(Of FascicleType?)("FascicleType", Nothing)
        End Get
    End Property

    Public ReadOnly Property FascicleBehavioursEnabled As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("FascicleBehavioursEnabled", False)
        End Get
    End Property

    Protected ReadOnly Property RoleToPage As String
        Get
            If Role.HasValue Then
                Return Role.Value.ToString()
            End If
            Return "null"
        End Get
    End Property

    Public ReadOnly Property Role As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("Role", Nothing)
        End Get
    End Property

    Protected ReadOnly Property ContainerToPage As String
        Get
            If Container.HasValue Then
                Return Container.Value.ToString()
            End If
            Return "null"
        End Get
    End Property

    Public ReadOnly Property Container As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("Container", Nothing)
        End Get
    End Property

    Protected ReadOnly Property ParentIdToPage As String
        Get
            If ParentId.HasValue Then
                Return ParentId.Value.ToString()
            End If
            Return "null"
        End Get
    End Property

    Public ReadOnly Property ParentId As Short?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Short?)("ParentId", Nothing)
        End Get
    End Property

    Public ReadOnly Property IncludeParentDescendants As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("IncludeParentDescendants", False)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
    End Sub
#End Region

#Region " Methods "

#End Region

End Class