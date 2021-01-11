Public Interface IInsertController(Of T)

    Sub Initialize()
    Sub InitializeDelegates()
    Sub AttachEvents()
    Sub BindDataToObject(ByRef domainObject As T)

    Function ValidateData(ByRef errorMessage As String) As Boolean
End Interface
