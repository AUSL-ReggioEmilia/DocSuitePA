Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API

Public Class OrgDeptDTO
    Implements IOrgDeptDTO

#Region " Enums "

    Public Enum CommandTypes
        Undefined = 0
        Recode = 1
        SaveOrUpdate = 2
        Delete = 3
    End Enum

#End Region

#Region " Properties "

    Public Property CodUO As String Implements IOrgDeptDTO.CodUO
    Public Property CodUOPadre As String Implements IOrgDeptDTO.CodUOPadre
    Public Property DataCreazione As Date? Implements IOrgDeptDTO.DataCreazione
    Public Property DataDismissione As Date? Implements IOrgDeptDTO.DataDismissione
    Public Property DataUltimoJeep As Date? Implements IOrgDeptDTO.DataUltimoJeep
    Public Property DataUltimoUpdate As Date? Implements IOrgDeptDTO.DataUltimoUpdate
    Public Property Nome As String Implements IOrgDeptDTO.Nome

    Public ReadOnly Property Type As CommandTypes
        Get
            Select Case True
                Case DataCreazione.HasValue
                    Return CommandTypes.SaveOrUpdate
                Case DataDismissione.HasValue
                    Return CommandTypes.Delete
                Case Not DataCreazione.HasValue AndAlso Not DataDismissione.HasValue
                    Return CommandTypes.Recode
            End Select
            Return CommandTypes.Undefined
        End Get
    End Property
    Public ReadOnly Property RequiresReplication As Boolean
        Get
            Return Type.Equals(CommandTypes.SaveOrUpdate) OrElse Type.Equals(CommandTypes.Delete)
        End Get
    End Property
    Public ReadOnly Property ReferenceDate As DateTime?
        Get
            If DataCreazione.HasValue Then
                Return DataCreazione.Value.Date
            End If
            If DataDismissione.HasValue Then
                Return DataDismissione.Value.Date
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property ReferenceDateOrDefault As DateTime
        Get
            Return ReferenceDate.GetValueOrDefault(DateTime.MinValue)
        End Get
    End Property
    Public ReadOnly Property IsRoot As Boolean
        Get
            Return String.IsNullOrEmpty(CodUOPadre)
        End Get
    End Property
    Public ReadOnly Property Parent As OrgDeptDTO
        Get
            If IsRoot Then
                Return Nothing
            End If

            Return New OrgDeptDTO() With {
                .CodUO = CodUOPadre,
                .Nome = String.Format("Generato da: {0}", CodUO),
                .DataCreazione = DataCreazione,
                .DataDismissione = DataDismissione,
                .DataUltimoJeep = DataUltimoJeep,
                .DataUltimoUpdate = DataUltimoUpdate
            }
        End Get
    End Property

#End Region

#Region " Methods "

    Public Function Comply(item As OChartItem) As Boolean
        Select Case Type
            Case CommandTypes.Delete, CommandTypes.SaveOrUpdate
                Return CodUO.Equals(item.Code) AndAlso ComplyHeader(item.OrganizationChart)
            Case CommandTypes.Recode
                Return Nome.Equals(item.Title, StringComparison.InvariantCultureIgnoreCase) AndAlso ComplyHeader(item.OrganizationChart) AndAlso Not CodUO.Equals(item.Code)
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

        For Each item As OChartItem In items
            If item.HasItems AndAlso ComplyRecursively(item.Items) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function ComplyHeader(header As OChart) As Boolean
        Select Case Type
            Case CommandTypes.Delete, CommandTypes.SaveOrUpdate
                Return header.StartDateOrDefault >= ReferenceDateOrDefault
            Case CommandTypes.Recode
                Return True
        End Select
        Return False
    End Function

#End Region

End Class
