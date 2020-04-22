Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolContactManualFacade
    Inherits BaseProtocolFacade(Of ProtocolContactManual, YearNumberIdCompositeKey, NHibernateProtocolContactManualDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce per un protocollo i relativi contatti manuali della tipologia specificata. </summary>
    ''' <param name="year">Anno del protocollo.</param>
    ''' <param name="number">Numero del protocollo.</param>
    ''' <param name="type">Tipologia del contatto manuale.</param>
    Public Function GetByComunicationType(ByVal year As Short, ByVal number As Integer, ByVal type As String) As IList(Of ProtocolContactManual)
        Return _dao.GetByComunicationType(year, number, type)
    End Function
    Public Function GetByComunicationType(protocol As Protocol, type As String) As IList(Of ProtocolContactManual)
        Return GetByComunicationType(protocol.Id.Year.Value, protocol.Id.Number.Value, type)
    End Function
    Public Function GetByProtocol(protocol As Protocol) As IList(Of ProtocolContactManual)
        Return GetByComunicationType(protocol, Nothing)
    End Function

    Public Function GetCountByProtocol(ByVal year As Short, ByVal number As Integer, ByVal comunicationType As String) As Integer
        Return _dao.GetCountByProtocol(year, number, comunicationType)
    End Function

    Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of ProtocolContactJournalDTO)
        Return _dao.GetJournalPrint(idContainers, dateFrom, dateTo, idStatus)
    End Function

    ''' <summary> Aggiunge un contatto manuale al protocollo </summary>
    ''' <remarks> Verifica integrità dei dati </remarks>
    Public Shared Sub BindContactToProtocol(ByRef protocol As Protocol, ByVal contact As Contact, ByVal comunicationType As Char, ByVal copiaConoscenza As Boolean)
        Dim pcm As New ProtocolContactManual()
        pcm.ComunicationType = comunicationType
        If copiaConoscenza Then
            pcm.Type = "CC"
        End If
        pcm.Contact = contact
        pcm.Protocol = protocol
        pcm.UniqueIdProtocol = protocol.UniqueId
        pcm.Id.Id = protocol.ManualContacts.Max(Function(x) x.Id.Id) + 1
        protocol.ManualContacts.Add(pcm)
        pcm.Contact.FullIncrementalPath = pcm.Id.Id.ToString()
    End Sub


End Class