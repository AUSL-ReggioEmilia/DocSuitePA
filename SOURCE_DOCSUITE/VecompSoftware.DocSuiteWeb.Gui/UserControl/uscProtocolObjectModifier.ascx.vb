Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscProtocolObjectModifier
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Private _protocol As Protocol = Nothing
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, pnlObjectData)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, uscObject.TextBoxControl)
    End Sub

#Region "Properties: Protocol"
    Public Property DataSource() As Protocol
        Get
            Return _protocol
        End Get
        Set(ByVal value As Protocol)
            _protocol = value
        End Set
    End Property
#End Region

#Region "Properties: Controls"
    Public ReadOnly Property PanelObjectData() As Panel
        Get
            Return pnlObjectData
        End Get
    End Property

    Public ReadOnly Property ObjectControl() As uscOggetto
        Get
            Return uscObject
        End Get
    End Property

    Public ReadOnly Property ConfirmButtonControl() As Button
        Get
            Return btnConferma
        End Get
    End Property
#End Region

#Region "Bind Protocol"
    Public Overrides Sub DataBind()
        If _protocol IsNot Nothing Then
            lblYear.Text = _protocol.Year.ToString()
            lblNumber.Text = _protocol.Number.ToString()
            lblContainer.Text = _protocol.Container.Name
            lblClassification.Text = String.Format("{0} - {1}", Facade.ProtocolFacade.GetCategoryCode(_protocol), Facade.ProtocolFacade.GetCategoryDescription(_protocol))
            uscObject.Text = _protocol.ProtocolObject
        End If
    End Sub
#End Region

#Region "Public Functions"
    Public Sub Clear()
        _protocol = Nothing
        lblClassification.Text = String.Empty
        lblContainer.Text = String.Empty
        lblNumber.Text = String.Empty
        lblYear.Text = String.Empty
        uscObject.Text = String.Empty
    End Sub

    Public Sub Enable()
        btnConferma.Enabled = True
    End Sub

    Public Sub Disable()
        btnConferma.Enabled = False
    End Sub
#End Region

#Region "Button Events"
    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConferma.Click
        RaiseEvent DoConfirm(Me, New EventArgs())
    End Sub
#End Region

#Region "Events"
    Public Delegate Sub DoConfirmEventHandler(ByVal sender As Object, ByVal e As EventArgs)

    Public Event DoConfirm As DoConfirmEventHandler
#End Region
End Class