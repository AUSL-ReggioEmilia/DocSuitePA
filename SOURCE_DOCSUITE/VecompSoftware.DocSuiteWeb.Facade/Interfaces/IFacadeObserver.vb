Namespace Interfaces
    Public Interface IFacadeObserver(Of T)
        Sub Observe(target As T)
        Sub Disregard(target As T)
    End Interface
End Namespace