
Public Interface IBiblosImporter(Of T, IdT)
    Function LoadRecords() As IList(Of T)
    Function ImportRecord(ByRef record As IdT) As Boolean
End Interface
