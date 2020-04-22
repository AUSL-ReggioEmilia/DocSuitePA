Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public MustInherit Class BaseCommExport

    Protected _dirOutput As String = String.Empty

    Protected _listId As IList(Of YearNumberCompositeKey)

    Protected _sBiblosServer As String = String.Empty
    Protected _sBiblosArchivio As String = String.Empty

    Protected Shared _resultTable As DataTable
    Protected Shared _errorsTable As DataTable
    Protected Shared _errors As Long
    Protected Shared _exported As Long
    Protected Shared _total As Long

    Public Shared Property Errors() As Long
        Get
            Return _errors
        End Get
        Set(ByVal value As Long)
            _errors = value
        End Set
    End Property

    Public Shared Property Exported() As Long
        Get
            Return _exported
        End Get
        Set(ByVal value As Long)
            _exported = value
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

    Public Sub New(Optional ByVal dirOutput As String = "")
        _dirOutput = dirOutput
    End Sub
End Class