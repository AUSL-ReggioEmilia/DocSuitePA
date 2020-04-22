Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscFasciclePreview
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Private _currentFascicle As Fascicle = Nothing
    Public Property CurrentFascicle() As Fascicle
        Get
            Return _currentFascicle
        End Get
        Set(ByVal value As Fascicle)
            _currentFascicle = value
        End Set
    End Property
#End Region

    Public Sub Show()
        Dim fullId As String = String.Format("{0}-{1}-{2}", CurrentFascicle.Year, CurrentFascicle.Category.FullCodeDotted, CurrentFascicle.Number)
        lblId.Text = String.Format("{0} del {1:dd/MM/yyyy}", fullId, CurrentFascicle.RegistrationDate)
        lblObject.Text = CurrentFascicle.FascicleObject
        lblCategory.Text = String.Format("{0}<BR>{1}", Facade.CategoryFacade.GetCodeDotted(CurrentFascicle.Category), Facade.CategoryFacade.GetFullIncrementalName(CurrentFascicle.Category))
    End Sub

End Class