Imports System.Configuration
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports NHibernate.Cfg.MappingSchema
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Config
Imports VecompSoftware.NHibernateManager.SessionFactory
Imports Configuration = NHibernate.Cfg.Configuration

Public NotInheritable Class NHibernateConfiguration
    Implements INHibernateConfiguration

#Region "Fields"

    Private _cfg As Configuration
    Private ReadOnly _vAssembly As Assembly = Nothing
    Private _factory As SessionFactoryElement
    Private _xmlDocumentsCommon As IList(Of XmlDocument) = Nothing
    Private ReadOnly _xmlDocumentsProtocol As IList(Of XmlDocument) = Nothing
    Private ReadOnly _xmlDocumentsResolution As IList(Of XmlDocument) = Nothing
    Private ReadOnly _xmlDocumentsDocument As IList(Of XmlDocument) = Nothing
    Private _protocolMapping As Nullable(Of MappingType)
    Private _resolutionMapping As Nullable(Of MappingType)
    Private _documentMapping As Nullable(Of MappingType)
    Private _connectionSettingsProtocol As ConnectionStringSettings = Nothing
    Private _connectionSettingsResolution As ConnectionStringSettings = Nothing
    Private _connectionSettingsDocument As ConnectionStringSettings = Nothing

    Private Const _xpath_FindNavigation As String = "/nh:hibernate-mapping/nh:class/nh:many-to-one[@class='{0}']"
    Private Const _xpath_FindBag As String = "/nh:hibernate-mapping/nh:class/nh:bag[@table='{0}']"
    Private Const _xpath_Classes As String = "/nh:hibernate-mapping/nh:class[@table or @name]"
    Private Const _xpath_HibernateMapping As String = "/nh:hibernate-mapping"
    Private Const _maniefestParameter As String = "VecompSoftware.DocSuiteWeb.Data.ParameterEnv.hbm.xml"
    Private Const _maniefestCommon As String = "VecompSoftware.DocSuiteWeb.Data.Common.hbm.xml"
    Private Const _maniefestProtocols As String = "VecompSoftware.DocSuiteWeb.Data.Protocol_"
    Private Const _maniefestResolutions As String = "VecompSoftware.DocSuiteWeb.Data.Resolution_"
    Private Const _maniefestDocuments As String = "VecompSoftware.DocSuiteWeb.Data.Document_"
    Private Const _urn_hibernateMapping As String = "urn:nhibernate-mapping-2.2"

#End Region

#Region "Constructors"

    Public Sub New()
        _cfg = New Configuration()
        _vAssembly = AppDomain.CurrentDomain.GetAssemblies().First(Function(a) a.FullName.StartsWith("VecompSoftware.DocSuiteWeb.Data, ", StringComparison.InvariantCultureIgnoreCase))

        _xmlDocumentsProtocol = GetMappingXmlDocument(_vAssembly, String.Empty, _maniefestProtocols, If(HasProtocol, BaseEnvironment.DatabaseNameFromConnectionString(_connectionSettingsProtocol.ConnectionString), String.Empty))
        _xmlDocumentsResolution = GetMappingXmlDocument(_vAssembly, String.Empty, _maniefestResolutions, If(HasResolution, BaseEnvironment.DatabaseNameFromConnectionString(_connectionSettingsResolution.ConnectionString), String.Empty))
        _xmlDocumentsDocument = GetMappingXmlDocument(_vAssembly, String.Empty, _maniefestDocuments, If(HasDocument, BaseEnvironment.DatabaseNameFromConnectionString(_connectionSettingsDocument.ConnectionString), String.Empty))
    End Sub

#End Region

#Region "Properties"
    Friend ReadOnly Property CommonMapping() As IList(Of XmlDocument)
        Get
            If _xmlDocumentsCommon Is Nothing Then
                _xmlDocumentsCommon = CleanNavigations(GetMappingXmlDocument(_vAssembly, _maniefestCommon))
            End If
            Return _xmlDocumentsCommon
        End Get
    End Property

    Friend ReadOnly Property ProtocolMapping() As Nullable(Of MappingType)
        Get
            If Not _protocolMapping.HasValue Then
                If HasProtocol Then

                    _protocolMapping = New MappingType() With {
                        .Mappings = CleanNavigations(_xmlDocumentsProtocol),
                        .AssemblyNames = GetAssemblyNames(_xmlDocumentsProtocol)
                    }
                End If
            End If
            Return _protocolMapping
        End Get
    End Property

    Friend ReadOnly Property ResolutionMapping() As Nullable(Of MappingType)
        Get
            If Not _resolutionMapping.HasValue Then
                If HasResolution Then
                    _resolutionMapping = New MappingType() With {
                        .Mappings = CleanNavigations(_xmlDocumentsResolution),
                        .AssemblyNames = GetAssemblyNames(_xmlDocumentsResolution)
                    }
                End If
            End If
            Return _resolutionMapping
        End Get
    End Property

    Friend ReadOnly Property DocumentMapping() As Nullable(Of MappingType)
        Get
            If Not _documentMapping.HasValue Then
                If HasDocument Then
                    _documentMapping = New MappingType() With {
                        .Mappings = CleanNavigations(_xmlDocumentsDocument),
                        .AssemblyNames = GetAssemblyNames(_xmlDocumentsDocument)
                    }
                End If
            End If
            Return _documentMapping
        End Get
    End Property

    Friend ReadOnly Property HasProtocol() As [Boolean]
        Get
            If _connectionSettingsProtocol Is Nothing Then
                _connectionSettingsProtocol = NHibernateSessionManager.Instance.SessionConnectionStrings("ProtConnection")
            End If
            Return _connectionSettingsProtocol IsNot Nothing
        End Get
    End Property

    Friend ReadOnly Property HasResolution() As [Boolean]
        Get
            If _connectionSettingsResolution Is Nothing Then
                _connectionSettingsResolution = NHibernateSessionManager.Instance.SessionConnectionStrings("ReslConnection")
            End If
            Return _connectionSettingsResolution IsNot Nothing
        End Get
    End Property

    Friend ReadOnly Property HasDocument() As [Boolean]
        Get
            If _connectionSettingsDocument Is Nothing Then
                _connectionSettingsDocument = NHibernateSessionManager.Instance.SessionConnectionStrings("DocmConnection")
            End If
            Return _connectionSettingsDocument IsNot Nothing
        End Get
    End Property

#End Region

#Region "Methods"
    Public Function GetConfigurationFor(factoryElement As SessionFactoryElement) As Configuration Implements INHibernateConfiguration.GetConfigurationFor

        _factory = factoryElement

        _cfg.Configure(CompleteFactoryConfigPath(factoryElement))
        _cfg.AddResource(_maniefestParameter, _vAssembly)

        Dim mapping As New MappingType
        mapping.AssemblyNames = New List(Of String)
        mapping.Mappings = New List(Of XmlDocument)
        mapping.Mappings = mapping.Mappings.Concat(CommonMapping).ToList()
        If (ProtocolMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(ProtocolMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(ProtocolMapping.Value.AssemblyNames).ToList()
        End If
        If (ResolutionMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(ResolutionMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(ResolutionMapping.Value.AssemblyNames).ToList()
        End If
        If (DocumentMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(DocumentMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(DocumentMapping.Value.AssemblyNames).ToList()
        End If
        mapping.AssemblyNames = mapping.AssemblyNames.Distinct().ToList()
        _cfg = MappingNHConfiguration(_cfg, _vAssembly, mapping)
        Return _cfg
    End Function

    Public Function GetSingleConfigurationFor(factoryElement As SessionFactoryElement) As Configuration Implements INHibernateConfiguration.GetSingleConfigurationFor
        Dim retval As New Configuration()

        _factory = factoryElement
        retval.Configure(CompleteFactoryConfigPath(factoryElement))

        Dim mapping As New MappingType
        mapping.AssemblyNames = New List(Of String)
        mapping.Mappings = New List(Of XmlDocument)
        mapping.Mappings = mapping.Mappings.Concat(CommonMapping).ToList()
        If (factoryElement IsNot Nothing AndAlso System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB).Equals(factoryElement.Name) AndAlso ProtocolMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(ProtocolMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(ProtocolMapping.Value.AssemblyNames).ToList()
        End If
        If (factoryElement IsNot Nothing AndAlso System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB).Equals(factoryElement.Name) AndAlso ResolutionMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(ResolutionMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(ResolutionMapping.Value.AssemblyNames).ToList()
        End If
        If (factoryElement IsNot Nothing AndAlso System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB).Equals(factoryElement.Name) AndAlso DocumentMapping.HasValue) Then
            mapping.Mappings = mapping.Mappings.Concat(DocumentMapping.Value.Mappings).ToList()
            mapping.AssemblyNames = mapping.AssemblyNames.Concat(DocumentMapping.Value.AssemblyNames).ToList()
        End If

        Return MappingNHConfiguration(retval, _vAssembly, mapping)
    End Function

    Private Function MappingNHConfiguration(nhConfiguration As Configuration, paramsAssembly As Assembly, mapping As MappingType) As Configuration
        Dim name As String = paramsAssembly.GetName().Name
        mapping.AssemblyNames = mapping.AssemblyNames.Where(Function(f) Not name.Equals(f)).ToList()
        For Each assemblyName As String In mapping.AssemblyNames
            nhConfiguration.AddAssembly(Assembly.Load(assemblyName))
        Next
        For Each xmlDocument As XmlDocument In mapping.Mappings
            Try
                nhConfiguration.AddDocument(xmlDocument)
            Catch ex As Exception
            End Try

        Next
        Return nhConfiguration
    End Function

    Private Function GetAssemblyNames(mappings As ICollection(Of XmlDocument)) As ICollection(Of String)
        Dim results As IList(Of String) = New List(Of String)
        Dim serializer As XmlSerializer = New XmlSerializer(GetType(HbmMapping))
        Dim assemplyNames As IList(Of String) = New List(Of String)
        Dim hbmMapping As HbmMapping
        For Each mapping As XmlDocument In mappings
            Using stream As New MemoryStream
                mapping.Save(stream)
                stream.Flush()
                stream.Position = 0
                hbmMapping = DirectCast(serializer.Deserialize(stream), HbmMapping)
                results.Add(hbmMapping.assembly)
            End Using
        Next
        Return results
    End Function

    Private Function GetMappingXmlDocument(pAssembly As Assembly, manifestResourceName As String, Optional manifestResourcesName As String = "", Optional databaseName As String = "") As IList(Of XmlDocument)
        Dim mappings As IList(Of XmlDocument) = New List(Of XmlDocument)
        Dim mapping As XmlDocument

        Dim results As ICollection(Of String) = New List(Of String)
        If (Not String.IsNullOrEmpty(manifestResourceName)) Then
            results.Add(manifestResourceName)
        End If
        If (Not String.IsNullOrEmpty(manifestResourcesName)) Then
            results = results.Concat(pAssembly.GetManifestResourceNames().Where(Function(s) s.StartsWith(manifestResourcesName) AndAlso s.EndsWith("hbm.xml"))).ToList()
        End If

        For Each manifestName As String In results
            Using stream As Stream = pAssembly.GetManifestResourceStream(manifestName)
                mapping = New XmlDocument()
                mapping.Load(stream)
            End Using
            mappings.Add(mapping)
            If Not String.IsNullOrEmpty(databaseName) Then
                Dim [nameSpace] As XmlNamespaceManager = GetNamespaceManager(mapping)
                Dim mappingNode As XmlNode = mapping.SelectSingleNode(_xpath_HibernateMapping, [nameSpace])
                If mappingNode IsNot Nothing Then
                    Dim catalog As XmlAttribute = mappingNode.Attributes("catalog")
                    If catalog Is Nothing Then
                        catalog = mappingNode.OwnerDocument.CreateAttribute("catalog", [nameSpace].DefaultNamespace)
                        mappingNode.Attributes.Append(catalog)
                    End If
                    catalog.Value = databaseName
                End If
            End If
        Next
        Return mappings
    End Function

    Private Function CleanNavigations(sources As IList(Of XmlDocument)) As IList(Of XmlDocument)
        If Not HasProtocol Then
            sources = RemoveFoundNavigations(sources, _xmlDocumentsProtocol)
        End If
        If Not HasResolution Then
            sources = RemoveFoundNavigations(sources, _xmlDocumentsResolution)
        End If
        If Not HasDocument Then
            sources = RemoveFoundNavigations(sources, _xmlDocumentsDocument)
        End If
        Return sources
    End Function

    Private Function RemoveFoundNavigations(sources As IList(Of XmlDocument), deleteEntities As ICollection(Of XmlDocument)) As IList(Of XmlDocument)
        Dim source As XmlDocument
        Dim tableName As String = String.Empty
        Dim className As String = String.Empty
        Dim results As XmlNodeList = Nothing
        Dim entityBagsNameSpace As XmlNamespaceManager
        Dim sourceNameSpace As XmlNamespaceManager

        For index As Integer = 0 To sources.Count - 1
            source = sources.ElementAt(index)
            sourceNameSpace = GetNamespaceManager(source)
            For Each deleteMapping As XmlDocument In deleteEntities
                entityBagsNameSpace = GetNamespaceManager(deleteMapping)
                For Each entityToRemove As XmlNode In deleteMapping.SelectNodes(_xpath_Classes, entityBagsNameSpace)
                    If TryGetTableAttribute(entityToRemove, tableName) AndAlso TryGetNameAttribute(entityToRemove, className) Then
                        results = source.SelectNodes(String.Format(_xpath_FindNavigation, className), sourceNameSpace)
                        RemoveItems(results)
                        results = source.SelectNodes(String.Format(_xpath_FindBag, tableName), sourceNameSpace)
                        RemoveItems(results)
                    End If
                Next
            Next
            sources(index) = source
        Next

        Return sources
    End Function
    Private Sub RemoveItems(results As XmlNodeList)
        If results Is Nothing OrElse results.Count <= 0 Then
            Return
        End If
        For i As Integer = results.Count - 1 To 0 Step -1
            results(i).ParentNode.RemoveChild(results(i))
        Next
    End Sub
#End Region

#Region "Static Methods"

    Private Shared Function GetCustomizatorName() As String
        Return ConfigurationManager.AppSettings("DocSuiteWeb.CustomizatorName")
    End Function

    Private Shared Function GetNamespaceManager(oDoc As XmlDocument) As XmlNamespaceManager
        Dim nsmgr As New XmlNamespaceManager(oDoc.NameTable)
        nsmgr.AddNamespace("nh", _urn_hibernateMapping)
        Return nsmgr
    End Function

    Private Shared Function TryGetTableAttribute(node As XmlNode, ByRef tableName As String) As [Boolean]
        tableName = String.Empty
        Dim retState As [Boolean] = False
        Dim name As XmlAttribute = Nothing
        Try
            If (InlineAssignHelper(name, node.Attributes("table"))) IsNot Nothing Then
                tableName = name.Value.Trim()
                retState = True
            Else
                retState = TryGetNameAttribute(node, tableName)
            End If
        Catch generatedExceptionName As Exception
        End Try
        Return retState
    End Function

    Private Shared Function TryGetNameAttribute(node As XmlNode, ByRef bagName As String) As [Boolean]
        bagName = String.Empty
        Dim retState As [Boolean] = False
        Dim name As XmlAttribute = Nothing
        Try
            If (InlineAssignHelper(name, node.Attributes("name"))) IsNot Nothing Then
                bagName = name.Value.Trim()
                retState = True
            End If
        Catch generatedExceptionName As Exception
        End Try
        Return retState
    End Function
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function

    Private Function CompleteFactoryConfigPath(factoryElement As SessionFactoryElement) As String
        Dim factoryConfigPath As String = factoryElement.FactoryConfigPath
        factoryConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, factoryConfigPath.Replace("/"c, "\"c))

        If Not File.Exists(factoryConfigPath) Then
            Throw New DocSuiteException("NHibernateConfiguration") With {
                .Descrizione = String.Format("Impossibile trovare il seguente file di configurazione '{0}'", factoryConfigPath)
            }
        End If

        Return factoryConfigPath
    End Function
#End Region

End Class