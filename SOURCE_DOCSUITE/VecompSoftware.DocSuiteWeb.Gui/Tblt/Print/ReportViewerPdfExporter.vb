Imports System.IO
Imports Microsoft.Reporting.WebForms
Imports System.Data

Public Class ReportViewerPdfExporter
    Inherits BasePrintRpt

#Region "Fields"
    Private _rdlcFilenameSuffix As String
    Private _primaryTableName As String
    Private _dataSource As DataSet
#End Region

#Region "Properties"
    Public Property DataSource() As DataSet
        Get
            Return _dataSource
        End Get
        Set(ByVal value As DataSet)
            _dataSource = value
        End Set
    End Property

    Public Property PrimaryTableName() As String
        Get
            Return _primaryTableName
        End Get
        Set(ByVal value As String)
            _primaryTableName = value
        End Set
    End Property

    Public Property RdlcFilenameSuffix() As String
        Get
            Return _rdlcFilenameSuffix
        End Get
        Set(ByVal value As String)
            _rdlcFilenameSuffix = value
        End Set
    End Property
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        Dim primaryTableSource As DataTable

        If (_dataSource IsNot Nothing AndAlso _dataSource.Tables.Count > 0) Then
            If Not String.IsNullOrEmpty(_primaryTableName) Then
                primaryTableSource = _dataSource.Tables(_primaryTableName)
            Else
                primaryTableSource = _dataSource.Tables(0)
            End If

            Dim rdlcFilePath As String

            If Not String.IsNullOrEmpty(_rdlcFilenameSuffix) Then
                rdlcFilePath = RdlcPrint.Replace(".rdlc", "_" & _rdlcFilenameSuffix & ".rdlc")
                If File.Exists(rdlcFilePath) Then RdlcPrint = rdlcFilePath
            End If

            TablePrint.LocalReport.ReportPath = RdlcPrint
            TablePrint.LocalReport.EnableExternalImages = True
            TablePrint.LocalReport.DataSources.Clear()

            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(String.Format("{0}_{1}", _dataSource.DataSetName, primaryTableSource.TableName), primaryTableSource))
        Else
            Throw New ArgumentException("Ricerca Nulla")
        End If
    End Sub
#End Region

End Class
