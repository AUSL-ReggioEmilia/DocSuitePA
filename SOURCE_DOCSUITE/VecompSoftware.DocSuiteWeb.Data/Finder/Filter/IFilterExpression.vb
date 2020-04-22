Public Interface IFilterExpression

    Property PropertyName() As String
    Property PropertyType() As Type
    Property FilterValue() As Object
    Property FilterExpression() As FilterExpression.FilterType
    Property SQLExpression() As String
    Property CriteriaImpl() As Object
End Interface
