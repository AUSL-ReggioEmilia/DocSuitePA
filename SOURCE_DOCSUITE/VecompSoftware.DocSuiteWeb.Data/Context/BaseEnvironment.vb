Imports System.Collections.Specialized
Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports System.Data.SqlClient
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers.Security

Public Class BaseEnvironment

#Region " Fields "

    ''' <summary> Dizionario chiave/valore dei parametri caricati. </summary>
    Private _parameters As StringDictionary

    ''' <summary> Lista dei <see>ParameterEnv</see> caricati. </summary>
    Private _parametersList As IList(Of ParameterEnv)

    Private _connectionName As String
    Private _connectionString As String

    ''' <summary> Contesto dal quale ricavare impostazioni di configurazione. </summary>
    Private _context As DocSuiteContext

    Private Shared _jsonSerializerSettings As New JsonSerializerSettings() With {
        .NullValueHandling = NullValueHandling.Ignore, .TypeNameHandling = TypeNameHandling.Objects, .ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

#End Region

#Region " Constructors "

    Public Sub New(ByVal connectionName As String, ByRef context As DocSuiteContext)
        _connectionName = connectionName
        _context = context
        _parameters = New StringDictionary()
        _parametersList = New List(Of ParameterEnv)
    End Sub

    Public Sub New(ByVal connectionName As String, ByRef context As DocSuiteContext, ByRef parameters As IEnumerable(Of ParameterEnv))
        Me.New(connectionName, context)

        For Each param As ParameterEnv In parameters
            If (_parameters.ContainsKey(param.Id.Trim())) Then
                Continue For
            End If

            _parameters.Add(param.Id.Trim(), param.Value)
            _parametersList.Add(param)
        Next
    End Sub

#End Region

#Region " Properties "

    Public ReadOnly Property Context() As DocSuiteContext
        Get
            Return _context
        End Get
    End Property

    Public ReadOnly Property ConnectionName() As String
        Get
            Return _connectionName
        End Get
    End Property

    Public ReadOnly Property ParameterEnvList() As IList(Of ParameterEnv)
        Get
            Return _parametersList
        End Get
    End Property

    Public ReadOnly Property ConnectionString() As String
        Get
            If String.IsNullOrEmpty(_connectionString) Then
                Dim cn As ConnectionStringSettings = NHibernateSessionManager.Instance.SessionConnectionStrings(_connectionName)

                Dim constr As String = ""
                If cn IsNot Nothing Then
                    constr = cn.ConnectionString
                End If
                _connectionString = constr
            End If

            Return _connectionString
        End Get
    End Property

    Public ReadOnly Property DataBaseName() As String
        Get
            Return ExtractDatabaseNameFromConnectionString()
        End Get
    End Property

    Public ReadOnly Property Parameters() As StringDictionary
        Get
            Return _parameters
        End Get
    End Property

#End Region

#Region " Private Methods "

    Public Shared Function DatabaseNameFromConnectionString(connectionString As String) As String
        Dim databaseName As String = String.Empty
        If String.IsNullOrEmpty(connectionString) Then
            Return String.Empty
        End If

        Try
            Dim builder As SqlConnectionStringBuilder = New SqlConnectionStringBuilder(connectionString)
            databaseName = builder.InitialCatalog
        Catch
        End Try
        Return databaseName

    End Function

    Private Function ExtractDatabaseNameFromConnectionString() As String
        ' if this is a Jet database, find the index of the "data source" setting
        Dim startIndex As Integer
        If ConnectionString.IndexOf("jet.oledb", StringComparison.OrdinalIgnoreCase) > -1 Then
            startIndex = ConnectionString.IndexOf("data source=", StringComparison.OrdinalIgnoreCase)
            If startIndex > -1 Then
                startIndex += 12
            End If
        Else
            ' if this is a SQL Server database, find the index of the "initial 
            ' catalog" or "database" setting
            startIndex = ConnectionString.IndexOf("initial catalog=", StringComparison.OrdinalIgnoreCase)
            If startIndex > -1 Then
                startIndex += 16
            Else ' if the "Initial Catalog" setting is not found,
                '  try with "Database"
                startIndex = ConnectionString.IndexOf("database=", StringComparison.OrdinalIgnoreCase)
                If startIndex > -1 Then
                    startIndex += 9
                End If
            End If
        End If

        ' if the "database", "data source" or "initial catalog" values are not 
        ' found, return an empty string
        If startIndex = -1 Then
            Return String.Empty
        End If

        ' find where the database name/path ends
        Dim endIndex As Integer = ConnectionString.IndexOf(";", startIndex, StringComparison.OrdinalIgnoreCase)
        If endIndex = -1 Then
            endIndex = ConnectionString.Length
        End If

        ' return the substring with the database name/path
        Return ConnectionString.Substring(startIndex, endIndex - startIndex)
    End Function

#End Region

#Region " Public Methods "

    ''' <summary> Ritorna il valore del parametro in memoria. </summary>
    ''' <returns> Se presente dà la precedenza al parametro d'istanza, altrimenti al parametro ufficiale. </returns>
    Public Function GetParameter(ByVal key As String, Optional ByVal isEncrypted As Boolean = True) As String
        Dim value As String = Nothing

        Dim hasInstanceParameter As Boolean = False
        If Context.IsCustomInstance Then
            Dim instanceKey As String = DocSuiteContext.CustomInstanceName & key
            If Parameters.ContainsKey(instanceKey) Then
                value = Parameters.Item(instanceKey)
                If isEncrypted Then
                    value = DecryptParameterToken(value)
                End If
                hasInstanceParameter = True
            End If
        End If

        If Not hasInstanceParameter AndAlso Parameters.ContainsKey(key) Then
            value = Parameters.Item(key)
            If isEncrypted Then
                value = DecryptParameterToken(value)
            End If
        End If

        Return value
    End Function

    ''' <summary> Imposta il parametro in memoria. </summary>
    ''' <param name="value"> Valore del parametro, se Nothing il parametro viene rimosso. </param>
    Public Sub SetParameter(ByVal key As String, ByVal value As String)
        Dim realKey As String = If(Context.IsCustomInstance, DocSuiteContext.CustomInstanceName & key, key)

        If value Is Nothing Then
            ' cancellazione
            _parameters.Remove(realKey)

            For i As Integer = 0 To _parametersList.Count - 1 Step 1
                If _parametersList(i).Id.Eq(realKey) Then
                    _parametersList.Remove(_parametersList(i))
                    Exit For
                End If
            Next

        ElseIf _parameters.ContainsKey(realKey) Then
            ' modifica
            _parameters(realKey) = BaseEnvironment.EncryptParameterToken(value)

            For i As Integer = 0 To _parametersList.Count - 1 Step 1
                If _parametersList(i).Id.Eq(realKey) Then
                    _parametersList(i).Value = BaseEnvironment.EncryptParameterToken(value)
                    Exit For
                End If
            Next
        Else
            ' inserimento
            _parameters.Add(realKey, BaseEnvironment.EncryptParameterToken(value))

            Dim newParam As New ParameterEnv()
            newParam.Id = realKey
            newParam.Value = BaseEnvironment.EncryptParameterToken(value)
            _parametersList.Add(newParam)
        End If
    End Sub

    Public Function GetString(ByVal key As String, defaultValue As String) As String
        Dim temp As String = GetString(key)
        If String.IsNullOrEmpty(temp) Then
            temp = defaultValue
        End If
        Return temp
    End Function

    Public Function GetString(ByVal key As String) As String
        Return GetParameter(key)
    End Function
    Public Function GetJson(Of T)(ByVal key As String, defaultValue As String) As T
        Dim temp As String = GetString(key)
        If String.IsNullOrEmpty(temp) Then
            temp = defaultValue
        End If
        Dim deserialized As T = JsonConvert.DeserializeObject(Of T)(temp, _jsonSerializerSettings)
        Return deserialized
    End Function

    Public Function GetInteger(ByVal key As String) As Integer
        Return GetInteger(key, 0)
    End Function

    Public Function GetInteger(ByVal key As String, defaultValue As Integer) As Integer
        Dim temp As String = GetParameter(key)
        Dim value As Integer = 0
        If Not Integer.TryParse(temp, value) Then
            value = defaultValue
        End If
        Return value
    End Function

    Public Function GetNullableInt(ByVal key As String) As Integer?
        Dim temp As String = GetParameter(key)
        Dim value As Integer = 0
        If Not Integer.TryParse(temp, value) Then
            Return Nothing
        End If
        Return value
    End Function

    Public Function GetShort(ByVal key As String) As Short
        Return GetShort(key, -1)
    End Function

    Public Function GetShort(ByVal key As String, ByVal defaultValue As Short) As Short
        Dim temp As String = GetParameter(key)
        Dim value As Short = -1
        If Not Short.TryParse(temp, value) Then
            value = defaultValue
        End If
        Return value
    End Function

    Public Function GetBoolean(ByVal key As String) As Boolean
        Return GetBoolean(key, False)
    End Function

    Public Function GetBoolean(ByVal key As String, defaultValue As Boolean) As Boolean
        Dim temp As String = GetParameter(key)
        Dim value As Boolean = defaultValue
        If Not String.IsNullOrEmpty(temp) Then
            value = temp.Eq("1") OrElse temp.Eq("true")
        End If
        Return value
    End Function

    Public Function GetPipedString(ByVal key As String) As String
        Return GetPipedString(key, 0)
    End Function

    Public Function GetPipedString(ByVal key As String, ByVal index As Integer) As String
        Dim s As String = GetParameter(key)
        If String.IsNullOrEmpty(s) Then
            Return String.Empty
        End If
        Dim values As String() = s.Split("|"c)
        If values.Length < index + 1 Then
            Return String.Empty
        End If

        Return values(index)
    End Function

    ''' <summary>
    ''' Recupera una collezione di stringhe partendo da un parametro suddiviso da pipe
    ''' </summary>
    Public Function GetStrings(key As String) As ICollection(Of String)
        Dim values As String = GetParameter(key)
        If String.IsNullOrEmpty(values) Then Return New List(Of String)()

        Return values.Split("|".ToCharArray()).ToList()
    End Function

    ''' <summary>
    ''' Recupera una collezione di numeri interi partendo da un parametro suddiviso da pipe
    ''' </summary>
    Public Function GetIntegers(key As String) As ICollection(Of Integer)
        Return GetIntegers(key, String.Empty)
    End Function

    ''' <summary>
    ''' Recupera una collezione di numeri interi partendo da un parametro suddiviso da pipe
    ''' </summary>
    Public Function GetIntegers(key As String, defaultValue As String) As ICollection(Of Integer)
        Dim results As ICollection(Of Integer) = New List(Of Integer)

        Dim values As String = GetParameter(key)
        If String.IsNullOrEmpty(values) Then
            values = defaultValue
        End If
        TryGetIntegers(values, results)
        Return results
    End Function

    Public Shared Function TryGetIntegers(values As String, ByRef outputs As ICollection(Of Integer)) As Boolean
        outputs = New List(Of Integer)
        If String.IsNullOrEmpty(values) Then
            Return False
        End If

        Dim splittedValues As String() = values.Split("|".ToCharArray())
        Dim local As List(Of Integer) = New List(Of Integer)
        If Not splittedValues.All(Function(f) IsNumeric(f)) Then
            FileLogger.Warn(LogName.FileLog, "Alcuni valori non sono numerici.")
            Return False
        End If
        local.AddRange(splittedValues.Select(Function(x) Integer.Parse(x)))
        outputs = local
        Return True
    End Function

    ''' <summary> Metodo che ritorna l'oggetto <see>XmlEnvironment</see> dell'ambiente specificato. </summary>
    ''' <param name="environment">Ambiente</param>
    Public Shared Function GetXmlEnvironment(ByVal environment As EnvironmentDataCode) As XmlEnvironment
        ' nome del file di risorse xml
        Dim resourceXmlSignature As String
        Select Case environment
            Case EnvironmentDataCode.DocmDB
                resourceXmlSignature = "VecompSoftware.DocSuiteWeb.Data.EnvironmentPratiche.xml"
            Case EnvironmentDataCode.ProtDB
                resourceXmlSignature = "VecompSoftware.DocSuiteWeb.Data.EnvironmentProtocollo.xml"
            Case EnvironmentDataCode.ReslDB
                resourceXmlSignature = "VecompSoftware.DocSuiteWeb.Data.EnvironmentAtti.xml"
            Case Else
                Throw New NotImplementedException()
        End Select

        ' carico l'assembly corrente (contenente le risorse)
        Dim asm As Assembly = Assembly.GetAssembly(GetType(XmlEnvironment))
        Dim resource As Stream = asm.GetManifestResourceStream(resourceXmlSignature)
        If resource Is Nothing Then
            Throw New DocSuiteException("Ritiro XML ambientale", String.Format("Impossibile trovare il file di risorse [{0}].", resourceXmlSignature))
        End If

        Dim env As XmlEnvironment
        Using stream As New StreamReader(resource)
            Dim serializer As New XmlSerializer(GetType(XmlEnvironment), "www.vecompsoftware.it")
            env = DirectCast(serializer.Deserialize(stream), XmlEnvironment)
        End Using

        Return env
    End Function

    Public Shared Function EncryptParameterToken(ByVal parameterToken As String) As String
        If String.IsNullOrEmpty(parameterToken) Then
            Return Nothing
        End If
        Return EncryptionHelper.EncryptString(parameterToken, DocSuiteContext.PasswordEncryptionKey)
    End Function

    Public Shared Function DecryptParameterToken(ByVal parameterToken As String) As String
        If String.IsNullOrEmpty(parameterToken) Then
            Return Nothing
        End If
        Return EncryptionHelper.DecryptString(parameterToken, DocSuiteContext.PasswordEncryptionKey)
    End Function

#End Region

End Class

