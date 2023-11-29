Imports Newtonsoft.Json
Imports System.Text

<Serializable()> _
Partial Public Class Category
    Inherits DomainObject(Of Integer)
    Implements IAuditable
    Implements ISupportTenant

#Region " Fields "

#End Region

#Region " Properties "

    Public Overridable Property Name As String

    Public Overridable Property FullSearchComputed As String

    <JsonIgnore()>
    Public Overridable Property Parent As Category

    ''' <summary> Torna la categoria radice della sottocategoria. </summary>
    <JsonIgnore()>
    Public Overridable ReadOnly Property Root() As Category
        Get
            Return GetRoot()
        End Get
    End Property

    Public Overridable Property IsActive As Boolean

    Public Overridable Property Code As Integer

    Public Overridable Property FullIncrementalPath As String

    Public Overridable Property FullCode As String

    <JsonIgnore()>
    Public Overridable ReadOnly Property FullCodeDotted() As String
        Get

            Dim sRet As New StringBuilder()
            Dim sArr As String() = FullCode.Split("|"c)
            For Each s As String In sArr
                sRet.AppendFormat("{0}.", s.TrimStart("0"c))
            Next
            If (sRet.Length > 0) Then
                sRet.Remove(sRet.Length - 1, 1)
            End If
            Return sRet.ToString()
        End Get
    End Property

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    <JsonIgnore()>
    Public Overridable Property Children As IList(Of Category)

    <JsonIgnore()>
    Public Overridable ReadOnly Property HasChildren As Boolean
        Get
            Return (Not Children Is Nothing AndAlso Children.Count > 0)
        End Get
    End Property

    <JsonIgnore()>
    Public Overridable ReadOnly Property Level() As Integer
        Get
            Return FullIncrementalPath.Split("|"c).Length - 1
        End Get
    End Property

    'TODO: Questa gestione dovrebbe essere presente solo lato Web API
    Public Overridable Property IdMassimarioScarto As Guid?
    Public Overridable Property StartDate As DateTimeOffset
    Public Overridable Property EndDate As DateTimeOffset?
    Public Overridable Property CategorySchema As CategorySchema
    Public Overridable Property IdMetadataRepository As Guid?
    Public Overridable Property IdTenantAOO As Guid? Implements ISupportTenant.IdTenantAOO

#End Region

#Region " Constructors "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

#Region " Methods "

    Public Overridable Function GetFullName() As String
        If Code = 0 Then
            Return Name
        End If
        Return $"{Code}.{Name}"
    End Function

    Private Function GetRoot() As Category
        If Parent Is Nothing OrElse Parent.Code.Equals(0) Then
            Return Me
        End If
        Return Parent.GetRoot()
    End Function

#End Region

End Class

