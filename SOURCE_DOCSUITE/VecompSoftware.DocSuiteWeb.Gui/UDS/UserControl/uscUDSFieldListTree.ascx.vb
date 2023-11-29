Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.UDS
Imports Newtonsoft

Public Class uscUDSFieldListTree
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private Const FIELD_NAME_ATTRIBUTE_NAME As String = "FieldName"
#End Region

#Region "Properties"
    Public Property IdUDSRepository As Guid?
    Public Property UDSFieldListChildren As String
    Public Property IsReadOnly As Boolean
    Public Property HiddenFieldId As String
    Public ReadOnly Property SelectedNodeValue As String
        Get
            Return If(rddtUDSFieldList.EmbeddedTree.SelectedNode IsNot Nothing, rddtUDSFieldList.EmbeddedTree.SelectedNode.Value, String.Empty)
        End Get
    End Property

    Public Property IsRequired As Boolean
        Get
            Return rfvRddtUDSFieldList.Enabled
        End Get
        Set(ByVal value As Boolean)
            rfvRddtUDSFieldList.Enabled = value
        End Set
    End Property
    Public Property ErrorMessage As String
        Get
            Return rfvRddtUDSFieldList.ErrorMessage
        End Get
        Set(ByVal value As String)
            rfvRddtUDSFieldList.ErrorMessage = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region "Methods"
    Public Sub LoadUDSFieldListTree(idUDSRepository As Guid?)
        Me.IdUDSRepository = idUDSRepository
        If Not IsReadOnly Then
            Dim script As String = $"loadUDSFieldListTree(""{If(idUDSRepository.HasValue, idUDSRepository.Value.ToString(), String.Empty)}"");"
            ScriptManager.RegisterStartupScript(Me, GetType(uscUDSFieldListTree), "loadUDSFieldListTree", script, True)
        End If
    End Sub

    Public Sub SetFieldNameAttribute(fieldName As String)
        rddtUDSFieldList.EmbeddedTree.Attributes.Add(FIELD_NAME_ATTRIBUTE_NAME, fieldName)
    End Sub

    Public Sub LoadUDSFielsListParents(udsFieldListChildren As List(Of KeyValuePair(Of String, Guid)))
        Me.UDSFieldListChildren = JsonConvert.SerializeObject(udsFieldListChildren)
    End Sub
#End Region
End Class