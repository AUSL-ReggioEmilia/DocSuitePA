Imports System.IO
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade

Public MustInherit Class BaseCommImport : Implements ICommImport

    Protected _basePath As String = String.Empty
    Protected _metadata As DataTable
    Protected _dirInput As String = String.Empty
    Protected _dirOutput As String = String.Empty
    Protected _dirErrors As String = String.Empty
    Protected _protocol As Protocol
    Protected _inputFileNames() As String
    Protected _sBiblosServer As String = String.Empty
    Protected _sBiblosArchivio As String = String.Empty
    Protected _sImportSubFolder As String = String.Empty
    Protected _inputDirInfo As DirectoryInfo = Nothing
    Protected _outputDirErrorsInfo As DirectoryInfo = Nothing
    Protected _outputDirOutInfo As DirectoryInfo = Nothing
    Protected _idCategory As Integer
    Protected _idDocType As Integer
    Protected _idStatus As Integer
    Protected _idType As Integer
    Protected _idContainer As Integer
    Protected _sIdContainer As String = String.Empty
    Protected _idLocation As Integer
    Protected _sNote As String = String.Empty
    Protected _dtOptMetadata As DataTable

    Protected Shared _resultTable As DataTable
    Protected Shared _errorsTable As DataTable
    Protected Shared _errors As Long
    Protected Shared _imported As Long
    Protected Shared _total As Long

    Public Shared Property Errors() As Long
        Get
            Return _errors
        End Get
        Set(ByVal value As Long)
            _errors = value
        End Set
    End Property

    Public Shared Property Imported() As Long
        Get
            Return _imported
        End Get
        Set(ByVal value As Long)
            _imported = value
        End Set
    End Property

    Public Shared Property Total() As Long
        Get
            Return _total
        End Get
        Set(ByVal value As Long)
            _total = value
        End Set
    End Property

    Public Shared ReadOnly Property ResultTable() As DataTable
        Get
            Return _resultTable
        End Get
    End Property

    Public Shared ReadOnly Property ErrorsTable() As DataTable
        Get
            Return _errorsTable
        End Get
    End Property

    Public ReadOnly Property CurrentTenant As Tenant
        Get
            If HttpContext.Current.Session(CommonShared.USER_CURRENT_TENANT) IsNot Nothing Then
                Return DirectCast(HttpContext.Current.Session(CommonShared.USER_CURRENT_TENANT), Tenant)
            End If
            Return Nothing
        End Get
    End Property

    Public MustOverride Function CheckFiles() As DataTable Implements ICommImport.CheckFiles

    Public Sub New(Optional ByVal container As String = "", Optional ByVal type As String = "Lettera")

        Select Case type
            Case "Fattura"
                _basePath = DocSuiteContext.Current.ProtocolEnv.InvoicePathImport

                '#If DEBUG Then
                '                _basePath = "C:\temp\FastInput"
                '#End If

                _dirInput = _basePath & "\" & container & "\In"
                _dirOutput = _basePath & "\" & container & "\Out"
                _dirErrors = _basePath & "\" & container & "\Err"
            Case "Lettera"
                _basePath = DocSuiteContext.Current.ProtocolEnv.WordPathImport

                _dirInput = _basePath
                _dirOutput = _basePath & "\Out"
                _dirErrors = _basePath & "\Err"
            Case "Massiva"
                _basePath = DocSuiteContext.Current.ProtocolEnv.FastInputImportPath

                _dirInput = _basePath & "\Export"
                _dirOutput = _basePath & "\Out"
                _dirErrors = _basePath & "\Err"
            Case "Excel"
                _basePath = DocSuiteContext.Current.ProtocolEnv.ExcelPathImport

                _dirInput = _basePath
                _dirOutput = _basePath & "\Out"
                _dirErrors = _basePath & "\Err"
        End Select

        _metadata = New DataTable("MetaDati")
        _metadata.Columns.Add("KEY", System.Type.GetType("System.String"))
        _metadata.Columns.Add("VALUE", System.Type.GetType("System.String"))

    End Sub


End Class