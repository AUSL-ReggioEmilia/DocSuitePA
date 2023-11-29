Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class uscProtocolPreview
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _currentProtocol As Protocol

#End Region

#Region " Properties "

    Public Property CurrentProtocol() As Protocol
        Get
            If _currentProtocol Is Nothing AndAlso CurrentProtocolId.HasValue Then
                _currentProtocol = Facade.ProtocolFacade.GetById(CurrentProtocolId.Value)
            End If
            Return _currentProtocol
        End Get
        Set(ByVal value As Protocol)
            _currentProtocol = value
            CurrentProtocolId = value.Id
        End Set
    End Property

    Public Property CurrentProtocolId As Guid?
        Get
            If ViewState("CurrentProtocolID") IsNot Nothing Then
                Return DirectCast(ViewState("CurrentProtocolID"), Guid)
            End If
            Return Nothing
        End Get
        Set(value As Guid?)
            ViewState("CurrentProtocolID") = value
        End Set
    End Property

    Public Property Title As String
        Get
            Return lblTitle.Text
        End Get
        Set(value As String)
            lblTitle.Text = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Public Sub Initialize()
        If CurrentProtocol Is Nothing Then
            Exit Sub
        End If

        ' Numero e data
        cmdProtocol.Icon.PrimaryIconUrl = ""
        cmdProtocol.Icon.PrimaryIconUrl = "../Comm/Images/DocSuite/Protocollo16.png"
        cmdProtocol.Icon.PrimaryIconHeight = Unit.Pixel(16)
        cmdProtocol.Icon.PrimaryIconWidth = Unit.Pixel(16)
        cmdProtocol.Text = String.Format("{0} del {1}", CurrentProtocol.FullNumber, String.Format(ProtocolEnv.ProtRegistrationDateFormat, CurrentProtocol.RegistrationDate.ToLocalTime()))
        Dim params As String = String.Format("UniqueId={0}&Type=Prot", CurrentProtocol.Id)
        cmdProtocol.NavigateUrl = String.Concat("~/Prot/ProtVisualizza.aspx?", CommonShared.AppendSecurityCheck(params))

        lblType.Text = CurrentProtocol.Type.Description
        ' Descrizione
        If CurrentProtocol.DocumentType IsNot Nothing Then
            trProtocolType.Visible = True
            lblDocType.Text = CurrentProtocol.DocumentType.Description
        End If
        ' Contenitore
        If CurrentProtocol.Container IsNot Nothing Then
            lblContainer.Text = CurrentProtocol.Container.Name
        End If
        ' Oggetto
        lblObject.Text = CurrentProtocol.ProtocolObject
        ' Codice categoria
        lblCategoryCode.Text = Facade.ProtocolFacade.GetCategoryCode(CurrentProtocol)
        ' Descrizione categoria
        lblCategoryDescription.Text = Facade.ProtocolFacade.GetCategoryDescription(CurrentProtocol)
    End Sub

#End Region

End Class