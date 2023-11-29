Imports Newtonsoft.Json
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Helpers.ExtensionMethods


Public Class OChartItemDTO
    Implements IOChartItemDTO

#Region " Constructors "

    Public Sub New()
    End Sub

    Public Sub New(item As OChartItem)
        Code = item.Code
        FullCode = item.FullCode
        Title = item.Title
        Description = item.Description
        Acronym = item.Acronym

        If item.HasItems Then
            Items = item.Items.ReplicateListAsDTO(Me).Distinct().ToArray()
        End If
    End Sub

#End Region

#Region " Properties "

    Public Property Code As String Implements IOChartItemDTO.Code
    Public Property ParentCode As String Implements IOChartItemDTO.ParentCode
    Public Property FullCode As String Implements IOChartItemDTO.FullCode
    Public Property Title As String Implements IOChartItemDTO.Title
    Public Property Description As String Implements IOChartItemDTO.Description
    Public Property Acronym As String Implements IOChartItemDTO.Acronym

    <JsonIgnore>
    Public Property Parent As IOChartItemDTO Implements IOChartItemDTO.Parent
    Public Property Items As IOChartItemDTO() Implements IOChartItemDTO.Items

#End Region

#Region " Methods "

    Public Overridable ReadOnly Property HasItems As Boolean
        Get
            Return Not Items.IsNullOrEmpty()
        End Get
    End Property

#End Region

End Class
