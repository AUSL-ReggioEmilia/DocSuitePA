Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Public Class CommonSelOggetto
    Inherits CommBasePage

#Region " Fields "

    Private _dbName As String

#End Region

#Region " Properties "

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
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
            txtCercaDes.Focus()
        End If
    End Sub

    Private Sub BtnCerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BtnCerca.Click
        Search()
    End Sub

    Private Sub RadGridObject_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles RadGridObject.ItemCreated
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

    Protected Sub bntInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles bntInserimento.Click
        Dim objectInstance As New CommonObject()
        objectInstance.Description = txtCercaDes.Text
        objectInstance.Code = txtCercaCode.Text
        Facade.CommonObjectFacade.Save(objectInstance, _dbName)

        Search()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(BtnCerca, RadGridObject, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(bntInserimento, RadGridObject, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Search()
        Dim clause As NHibernateObjectDao.DescriptionSearchType
        Select Case rblClausola.SelectedItem.Value
            Case "AND"
                clause = NHibernateObjectDao.DescriptionSearchType.All
            Case "OR"
                clause = NHibernateObjectDao.DescriptionSearchType.One
        End Select

        RadGridObject.DataSource = Facade.CommonObjectFacade.GetObjectByDescritpion(txtCercaDes.Text, clause, txtCercaCode.Text)
        RadGridObject.DataBind()
    End Sub

#End Region

End Class