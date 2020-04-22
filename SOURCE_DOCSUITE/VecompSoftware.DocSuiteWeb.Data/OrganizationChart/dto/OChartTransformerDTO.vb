Imports System
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.API

Public Class OChartTransformerDTO

#Region " Constructors "

    Public Sub New()
    End Sub
    Public Sub New(dto As OrgDeptDTO)
        RegistrationDate = dto.DataUltimoUpdate
        StartDate = dto.DataCreazione
        EndDate = dto.DataDismissione
        Code = dto.CodUO
        Title = dto.Nome
        Description = dto.Nome
        Imported = True

        If Not dto.IsRoot Then
            Parent = New OChartTransformerDTO() With {
                .StartDate = StartDate,
                .EndDate = EndDate,
                .Code = dto.CodUOPadre,
                .Imported = True
            }
        End If
    End Sub

#End Region

#Region " Enums "

    Public Enum TransformTypes
        Undefined = 0
        Recode = 1
        SaveOrUpdate = 2
        Delete = 3
        RemoveResources = 4
    End Enum

#End Region

#Region " Properties "

    Public Property RegistrationDate As DateTime?

    Public Property StartDate As DateTime?
    Public Property EndDate As DateTime?
    Public Property Reparenting As Boolean?
    Public Property OldCode As String
    Public Property Code As String
    Public Property Title As String
    Public Property Description As String
    Public Property Acronym As String
    Public Property Imported As Boolean?
    Public Property Parent As OChartTransformerDTO

    Public Property Contacts As IEnumerable(Of Contact)
    Public Property Containers As IEnumerable(Of OChartItemContainer)
    Public Property Mailboxes As IEnumerable(Of PECMailBox)
    Public Property Roles As IEnumerable(Of Role)

    Public ReadOnly Property RegistrationDateOrDefault As DateTime
        Get
            Return RegistrationDate.GetValueOrDefault(DateTime.MinValue)
        End Get
    End Property

    Public ReadOnly Property Type As TransformTypes
        Get
            Select Case True
                Case StartDate.HasValue
                    Return TransformTypes.SaveOrUpdate
                Case EndDate.HasValue AndAlso HasResources
                    Return TransformTypes.RemoveResources
                Case EndDate.HasValue
                    Return TransformTypes.Delete
                Case Not StartDate.HasValue AndAlso Not EndDate.HasValue, Not String.IsNullOrEmpty(OldCode)
                    Return TransformTypes.Recode
            End Select
            Return TransformTypes.Undefined
        End Get
    End Property
    Public ReadOnly Property RequiresReplication As Boolean
        Get
            Return Type.Equals(TransformTypes.SaveOrUpdate) OrElse Type.Equals(TransformTypes.Delete)
        End Get
    End Property
    Public ReadOnly Property ReferenceDate As DateTime?
        Get
            If StartDate.HasValue Then
                Return StartDate.Value.Date
            End If
            If EndDate.HasValue Then
                Return EndDate.Value.Date
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property ReferenceDateOrDefault As DateTime
        Get
            Return ReferenceDate.GetValueOrDefault(DateTime.MinValue)
        End Get
    End Property
    Public ReadOnly Property IsReparenting As Boolean
        Get
            Return Reparenting.GetValueOrDefault(True)
        End Get
    End Property
    Public ReadOnly Property PaddedCode As String
        Get
            Return Code.PadLeft(20, "0"c)
        End Get
    End Property
    Public ReadOnly Property IsImported As Boolean
        Get
            Return Imported.GetValueOrDefault(False)
        End Get
    End Property
    Public ReadOnly Property IsRoot As Boolean
        Get
            Return Parent Is Nothing
        End Get
    End Property

    Public ReadOnly Property HasContacts As Boolean
        Get
            Return Not Contacts.IsNullOrEmpty()
        End Get
    End Property
    Public ReadOnly Property HasContainers As Boolean
        Get
            Return Not Containers.IsNullOrEmpty()
        End Get
    End Property
    Public ReadOnly Property HasMailboxes As Boolean
        Get
            Return Not Mailboxes.IsNullOrEmpty()
        End Get
    End Property
    Public ReadOnly Property HasRoles As Boolean
        Get
            Return Not Roles.IsNullOrEmpty()
        End Get
    End Property

    Public ReadOnly Property HasResources As Boolean
        Get
            Return HasContacts OrElse HasContainers OrElse HasMailboxes OrElse HasRoles
        End Get
    End Property

#End Region

#Region " Methods "

    Public Function Validate() As Boolean
        Return Not String.IsNullOrEmpty(Code) AndAlso Not Type.Equals(TransformTypes.Undefined)
    End Function

    Public Function Comply(item As OChartItem) As Boolean
        Select Case Type
            Case TransformTypes.SaveOrUpdate, TransformTypes.Delete, TransformTypes.RemoveResources
                Return Code.Equals(item.Code) AndAlso ComplyHeader(item.OrganizationChart)
            Case TransformTypes.Recode
                Return (OldCode.Equals(item.Code) OrElse Title.Equals(item.Title, StringComparison.InvariantCultureIgnoreCase)) _
                    AndAlso Not Code.Equals(item.Code) AndAlso ComplyHeader(item.OrganizationChart)
        End Select
        Return False
    End Function
    Public Function Comply(items As IEnumerable(Of OChartItem)) As Boolean
        Return items.Any(Function(i) Me.Comply(i))
    End Function
    Public Function ComplyRecursively(items As IEnumerable(Of OChartItem)) As Boolean
        If Me.Comply(items) Then
            Return True
        End If
        Return items.Where(Function(i) i.HasItems).Any(Function(i) ComplyRecursively(i.Items))
    End Function

    Public Function ComplyHeader(header As OChart) As Boolean
        Select Case Type
            Case TransformTypes.SaveOrUpdate, TransformTypes.Delete, TransformTypes.RemoveResources
                Return header.StartDateOrDefault >= ReferenceDateOrDefault
            Case TransformTypes.Recode
                Return True
        End Select
        Return False
    End Function

    Public Sub SaveOrUpdateFor(item As OChartItem)
        StartDate = item.OrganizationChart.StartDate
        Code = item.Code
        If Not item.IsRoot Then
            Parent = New OChartTransformerDTO() With {
                .StartDate = ReferenceDate,
                .Code = item.Parent.Code
            }
        End If
    End Sub


#End Region

End Class
