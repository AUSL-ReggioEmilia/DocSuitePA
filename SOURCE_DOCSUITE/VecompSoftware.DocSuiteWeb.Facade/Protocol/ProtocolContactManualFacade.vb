Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolContactManualFacade
    Inherits BaseProtocolFacade(Of ProtocolContactManual, Guid, NHibernateProtocolContactManualDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce per un protocollo i relativi contatti manuali della tipologia specificata. </summary>
    Public Function GetByComunicationType(protocol As Protocol, type As String) As IList(Of ProtocolContactManual)
        Return _dao.GetByComunicationType(protocol.Id, type)
    End Function
    Public Function GetByProtocol(protocol As Protocol) As IList(Of ProtocolContactManual)
        Return GetByComunicationType(protocol, Nothing)
    End Function

    Public Function GetByProtocolAndIncremental(protocol As Protocol, incremental As Integer) As ProtocolContactManual
        Return _dao.GetByProtocolAndIncremental(protocol.Id, incremental)
    End Function

    Public Function GetCountByProtocol(protocol As Protocol, ByVal comunicationType As String) As Integer
        Return _dao.GetCountByProtocol(protocol.Id, comunicationType)
    End Function

    Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of ProtocolContactJournalDTO)
        Return _dao.GetJournalPrint(idContainers, dateFrom, dateTo, idStatus)
    End Function

    ''' <summary> Aggiunge un contatto manuale al protocollo </summary>
    ''' <remarks> Verifica integrità dei dati </remarks>
    Public Shared Sub BindContactToProtocol(ByRef protocol As Protocol, ByVal contact As Contact, ByVal comunicationType As Char, ByVal copiaConoscenza As Boolean)
        Dim pcm As ProtocolContactManual = New ProtocolContactManual()
        pcm.ComunicationType = comunicationType
        If copiaConoscenza Then
            pcm.Type = "CC"
        End If
        pcm.Contact = contact
        pcm.Protocol = protocol
        pcm.Incremental = protocol.ManualContacts.Max(Function(x) x.Incremental) + 1
        protocol.ManualContacts.Add(pcm)
        pcm.Contact.FullIncrementalPath = pcm.Incremental.ToString()
    End Sub


End Class