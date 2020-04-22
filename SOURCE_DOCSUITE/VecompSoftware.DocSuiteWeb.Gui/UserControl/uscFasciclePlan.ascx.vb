Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons

Public Class uscFasciclePlan
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private _currentCategoryFascicleFacade As CategoryFascicleFacade
#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property CurrentCategoryId As String

    Public ReadOnly Property CurrentResolutionName As String
        Get
            Return Facade.TabMasterFacade.TreeViewCaption
        End Get
    End Property

    Public ReadOnly Property CurrentDocumentSeriesName As String
        Get
            Return DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName
        End Get
    End Property

    Private ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _currentCategoryFascicleFacade Is Nothing Then
                _currentCategoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _currentCategoryFascicleFacade
        End Get
    End Property
#End Region

#Region " Methods "

    Protected Sub uscFasciclePlan_AjaxRequest(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        Select Case ajaxModel.ActionName
            Case "ProcedureExternalDataCallback"
                If ajaxModel.Value IsNot Nothing Then
                    Dim categoryFascicle As CategoryFascicle = JsonConvert.DeserializeObject(Of CategoryFascicle)(ajaxModel.Value(0))
                    If categoryFascicle IsNot Nothing Then
                        Dim currentCategory As Category = Facade.CategoryFacade.GetById(categoryFascicle.Category.Id)
                        categoryFascicle.Category = currentCategory
                        CurrentCategoryFascicleFacade.Save(categoryFascicle)
                        AjaxManager.RaisePostBackEvent(String.Concat("ReloadNodes|", currentCategory.Id.ToString()))
                    End If
                End If
                Exit Select
        End Select
    End Sub
#End Region

End Class