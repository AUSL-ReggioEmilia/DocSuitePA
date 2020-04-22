Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public MustInherit Class BaseResolutionFinderController
    Inherits BaseFinderController(Of NHibernateResolutionFinder)

#Region "Fields"
    Protected _uscFinder As uscResolutionFinder
    Private _baseResolutionController As BaseResolutionController
#End Region

#Region "Constructor"
    Public Sub New(ByRef uscControl As uscResolutionFinder)
        MyBase.New(uscControl)
        _uscFinder = uscControl
        _baseResolutionController = New BaseResolutionController()
    End Sub
#End Region

#Region "Facade Factory"
    Public Overrides Function CreateFactory() As FacadeFactory
        Return _baseResolutionController.CreateFactory()
    End Function
#End Region

#Region "Finder"
    Public Overrides Function LoadFinder() As NHibernateResolutionFinder
        Return _uscFinder.Finder
    End Function

#End Region

#Region "BindControls"
    Public Overrides Sub BindControls()
        If _uscFinder IsNot Nothing Then
            _uscFinder.BindContainersDelegate = AddressOf BindContainers
            _uscFinder.BindControllerStatusDelegate = AddressOf BindControllerStatus
            _uscFinder.BindControls()
        End If
    End Sub

    Protected Overridable Sub BindContainers(ByRef comboBox As DropDownList)
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("Resl", Nothing)
        Dim selected As String = comboBox.SelectedValue
        comboBox.Items.Clear()
        WebUtils.ObjDropDownListAdd(comboBox, String.Empty, String.Empty)
        If Not containers.IsNullOrEmpty() Then
            '' Tengo separati i contenitori disabilitati
            Dim disabledContainers As New List(Of ListItem)
            For Each container As Container In containers
                Dim li As New ListItem(container.Name, container.Id.ToString())
                If container.IsActive = 1 AndAlso container.IsActiveRange() Then
                    '' Se è attivo e valido lo aggiungo direttamente al controllo
                    comboBox.Items.Add(li)
                Else
                    '' Altrimenti lo disabilito e lo tengo da parte per aggiungerlo alla fine
                    li.Attributes.Add("style", "color:grey;")
                    disabledContainers.Add(li)
                End If
            Next
            '' Aggiungo tutti i contenitori disabilitati
            comboBox.Items.AddRange(disabledContainers.ToArray())
        End If
        Dim item As ListItem = comboBox.Items.FindByValue(selected)
        If item IsNot Nothing Then
            item.Selected = True
        End If
    End Sub

    Protected Overridable Sub BindControllerStatus(ByRef comboBox As DropDownList)
        Dim controllers As IList(Of ControllerStatusResolution) = Facade.ControllerStatusResolutionFacade.GetAll()
        If controllers.Count > 0 Then
            WebUtils.ObjDropDownListAdd(comboBox, "", "")
            For Each controller As ControllerStatusResolution In controllers
                WebUtils.ObjDropDownListAdd(comboBox, controller.Acronym & " - " & controller.Description, controller.Id)
            Next
        End If
    End Sub
#End Region

    Public MustOverride Overrides Sub Initialize()
End Class
