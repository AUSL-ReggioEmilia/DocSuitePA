Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Partial Public Class uscDocumentFolder
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _document As Document
    Private _year As Short
    Private _number As Integer
    Private _add As String
    Private _incremental As Short
    Private _incrementalFolder As Short
    Private _action As String

#End Region

#Region " Properties "

    Public Property Document() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
        End Set
    End Property

    Public ReadOnly Property Destination() As RadTreeView
        Get
            Return tvwDestination
        End Get
    End Property

    Public ReadOnly Property Origin() As RadTreeView
        Get
            Return tvwOrigin
        End Get
    End Property

    Public Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Property Number() As Integer
        Get
            Return _number
        End Get
        Set(ByVal value As Integer)
            _number = value
        End Set
    End Property

    Public Property Incremental() As Short
        Get
            Return _incremental
        End Get
        Set(ByVal value As Short)
            _incremental = value
        End Set
    End Property

    Public Property IncrementalFolder() As Short
        Get
            Return _incrementalFolder
        End Get
        Set(ByVal value As Short)
            _incrementalFolder = value
        End Set
    End Property

    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property

    Public Property Add() As String
        Get
            Return _add
        End Get
        Set(ByVal value As String)
            _add = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        InitializeControls()
        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeControls()
        WebUtils.ExpandOnClientNodeAttachEvent(tvwOrigin)
        WebUtils.ExpandOnClientNodeAttachEvent(tvwDestination)
    End Sub

    Private Sub Initialize()
        Dim fullIncremental As String = String.Empty
        Dim idFolderRole As Integer
        Dim idFolderRoleIncremental As Short
        Dim titolo As String = String.Empty
        Dim iNewIncremental As Integer

        Dim yearNumberIncrementalKey As New YearNumberIncrCompositeKey(Year, Number, Incremental)
        Dim docFolder As DocumentFolder = Facade.DocumentFolderFacade.GetById(yearNumberIncrementalKey)

        If Not docFolder Is Nothing Then
            FncCalcolaPath(docFolder, idFolderRole, idFolderRoleIncremental, fullIncremental, titolo, iNewIncremental)

            FncDocumentFullIncremental(tvwOrigin, Year, Number, fullIncremental)

            FncDocumentFolder(tvwDestination, Year, Number, Document.Role, idFolderRoleIncremental, IncrementalFolder, True, Nothing)

        End If

    End Sub

    ''' <summary>ricostruisce l'albero dei documenti sulla treeview Destination</summary>
    Public Function FncDocumentFolder(ByRef tvw As RadTreeView, _
                    ByVal year As Short, ByVal number As Integer, _
                    ByVal role As Role, ByVal idRoleIncremental As Short, ByVal objIncremental As Integer, _
                    ByVal root As Boolean, ByVal nodo As RadTreeNode) As Boolean

        Dim tn As RadTreeNode

        If root Then
            tn = New RadTreeNode
            tn.Text = role.Name
            tn.Value = idRoleIncremental.ToString()
            tn.ImageUrl = "../Docm/images/RoleOnP.gif"
            tn.Expanded = True
            If idRoleIncremental = objIncremental Then
                tn.Style.Add("font-weight", "bold")
            End If
            tvw.Nodes(0).Expanded = True
            tvw.Nodes(0).Nodes.Add(tn)
            nodo = tn
        End If

        Dim docfolderroot As IList(Of DocumentFolder) = Facade.DocumentFolderFacade.GetRoot(year, number, idRoleIncremental)
        For Each folder As DocumentFolder In docfolderroot
            tn = New RadTreeNode
            If folder.Role Is Nothing Then
                tn.Text = folder.FolderName
                tn.Value = folder.Incremental.ToString()
                tn.ImageUrl = "../Comm/images/folderopen16.gif"
                tn.Expanded = True
                If folder.Incremental = objIncremental Then
                    tn.Font.Bold = True
                End If
                nodo.Nodes.Add(tn)
                FncDocumentFolder(tvw, year, number, role, folder.Incremental, objIncremental, False, tn)
            End If
        Next
        Return True
    End Function

    ''' <summary>ricostruisco il fullIncremental (C1|C2|C3) sull'ultimo nodo docFolder</summary>
    Public Function FncCalcolaPath(ByVal docFolder As DocumentFolder, ByRef idRole As Integer, ByRef idRoleIncremental As Short, _
                                ByRef fullIncremental As String, ByRef titolo As String, ByRef newIncremental As Integer) As Boolean

        If Not docFolder Is Nothing Then
            If titolo <> "" Then
                titolo = "/" & titolo
            End If
            If fullIncremental <> "" Then
                fullIncremental = "|" & fullIncremental
            End If
            fullIncremental = docFolder.Incremental & fullIncremental
            If docFolder.Role Is Nothing Then
                titolo = docFolder.FolderName & titolo
            Else
                titolo = docFolder.Role.Name & titolo
                If idRole = 0 Then
                    idRole = docFolder.Role.Id
                    idRoleIncremental = docFolder.Incremental
                End If
            End If
            If docFolder.IncrementalFather.HasValue Then
                Dim father As DocumentFolder = Facade.DocumentFolderFacade.GetById(docFolder.Year, docFolder.Number, docFolder.IncrementalFather.Value)
                FncCalcolaPath(father, idRole, idRoleIncremental, fullIncremental, titolo, docFolder.IncrementalFather.Value)
            End If
        End If

    End Function

    ''' <summary>
    ''' Rappresento il fullIncremental sull'ultimo nodo docFolder sulla treeview Origin
    ''' </summary>
    ''' <remarks>
    '''  C1
    '''    |_>C2
    '''        |_>C3
    ''' Che carino questo grafico!
    ''' </remarks>
    Public Shared Function FncDocumentFullIncremental(ByRef tvw As RadTreeView, ByVal year As Short, ByVal number As Integer, ByVal fullIncremental As String) As Boolean
        Dim nodo As RadTreeNode = tvw.Nodes(0)

        nodo.Expanded = True

        Dim a As Array = Split(fullIncremental, "|")
        Dim i As Integer
        Dim docFolderFacade As New DocumentFolderFacade

        For i = 0 To a.Length - 1
            Dim yearNumberIncrementalKey As New YearNumberIncrCompositeKey(year, number, CShort(a(i)))
            Dim docFolder As DocumentFolder = docFolderFacade.GetById(yearNumberIncrementalKey)

            If Not docFolder Is Nothing Then
                Dim tn As RadTreeNode = New RadTreeNode
                If Not docFolder.role Is Nothing Then
                    If "" & docFolder.role.Name <> "" Then
                        tn.Text = docFolder.role.Name
                        tn.ImageUrl = "../Docm/images/RoleOnP.gif"
                    Else
                        tn.Text = docFolder.FolderName
                        tn.ImageUrl = "../Comm/images/folderopen16.gif"
                    End If
                Else
                    tn.Text = docFolder.FolderName
                    tn.ImageUrl = "../Comm/images/folderopen16.gif"
                End If
                tn.Expanded = True
                If i = a.Length - 1 Then tn.Style.Add("font-weight", "bold")
                nodo.Nodes.Add(tn)
                nodo = tn
            End If
        Next i
    End Function

#End Region

End Class