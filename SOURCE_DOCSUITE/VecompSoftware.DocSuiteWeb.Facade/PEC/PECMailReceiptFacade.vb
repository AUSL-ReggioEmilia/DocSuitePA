Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports System.Globalization
Imports VecompSoftware.Helpers.ExtensionMethods



<DataObject()> _
Public Class PECMailReceiptFacade
    Inherits BaseProtocolFacade(Of PECMailReceipt, Integer, NHibernatePECMailReceiptDao)

#Region " Constructor "
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Methods "

    Private Function TryGet(attribute As XmlAttribute, getInnerText As Boolean) As String
        If attribute Is Nothing Then
            Return String.Empty
        End If
        If getInnerText Then
            Return attribute.InnerText
        End If
        Return attribute.Value
    End Function

    Private Function TryGet(attribute As XmlNode, getInnerText As Boolean) As String
        If attribute Is Nothing Then
            Return String.Empty
        End If
        If getInnerText Then
            Return attribute.InnerText
        End If
        Return attribute.Value
    End Function

    Public Function CreateFromDaticert(pec As PECMail, doc As XmlDocument) As PECMailReceipt
        Dim receipt As New PECMailReceipt With {
            .Parent = pec,
            .ErrorShort = TryGet(doc.DocumentElement.Attributes("errore"), False),
            .ReceiptType = TryGet(doc.DocumentElement.Attributes("tipo"), False)
        }

        Dim intestazione As XmlNode = doc.DocumentElement.SelectSingleNode("./intestazione")
        receipt.Sender = TryGet(intestazione.SelectSingleNode("./mittente"), True)
        receipt.Receiver = TryGet(intestazione.SelectSingleNode("./destinatari"), True)
        receipt.ReceiverType = TryGet(intestazione.SelectSingleNode("./destinatari").Attributes("tipo"), False)
        receipt.Subject = TryGet(intestazione.SelectSingleNode("./oggetto"), True)

        Dim dati As XmlNode = doc.DocumentElement.SelectSingleNode("./dati")
        receipt.Provider = TryGet(dati.SelectSingleNode("./gestore-emittente"), True)

        Dim data As XmlNode = dati.SelectSingleNode("./data")
        receipt.DateZone = TryGet(data.Attributes("zona"), False)
        Dim temp As String = $"{TryGet(data.SelectSingleNode("./giorno"), True)} {TryGet(data.SelectSingleNode("./ora"), True)}"

        receipt.ReceiptDate = Date.ParseExact(temp, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)

        receipt.Identification = TryGet(dati.SelectSingleNode("./identificativo"), True)
        If dati.SelectSingleNode("./msgid") IsNot Nothing Then
            receipt.MSGID = TryGet(dati.SelectSingleNode("./msgid"), True)
        Else
            receipt.MSGID = receipt.Identification
        End If
        receipt.ErrorDescription = TryGet(dati.SelectSingleNode("./errore-esteso"), True)
        If String.IsNullOrEmpty(receipt.ErrorDescription) Then
            receipt.ErrorDescription = Nothing
        End If
        LinkToPEC(receipt)

        Return receipt

    End Function

    Public Function GetPECReceipts(pec As PECMail) As IList(Of PECMailReceipt)
        Return _dao.GetPECReceipts(pec)
    End Function

    Public Function HasWarnings(pec As PECMail) As Boolean
        For Each receipt As PECMailReceipt In GetPECReceipts(pec)
            If receipt.ReceiptType.Eq("preavviso-errore-consegna") Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function HasErrors(pec As PECMail) As Boolean
        For Each receipt As PECMailReceipt In GetPECReceipts(pec)
            If receipt.ReceiptType.Eq("non-accettazione") OrElse receipt.ReceiptType.Eq("errore-consegna") Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function LinkToPEC(receipt As PECMailReceipt) As Boolean
        Select Case receipt.ReceiptType
            Case "posta-certificata"
                receipt.PECMail = receipt.Parent
                Return True
            Case Else
                If receipt.MSGID Is Nothing Then
                    Throw New DocSuiteException("Impossibile associare ricevuta sprovvista di MSGID.")
                End If
                Dim pec As PECMail = Factory.PECMailFacade.GetOutgoingMailByXRiferimentoMessageID(receipt.MSGID)
                If Not pec Is Nothing Then
                    receipt.PECMail = pec
                    Return True
                End If
        End Select
        Return False
    End Function

#End Region

End Class
