Public Class Fascicle
    Inherits AuditableDomainObject(Of Guid)

#Region "Properties"

    Public Overridable Property Year As Short
    Public Overridable Property Number As Integer
    Public Overridable Property Conservation As Short?
    Public Overridable Property StartDate As DateTimeOffset
    Public Overridable Property EndDate As DateTimeOffset?
    Public Overridable Property Title As String
    Public Overridable Property Name As String
    Public Overridable Property FascicleObject As String
    Public Overridable Property Manager As String
    Public Overridable Property Rack As String
    Public Overridable Property Note As String
    Public Overridable Property FascicleType As FascicleType
    Public Overridable Property VisibilityType As VisibilityType
    Public Overridable Property DSWEnvironment As Integer?
    Public Overridable Property Category As Category
    Public Overridable Property MetadataValues As String
    Public Overridable Property IdMetadataRepository As Guid?
    Public Overridable Property FascicleDocumentUnits As IList(Of FascicleDocumentUnit)
    Public Overridable Property Container As Container

    Public Overridable ReadOnly Property CalculatedLink As String
        Get
            Return Year.ToString & "|" & Category.Id.ToString() & "|" & Number & "|" & String.Format("{0:dd/MM/yyyy}", RegistrationDate)
        End Get
    End Property
#End Region

#Region "Ctor/init"
    Public Sub New()
        FascicleDocumentUnits = New List(Of FascicleDocumentUnit)
    End Sub
#End Region
End Class


