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

    Public Function CreateFromDaticert(pec As PECMail, doc As XmlDocument) As PECMailReceipt

        Dim receipt As New PECMailReceipt()

        receipt.Parent = pec

        receipt.ErrorShort = doc.DocumentElement.Attributes("errore").Value
        receipt.ReceiptType = doc.DocumentElement.Attributes("tipo").Value

        Dim intestazione As XmlNode = doc.DocumentElement.SelectSingleNode("./intestazione")
        receipt.Sender = intestazione.SelectSingleNode("./mittente").InnerText
        receipt.Receiver = intestazione.SelectSingleNode("./destinatari").InnerText
        receipt.ReceiverType = intestazione.SelectSingleNode("./destinatari").Attributes("tipo").Value
        receipt.Subject = intestazione.SelectSingleNode("./oggetto").InnerText

        Dim dati As XmlNode = doc.DocumentElement.SelectSingleNode("./dati")
        receipt.Provider = dati.SelectSingleNode("./gestore-emittente").InnerText

        Dim data As XmlNode = dati.SelectSingleNode("./data")
        receipt.DateZone = data.Attributes("zona").Value
        Dim temp As String = data.SelectSingleNode("./giorno").InnerText & " " & data.SelectSingleNode("./ora").InnerText

        receipt.ReceiptDate = DateTime.ParseExact(temp, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)

        receipt.Identification = dati.SelectSingleNode("./identificativo").InnerText
        If dati.SelectSingleNode("./msgid") IsNot Nothing Then
            receipt.MSGID = dati.SelectSingleNode("./msgid").InnerText
        Else
            receipt.MSGID = receipt.Identification
        End If

        Dim esteso As XmlNode = dati.SelectSingleNode("./errore-esteso")
        If Not IsNothing(esteso) Then
            receipt.ErrorDescription = esteso.InnerText()
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
