Imports System
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Protocols

<ComponentModel.DataObject()>
Public Class ProtocolParerFacade
    Inherits BaseProtocolFacade(Of ProtocolParer, Guid, NHibernateProtocolParerDao)

    ''' <summary>
    ''' Status previsti per il Parer
    ''' ATTENZIONE!! Tali numerazioni vengono utilizzate in NHibernateProtocolFinder.
    ''' Se venissero cambiate qui devono essere cambiate anche la.
    ''' </summary>
    Public Enum ProtocolParerConservationStatus
        [Nothing] = -1
        Undefined
        Correct
        Warning
        [Error]
        NotNeeded
    End Enum

    ''' <summary> Controlla se soggetto alla conservazione sostitutiva. </summary>
    Public Function Exists(parent As Protocol) As Boolean
        Return _dao.ExistsProtocol(parent.Id)
    End Function

    ''' <summary> Controlla se soggetto alla conservazione sostitutiva. </summary>
    Public Function Exists(year As Short, number As Integer) As Boolean
        Return _dao.ExistsProtocol(year, number)
    End Function

    Public Function GetByProtocol(protocol As Protocol) As ProtocolParer
        Return _dao.GetByProtocol(protocol)
    End Function

    Public Function GetByProtocol(year As Short, number As Integer) As ProtocolParer
        Return _dao.GetByProtocol(year, number)
    End Function

    ''' <summary> Dizionario statico che racchiude tutti i possibili stati del Parer con relativa descrizione. </summary>
    ''' <remarks>
    ''' TODO: che sia il caso di usare un attributo legato all'enumeratore?
    ''' </remarks>
    Public Shared ConservationsStatus As Dictionary(Of ProtocolParerConservationStatus, String) =
        New Dictionary(Of ProtocolParerConservationStatus, String)() From
            {{ProtocolParerConservationStatus.Nothing, "Documento non ancora versato"},
             {ProtocolParerConservationStatus.Correct, "Conservazione corretta"},
             {ProtocolParerConservationStatus.Warning, "Conservazione con avviso"},
             {ProtocolParerConservationStatus.Error, "Conservazione con errori"},
             {ProtocolParerConservationStatus.Undefined, "Stato conservazione non definito"},
             {ProtocolParerConservationStatus.NotNeeded, "Documento non soggetto a versamento"}
        }

    Public Function GetConservationStatus(protocol As Protocol) As ProtocolParerConservationStatus
        Return GetConservationStatus(GetByProtocol(protocol))
    End Function

    Public Shared Function GetConservationStatus(item As ProtocolParer) As ProtocolParerConservationStatus
        '' Se ricevo un protocolParer nullo significa che il protocollo non è soggetto a Parer
        If item Is Nothing Then Return ProtocolParerConservationStatus.Nothing

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError _
            AndAlso String.IsNullOrEmpty(item.LastError) Then
            If item.ParerUri.StartsWith("urn:") Then
                Return ProtocolParerConservationStatus.Correct
            End If
            Return ProtocolParerConservationStatus.NotNeeded
        End If

        If String.IsNullOrEmpty(item.ParerUri) _
            AndAlso item.HasError _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ProtocolParerConservationStatus.Error
        End If

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ProtocolParerConservationStatus.Warning
        End If

        Return ProtocolParerConservationStatus.Undefined

    End Function

    Public Function GetWebApiConservationStatus(item As ProtocolParerModel) As ProtocolParerConservationStatus
        '' Se ricevo un protocolParer nullo significa che il protocollo non è soggetto a Parer
        If item Is Nothing Then Return ProtocolParerConservationStatus.Nothing

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError _
            AndAlso String.IsNullOrEmpty(item.LastError) Then
            If item.ParerUri.StartsWith("urn:") Then
                Return ProtocolParerConservationStatus.Correct
            End If
            Return ProtocolParerConservationStatus.NotNeeded
        End If

        If String.IsNullOrEmpty(item.ParerUri) _
            AndAlso item.HasError.Value _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ProtocolParerConservationStatus.Error
        End If

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError.Value _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ProtocolParerConservationStatus.Warning
        End If

        Return ProtocolParerConservationStatus.Undefined

    End Function

    ''' <summary>
    ''' Ritorna la descrizione dello status Parer per il protocollo ricercato (ProtocolHeader)
    ''' </summary>
    ''' <param name="item">Protocollo ricercato</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConservationStatusDescription(item As ProtocolHeader) As String
        Dim protocolConservationStatus As ProtocolParerConservationStatus = GetConservationStatus(item.ProxiedProtocolParer)
        Return ConservationsStatus(protocolConservationStatus)
    End Function

End Class
