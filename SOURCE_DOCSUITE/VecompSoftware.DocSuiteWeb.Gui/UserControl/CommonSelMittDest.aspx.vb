Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class CommonSelMittDest
    Inherits CommBasePage

#Region "Fields"
    Private _dbName As String
#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Setto l'ajax per controllare la grid con la dropdownlist
        AjaxManager.AjaxSettings.AddAjaxSetting(lista, RadGridDistributionList, MasterDocSuite.AjaxLoadingPanel)

        'Recupero il DB in base al Type
        Select Case Type
            Case "Docm" : _dbName = "DocmDB"
            Case "Prot" : _dbName = "ProtDB"
            Case "Resl" : _dbName = "ReslDB"
            Case "Comm" : _dbName = "ProtDB"
        End Select

        'Se non è un postback
        If Not Me.IsPostBack Then
            Initialize()
        End If

    End Sub

    Private Sub lista_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lista.SelectedIndexChanged
        'Recupero l'id
        Dim Sel As String = lista.Items(lista.SelectedIndex).Value
        InitializeGrid(Sel) 'aggiorno la grid
    End Sub

    Private Sub RadGridObject_ItemCreated(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles RadGridDistributionList.ItemCreated

        Dim item As GridItemType
        Dim row As TableRow
        Dim rowColor As String = String.Empty
        Dim rowAltColor As String = String.Empty

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

        item = e.Item.ItemType
        Select Case item
            Case GridItemType.Item
                row = CType(e.Item, TableRow)
                row.Attributes.Add("onmouseover", "this.style.backgroundColor = 'yellow';this.style.cursor = 'pointer';")
                row.Attributes.Add("onmouseout", "this.style.backgroundColor = '" & rowColor & "';")

            Case GridItemType.AlternatingItem
                row = CType(e.Item, TableRow)
                row.Attributes.Add("onmouseover", "this.style.backgroundColor = 'yellow';this.style.cursor = 'pointer';")
                row.Attributes.Add("onmouseout", "this.style.backgroundColor = '" & rowAltColor & "';")
            Case Else

        End Select
    End Sub
#End Region

#Region "Functions"

    Private Sub Initialize()
        'Istanzio il Facade
        Dim distFacade As DistributionListFacade = New DistributionListFacade(_dbName)
        Dim DistList As IList = distFacade.GetAllOrderedByName() 'recupero la lista

        'Aggiungo un item vuoto
        WebUtils.ObjDropDownListAdd(lista, "", "")
        'Aggiungo tutti gli item
        For Each dis As DistributionList In DistList
            WebUtils.ObjDropDownListAdd(lista, dis.Name, dis.Id.ToString())
        Next

        'innizializzo la grid
        InitializeGrid()

    End Sub

    Private Sub InitializeGrid(Optional ByVal id As String = "")
        If String.IsNullOrEmpty(id) Then
            'Istanzio il Facade
            Dim recFacade As RecipientFacade = New RecipientFacade(_dbName)
            Dim RecList As IList = recFacade.GetAll() 'recupero la lista
            'Bindo la lista sulla grid
            RadGridDistributionList.DataSource = RecList
            RadGridDistributionList.DataBind()
        Else
            'Istanzio il Facade
            Dim distFacade As DistributionListFacade = New DistributionListFacade(_dbName)
            Dim Dist As DistributionList = distFacade.GetById(id) 'recupero

            'Bindo la lista sulla grid
            RadGridDistributionList.DataSource = Dist.Recipients
            RadGridDistributionList.DataBind()
        End If
    End Sub

    Public Function SetMenu() As String
        SetMenu = String.Empty
        If Type = "Comm" Then
            ' TODO: aggiungere al RadScriptManager
            SetMenu = "<script language=""javascript"" src=""../js/NascondiMenu.js""></script>"
        End If
    End Function

    Public Function SetType() As String
        Return Type
    End Function

#End Region

End Class


