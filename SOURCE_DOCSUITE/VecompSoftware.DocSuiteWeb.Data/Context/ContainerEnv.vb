

Imports System.Linq
Imports Newtonsoft.Json

Public Class ContainerEnv

#Region " Fields "

    Private _context As DocSuiteContext
    Private _container As Container
    Private _propertiesList As IList(Of ContainerProperty)
    Private Shared _jsonSerializerSettings As New JsonSerializerSettings() With {
        .NullValueHandling = NullValueHandling.Ignore, .TypeNameHandling = TypeNameHandling.Objects, .ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

#End Region

#Region " Constructors "

    Public Sub New(context As DocSuiteContext, ByRef container As Container)
        Dim _dao As New NHibernateContainerPropertyDao()
        _context = context
        _container = container
        _propertiesList = container.ContainerProperties
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property Context() As DocSuiteContext
        Get
            Return _context
        End Get
    End Property

    Public ReadOnly Property SelectedContainer() As Container
        Get
            Return _container
        End Get
    End Property

    Public ReadOnly Property PropertiesList() As IList(Of ContainerProperty)
        Get
            Return _propertiesList
        End Get
    End Property

    Public ReadOnly Property InvoicePAContactSDI As String
        Get
            If _context.ProtocolEnv.IsInvoiceEnabled Then
                Return GetString("InvoicePAContactSDI", String.Empty)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property InvoicePAMailboxSenderId As Short?
        Get
            If _context.ProtocolEnv.IsInvoiceEnabled Then
                Return GetShort("InvoicePAMailboxSenderId", Nothing)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ProtocolKindContainers As String
        Get
            If _context.ProtocolEnv.IsInvoiceEnabled Then
                Return GetString("ProtocolKindContainers", String.Empty)
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property ImportInvoiceContactGroupName As String
        Get
            If _context.ProtocolEnv.IsInvoiceEnabled Then
                Return GetString("ImportInvoiceContactGroupName", String.Empty)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property LinkedContainers As IList(Of Integer)
        Get
            Return GetJson(Of IList(Of Integer))("LinkedContainers", String.Empty)
        End Get
    End Property
#End Region

#Region " Methods "

    Private Function GetGeneric(Of T)(keyName As String, defaultValue As T, eval As Func(Of ContainerProperty, T)) As T
        Dim ret As T = defaultValue
        Dim key As String = keyName
        If Context.IsCustomInstance Then
            key = String.Concat(DocSuiteContext.CustomInstanceName, keyName)
        End If
        Dim value As ContainerProperty = PropertiesList.SingleOrDefault(Function(x) x.Name.Equals(key))
        If (value IsNot Nothing) Then
            ret = eval(value)
        End If
        Return ret
    End Function

    Private Function GetString(keyName As String, defaultValue As String) As String
        Return GetGeneric(Of String)(keyName, defaultValue, Function(x) x.ValueString)
    End Function

    Private Function GetInt(keyName As String, defaultValue As Long?) As Long?
        Return GetGeneric(Of Long?)(keyName, defaultValue, Function(x) x.ValueInt)
    End Function
    Private Function GetShort(keyName As String, defaultValue As Short?) As Short?
        Return GetGeneric(Of Short?)(keyName, defaultValue, Function(x) Convert.ToInt16(x.ValueInt))
    End Function

    Private Function GetDoubl(keyName As String, defaultValue As Double?) As Double?
        Return GetGeneric(Of Double?)(keyName, defaultValue, Function(x) x.ValueDouble)
    End Function

    Private Function GetBoolean(keyName As String, defaultValue As Boolean) As Boolean?
        Return GetGeneric(Of Boolean?)(keyName, defaultValue, Function(x) x.ValueBoolean)
    End Function

    Private Function GetGuid(keyName As String, defaultValue As Guid?) As Guid?
        Return GetGeneric(Of Guid?)(keyName, defaultValue, Function(x) x.ValueGuid)
    End Function

    Private Function GetDateTime(keyName As String, defaultValue As DateTime?) As DateTime?
        Return GetGeneric(Of DateTime?)(keyName, defaultValue, Function(x) x.ValueDate)
    End Function

    Public Function GetJson(Of T)(ByVal key As String, defaultValue As String) As T
        Dim temp As String = GetString(key, defaultValue)
        Dim deserialized As T = JsonConvert.DeserializeObject(Of T)(temp, _jsonSerializerSettings)
        Return deserialized
    End Function

#End Region

End Class