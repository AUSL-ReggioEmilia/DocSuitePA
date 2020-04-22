'<Serializable()> _
'Public Class ADGroupRights

'#Region "private data"

'    Private _name As String
'    Private _protocolRights As ContainerProtocolRightType
'    Private _resolutionRights As String
'    Private _documentRights As String

'#End Region

'#Region "Properties"

'    Public Overridable Property Name() As String
'        Get
'            Return _name
'        End Get
'        Set(ByVal value As String)
'            _name = value
'        End Set
'    End Property

'    Public Overridable Property ProtocolRights() As ContainerProtocolRightType
'        Get
'            Return _protocolRights
'        End Get
'        Set(ByVal value As ContainerProtocolRightType)
'            _protocolRights = value
'        End Set
'    End Property

'    Public Overridable ReadOnly Property CanProtocolInsert() As Boolean
'        Get
'            Return ContainerProtocolRightType.Insert = ProtocolRights And ContainerProtocolRightType.Insert
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolModify() As Boolean
'        Get
'            Return ContainerProtocolRightType.Modify = ProtocolRights And ContainerProtocolRightType.Modify
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolViewDocuments() As Boolean
'        Get
'            Return ContainerProtocolRightType.ViewDocuments = ProtocolRights And ContainerProtocolRightType.ViewDocuments
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolRead() As Boolean
'        Get
'            Return ContainerProtocolRightType.Read = ProtocolRights And ContainerProtocolRightType.Read
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolCancel() As Boolean
'        Get
'            Return ContainerProtocolRightType.Cancel = ProtocolRights And ContainerProtocolRightType.Cancel
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolInteropIn() As Boolean
'        Get
'            Return ContainerProtocolRightType.InteropInRights = ProtocolRights And ContainerProtocolRightType.InteropInRights
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolInteropOut() As Boolean
'        Get
'            Return ContainerProtocolRightType.InteropOutRights = ProtocolRights And ContainerProtocolRightType.InteropOutRights
'        End Get
'    End Property

'    Public Overridable ReadOnly Property CanProtocolDocDistribution() As Boolean
'        Get
'            Return ContainerProtocolRightType.DocDistribution = ProtocolRights And ContainerProtocolRightType.DocDistribution
'        End Get
'    End Property

'#End Region

'#Region "Ctor/init"

'    Public Sub New()

'    End Sub

'#End Region

'#Region "Public methods"

'    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
'        Dim second As ADGroupRights = TryCast(obj, ADGroupRights)
'        If second IsNot Nothing Then
'            Return Me.Name = second.Name
'        End If
'        Return False
'    End Function

'    Public Shared Operator =(ByVal first As ADGroupRights, ByVal second As ADGroupRights) As Boolean
'        If [Object].ReferenceEquals(first, second) Then
'            Return True
'        End If
'        If DirectCast(first, Object) Is Nothing AndAlso DirectCast(second, Object) Is Nothing Then
'            Return True
'        End If
'        If DirectCast(first, Object) Is Nothing AndAlso DirectCast(second, Object) IsNot Nothing Then
'            Return False
'        End If

'        Return first.Equals(second)
'    End Operator

'    Public Shared Operator <>(ByVal first As ADGroupRights, ByVal second As ADGroupRights) As Boolean
'        Return Not (first = second)
'    End Operator

'    Public Overloads Overrides Function GetHashCode() As Integer
'        Return Name.GetHashCode()
'    End Function

'#End Region

'End Class


