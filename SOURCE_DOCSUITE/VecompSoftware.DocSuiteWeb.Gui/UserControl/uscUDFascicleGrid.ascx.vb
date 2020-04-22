
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits

Partial Public Class uscUDFascicleGrid
    Inherits GridControl

#Region " Fields "
    Public Const COLUMN_CONTAINER_NAME As String = "colContainerName"
    Public Const COLUMN_CATEGORY_NAME As String = "colCategoryName"
    Public Const COLUMN_DOCUMENT_UNIT_NAME As String = "Entity.DocumentUnitName"

    Private _gridDataSource As IList(Of DocumentUnitModel)
    Private _udNameList As IList(Of String)

#End Region

#Region " Properties "

    Private ReadOnly Property GridDataSource As IList(Of DocumentUnitModel)
        Get
            If _gridDataSource Is Nothing Then
                _gridDataSource = DirectCast(grdUDFascicle.DataSource, IList(Of DocumentUnitModel))
            End If
            Return _gridDataSource
        End Get
    End Property

    Public ReadOnly Property Grid As BindGrid
        Get
            Return grdUDFascicle
        End Get
    End Property

#End Region

#Region " Events "

    'Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
    '    InitializeColumns()
    'End Sub

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(grdUDFascicle, grdUDFascicle)
        grdUDFascicle.Visible = True
    End Sub

    Protected Sub MaskText_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim maskTextBox As RadMaskedTextBox = DirectCast(sender, RadMaskedTextBox)
        Dim filter As String = maskTextBox.Text
        If String.IsNullOrEmpty(filter) Then
            Exit Sub
        End If

        Dim filterItem As GridFilteringItem = DirectCast(maskTextBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()

        ' Filtro per colonne di stringhe
        Dim year As Short = Short.Parse(maskTextBox.TextWithLiterals.Split("/"c)(0))
        filters.Add(New Data.FilterExpression("Year", GetType(Integer), year, Data.FilterExpression.FilterType.EqualTo))

        Dim numberString As String = maskTextBox.TextWithLiterals.Split("/"c)(1)
        Dim number As Integer
        Dim type As Type = GetType(String)
        Dim filterValue As Object = numberString
        If Integer.TryParse(numberString, number) Then
            type = GetType(Integer)
            filterValue = number
        End If

        filters.Add(New Data.FilterExpression("Number", type, filterValue, Data.FilterExpression.FilterType.EqualTo))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub



    Private Sub grdUDFascicle_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles grdUDFascicle.ItemDataBound

        If (TypeOf e.Item Is GridFilteringItem) Then
            Dim filterItem As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
            Dim combo As RadComboBox = DirectCast(filterItem.FindControl("cmbDocumentUnitName"), RadComboBox)

            combo.Items.AddRange(BasePage.FillComboBoxDocumentUnitNames())

            If grdUDFascicle.Finder.FilterExpressions.Any(Function(x) x.Key.Eq(COLUMN_DOCUMENT_UNIT_NAME)) Then
                Dim control As Control = filterItem.FindControl("cmbDocumentUnitName")
                Dim value As String = grdUDFascicle.Finder.FilterExpressions(COLUMN_DOCUMENT_UNIT_NAME).FilterValue.ToString()
                FilterHelper.SetFilterValue(control, value)
            End If
        End If

        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim boundHeader As WebAPIDto(Of DocumentUnitModel) = DirectCast(e.Item.DataItem, WebAPIDto(Of DocumentUnitModel))
        Dim number As String = String.Empty

        If boundHeader.Entity.DocumentUnitName.Eq("Protocollo") Then
            number = boundHeader.Entity.Number
        End If
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso (boundHeader.Entity.DocumentUnitName.Eq(Facade.ResolutionTypeFacade.DeterminaCaption()) OrElse boundHeader.Entity.DocumentUnitName.Eq(Facade.ResolutionTypeFacade.DeliberaCaption())) Then
            number = boundHeader.Entity.EntityId.ToString()
        End If

        Dim hiddenId As String = String.Format("{0}|{1}|{2}", boundHeader.Entity.Year, number, boundHeader.Entity.DocumentUnitName)

        Dim warningIcon As Image = DirectCast(e.Item.FindControl("imgWarningIcon"), Image)
        warningIcon.Visible = False
        If (Date.Today >= boundHeader.Entity.RegistrationDate.Value.Date.AddDays(60)) Then
            warningIcon.Visible = True
            warningIcon.ImageUrl = "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png"
            warningIcon.ToolTip = "Attenzione: è stata superata la soglia di 60 giorni prevista dalla normativa per la fascicolazione."
        ElseIf (Date.Today >= boundHeader.Entity.RegistrationDate.Value.Date.AddDays(DocSuiteContext.Current.ProtocolEnv.FascicolableThreshold)) Then
            warningIcon.Visible = True
            warningIcon.ImageUrl = "../App_Themes/DocSuite2008/imgset16/StatusSecurityWarning_16x.png"
            warningIcon.ToolTip = "Attenzione: si sta avvicinando la soglia di 60 giorni prevista dalla normativa per la fascicolazione."
        End If
        With DirectCast(e.Item.FindControl("lbtViewUD"), LinkButton)
            .Text = boundHeader.Entity.FullUDNumber
            .CommandArgument = hiddenId

            If RedirectOnParentPage Then
                Dim parentPageUrl As String = String.Empty

                If boundHeader.Entity.DocumentUnitName.Eq("Protocollo") Then
                    Dim parameters As String = String.Format("Year={0}&Number={1}&Type=Prot", boundHeader.Entity.Year, boundHeader.Entity.Number)
                    parameters = CommonShared.AppendSecurityCheck(parameters)
                    parentPageUrl = String.Concat("../Prot/ProtVisualizza.aspx?", parameters)
                End If
                If DocSuiteContext.Current.IsResolutionEnabled AndAlso (boundHeader.Entity.DocumentUnitName.Eq(Facade.ResolutionTypeFacade.DeterminaCaption()) OrElse boundHeader.Entity.DocumentUnitName.Eq(Facade.ResolutionTypeFacade.DeliberaCaption())) Then
                    Dim parameters As String = String.Format("IdResolution={0}&Type=Resl", boundHeader.Entity.EntityId)
                    parameters = CommonShared.AppendSecurityCheck(parameters)
                    parentPageUrl = String.Concat("../Resl/ReslVisualizza.aspx?", parameters)
                End If

                .OnClientClick = grdUDFascicle.GetRedirectParentPageScript(parentPageUrl)

            End If
        End With

        DirectCast(e.Item.FindControl("lblUDName"), Label).Text = boundHeader.Entity.DocumentUnitName
        DirectCast(e.Item.FindControl("lblUDObject"), Label).Text = boundHeader.Entity.Subject
        DirectCast(e.Item.FindControl("lblContainer"), Label).Text = getTextByHeader(boundHeader.Entity, COLUMN_CONTAINER_NAME)
        DirectCast(e.Item.FindControl("lblCategory"), Label).Text = getTextByHeader(boundHeader.Entity, COLUMN_CATEGORY_NAME)

    End Sub

    Private Sub grdUDFascicle_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdUDFascicle.ItemCommand

        Select Case e.CommandName
            Case "ViewUD"
                Dim split As String() = e.CommandArgument.ToString().Split("|"c)
                Dim year As Short = Short.Parse(split(0))
                Dim number As Integer = Integer.Parse(split(1))
                If split(2).Eq("Protocollo") Then
                    Dim prot As Protocol = Facade.ProtocolFacade.GetById(year, number, False)
                    If prot IsNot Nothing Then
                        RedirectOnPage(String.Concat("../Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}&Type=Prot", year, number))))
                    Else
                        AjaxManager.Alert(String.Format("Il Protocollo {0}/{1} non è stato trovato", Short.Parse(split(0)), Integer.Parse(split(1))))
                    End If
                End If
                If DocSuiteContext.Current.IsResolutionEnabled AndAlso (split(2).Eq(Facade.ResolutionTypeFacade.DeterminaCaption()) OrElse split(2).Eq(Facade.ResolutionTypeFacade.DeliberaCaption())) Then
                    Dim resl As Resolution = Facade.ResolutionFacade.GetById(number)
                    If resl IsNot Nothing Then
                        RedirectOnPage(String.Concat("../Resl/ReslVisualizza.aspx?", CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&Type=Resl", number))))
                    Else
                        AjaxManager.Alert(String.Format("L'Atto {0}/{1} non è stato trovato", Short.Parse(split(0)), Integer.Parse(split(1))))
                    End If
                End If

        End Select
    End Sub

    Protected Sub cmbDocumentUnitName_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        Dim comboBox As RadComboBox = DirectCast(sender, RadComboBox)

        Dim filterItem As GridFilteringItem = DirectCast(comboBox.NamingContainer, GridFilteringItem)
        Dim filters As IList(Of IFilterExpression) = New List(Of IFilterExpression)()
        Dim filterType As Data.FilterExpression.FilterType = Data.FilterExpression.FilterType.NoFilter
        If (Not String.IsNullOrEmpty(comboBox.SelectedValue)) Then
            filterType = Data.FilterExpression.FilterType.EqualTo
        End If
        filters.Add(New Data.FilterExpression(COLUMN_DOCUMENT_UNIT_NAME, GetType(Short), comboBox.SelectedValue, filterType))
        filterItem.FireCommandEvent("CustomFilter", filters)
    End Sub

#End Region

#Region " Methods "
    Private Function getTextByHeader(header As DocumentUnitModel, discriminator As String) As String
        Select Case discriminator
            Case COLUMN_CONTAINER_NAME
                If header.Container IsNot Nothing Then
                    Return header.Container.Name
                End If

            Case COLUMN_CATEGORY_NAME
                If header.Category IsNot Nothing Then
                    Return header.Category.Name
                End If
        End Select
        Return String.Empty
    End Function

#End Region

End Class