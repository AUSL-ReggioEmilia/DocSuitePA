Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq

Public Module CoccardaManager

    ''' <summary>
    ''' Enumeratore che descrive gli stati della coccarda.
    ''' PecValida: RED
    ''' PecUscita: PINK
    ''' Mail o Anomala:  BLU
    ''' Interoperabile: VIOLET
    ''' Ricevuta: GREEN
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum PecToCoccarda
        PEC
        PECOutgoing
        Interoperabile
        Anomaly
        Receipt
        None
    End Enum

    ''' <summary>
    ''' Converte un PECMailHeader in PECMail e restituisce l'immagine della coccarda
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImage(prot As ProtocolHeader, coccardaEnabled As Boolean, Optional dimension As Boolean = False) As KeyValuePair(Of String, String)
        Return GetImageFromCoccardaType(GetCoccardaType(prot, coccardaEnabled), dimension)
    End Function

    Public Function GetImage(item As PecMailHeader, coccardaEnabled As Boolean, Optional dimension As Boolean = False) As KeyValuePair(Of String, String)
        Dim pec As PECMail = New PECMail() With {.PECType = item.PECType, .Segnatura = item.Segnatura}

        Return GetImageFromCoccardaType(GetCoccardaType(pec, coccardaEnabled), dimension)
    End Function

    Public Function GetImage(item As PECMail, coccardaEnabled As Boolean, Optional dimension As Boolean = False) As KeyValuePair(Of String, String)
        Return GetImageFromCoccardaType(GetCoccardaType(item, coccardaEnabled), dimension)
    End Function

    ''' <summary>
    ''' Applica l'immagine della coccarda dato un protocollo
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImage(item As Protocol, coccardaEnabled As Boolean) As KeyValuePair(Of String, String)
        Dim result As KeyValuePair(Of String, String) = New KeyValuePair(Of String, String)(String.Empty, String.Empty)
        Dim coccardaType As PecToCoccarda = PecToCoccarda.None
        ' Setto una coccarda con l'immagine vuota
        result = GetImageFromCoccardaType(coccardaType, True)

        ' Protocollo da pec con coccarde colorate
        If coccardaEnabled Then
            ' Protocollo con PEC in ingresso
            If Not item.IngoingPecMails.IsNullOrEmpty() Then

                If item.IngoingPecMails.Any(Function(f) Not String.IsNullOrEmpty(f.Segnatura)) Then
                    Return GetImageFromCoccardaType(PecToCoccarda.Interoperabile)
                End If

                If item.IngoingPecMails.Any(Function(f) f.PECType.HasValue AndAlso f.PECType.Value = PECMailType.PEC) Then
                    Return GetImageFromCoccardaType(PecToCoccarda.PEC)
                End If

                If item.IngoingPecMails.Any(Function(f) f.PECType.HasValue AndAlso f.PECType.Value = PECMailType.Anomalia) Then
                    Return GetImageFromCoccardaType(PecToCoccarda.Anomaly)
                End If

                If item.IngoingPecMails.Any(Function(f) f.PECType.HasValue AndAlso f.PECType.Value = PECMailType.Receipt) Then
                    Return GetImageFromCoccardaType(PecToCoccarda.Receipt)
                End If

                If Not item.OutgoingPecMailAll.IsNullOrEmpty() Then
                    Return GetImageFromCoccardaType(PecToCoccarda.PECOutgoing)
                End If

            End If
        Else
            If Not item.PecMails.IsNullOrEmpty() AndAlso (item.IngoingPecMails.Any(Function(f) f.PECType.HasValue AndAlso f.PECType.Value = PECMailType.PEC) OrElse Not item.OutgoingPecMailAll.IsNullOrEmpty()) Then
                result = GetImageFromCoccardaType(PecToCoccarda.PEC, True)
            End If
        End If

        Return result
    End Function

    ''' <summary>
    ''' Applica l'immagine della coccarda dato un protocollo
    ''' </summary>
    ''' <param name="item"></param>
    ''' <param name="dimension"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCoccardaType(item As PECMail, coccardaEnabled As Boolean) As PecToCoccarda
        Dim coccardaType As PecToCoccarda = PecToCoccarda.None
        If coccardaEnabled Then
            If Not String.IsNullOrEmpty(item.Segnatura) Then
                coccardaType = PecToCoccarda.Interoperabile
            ElseIf item.PECType = PECMailType.PEC Then
                coccardaType = PecToCoccarda.PEC
            ElseIf item.PECType = PECMailType.Anomalia Then
                coccardaType = PecToCoccarda.Anomaly
            ElseIf item.PECType = PECMailType.Receipt Then
                coccardaType = PecToCoccarda.Receipt
            ElseIf item.Direction = PECMailDirection.Outgoing Then
                coccardaType = PecToCoccarda.PECOutgoing
            End If
        Else
            If item.PECType = PECMailType.PEC Then
                coccardaType = PecToCoccarda.PEC
            ElseIf item.Direction = PECMailDirection.Outgoing Then
                coccardaType = PecToCoccarda.PECOutgoing
            End If
        End If
        Return coccardaType
    End Function

    ''' <summary>
    ''' Dato un protocolHeader, identifico la tipologia di PEC incluse nel protocollo.
    ''' <see cref="uscProtGrid">Viene utilizzata per settare le icone nella griglia</see>
    ''' </summary>
    ''' <param name="prot"></param>
    ''' <param name="coccardaEnabled"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCoccardaType(prot As ProtocolHeader, coccardaEnabled As Boolean) As PecToCoccarda
        Dim coccardaType As PecToCoccarda = PecToCoccarda.None
        ' Ci deve essere almeno una PEC valida
        If coccardaEnabled Then
            If prot.HasPECSegnatura Then
                coccardaType = PecToCoccarda.Interoperabile
            ElseIf prot.HasPECInPECStatus Then
                coccardaType = PecToCoccarda.PEC
            ElseIf prot.HasPECInAnomaliaStatus Then
                coccardaType = PecToCoccarda.Anomaly
            ElseIf prot.HasPECInReceiptStatus Then
                coccardaType = PecToCoccarda.Receipt
            ElseIf prot.HasPECOutgoing Then
                coccardaType = PecToCoccarda.PECOutgoing
            End If
        Else
            If prot.HasPECInPECStatus OrElse prot.HasPECOutgoing Then
                coccardaType = PecToCoccarda.PEC
            End If
        End If
        Return coccardaType
    End Function

    ''' <summary>
    ''' Ritorna le informazioni relative all'immagine e al tooltip
    ''' </summary>
    ''' <param name="coccardaType"></param>
    ''' <param name="dimension"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetImageFromCoccardaType(coccardaType As PecToCoccarda, Optional dimension As Boolean = True) As KeyValuePair(Of String, String)
        Dim result As KeyValuePair(Of String, String) = New KeyValuePair(Of String, String)()

        Select Case coccardaType
            Case PecToCoccarda.PEC
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Red"), "Proveniente da PEC")
            Case PecToCoccarda.Anomaly
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Blue"), "Proveniente da Mail o Anomala")
            Case PecToCoccarda.Interoperabile
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Violet"), "Proveniente da PEC interoperabile")
            Case PecToCoccarda.Receipt
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Green"), "Proveniente da ricevuta")
            Case PecToCoccarda.PECOutgoing
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Pink"), "PEC inviate")
            Case PecToCoccarda.None
                result = New KeyValuePair(Of String, String)(GetImageString(dimension, "Nothing"), String.Empty)
        End Select

        Return result
    End Function

    ''' <summary>
    ''' restituisce la stringa dell'immagine a partire dalla dimensione e dal colore
    ''' </summary>
    ''' <param name="dimension"></param>
    ''' <param name="color"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetImageString(dimension As Boolean, color As String) As String
        Select Case color
            Case "Violet"
                If dimension Then
                    Return ImagePath.BigDocumentSignatureViolet
                Else
                    Return ImagePath.SmallDocumentSignatureViolet
                End If
            Case "Azure"
                If dimension Then
                    Return ImagePath.BigDocumentSignatureAzure
                Else
                    Return ImagePath.SmallDocumentSignatureAzure
                End If
            Case "Green"
                If dimension Then
                    Return ImagePath.BigDocumentSignatureGreen
                Else
                    Return ImagePath.SmallDocumentSignatureGreen
                End If
            Case "Yellow"
                If dimension Then
                    Return ImagePath.BigDocumentSignatureYellow
                Else
                    Return ImagePath.SmallDocumentSignatureYellow
                End If
            Case "Blue"
                If dimension Then
                    Return ImagePath.BigDocumentSignatureBlue
                Else
                    Return ImagePath.SmallDocumentSignatureBlue
                End If
            Case "Pink"
                If dimension Then
                    Return ImagePath.BigDocumentSignaturePink
                Else
                    Return ImagePath.SmallDocumentSignaturePink
                End If
            Case "Nothing"
                If dimension Then
                    Return String.Empty
                Else
                    Return String.Empty
                End If
            Case Else
                If dimension Then
                    Return ImagePath.BigDocumentSignature
                Else
                    Return ImagePath.SmallDocumentSignature
                End If
        End Select

    End Function
End Module
