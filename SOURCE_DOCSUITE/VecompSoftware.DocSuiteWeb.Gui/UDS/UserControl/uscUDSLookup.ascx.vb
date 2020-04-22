Imports System.Linq
Imports Telerik.Web.UI

Public Class uscUDSLookup
    Inherits DocSuite2008BaseControl

    Public Property UDSName As String
    Public Property PropertyName As String
    Public Property LookupValue As String
    Public Property LookupLabel As String
    Public Property HiddenFieldId As String

    Public ReadOnly Property SelectedValue As String
        Get
            Return rcbLookup.SelectedValue
        End Get
    End Property

    Public Property Enabled As Boolean
        Get
            Return rcbLookup.Enabled
        End Get
        Set(ByVal value As Boolean)
            rcbLookup.Enabled = value
        End Set
    End Property

    Public Property IsRequired As Boolean
        Get
            Return rfvLookup.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvLookup.Enabled = value
        End Set
    End Property

    Public Property ErrorMessage As String
        Get
            Return rfvLookup.ErrorMessage
        End Get
        Set(ByVal value As String)
            rfvLookup.ErrorMessage = value
        End Set
    End Property

    Public ReadOnly Property MaxNumberDropdownElements As Integer
        Get
            Return ProtocolEnv.MaxNumberDropdownElements
        End Get
    End Property

    Public Property CheckBoxesEnabled As Boolean
        Get
            Return rcbLookup.CheckBoxes
        End Get
        Set(value As Boolean)
            rcbLookup.CheckBoxes = value
        End Set
    End Property

#Region "Events"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub
#End Region

#Region "Methods"


#End Region
End Class