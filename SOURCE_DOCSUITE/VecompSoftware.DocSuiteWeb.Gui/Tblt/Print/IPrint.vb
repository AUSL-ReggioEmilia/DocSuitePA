Public Interface IPrint

    ReadOnly Property TablePrint() As DSTable
    Property TitlePrint() As String

    Property MantainResultsOrder As Boolean

    Sub DoPrint()

End Interface
