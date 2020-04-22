Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
Imports System.Web
Imports System.Linq

Public Class ReslBasePage
    Inherits CommonBasePage

#Region " Fields "

    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "Resl"

    Public Const ConfTo As String = "ASL3-TO"
    Public Const ConfAuslPc As String = "AUSL-PC"
    Public Const ConfAFOL As String = "AFOLMB"

    Private _idResolution As Integer?
    Private _currentResolution As Resolution
    Private _currentResolutionRight As ResolutionRights
    Private _currentResolutionKindFacade As ResolutionKindFacade
    Private _currentResolutionDocuments As IList(Of PublicationResolutionDocumentModel)
#End Region

#Region " Properties "

    Public ReadOnly Property HasIdResolution As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.AllKeys.Any(Function(f) f.Eq("IdResolution"))
        End Get
    End Property
    Public Property IdResolution() As Integer
        Get
            If Not _idResolution.HasValue Then
                _idResolution = GetKeyValue(Of Integer)("IdResolution")
            End If
            Return _idResolution.Value
        End Get
        Set(ByVal value As Integer)
            ''Salvo in ViewState il valore (solamente se già non presente)
            If ViewState("IdResolution") = Nothing OrElse String.IsNullOrEmpty(ViewState("IdResolution").ToString()) Then
                ViewState("IdResolution") = value
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentResolution() As Resolution
        Get
            If _currentResolution Is Nothing Then
                _currentResolution = Facade.ResolutionFacade.GetById(IdResolution)
            End If
            Return _currentResolution
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionRight As ResolutionRights
        Get
            If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentResolutionRight = New ResolutionRights(CurrentResolution)
            End If
            Return _currentResolutionRight
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionKindFacade As ResolutionKindFacade
        Get
            If _currentResolutionKindFacade Is Nothing Then
                _currentResolutionKindFacade = New ResolutionKindFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentResolutionKindFacade
        End Get
    End Property

    Public Property CurrentResolutionDocuments As IList(Of PublicationResolutionDocumentModel)
        Get
            Return TryCast(Session("ResolutionDocuments"), IList(Of PublicationResolutionDocumentModel))
        End Get
        Set(value As IList(Of PublicationResolutionDocumentModel))
            If value Is Nothing Then
                Session.Remove("ResolutionDocuments")
            Else
                Session("ResolutionDocuments") = value
            End If
        End Set
    End Property

#End Region

#Region " Methods "

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, ReslBasePage)(key)
    End Function

    Public Sub PublicateResolutionOnSharePoint(ByRef resolution As Resolution, publicationDate As Date, addWatermark As Boolean)
        '' Metadati per la pubblicazione
        Dim strXmlOther As String = Facade.ResolutionWPFacade.GetXmlOther(resolution)

        '' Documento da pubblicare
        Dim watermark As String = If(addWatermark, DocSuiteContext.Current.ResolutionEnv.WebPublishWatermark, String.Empty)
        Dim documentToPublishIdChain As Integer
        Dim strXmlDoc As String = Facade.ResolutionWPFacade.GetSharepointPublicationXml(resolution, watermark, documentToPublishIdChain)

        '' Verifico se la pubblicazione è di tipo Privacy (ovvero se è calcolata la colonna idPrivacyPublication su FileResolution)
        Dim fileResolution As IList(Of FileResolution) = Facade.FileResolutionFacade.GetByResolution(resolution)
        Dim isPrivacy As Boolean = fileResolution IsNot Nothing AndAlso fileResolution.Count > 0 AndAlso fileResolution(0).IdPrivacyPublicationDocument.HasValue AndAlso fileResolution(0).IdPrivacyPublicationDocument.Value > 0

        '' Pubblicazione effettiva su SharePoint
        VecompSoftware.DocSuiteWeb.Facade.Common.SharePointFacade.Publish(resolution, publicationDate, publicationDate.AddDays(15), strXmlDoc, strXmlOther, documentToPublishIdChain, 0, isPrivacy)
    End Sub

    ''' <summary> Ottiene la pagina di visualizzazione dell'atto. </summary>
    Public Shared Function GetViewPageName(ByVal idResolution As Integer) As String
        Dim reslWorkflow As ResolutionWorkflow = (New ResolutionWorkflowFacade).SqlResolutionWorkflowSearch(idResolution, 0)
        If reslWorkflow IsNot Nothing Then
            Return "ReslVisualizza.aspx"
        End If
        Return "ReslVisualizzaStorico.aspx"
    End Function

    ''' <summary> Restituisce l'icona corrispondente al tipo/stato della resolution. </summary>
    Public Shared Function DefineIcon(ByVal type As ResolutionType, ByVal statusId As Integer, ByVal largeIcon As Boolean) As String
        ' TODO: Spostare in GUI nella classe imagepath 
        Return DefineIcon(type.Id, statusId, largeIcon)
    End Function

    Public Shared Function DefineIcon(idType As Short, statusId As Integer, largeIcon As Boolean) As String
        Dim imagePath As String = "../Resl/images/"
        Select Case idType
            Case ResolutionType.IdentifierDelibera
                Select Case statusId
                    Case 0 : imagePath &= "Delibera"
                    Case -10 : imagePath &= "Delibera"
                    Case -1 : imagePath &= "DeliberaErrata"
                    Case -2 : imagePath &= "DeliberaAnnullata"
                    Case -3 : imagePath &= "DeliberaRevocata"
                    Case -4 : imagePath &= "DeliberaRettificata"
                    Case -99 : imagePath &= "X16" ' IMPORT FROM DOCSUITE-TO
                    Case Else : imagePath &= "Delibera"

                End Select
            Case ResolutionType.IdentifierDetermina
                Select Case statusId
                    Case 0 : imagePath &= "Atto"
                    Case -10 : imagePath &= "Atto"
                    Case -1 : imagePath &= "AttoErrato"
                    Case -2 : imagePath &= "AttoAnnullato"
                    Case -3 : imagePath &= "AttoRevocato"
                    Case -4 : imagePath &= "AttoRettificato"
                    Case -99 : imagePath &= "X16" ' IMPORT FROM DOCSUITE-TO
                    Case Else : imagePath &= "Atto"
                End Select
        End Select
        If largeIcon Then
            imagePath &= "32"
        End If
        Return imagePath & ".gif"
    End Function

    Protected Sub ResetCurrentResolutionState()
        Me._currentResolution = Nothing
        Me._currentResolutionRight = Nothing
    End Sub

#End Region

End Class
