
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports System.Web
Imports System.Linq
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslUltimaPaginaPrintPdf
    Inherits ReportViewerPdfExporter


#Region "Fields"
    Private ReadOnly _omissis As Boolean
    Private _dataSource As IList(Of Resolution)
    Private _internalDS As New UltimaPagina
#End Region

#Region "Ctor"
    Public Sub New(ByVal omissis As Boolean)
        Me._omissis = omissis
    End Sub
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
        MyBase.DataSource = Me._internalDS
        MyBase.PrimaryTableName = Me._internalDS.UltimaPaginaTable.TableName
        MyBase.DoPrint()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub AppendRowFromResolution(ByVal resl As Resolution)

        Dim reslFacade As New ResolutionFacade()
        Dim row As UltimaPagina.UltimaPaginaTableRow = Me._internalDS.UltimaPaginaTable.NewUltimaPaginaTableRow()

        row.Numero = Facade.ResolutionFacade.CalculateFullNumber(resl, resl.Type.Id, False)
        row.Anno = If(resl.Year.HasValue, "" & resl.Year, "")
        row.DataAdozione = String.Format("{0:dd MMMM yyyy}", resl.AdoptionDate).ToUpper()
        row.IE = If(resl.ImmediatelyExecutive.GetValueOrDefault(False), "IMMEDIATAMENTE ESEGUIBILE", "")
        If Not String.IsNullOrEmpty(resl.SupervisoryBoardProtocolLink) Then
            Dim s() As String = Split(resl.SupervisoryBoardProtocolLink, "|")
            row.NumProt = ProtocolFacade.ProtocolFullNumber(s(0), s(1))
            row.DataProt = String.Format("{0:dd/MM/yyyy}", CDate(s(3)))
        Else
            row.NumProt = ""
            row.DataProt = ""
        End If
        If (resl.OCRegion.GetValueOrDefault(False)) Then
            row.DataSpedRegione = String.Format("{0:dd/MM/yyyy}", resl.WarningDate)
            If Not String.IsNullOrEmpty(resl.RegionProtocolLink) Then
                Dim s() As String = Split(resl.RegionProtocolLink, "|")
                row.NumeroProtSpedRegione = ProtocolFacade.ProtocolFullNumber(s(0), s(1))
            Else
                row.NumeroProtSpedRegione = ""
            End If
            row.DataRicRegione = String.Format("{0:dd/MM/yyyy}", resl.ConfirmDate)
            row.Dgr = resl.DGR
            row.DataRispRegione = String.Format("{0:dd/MM/yyyy}", resl.ResponseDate)
            Dim sCommentoRegione As String = String.Empty
            If resl.ControllerStatus IsNot Nothing Then sCommentoRegione = resl.ControllerStatus.Description
            row.CommentoRegione = sCommentoRegione
            row.ApprovalNote = resl.ApprovalNote
            row.DeclineNote = resl.DeclineNote
        Else
            row.DataSpedRegione = ""
            row.NumeroProtSpedRegione = ""
            row.DataRicRegione = ""
            row.Dgr = ""
            row.DataRispRegione = ""
            row.CommentoRegione = ""
            row.ApprovalNote = ""
            row.DeclineNote = ""
        End If
        If (resl.OCManagement.GetValueOrDefault(False)) Then
            row.DataProtGestione = String.Format("{0:dd/MM/yyyy}", resl.ManagementWarningDate)
            If Not String.IsNullOrEmpty(resl.ManagementProtocolLink) Then
                Dim s() As String = Split(resl.ManagementProtocolLink, "|")
                row.NumProtGestione = ProtocolFacade.ProtocolFullNumber(s(0), s(1))
            Else
                row.NumProtGestione = ""
            End If
        Else
            row.DataProtGestione = ""
            row.NumProtGestione = ""
        End If
        row.DataPubblicazione = String.Format("{0:dd MMMM yyyy}", resl.PublishingDate).ToUpper
        row.DataEsecutivita = String.Format("{0:dd MMMM yyyy}", resl.EffectivenessDate).ToUpper
        row.DataOdierna = String.Format("{0:dd MMMM yyyy}", DateTime.Now)
        row.Oggetto = ResolutionUtil.OggettoPrivacy(resl, Me._omissis)
        row.Note = StringHelper.ReplaceCrLf(resl.Note)
        row.AnnoLet = StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Year))
        row.GiornoLet = StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Day))
        row.MeseLet = StringHelper.UppercaseFirst(resl.AdoptionDate.Value.MonthName)
        row.Art14 = If(resl.OCSupervisoryBoard.GetValueOrDefault(False), "art.14", "")
        row.Regione = If(resl.OCRegion.GetValueOrDefault(False), "Regione", "")
        row.Gestione = If(resl.OCManagement.GetValueOrDefault(False), "Controllo Gestione", "")
        row.OCCorteConti = If(resl.OCCorteConti.GetValueOrDefault(False), "Corte dei Conti", "")


        row.NumProtOCCorteConti = String.Empty
        row.DataProtOCCorteConti = String.Empty
        If Not String.IsNullOrEmpty(resl.CorteDeiContiProtocolLink) Then
            Dim s() As String = Split(resl.CorteDeiContiProtocolLink, "|")
            row.NumProtOCCorteConti = ProtocolFacade.ProtocolFullNumber(s(0), s(1))
            row.DataProtOCCorteConti = String.Format("{0:dd/MM/yyyy}", resl.CorteDeiContiWarningDate)
        End If

        Me._internalDS.UltimaPaginaTable.AddUltimaPaginaTableRow(row)

    End Sub
#End Region

    Public Function GeneraUltimaPagina(ByVal resolution As Resolution, ByVal archive As Boolean) As String
        Dim tempSingoloFileUltimaPagina As String = CommonUtil.GetInstance().AppTempPath & CommonUtil.UserDocumentName & "-Print-" & String.Format("{0:HHmmss}", Now()) & "_up_" & resolution.Id.ToString() & FileHelper.PDF
        Dim resls As New List(Of Resolution)
        resls.Add(resolution)

        DataSource = resls

        RdlcPrint = String.Format("{0}/UltimaPagina.rdlc", Facade.ResolutionFacade.FullPrintPath)
        DoPrint()

        Dim publicationUser As String = CommonAD.GetDisplayName(resolution.PublishingUser)
        Me.TablePrint.LocalReport.SetParameters(New ReportParameter("UtentePubblicazione", publicationUser))
        Me.TablePrint.LocalReport.SetParameters(New ReportParameter("ResolutionType", resolution.Type.Id.ToString()))

        Dim buffer As Byte() = Me.TablePrint().LocalReport.Render("PDF", Nothing, "", "", "", Nothing, Nothing)
        File.WriteAllBytes(tempSingoloFileUltimaPagina, buffer)

        If archive Then
            Dim doc As New MemoryDocumentInfo(buffer, "UltimaPagina.pdf")
            doc.Signature = Facade.ResolutionFacade.SqlResolutionGetNumber(idResolution:=resolution.Id, complete:=True)
            Dim idChain As Integer = doc.ArchiveInBiblos(resolution.Location.DocumentServer, resolution.Location.ReslBiblosDSDB).BiblosChainId
            Facade.ResolutionFacade.SqlResolutionDocumentUpdate(resolution.Id, idChain, ResolutionFacade.DocType.UltimaPagina)
        End If

        Return tempSingoloFileUltimaPagina
    End Function


End Class
