
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports System.Web
Imports System.Threading.Tasks

''' <summary>
''' Classe di gestione Frontalino rdlc per Torino
''' </summary>
Public Class ReslFrontalinoPrintPdfTO
    Inherits ReportViewerPdfExporter


#Region "Fields"
    Private _dataSource As IList(Of Resolution)
    Private _frontalinoDS As New Frontalino
    Public Const FRONTALINO_NAME As String = "Frontalino.pdf"
    Public Const FRONTALINO_PRIVACY_NAME As String = "FrontalinoOmissis.pdf"
    Private Const FRONTALINO_PATH As String = "~/{0}Frontalino.rdlc"
#End Region

#Region "Properties"
    Public Shadows Property DataSource() As IList(Of Resolution)
        Get
            Return _dataSource
        End Get
        Set(ByVal value As IList(Of Resolution))
            _dataSource = value
            If (Me._dataSource IsNot Nothing) Then
                For Each resl As Resolution In Me._dataSource
                    AppendRowFromResolution(resl)
                Next
            End If

        End Set
    End Property
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        MyBase.DataSource = Me._frontalinoDS
        MyBase.PrimaryTableName = Me._frontalinoDS.FrontalinoTable.TableName
        MyBase.DoPrint()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub AppendRowFromResolution(ByVal resl As Resolution)

        Dim row As Frontalino.FrontalinoTableRow = Me._frontalinoDS.FrontalinoTable.NewFrontalinoTableRow()

        row.Numero = resl.NumberFormat("{0}")
        row.Anno = resl.Year
        row.DataAdozione = StrConv(Format(resl.AdoptionDate, "dd MMMM yyyy"), vbProperCase)
        row.Oggetto = resl.ResolutionObject
        row.AnnoLet = StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Year))
        row.GiornoLet = StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Day))
        row.MeseLet = StringHelper.UppercaseFirst(resl.AdoptionDate.Value.MonthName)
        row.Art14 = If(resl.OCSupervisoryBoard.GetValueOrDefault(False), "art.14", "")
        row.Regione = If(resl.OCRegion.GetValueOrDefault(False), "Regione", "")
        row.Gestione = If(resl.OCManagement.GetValueOrDefault(False), "Controllo Gestione", "")
        row.OCCorteConti = If(resl.OCCorteConti.GetValueOrDefault(False), "Corte dei Conti", "")
        row.Altro = If(resl.OCOther.GetValueOrDefault(False), resl.OtherDescription, "")
        row.HeadingF = resl.Container.HeadingFrontalino


        If resl.ResolutionContactProposers IsNot Nothing AndAlso
         resl.ResolutionContactProposers.Count > 0 AndAlso
         resl.ResolutionContactProposers.ElementAt(0).Contact IsNot Nothing AndAlso
         Not String.IsNullOrEmpty(resl.ResolutionContactProposers.ElementAt(0).Contact.Code) Then

            row.CodiceServ = resl.ResolutionContactProposers.ElementAt(0).Contact.Code
        End If

        Me._frontalinoDS.FrontalinoTable.AddFrontalinoTableRow(row)

    End Sub

    Private Sub ClearDataSource()
        Me._frontalinoDS.FrontalinoTable.Clear()
    End Sub
#End Region

    ''' <summary>
    ''' Genera il file del frontalino e lo associa automaticamente alla Resolution.
    ''' In caso contenitore soggetto a Privacy i frontalini creati sono due-
    ''' </summary>
    ''' <param name="resolution">Resolution soggetto del frontalino</param>
    ''' <returns>Restituisce l'elenco dei frontalini creati.</returns>
    ''' <remarks></remarks>
    Public Function GeneraFrontalini(ByVal resolution As Resolution) As ICollection(Of ResolutionFrontispiece)
        Dim resolutionFrontispieces As ICollection(Of ResolutionFrontispiece) = New List(Of ResolutionFrontispiece)
        Try
            Dim frontName As String = String.Concat(CommonUtil.GetInstance().AppTempPath, CommonUtil.UserDocumentName, "-Print-", String.Format("{0:HHmmss}", Now()), "_fr_", resolution.Id.ToString(), FileHelper.PDF)
            GeneraFrontalinoPdf(frontName, resolution, False)

            resolutionFrontispieces.Add(New ResolutionFrontispiece() With {.Name = Path.GetFileName(frontName), .Path = frontName, .IsPrivacy = False})
            If resolution.Container.Privacy.HasValue AndAlso Convert.ToBoolean(resolution.Container.Privacy.Value) Then
                ClearDataSource()
                Dim frontPrivacyName As String = String.Concat(CommonUtil.GetInstance().AppTempPath, CommonUtil.UserDocumentName, "-PrintPrivacy-", String.Format("{0:HHmmss}", Now()), "_fr_", resolution.Id.ToString(), FileHelper.PDF)
                GeneraFrontalinoPdf(frontPrivacyName, resolution, True)

                resolutionFrontispieces.Add(New ResolutionFrontispiece() With {.Name = Path.GetFileName(frontPrivacyName), .Path = frontPrivacyName, .IsPrivacy = True})
            End If
            Return resolutionFrontispieces
        Catch ex As Exception
            Throw New DocSuiteException(ex.Message, ex)
        End Try

    End Function

    'Metodo che salva in biblos i frontalini e aggiorna la FileResolution
    Public Sub SaveBiblosFrontispieces(frontispieces As ICollection(Of ResolutionFrontispiece), resolution As Resolution, signature As String)
        Dim frontalini As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        Dim toSaveFileInfo As FileDocumentInfo = Nothing

        For Each item As ResolutionFrontispiece In frontispieces
            toSaveFileInfo = New FileDocumentInfo(New FileInfo(item.Path))
            If item.IsPrivacy Then
                toSaveFileInfo.Name = FRONTALINO_PRIVACY_NAME
            Else
                toSaveFileInfo.Name = FRONTALINO_NAME
            End If

            'I frontespizi hanno privacy level 0
            If Not toSaveFileInfo.Attributes.Any(Function(x) x.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
                toSaveFileInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, 0)
            End If

            frontalini.Add(toSaveFileInfo)
        Next

        Dim idCatena As Integer = 0
        ResolutionFacade.SaveBiblosDocuments(resolution, frontalini, Nothing, signature, idCatena, 0)
        ' registrazione del id catena in db
        Facade.ResolutionFacade.SqlResolutionDocumentUpdate(resolution.Id, idCatena, ResolutionFacade.DocType.Frontespizio)
    End Sub

    ''' <summary>
    ''' Metodo che genera i frontalini in PDF tramite report viewer
    ''' </summary>
    ''' <remarks>
    ''' A Torino, se si sta gestendo un contenitore con privacy attiva, il contenuto dell'oggetto standard
    ''' deve essere associato al frontalino omissis (o privacy), altrimenti se il contenitore non ha privacy attiva
    ''' il contenuto dell'oggetto standard deve essere associato al Frontalino standard.
    ''' Viceversa se si sta gestendo un contenitore con privacy attiva, il contenuto dell'oggetto privacy (o integrale) deve 
    ''' essere associato al Frontalino standard, altrimenti se il contenitore non ha privacy attiva non verrà generato nessun Frontalino
    ''' omissis (o privacy).
    ''' </remarks>
    ''' <param name="filename"></param>
    ''' <param name="resolution"></param>
    ''' <param name="setPrivacy"></param>
    Private Sub GeneraFrontalinoPdf(filename As String, resolution As Resolution, setPrivacy As Boolean)
        Dim isContainerPrivacy As Boolean = resolution.Container.Privacy.HasValue AndAlso Convert.ToBoolean(resolution.Container.Privacy.Value)
        If Not isContainerPrivacy AndAlso setPrivacy Then
            Throw New DocSuiteException("GeneraFrontalinoPdf -> Non è possibile generare un Frontalino Privacy per un contenitore con Privacy disabilitata")
        End If

        Dim tmpResolutionObject As String = resolution.ResolutionObject
        If isContainerPrivacy AndAlso Not setPrivacy Then
            resolution.ResolutionObject = resolution.ResolutionObjectPrivacy
        End If

        Dim resolutions As IList(Of Resolution) = New List(Of Resolution)
        resolutions.Add(resolution)
        DataSource = resolutions
        RdlcPrint = HttpContext.Current.Server.MapPath(String.Format(FRONTALINO_PATH, ResolutionFacade.PathStampeTo))
        RdlcFilenameSuffix = resolution.Container.Id.ToString()
        DoPrint()

        Dim bytes As Byte()

        Try
            Dim generateReport As Task = Task.Factory.StartNew(Sub()
                                                                   bytes = TablePrint().LocalReport.Render("PDF", Nothing, "", "", "", Nothing, Nothing)
                                                               End Sub)
            Task.WaitAll({generateReport}, DocSuiteContext.Current.ResolutionEnv.ReportPrintTimer)
            If Not generateReport.IsCompleted Then
                Throw New Exception("La generazione del frontalino ha superato il tempo massimo impostato da parametro.")
                Exit Sub
            Else
                File.WriteAllBytes(filename, bytes)
                ' ripristino l'oggetto
                resolution.ResolutionObject = tmpResolutionObject
            End If
        Catch ex As AggregateException
            Throw ex.Flatten()
        End Try

    End Sub


End Class
