Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmSelezione
    Inherits DocmBasePage

#Region " Private fields "

    Private _link As String = String.Empty
    Private _incremental As Integer

#End Region

#Region " Properties "

    Public Property Link() As String
        Get
            Return _link
        End Get
        Set(ByVal value As String)
            _link = value
        End Set
    End Property

    Public Property Incremental() As Integer
        Get
            Return _incremental
        End Get
        Set(ByVal value As Integer)
            _incremental = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Link = Request.QueryString("Link")
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub DG_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles DG.ItemCommand
        Dim Pratica As String = e.CommandName
        
        Dim script As String = String.Format("<SCRIPT language='javascript'>{0}window.focus();{0}CloseWindow('{1}');{0}</script>{0}", vbNewLine, Pratica)

        ClientScript.RegisterStartupScript(Me.GetType(), "Ritorna", Script)
    End Sub
#End Region

#Region " Private methods "

    Private Sub Initialize()
        Dim docObjects As IList(Of DocumentObject) = Facade.DocumentObjectFacade.GetDocumentObjectLink(Type, Link)

        Dim finder As NHibernateDocumentObjectFinder = Facade.DocumentObjectFinder

        DG.Finder = finder
        DG.DataSource = docObjects
        DG.VirtualItemCount = docObjects.Count

        DG.DataSource = docObjects
        DG.DataBind()
    End Sub

#End Region

End Class