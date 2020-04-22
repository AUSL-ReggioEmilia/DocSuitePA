Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Public Class TenderLotFacade
    Inherits FacadeNHibernateBase(Of TenderLot, Guid, NHibernateTenderLotDao)

    Public Function GetByCIG(cig As String) As TenderLot
        Return _dao.GetByCIG(cig)
    End Function


    Public Sub SetPayment(lotto As TenderLot, cig As String, key As String, importo As Decimal)
        Dim payment As TenderLotPayment

        If lotto.Payments IsNot Nothing Then
            payment = lotto.Payments.FirstOrDefault(Function(p) p.PaymentKey.Equals(key))
        End If

        If payment Is Nothing Then
            payment = New TenderLotPayment()
            payment.PaymentKey = key
            payment.Amount = importo

            payment.RegistrationDate = DateTimeOffset.UtcNow
            payment.RegistrationUser = DocSuiteContext.Current.User.FullUserName

            lotto.AddPayment(payment)
        Else
            ' Aggiorno il valore
            payment.Amount = importo
        End If

        payment.LastChangedDate = DateTimeOffset.UtcNow
        payment.LastChangedUser = DocSuiteContext.Current.User.FullUserName
    End Sub

End Class
