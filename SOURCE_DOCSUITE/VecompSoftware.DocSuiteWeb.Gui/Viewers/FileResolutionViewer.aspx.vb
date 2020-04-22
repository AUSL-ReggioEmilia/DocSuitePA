Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Newtonsoft.Json
Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports NHibernate.Util

Namespace Viewers

    Public Class FileResolutionViewer
        Inherits ReslBasePage
        Implements ISendMail

#Region " Fields "

        Private _field As String
        Private _descriptions As String
        Private _fields As IDictionary(Of String, String)
#End Region

#Region " Properties "

        Private ReadOnly Property Field As String
            Get
                If ViewState("field") Is Nothing Then
                    _field = Request.QueryString.GetValueOrDefault(Of String)("field", Nothing)
                    ViewState("field") = _field
                Else
                    _field = CType(ViewState("field"), String)
                End If
                Return _field
            End Get
        End Property

        Private ReadOnly Property Description As String
            Get
                If ViewState("description") Is Nothing Then
                    _descriptions = Request.QueryString.GetValueOrDefault(Of String)("description", "Documenti")
                    ViewState("description") = _descriptions
                Else
                    _descriptions = CType(ViewState("description"), String)
                End If
                Return _descriptions
            End Get
        End Property

        ''' <summary> Dizionario con in chiave il nome del campo e in valore il nodo (descrizione) di destinazione. </summary>
        Private ReadOnly Property Descriptors As IDictionary(Of String, String)
            Get
                If _fields Is Nothing Then
                    If Not String.IsNullOrEmpty(Field) Then
                        ' Inizializzo da altri parametri
                        _fields = New Dictionary(Of String, String)
                        _fields.Add(Field, Description)
                    ElseIf ViewState("descriptors") Is Nothing Then
                        ' Inizializzo dalla querystring
                        Dim queryStringValue As String = Server.UrlDecode(Request.QueryString.GetValue(Of String)("descriptors"))
                        _fields = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(queryStringValue)
                        ViewState("descriptors") = _fields
                    Else
                        _fields = CType(ViewState("descriptors"), Dictionary(Of String, String))
                    End If
                End If
                Return _fields
            End Get
        End Property

        Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
            Get
                Return CommonInstance.UserDescription
            End Get
        End Property

        Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
            Get
                Return CommonInstance.UserMail
            End Get
        End Property

        Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
            Get
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get
                Dim list As IList(Of DocumentInfo) = ViewerLight.CheckedDocuments
                If list.IsNullOrEmpty() Then
                    list = GetFlatDocumentList()
                End If
                Return Facade.DocumentFacade.FilterAllowedDocuments(list, CurrentResolution.Container.Id, Nothing, Nothing, DSWEnvironment.Resolution, False)
            End Get
        End Property

        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                Return MailFacade.GetResolutionSubject(CurrentResolution)
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                Return MailFacade.GetResolutionBody(CurrentResolution)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Overridable Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            MasterDocSuite.TitleVisible = False
            ViewerLight.PrefixFileName = String.Concat("ATTI_", CurrentResolution.Year, "_", CurrentResolution.Number.ToString("0000000"))
            If Not CurrentResolution.AdoptionDate.HasValue OrElse CurrentResolution.EffectivenessDate.HasValue Then
                ViewerLight.IdContainer = CurrentResolution.Container.Id
            End If

            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                ViewerLight.DataSource = GetDataSource()
            End If
            ViewerLight.CheckViewableRight = True
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("FileResolutionViewer")
        End Sub

#End Region

#Region " Methods "

        Private Function GetFlatDocumentList() As IList(Of DocumentInfo)
            Dim list As New List(Of DocumentInfo)
            For Each item As KeyValuePair(Of String, String) In Descriptors
                Dim docs As IList(Of DocumentInfo) = Facade.ResolutionFacade.GetDocuments(CurrentResolution, item.Key, includeUniqueId:=True)
                If item.Value.Eq("Frontespizio") OrElse item.Value.Eq("Ultima pagina") OrElse item.Value.Eq("Rel. adozione") Then
                    For Each doc As DocumentInfo In docs
                        doc.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_IsPublic, "True")
                    Next
                End If
                list.AddRange(docs)
            Next
            Return list
        End Function

        Protected Function GetDataSource() As IList(Of DocumentInfo)
            If Descriptors.IsNullOrEmpty() Then
                Return Nothing
            End If

            Dim folder As New FolderInfo() With {.ID = CurrentResolution.Id.ToString(), .Name = Description}
            folder.AddChildren(GetFlatDocumentList())

            Dim dataSource As New List(Of DocumentInfo)
            dataSource.Add(folder)
            Return dataSource
        End Function

#End Region

    End Class

End Namespace