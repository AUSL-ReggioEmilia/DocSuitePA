Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class MessageDetails
    Inherits CommonBasePage

    Private _isMessageReadable As Boolean?

#Region " Fields "

#End Region

#Region " Properties "
    Private ReadOnly Property DocumentUnitId As Guid
        Get
            Return GetKeyValue(Of Guid, MessageDetails)("DocumentUnitId")
        End Get
    End Property

    Private ReadOnly Property IsProtocol As Boolean
        Get
            Return Context.Request.QueryString.GetValueOrDefault("IsProtocol", False)
        End Get
    End Property

    Protected ReadOnly Property IsMessageReadable As Boolean
        Get
            If Not IsProtocol Then
                Return True
            End If

            If Not _isMessageReadable.HasValue Then
                Dim protocol As Protocol = Facade.ProtocolFacade.GetById(DocumentUnitId)
                _isMessageReadable = New ProtocolRights(protocol).IsDocumentReadable
            End If
            Return _isMessageReadable.Value
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class