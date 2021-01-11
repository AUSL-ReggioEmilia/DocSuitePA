Imports System.Linq.Expressions

''' <summary>
''' Interfaccia per un generico Finder
''' </summary>
''' <remarks></remarks>
Public Interface IFinder
    ''' <summary>
    ''' Paginazione
    ''' </summary>
    Property PageIndex As Integer

    ''' <summary>
    ''' Numero elementi per pagina
    ''' </summary>
    Property PageSize As Integer

    ''' <summary>
    ''' Indice della pagina che si sta visualizzando
    ''' </summary>
    Property CustomPageIndex As Integer

    ''' <summary>
    ''' Conteggio risultati
    ''' </summary>
    Function Count() As Integer

    'Ordinamento
    ReadOnly Property SortExpressions() As IDictionary(Of String, String)
    ReadOnly Property FilterExpressions() As IDictionary(Of String, IFilterExpression)
End Interface

''' <summary>
''' Interfaccia per un generico Finder
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks></remarks>
Public Interface IFinder(Of T)
    Inherits IFinder

    'Ricerca
    Function DoSearch() As IList(Of T)
    Function DoSearch(ByVal sortExpr As String) As IList(Of T)
    Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal PageSize As Integer) As IList(Of T)

    Function DoSearch(sortExpr As ICollection(Of Expression(Of Func(Of T, Object)))) As ICollection(Of T)

    Function DoSearch(sortExpr As ICollection(Of Expression(Of Func(Of T, Object))), startRow As Integer, pageSize As Integer) As ICollection(Of T)

End Interface
