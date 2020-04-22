Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic

Partial Public Class UtltProtCambia
    Inherits UtltBasePage

#Region "Field"

    Dim idCategory As String = String.Empty
    Dim idContainer As String = String.Empty


#End Region

#Region "Page Load"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        '--
        idCategory = Request.QueryString("idCategory")
        idContainer = Request.QueryString("idContainer")
        '--
        WebUtils.ExpandOnClientNodeAttachEvent(tvwOldCategory)
        If Not Me.IsPostBack Then
            Initialize()
            InitializeControl()
        End If

    End Sub

#End Region

#Region "Initialize"

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Private Sub Initialize()

        tblRicerca.Visible = False

        Select Case Action
            Case "Category"
                Me.Title = "Protocollo Utilità - Modifica Classificazione"
                pnlContenitore.Visible = False
                ObjTreeViewCategoryAdd(tvwOldCategory, Nothing, idCategory, True, True, True, "", True)

            Case "Container"
                Me.Title = "Protocollo Utilità - Modifica Contenitore"
                pnlCategoria.Visible = False
                Dim contenitore As New Container
                Dim contenitori As IList(Of ContainerRightsDto)

                contenitore = Facade.ContainerFacade.GetById(idContainer, False, "ProtDB")
                contenitori = Facade.ContainerFacade.GetAllRights("Prot", 1)

                If contenitori.Count > 0 Then
                    WebUtils.ObjDropDownListAdd(ddlNewContainer, "", "")
                    For Each contenitore1 As ContainerRightsDto In contenitori
                        If contenitore1.LocationId = contenitore.ProtLocation.Id Then
                            If contenitore1.ContainerId = CShort(idContainer) Then
                                WebUtils.ObjDropDownListAdd(ddlOldContainer, contenitore1.Name, contenitore1.ContainerId.ToString())
                                ddlOldContainer.SelectedIndex = 0
                            Else
                                WebUtils.ObjDropDownListAdd(ddlNewContainer, contenitore1.Name, contenitore1.ContainerId.ToString())
                            End If
                        End If
                    Next
                End If
        End Select
    End Sub


    Private Sub InitializeControl()
        With UscClassificatore1
            .HeaderVisible = False
        End With
    End Sub

#End Region

#Region "Private Methods"
    Private Function ObjTreeViewCategoryAdd( _
          ByRef node As RadTreeView, ByVal nodoFiglio As RadTreeNode, _
          ByRef idCategory As String, _
          ByVal existRootNode As Boolean, _
          Optional ByVal nodeExpanded As Boolean = True, _
          Optional ByVal fontBold As Boolean = True, _
          Optional ByVal fontColor As String = "", _
          Optional ByVal insertFullIncremental As Boolean = False) As Boolean

        Dim nodoToAdd As New RadTreeNode
        Dim nodoExist As New RadTreeNode
        Dim categoria As Category = Facade.CategoryFacade.GetById(CShort(idCategory))

        If Not categoria Is Nothing Then
            Dim sFull As String = If(insertFullIncremental, "|" & Replace(categoria.FullIncrementalPath, "|", ","), "")
            nodoToAdd.Text = categoria.GetFullName()
            nodoToAdd.Value = categoria.Id & sFull

            If Not WebUtils.ObjTreeViewExistNode(node, nodoToAdd.Value, "", nodoExist) Then
                If categoria.Parent Is Nothing Then 'Aggiungo in primo livello
                    If existRootNode Then
                        node.Nodes(0).Nodes.Add(nodoToAdd)
                    Else
                        node.Nodes.Add(nodoToAdd)
                    End If
                Else
                    ObjTreeViewCategoryAdd(node, nodoToAdd, categoria.Parent.Id, existRootNode, nodeExpanded, fontBold, fontColor, insertFullIncremental)
                End If
                If categoria.Parent Is Nothing Then
                    nodoToAdd.ImageUrl = "../Comm/images/Classificatore.gif"
                Else
                    nodoToAdd.ImageUrl = "../Comm/images/folderopen16.gif"
                End If
                If categoria.IsActive <> 1 Then
                    nodoToAdd.Style.Add("color", "gray")
                Else
                    nodoToAdd.Style.Remove("color")
                End If
            Else
                nodoToAdd = nodoExist
            End If

            nodoToAdd.Expanded = True
        End If
        If Not IsNothing(nodoFiglio) Then
            nodoToAdd.Nodes.Add(nodoFiglio)
        Else
            If fontBold Then
                nodoToAdd.Style.Add("font-weight", "bold")
            End If
            If fontColor <> "" Then
                nodoToAdd.Style.Add("color", fontColor)
            End If
        End If
    End Function

#End Region

#Region "Button Events"

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConferma.Click


        Select Case Action
            Case "Category"
                If UscClassificatore1.HasSelectedCategories Then
                    Me.MasterDocSuite.AjaxManager.ResponseScripts.Add("OnClick('" & Action & "|" & UscClassificatore1.SelectedCategories.First().Id & "');")
                End If
            Case "Container"
                Me.MasterDocSuite.AjaxManager.ResponseScripts.Add("OnClick('" & Action & "|" & ddlNewContainer.SelectedValue & "');")
        End Select

    End Sub

#End Region

End Class