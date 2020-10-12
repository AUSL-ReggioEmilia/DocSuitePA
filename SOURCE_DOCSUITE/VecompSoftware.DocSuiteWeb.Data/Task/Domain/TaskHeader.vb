Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API

Public Class TaskHeader
    Inherits DomainObject(Of Integer)
    Implements IAuditable

    Public Sub New()
        SendingProcessStatus = TaskHeaderSendingProcessStatus.Todo
        SendedStatus = Nothing
    End Sub

    Public Overridable Property Code As String

    Public Overridable Property Title As String

    Public Overridable Property Description As String

    Public Overridable Property TaskType As TaskTypeEnum

    Public Overridable Property Status As TaskStatusEnum

    Public Overridable Property SendingProcessStatus As TaskHeaderSendingProcessStatus

    Public Overridable Property SendedStatus As TaskHeaderSendedStatus?

    Public Overridable Property Details As IList(Of TaskDetail)

    Public Overridable Property Parameters As IList(Of TaskParameter)

    Public Overridable Property Protocols As IList(Of TaskHeaderProtocol)

    Public Overridable Property PECMails As IList(Of TaskHeaderPECMail)

    Public Overridable Property POLRequests As IList(Of TaskHeaderPOLRequest)


    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser


#Region " Methods "

    Public Overridable Sub AddDetail(detail As TaskDetail)
        If Details Is Nothing Then
            Details = New List(Of TaskDetail)()
        End If
        detail.TaskHeader = Me
        Details.Add(detail)
    End Sub

    Public Overridable Sub AddParameter(item As TaskParameter)
        If Parameters Is Nothing Then
            Parameters = New List(Of TaskParameter)()
        End If
        item.TaskHeader = Me
        Parameters.Add(item)
    End Sub


    Public Overridable Sub AddProtocol(domain As Protocol)
        If Me.Protocols Is Nothing Then
            Me.Protocols = New List(Of TaskHeaderProtocol)
        End If

        If Me.Protocols.Any(Function(i) i.Protocol.Year.Equals(domain.Year) _
                                AndAlso i.Protocol.Number.Equals(domain.Number)) Then
            Return
        End If

        Dim item As New TaskHeaderProtocol()
        item.Header = Me
        item.Protocol = domain
        Me.Protocols.Add(item)
    End Sub

    Public Overridable Sub AddProtocol(dto As IProtocolDTO)
        Dim temp As New Protocol()
        temp.Id = dto.UniqueId.Value
        temp.Year = dto.Year.Value
        temp.Number = dto.Number.Value
        Me.AddProtocol(temp)
    End Sub

    Public Overridable Sub AddProtocols(dtos As IEnumerable(Of IProtocolDTO))
        dtos.ToList().ForEach(Sub(d) Me.AddProtocol(d))
    End Sub

    Public Overridable Sub AddPECMail(domain As PECMail)
        If domain.Id.Equals(0) Then
            Return
        End If

        If Me.PECMails Is Nothing Then
            Me.PECMails = New List(Of TaskHeaderPECMail)
        End If

        If Me.PECMails.Any(Function(i) i.PECMail.Id.Equals(domain.Id)) Then
            Return
        End If

        Dim item As New TaskHeaderPECMail()
        item.Header = Me
        item.PECMail = domain
        Me.PECMails.Add(item)
    End Sub

    Public Overridable Sub AddPECMail(dto As IMailDTO)
        Dim temp As New PECMail()
        temp.Id = CInt(dto.Id)
        Me.AddPECMail(temp)
    End Sub

    Public Overridable Sub AddPECMails(dtos As IEnumerable(Of IMailDTO))
        dtos.ToList().ForEach(Sub(d) Me.AddPECMail(d))
    End Sub

    Public Overridable Sub AddPOLRequest(domain As POLRequest)
        If domain.Id.Equals(Guid.Empty) Then
            Return
        End If

        If Me.POLRequests Is Nothing Then
            Me.POLRequests = New List(Of TaskHeaderPOLRequest)
        End If

        If Me.POLRequests.Any(Function(i) i.POLRequest.Id.Equals(domain.Id)) Then
            Return
        End If

        Dim item As New TaskHeaderPOLRequest()
        item.Header = Me
        item.POLRequest = domain
        Me.POLRequests.Add(item)
    End Sub

    Public Overridable Sub AddPOLRequest(dto As IMailDTO)
        Dim temp As New POLRequest()
        temp.Id = New Guid(dto.Id)
        Me.AddPOLRequest(temp)
    End Sub

    Public Overridable Sub AddPOLRequests(dtos As IEnumerable(Of IMailDTO))
        dtos.ToList().ForEach(Sub(d) Me.AddPOLRequest(d))
    End Sub

#End Region

End Class

Public Enum TaskTypeEnum
    ImportExcelToDocumentSeries = 0
    ImportExcelToAVCPAcquisti = 1
    ImportExcelToAVCPPagamenti = 2
    ImportOracleToAVCP = 3
    ImportENCOToAVCP = 4
    DocSeriesImporter = 100
    FastProtocolSender = 200
End Enum

Public Enum TaskStatusEnum
    <Description("In coda")>
    Queued = 0
    <Description("Errore")>
    OnError = 1
    <Description("Completato con Errori")>
    DoneWithErrors = 2
    <Description("Completato con Avvisi")>
    DoneWithWarnings = 3
    <Description("Completato")>
    Done = 100
End Enum
