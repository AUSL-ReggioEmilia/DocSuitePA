Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscResolutionPreview
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _currentResolution As Resolution = Nothing
    Private _provNumber As String = ""
    Private _fullNumber As String = ""

#End Region

#Region "Properties"

    Public Property CurrentResolution() As Resolution
        Get
            Return _currentResolution
        End Get
        Set(ByVal value As Resolution)
            _currentResolution = value
        End Set
    End Property

#End Region

#Region " Methods "
    
    Public Sub Show()
        Dim resolutionTypeDescription As String = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type)
        lblType.Text = String.Format("Dettaglio [{0}]", resolutionTypeDescription)

        Facade.ResolutionFacade.ReslFullNumber(CurrentResolution, CurrentResolution.Type.Id, _provNumber, _fullNumber)
        If Not String.IsNullOrEmpty(_provNumber) Then
            lblProvLabel.Text = "Numero prov.:"
            lblProvNumber.Text = _provNumber
            If Not String.IsNullOrEmpty(_fullNumber) Then
                lblNumberLabel.Text = resolutionTypeDescription
                lblNumberFull.Text = _fullNumber
            End If
        Else
            lblProvLabel.Text = resolutionTypeDescription
            lblProvNumber.Text = _fullNumber
            lblNumberLabel.Text = String.Empty
            lblNumberFull.Text = String.Empty
        End If

        lblContainer.Text = CurrentResolution.Container.Name

        If Facade.ResolutionFacade.IsManagedProperty("Category", CurrentResolution.Type.Id) Then
            lblCategoryDescription.Text = CurrentResolution.Category.Name
        Else
            trCategory.Visible = False
        End If

        lblObject.Text = CurrentResolution.ResolutionObject
    End Sub

#End Region

End Class