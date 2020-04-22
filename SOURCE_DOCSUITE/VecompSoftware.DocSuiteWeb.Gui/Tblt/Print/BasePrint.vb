Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary>
''' Classe base per la costruzione di un stampa
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public MustInherit Class BasePrint
    Implements IPrint

#Region "Facade Factory"
    Private _facade As FacadeFactory
    ''' <summary>
    ''' FacadeFactory
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property Facade() As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory()
            End If
            Return _facade
        End Get
    End Property
#End Region

#Region "Private Fields"
    Private _table As DSTable
    Private _title As String
    Private _finder As NHibernateResolutionFinder
#End Region

#Region "Table"
    ''' <summary>
    ''' Restituisce la stampa identificata da una tabella
    ''' </summary>
    ''' <value>DSTable</value>
    ''' <returns>DSTable</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TablePrint() As DSTable Implements IPrint.TablePrint
        Get
            If _table Is Nothing Then
                _table = New DSTable()
            End If
            Return _table
        End Get
    End Property

    ''' <summary>
    ''' Restituice o imposta il titolo della stampa
    ''' </summary>
    ''' <value>Titolo della stampa</value>
    ''' <returns>Titolo della stampa</returns>
    ''' <remarks></remarks>
    Public Property TitlePrint() As String Implements IPrint.TitlePrint
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Public Property MantainResultsOrder As Boolean Implements IPrint.MantainResultsOrder

#End Region
#Region "Properties"
    Public Property Finder() As NHibernateResolutionFinder
        Get
            If _finder IsNot Nothing AndAlso _finder.EagerLog Then
                _finder.EagerLog = False
            End If
            Return _finder
        End Get
        Set(ByVal value As NHibernateResolutionFinder)
            _finder = value
        End Set
    End Property
#End Region
    ''' <summary>
    ''' Esegue la stampa
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub DoPrint() Implements IPrint.DoPrint
End Class
