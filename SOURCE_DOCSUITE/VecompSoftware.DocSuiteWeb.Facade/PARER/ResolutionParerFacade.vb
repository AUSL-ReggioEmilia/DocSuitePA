Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class ResolutionParerFacade
    Inherits BaseProtocolFacade(Of ResolutionParer, Int32, NHibernateResolutionParerDao)

    Public Function Exists(resolution As Resolution) As Boolean
        Return _dao.Exists(resolution.Id)
    End Function

    Public Function Exists(id As Int32) As Boolean
        Return _dao.Exists(id)
    End Function

    Public Function GetByResolution(resolution As Resolution) As ResolutionParer
        Return _dao.GetByResolution(resolution)
    End Function

    ''' <summary>
    ''' Status previsti per il Parer
    ''' ATTENZIONE!! Tali numerazioni vengono utilizzate in NHibernateResolutionFinder.
    ''' Se venissero cambiate qui devono essere cambiate anche la.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ResolutionParerConservationStatus
        [Nothing] = -1
        Undefined
        Correct
        Warning
        [Error]
        NotNeeded
    End Enum

    ''' <summary>
    ''' Dizionario statico che racchiude tutti i possibili stati del Parer con relativa descrizione
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared ConservationsStatus As Dictionary(Of ResolutionParerConservationStatus, String) =
        New Dictionary(Of ResolutionParerConservationStatus, String)() From
            {{ResolutionParerConservationStatus.Nothing, "Documento non ancora versato"},
             {ResolutionParerConservationStatus.Correct, "Conservazione corretta"},
             {ResolutionParerConservationStatus.Warning, "Conservazione con avviso"},
             {ResolutionParerConservationStatus.Error, "Conservazione con errori"},
             {ResolutionParerConservationStatus.Undefined, "Stato conservazione non definito"},
             {ResolutionParerConservationStatus.NotNeeded, "Documento non soggetto a versamento"}
        }

    Public Function GetConservationStatus(resolution As Resolution) As ResolutionParerConservationStatus
        Return GetConservationStatus(GetByResolution(resolution))
    End Function

    Public Function GetConservationStatus(item As ResolutionParer) As ResolutionParerConservationStatus
        '' Se ricevo un resolutionParer nullo significa che l'atto non è soggetto a Parer
        If item Is Nothing Then
            Return ResolutionParerConservationStatus.Nothing
        End If

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError _
            AndAlso String.IsNullOrEmpty(item.LastError) Then
            If item.ParerUri.StartsWith("urn:") Then
                Return ResolutionParerConservationStatus.Correct
            End If
            Return ResolutionParerConservationStatus.NotNeeded
        End If

        If String.IsNullOrEmpty(item.ParerUri) _
            AndAlso item.HasError _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ResolutionParerConservationStatus.Error
        End If

        If Not String.IsNullOrEmpty(item.ParerUri) _
            AndAlso Not item.HasError _
            AndAlso Not String.IsNullOrEmpty(item.LastError) Then
            Return ResolutionParerConservationStatus.Warning
        End If

        Return ResolutionParerConservationStatus.Undefined
    End Function

    ''' <summary>
    ''' Ritorna la descrizione dello status Parer per il protocollo ricercato (ProtocolHeader)
    ''' </summary>
    ''' <param name="item">Protocollo ricercato</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConservationStatusDescription(item As ResolutionHeader) As String
        Dim protocolConservationStatus As ResolutionParerConservationStatus = GetConservationStatus(item.ProxiedResolutionParer)
        Return ConservationsStatus(protocolConservationStatus)
    End Function
End Class
