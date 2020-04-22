Imports System
Imports System.Configuration
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.NHibernateManager.SessionFactory
Imports VecompSoftware.NHibernateManager.Config
Imports NHibernate
Imports NHibernate.Mapping
Imports VecompSoftware.NHibernateManager
Imports System.Text.RegularExpressions


''' <summary>
''' Classe messa a disposizione per determinare le tabelle mancanti nel db, ma presenti nel mapping (NHibernate)
''' </summary>
''' <remarks></remarks>
Public Class CommonMapping

    Private Class RowMapping
        Private _dbname As String
        Private _classname As String
        Private _fieldname As String
        Private _classdatatype As String
        Private _tablename As String
        Private _dbdatatype As String
        Private _status As Boolean
        Private _reason As String

        Public Sub New(ByVal DbName As String, ByVal ClassName As String, ByVal FieldName As String, ByVal ClassDataType As String, ByVal TableName As String, ByVal DBDataType As String, ByVal Status As Boolean, ByVal Reason As String)
            _dbname = DbName
            _classname = ClassName
            _fieldname = FieldName
            _classdatatype = ClassDataType
            _tablename = TableName
            _dbdatatype = DBDataType
            _status = Status
            _reason = Reason
        End Sub

        Public Property DBName() As String
            Get
                Return _dbname
            End Get
            Set(ByVal value As String)
                _dbname = value
            End Set
        End Property

        Public Property ClassName() As String
            Get
                Return _classname
            End Get
            Set(ByVal value As String)
                _classname = value
            End Set
        End Property
        Public Property FieldName() As String
            Get
                Return _fieldname
            End Get
            Set(ByVal value As String)
                _fieldname = value
            End Set
        End Property

        Public Property ClassDataType() As String
            Get
                Return _classdatatype
            End Get
            Set(ByVal value As String)
                _classdatatype = value
            End Set
        End Property
        Public Property TableName() As String
            Get
                Return _tablename
            End Get
            Set(ByVal value As String)
                _tablename = value
            End Set
        End Property
        Public Property DbDataType() As String
            Get
                Return _dbdatatype
            End Get
            Set(ByVal value As String)
                _dbdatatype = value
            End Set
        End Property

        Public Property Status() As Boolean
            Get
                Return _status
            End Get
            Set(ByVal value As Boolean)
                _status = value
            End Set
        End Property

        Public Property Reason() As String
            Get
                Return _reason
            End Get
            Set(ByVal value As String)
                _reason = value
            End Set
        End Property
    End Class

#Region " Fields "

    Private RowsMapping As ArrayList
    Private Const namePattern As String = "[\[\]]"

#End Region

#Region " Constructors "

    ''' <summary> Crea una nuova CommonMapping </summary>
    Public Sub New()
        RowsMapping = New ArrayList()
    End Sub

#End Region

#Region " Methods "

    Public Function CheckMapping(ByVal p_dbName As String) As ArrayList
        Dim currentConfig As NHibernate.Cfg.Configuration = GetSingleConfigurationFor(p_dbName)
        Dim rowToAdd As RowMapping = Nothing
        For Each c As PersistentClass In currentConfig.ClassMappings
            Select Case c.GetType.ToString
                Case "NHibernate.Mapping.RootClass"
                    CheckRootClass(p_dbName, c)
                Case "NHibernate.Mapping.SingleTableSubclass"
                    CheckSingleTableSubclass(p_dbName, c)
                Case Else
                    rowToAdd = New RowMapping(p_dbName, c.GetType.ToString, c.GetType.ToString, String.Empty, c.GetType.ToString, String.Empty, False, c.GetType.ToString)
                    RowsMapping.Add(rowToAdd)
            End Select
        Next
        Return RowsMapping
    End Function

    Private Function GetTableInfo(ByVal p_dbName As String, ByVal p_tableName As String) As IList
        Dim s As String = "select upper(column_name) as column_name from information_schema.columns where table_name='{0}' order by ordinal_position asc"
        s = String.Format(s, p_tableName)

        Dim session As ISession = NHibernateSessionManager.Instance.GetSessionFrom(p_dbName)
        Dim q As IQuery = session.CreateSQLQuery(s)

        Return q.List
    End Function

    Private Function GetSingleConfigurationFor(ByVal p_dbName As String) As NHibernate.Cfg.Configuration
        Dim retval As NHibernate.Cfg.Configuration
        Dim session As OpenSessionInViewSection = TryCast(ConfigurationManager.GetSection("nhibernateSettings"), OpenSessionInViewSection)
        Dim element As SessionFactoryElement = session.SessionFactories(p_dbName)

        If session Is Nothing Then
            Throw New DocSuiteException("Configurazione NHibernate") With {.Descrizione = "Impossibile trovare la sezione nhibernateSettings nel ConfigurationManager."}
        End If
        If element Is Nothing Then
            Throw New DocSuiteException("Configurazione NHibernate") With {.Descrizione = String.Format("The SessionFactory with name '{0}' was not found with ConfigurationManager.", p_dbName)}
        End If

        Dim configurator As INHibernateConfiguration = CType(Activator.CreateInstance(Type.GetType(element.Configurator)), INHibernateConfiguration)
        retval = configurator.GetSingleConfigurationFor(element)

        Return retval
    End Function

    Private Sub CheckRootClass(ByVal p_dbName As String, ByVal p_rClass As RootClass)
        Dim tableInfo As IList = GetTableInfo(p_dbName, p_rClass.Table.Name)
        Dim currentColumn As Column
        Dim rowToAdd As RowMapping = Nothing
        If tableInfo.Count > 0 Then
            For Each p As Mapping.Property In p_rClass.PropertyIterator
                For Each c As ISelectable In p.ColumnIterator
                    If Not c.IsFormula Then
                        currentColumn = c
                        Dim columnName As String = Regex.Replace(currentColumn.Name, namePattern, String.Empty).ToUpperInvariant()
                        If Not tableInfo.Contains(columnName) Then
                            rowToAdd = New RowMapping(p_dbName, p_rClass.ClassName, currentColumn.Name, String.Empty, p_rClass.Table.Name, String.Empty, False, "Colonna mancante")
                            RowsMapping.Add(rowToAdd)
                        End If
                    End If
                Next
            Next
        Else
            Select Case p_rClass.Table.Name.ToUpper
                Case "INFORMATION_SCHEMA.COLUMNS" ' tabelle da ignorare
                Case Else
                    rowToAdd = New RowMapping(p_dbName, p_rClass.ClassName, String.Empty, String.Empty, p_rClass.Table.Name, String.Empty, False, "Tabella mancante")
                    RowsMapping.Add(rowToAdd)
            End Select
        End If
    End Sub

    Private Sub CheckSingleTableSubclass(ByVal p_dbName As String, ByVal p_stClass As SingleTableSubclass)
        Dim tableInfo As IList = GetTableInfo(p_dbName, p_stClass.Table.Name)
        Dim currentColumn As Column
        Dim rowToAdd As RowMapping = Nothing
        If tableInfo.Count > 0 Then
            For Each p As NHibernate.Mapping.Property In p_stClass.PropertyIterator
                For Each c As ISelectable In p.ColumnIterator
                    currentColumn = c
                    Dim columnName As String = Regex.Replace(currentColumn.Name, namePattern, String.Empty).ToUpperInvariant()
                    If Not tableInfo.Contains(columnName) Then
                        rowToAdd = New RowMapping(p_dbName, p_stClass.ClassName, currentColumn.Name, String.Empty, p_stClass.Table.Name, String.Empty, False, "Colonna mancante")
                        RowsMapping.Add(rowToAdd)
                    End If
                Next
            Next
        Else
            Select Case p_stClass.Table.Name.ToUpper
                Case "INFORMATION_SCHEMA.COLUMNS" ' tabelle da ignorare
                Case Else
                    rowToAdd = New RowMapping(p_dbName, p_stClass.ClassName, String.Empty, String.Empty, p_stClass.Table.Name, String.Empty, False, "Tabella mancante")
                    RowsMapping.Add(rowToAdd)
            End Select
        End If
    End Sub


#End Region

End Class
