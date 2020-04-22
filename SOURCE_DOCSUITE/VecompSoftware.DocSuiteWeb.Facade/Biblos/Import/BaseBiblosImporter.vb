
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public MustInherit Class BaseBiblosImporter(Of T, IdT)
    Implements IBiblosImporter(Of T, IdT)

#Region "Fields"
    Protected _recordsToImport As IList(Of T)
    Protected _biblosHelper As BiblosImporterHelper
#End Region

#Region "Constructor"
    Public Sub New()
        'Inizializzo il Context
        'DocSuiteContext.Current.InitContext(False)
        _biblosHelper = New BiblosImporterHelper()
    End Sub
#End Region

    Public Function ImportRecord(ByRef recordID As IdT) As Boolean Implements IBiblosImporter(Of T, IdT).ImportRecord
        Dim documents As IList(Of BiblosDocumentInfo) = Nothing
        Dim record As Object = Nothing
        Try
            record = LoadRecord(recordID)
            documents = GetRecordDocuments(record)
            If documents IsNot Nothing AndAlso documents.Count > 0 Then
                For Each document As BiblosDocumentInfo In documents
                    Import(document, record)
                Next
            Else
                Return False
            End If
        Catch ex As Exception
            ImportRecord = False
            Throw New DocSuiteException("Errore durante l'importazione", ex)
        End Try

        Return True
    End Function

    Public MustOverride Function LoadRecords() As IList(Of T) Implements IBiblosImporter(Of T, IdT).LoadRecords
    Public MustOverride Function LoadRecord(ByVal recordID As IdT) As T
    Public MustOverride Sub Import(ByRef document As BiblosDocumentInfo, ByRef record As T)
    Public MustOverride Function GetRecordDocuments(ByRef record As T, Optional ByVal extensions() As String = Nothing) As IList(Of BiblosDocumentInfo)
End Class
