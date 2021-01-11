Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class BidTypeFacade
    Inherits CommonFacade(Of BidType, Integer, NHibernateBidTypeDao)

    Public Sub New()
        MyBase.New()
    End Sub

End Class