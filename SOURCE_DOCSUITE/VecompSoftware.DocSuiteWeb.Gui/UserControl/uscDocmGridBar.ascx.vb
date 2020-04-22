Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscDocmGridBar
    Inherits BaseGridBar

#Region " Properties "

    Public Overrides ReadOnly Property DeselectButton() As Button
        Get
            Return btnDeselectAll
        End Get
    End Property
    Public Overrides ReadOnly Property DocumentsButton() As Button
        Get
            Return btnDocuments
        End Get
    End Property
    Public Overrides ReadOnly Property PrintButton() As Button
        Get
            Return btnStampa
        End Get
    End Property

    Public Overrides ReadOnly Property SelectButton() As Button
        Get
            Return btnSelectAll
        End Get
    End Property

    Public Overrides ReadOnly Property SetReadButton() As Button
        Get
            Return btnSetRead
        End Get
    End Property

    Public Overrides ReadOnly Property LeftPanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

    Public Overrides ReadOnly Property MiddlePanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

    Public Overrides ReadOnly Property RightPanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
    End Sub

#End Region
    
#Region " Methods "

    Protected Overrides Sub Print()
        Dim documentprint As New DocumentPrint()
        documentprint.ListId = GetSelectedItems()
        Session.Add("Printer", documentprint)
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Prot&PrintName=Document")
    End Sub

    Public Overrides Function GetSelectedItems() As IList
        Dim idList As New List(Of YearNumberCompositeKey)
        For Each item As GridDataItem In _grid.Items
            Dim cb As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If Not cb.Checked Then
                Continue For
            End If

            Dim lb As LinkButton = DirectCast(item.FindControl("lnkPratica"), LinkButton)
            Dim yn As New YearNumberCompositeKey()
            yn.Year = Mid(lb.CommandArgument, 1, InStr(lb.CommandArgument, "|") - 1)
            yn.Number = Mid(lb.CommandArgument, InStr(lb.CommandArgument, "|") + 1)
            idList.Add(yn)
        Next
        Return idList
    End Function

    Protected Overrides Sub ConfigureSetReadProperties()
        LogType = "DS"
        LogDescription = "Marcato come già letto"
        SetReadFunction = New SetReadDelegate(AddressOf Facade.DocumentLogFacade.Insert)
    End Sub

#End Region

End Class