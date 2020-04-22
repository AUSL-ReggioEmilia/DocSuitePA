Public Interface IFacadeBase(Of T, TKey, TResult)
    Function GetById(id As TKey) As TResult
    Function GetAll() As IList(Of TResult)
    Function Count() As Integer
End Interface
