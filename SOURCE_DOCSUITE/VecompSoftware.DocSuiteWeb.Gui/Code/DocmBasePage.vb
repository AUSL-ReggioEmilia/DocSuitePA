Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmBasePage
    Inherits CommonBasePage

#Region " Fields "

    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Docm"

    Private _docUtil As DocumentUtil
    Private _currentDocumentYear As Short?
    Private _currentDocumentNumber As Integer?
    Private _currentDocument As Document

#End Region

#Region " Properties "

    Public ReadOnly Property CurrentDocumentYear As Short
        Get
            If Not _currentDocumentYear.HasValue Then
                _currentDocumentYear = GetKeyValue(Of Short)("Year")
            End If
            Return _currentDocumentYear.Value
        End Get
    End Property

    Public ReadOnly Property CurrentDocumentNumber As Integer
        Get
            If Not _currentDocumentNumber.HasValue Then
                _currentDocumentNumber = GetKeyValue(Of Integer)("Number")
            End If
            Return _currentDocumentNumber.Value
        End Get
    End Property

    Public ReadOnly Property CurrentDocument() As Document
        Get
            If _currentDocument Is Nothing Then
                _currentDocument = Facade.DocumentFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber)
            End If
            Return _currentDocument
        End Get
    End Property

    Public ReadOnly Property DocUtil() As DocumentUtil
        Get
            If _docUtil Is Nothing Then
                _docUtil = DocumentUtil.GetInstance()
            End If
            Return _docUtil
        End Get
    End Property

    Public ReadOnly Property DocumentEnv() As DocumentEnv
        Get
            Return DocSuiteContext.Current.DocumentEnv
        End Get
    End Property

    Public ReadOnly Property CurrentYear As String
        Get
            Return Request.QueryString("txtSelYear")
        End Get
    End Property

    Public ReadOnly Property CurrentNumber As String
        Get
            Return Request.QueryString("txtSelNumber")
        End Get
    End Property

#End Region

#Region " Methods "

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, DocmBasePage)(key)
    End Function


    Public Sub RegisterFolderRefreshScript(ByVal incrementalFocus As String)
        AjaxManager.ResponseScripts.Add("parent.FolderRefreshIncr('" & incrementalFocus & "');")
    End Sub

    Public Sub RegisterFolderRefreshScript()
        AjaxManager.ResponseScripts.Add("FolderRefresh();")
    End Sub

    Public Sub RegisterFolderRefreshParentScript()
        AjaxManager.ResponseScripts.Add("parent.FolderRefresh();")
    End Sub

    ''' <summary>Script per l'aggiornamento del sommario</summary>
    Public Sub RegisterFolderRefreshFullScript()
        AjaxManager.ResponseScripts.Add("parent.FolderRefreshFull();")
    End Sub

#End Region

End Class
