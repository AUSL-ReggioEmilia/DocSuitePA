Imports VecompSoftware.DocSuiteWeb.Facade
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary>
''' Classe base per la costruzione di un stampa
''' </summary>
''' <remarks></remarks>
Public MustInherit Class BasePrintRpt
    Implements IPrintRpt

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
    Private _table As ReportViewer
    Private _title As String
    Private _rdlc As String
    Private _finder As NHibernateResolutionFinder
#End Region

#Region "Table"
    ''' <summary>
    ''' Restituisce la stampa identificata da una tabella
    ''' </summary>
    ''' <value>DSTable</value>
    ''' <returns>DSTable</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TablePrint() As ReportViewer Implements IPrintRpt.TablePrint
        Get
            If _table Is Nothing Then
                _table = New ReportViewer()
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
    Public Property TitlePrint() As String Implements IPrintRpt.TitlePrint
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    ''' <summary>
    ''' Restituice o imposta il template della stampa
    ''' </summary>
    ''' <value>Template della stampa</value>
    ''' <returns>TEmplate della stampa</returns>
    ''' <remarks></remarks>
    Public Property RdlcPrint() As String Implements IPrintRpt.RdlcPrint
        Get
            Return _rdlc
        End Get
        Set(ByVal value As String)
            _rdlc = value
        End Set
    End Property
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
    Public MustOverride Sub DoPrint() Implements IPrintRpt.DoPrint
End Class
