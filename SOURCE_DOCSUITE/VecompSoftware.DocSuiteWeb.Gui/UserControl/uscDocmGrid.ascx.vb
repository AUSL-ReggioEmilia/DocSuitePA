Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscDocmGrid
    Inherits GridControl

    Private _ProtocolYear As Short
    Private _ProtocolNumber As Integer
    Private _ResolutionId As Short

    Public Property ProtocolYear() As Short
        Get
            Return _ProtocolYear
        End Get
        Set(ByVal value As Short)
            _ProtocolYear = value
        End Set
    End Property

    Public Property ProtocolNumber() As Integer
        Get
            Return _ProtocolNumber
        End Get
        Set(ByVal value As Integer)
            _ProtocolNumber = value
        End Set
    End Property

    Public Property ResolutionId() As Short
        Get
            Return _ResolutionId
        End Get
        Set(ByVal value As Short)
            _ResolutionId = value
        End Set
    End Property

    Public Property EnableGridScrolling As Boolean = True

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            gvDocuments.Columns.FindByUniqueName(COLUMN_DOCDESCRIPTION).Visible = ColumnDocDescriptionVisible
            AjaxManager.AjaxSettings.AddAjaxSetting(gvDocuments, gvDocuments)
        End If
    End Sub
#End Region

#Region "ColumnsName"
    Public Const COLUMN_CLIENT_SELECT As String = "ClientSelectColumn"
    Public Const COLUMN_ICON As String = "cIcon"
    Public Const COLUMN_DOCUMENT As String = "Id"
    Public Const COLUMN_STARTDATE As String = "StartDate"
    Public Const COLUMN_FOLDER_NAME As String = "FolderName"
    Public Const COLUMN_FOLDER_EXPIRYDATE As String = "FolderExpiryDate"
    Public Const COLUMN_FOLDER_EXPIRY_DESCRIPTION As String = "FolderExpiryDescription"
    Public Const COLUMN_CONTAINER As String = "Container.Name"
    Public Const COLUMN_ROLE As String = "Role.Name"
    Public Const COLUMN_CATEGORY As String = "Category.Name"
    Public Const COLUMN_CONTACT_DESCRIPTION As String = "ContactDescription"
    Public Const COLUMN_SERVICENUMBER As String = "ServiceNumber"
    Public Const COLUMN_NAME As String = "Name"
    Public Const COLUMN_OBJECT As String = "DocumentObject"
    Public Const COLUMN_MANAGER As String = "Manager"
    Public Const COLUMN_DOCDESCRIPTION As String = "DO.Description"
#End Region

#Region "Show/Hide Columns"
    ''' <summary>
    ''' Visualizza/Nasconde colonna di selezione
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ColumnClientSelectVisible() As Boolean
        Get
            Return gvDocuments.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible
        End Get
        Set(ByVal value As Boolean)
            gvDocuments.Columns.FindByUniqueName(COLUMN_CLIENT_SELECT).Visible = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde colonna nome documento
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ColumnDocDescriptionVisible() As Boolean
        Get
            Dim colVisible As Boolean = False
            If (ViewState(COLUMN_DOCDESCRIPTION) IsNot Nothing) Then
                colVisible = ViewState(COLUMN_DOCDESCRIPTION)
            End If
            Return colVisible
        End Get
        Set(ByVal value As Boolean)
            ViewState(COLUMN_DOCDESCRIPTION) = value
        End Set
    End Property
#End Region

#Region "Grid"
    Public ReadOnly Property Grid() As BindGrid
        Get
            Return Me.gvDocuments
        End Get
    End Property
#End Region

#Region "Grid Events"
    Private Sub gvDocuments_Init(ByVal sender As Object, ByVal e As EventArgs) Handles gvDocuments.Init
        gvDocuments.EnableScrolling = EnableGridScrolling
    End Sub

    Private Sub gvDocuments_ItemCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles gvDocuments.ItemCommand
        Dim s As String = String.Empty
        Dim arguments As String() = Nothing

        Select Case e.CommandName
            Case "ShowDocm"
                If Request.QueryString("chbStatusCancel") = "on" Then s &= "&StatusCancel=on"
                arguments = Split(e.CommandArgument, "|", 2)
                s = "Year=" & arguments(0) & "&Number=" & arguments(1) & "&Type=Docm"

                If ProtocolYear > 0 And ProtocolNumber > 0 Then
                    s = CommonShared.AppendSecurityCheck(s & "&txtSelYear=" & ProtocolYear & "&txtSelNumber=" & ProtocolNumber & "&Action=Insert&Redirect=True&Titolo=Aggiungi Collegamento Protocollo")
                    Dim url As String = "../Docm/DocmProtocollo.aspx?" & s
                    RedirectOnPage(url)
                    Exit Sub
                End If

                If ResolutionId > 0 Then
                    s = CommonShared.AppendSecurityCheck(s & "&txtSelId=" & ResolutionId & "&Action=Insert&Redirect=True&Titolo=Aggiungi Collegamento Atti")
                    Dim url As String = "../Docm/DocmAtti.aspx?" & s
                    RedirectOnPage(url)
                    Exit Sub
                End If

                RedirectOnPage("../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s))

            Case "ShowDocmFolder"
                If Request.QueryString("chbStatusCancel") = "on" Then s &= "&StatusCancel=on"
                arguments = Split(e.CommandArgument, "|", 3)
                s = "Year=" & arguments(0) & "&Number=" & arguments(1) & "&IncrementalFolder=" & arguments(2) & "&Type=Docm"
                RedirectOnPage("../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(s))
        End Select
    End Sub

    Private Sub gvDocuments_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles gvDocuments.ItemDataBound
        If (e.Item.ItemType = Telerik.Web.UI.GridItemType.Item Or e.Item.ItemType = Telerik.Web.UI.GridItemType.AlternatingItem) Then
            Dim oDocument As DocumentHeader = CType(e.Item.DataItem, DocumentHeader)
            Dim lnk As LinkButton = CType(e.Item, GridDataItem)(COLUMN_DOCUMENT).FindControl("lnkPratica")
            If RedirectOnParentPage Then
                Dim page As String = "Year=" & oDocument.Year & "&Number=" & oDocument.Number & "&Type=Docm"
                page = "../Docm/DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(page)
                lnk.OnClientClick = gvDocuments.GetRedirectParentPageScript(page)
            End If
            lnk.Text = oDocument.Document
            lnk.CommandArgument = oDocument.Year & "|" & oDocument.Number

            lnk = CType(e.Item, GridDataItem)(COLUMN_FOLDER_NAME).FindControl("lnkFolder")
            If Not IsNothing(lnk) Then
                lnk.Text = oDocument.FolderName
                lnk.CommandArgument = oDocument.Year & "|" & oDocument.Number & "|" & oDocument.FolderIncremental
            End If

            Dim img As Image = CType(e.Item, GridDataItem)(COLUMN_ICON).FindControl("Image1")
            img.ImageUrl = GetStatusIcon(oDocument.Status)

        End If
    End Sub
#End Region

#Region "Grid Binding"
    Public Function GetIconaDocm(ByVal DocmCount As Integer) As String
        Dim s As String = String.Empty
        Select Case DocmCount
            Case 0
                s = ImagePath.SmallEmpty
            Case 1
                s = "../Comm/images/docsuite/Pratica16.gif"
            Case Else
                s = "../Comm/images/docsuite/Pratiche16.gif"
        End Select
        Return s
    End Function

    Public Function GetStatusIcon(ByVal status As DocumentTabStatus) As String
        If status IsNot Nothing Then
            Select Case status.Id
                Case "PA"
                    Return "../Docm/Images/DocmAnnulla.gif"
                Case "CP"
                    Return "../Docm/Images/DocmChiusura.gif"
                Case "AR"
                    Return "../Docm/Images/DocmArchivia.gif"
            End Select
        End If
        Return "../Docm/Images/Pratica.gif"
    End Function
#End Region

#Region "Util Funtions"
    Public Sub SetDataFieldColumnSafe(ByVal columnName As String, ByVal dataField As String)
        Dim col As GridColumn = gvDocuments.Columns.FindByUniqueNameSafe(columnName)
        If col IsNot Nothing Then
            If TypeOf (col) Is GridBoundColumn Then
                CType(col, GridBoundColumn).DataField = dataField
            End If
        End If
    End Sub

    Public Sub DisableColumn(ByVal columnName As String)
        Dim col As GridColumn = gvDocuments.Columns.FindByUniqueNameSafe(columnName)
        If col IsNot Nothing Then
            Try
                'nasconde la colonna
                col.Visible = False
                ''elimina il bind dei dati della colonna
                If TypeOf (col) Is GridTemplateColumn Then
                    CType(col, GridTemplateColumn).ItemTemplate = Nothing
                End If

                If TypeOf (col) Is GridBoundColumn Then
                    CType(col, GridBoundColumn).DataField = String.Empty
                End If

            Catch ex As Exception
                Throw New Exception("Unable to disable column " & col.UniqueName)
            End Try
        End If
    End Sub
#End Region

End Class