Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class CommonSelServiceCategory
    Inherits CommonBasePage

#Region " Fields "

    Private _titolo As String
    Private _nomecampodes As String
    Private _dbName As String

#End Region

#Region " Properties "

    Public ReadOnly Property Titolo() As String
        Get
            Return _titolo
        End Get
    End Property

    Public ReadOnly Property NomeCampoDes() As String
        Get
            Return _nomecampodes
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        MasterDocSuite.TitleVisible = False



        'Recupero il DB in base al Type
        Select Case Type
            Case "Docm" : _dbName = "DocmDB"
            Case "Prot" : _dbName = "ProtDB"
            Case "Resl" : _dbName = "ReslDB"
            Case "Comm" : _dbName = "ProtDB"
        End Select

        If Not IsPostBack Then
            Me.txtCercaDes.Focus()
        End If
    End Sub

    Private Sub BtnCerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnCerca.Click
        Search()
    End Sub

    Private Sub RadGridObject_ItemCreated(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles RadGridObject.ItemCreated
        Dim rowColor As String
        Dim rowAltColor As String

        Select Case Type
            Case "Docm"
                rowColor = "#e0ffff"
                rowAltColor = "#f0ffff"
            Case "Resl"
                rowColor = "#ffcc80"
                rowAltColor = "#ffe0c0"
            Case Else
                rowColor = "#dcdcdc"
                rowAltColor = "#f5f5f5"
        End Select

        Dim item As GridItemType = e.Item.ItemType
        Select Case item
            Case GridItemType.Item
                Dim row As TableRow = DirectCast(e.Item, TableRow)
                row.Attributes.Add("onmouseover", "this.style.backgroundColor = 'yellow';this.style.cursor = 'pointer';")
                row.Attributes.Add("onmouseout", "this.style.backgroundColor = '" & rowColor & "';")

            Case GridItemType.AlternatingItem
                Dim row As TableRow = DirectCast(e.Item, TableRow)
                row.Attributes.Add("onmouseover", "this.style.backgroundColor = 'yellow';this.style.cursor = 'pointer';")
                row.Attributes.Add("onmouseout", "this.style.backgroundColor = '" & rowAltColor & "';")

        End Select
    End Sub

    Protected Sub bntInserimento_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bntInserimento.Click
        Dim objectInstance As New ServiceCategory()
        objectInstance.Description = txtCercaDes.Text
        objectInstance.Code = txtCercaCode.Text

     

        Dim objectFacade As New ServiceCategoryFacade()
        objectFacade.Save(objectInstance, _dbName)

        Search()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(BtnCerca, RadGridObject, MasterDocSuite.AjaxLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(bntInserimento, RadGridObject, MasterDocSuite.AjaxLoadingPanel)
    End Sub

    Private Sub Search()
        Dim clause As NHibernateServiceCategoryDao.DescriptionSearchType
        Select Case rblClausola.SelectedItem.Value
            Case "AND"
                clause = NHibernateServiceCategoryDao.DescriptionSearchType.All
            Case "OR"
                clause = NHibernateServiceCategoryDao.DescriptionSearchType.One
        End Select

        Dim containerFacade As New ServiceCategoryFacade(_dbName)
        RadGridObject.DataSource = containerFacade.GetObjectByDescritpion(txtCercaDes.Text, clause, txtCercaCode.Text)
        RadGridObject.DataBind()
    End Sub

#End Region

End Class