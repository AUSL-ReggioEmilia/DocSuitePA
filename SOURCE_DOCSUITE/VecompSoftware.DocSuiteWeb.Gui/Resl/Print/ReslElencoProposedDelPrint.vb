Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Microsoft.Reporting.WebForms
Imports System.Text
Imports System.Linq

Public Class ReslElencoProposedDelPrint
    Inherits BasePrintRpt

#Region " Fields "

    Private _idContainers As String = String.Empty
    Private _containersName As String = String.Empty
    Private _tipologia As Short
    Private _sessionDate As Date?
    Private _gestione As Boolean
    Private _omissis As Boolean

#End Region

#Region " Properties "


    Public Property IdContainers() As String
        Get
            Return _idContainers
        End Get
        Set(ByVal value As String)
            _idContainers = value
        End Set
    End Property

    Public Property SessionDate() As Date?
        Get
            Return _sessionDate
        End Get
        Set(ByVal value As Date?)
            _sessionDate = value
        End Set
    End Property

    Public Property Gestione() As Boolean
        Get
            Return _gestione
        End Get
        Set(ByVal value As Boolean)
            _gestione = value
        End Set
    End Property

    Public Property Omissis() As Boolean
        Get
            Return _omissis
        End Get
        Set(ByVal value As Boolean)
            _omissis = value
        End Set
    End Property

    Public Property ContainersName() As String
        Get
            Return _containersName
        End Get
        Set(ByVal value As String)
            _containersName = value
        End Set
    End Property

#End Region

#Region " Methods "

    Private Sub createPrint()
        Dim resls As IList(Of Resolution)
        resls = Finder.DoSearch()

        If resls.Count = 0 Then
            Throw New Exception("Ricerca Nulla")
        End If

        TitlePrint = String.Format("TRASMISSIONE DELIBERE COLLEGIO SINDACALE SEDUTA DEL GIORNO {0:dd/MM/yyyy}", SessionDate)

        Dim dsResls As New DataSet("Resolutions")
        dsResls.Tables.Add("tblResolution")
        dsResls.Tables(0).Columns.Add("Numero", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Numero2", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Struttura", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Oggetto", GetType(System.String))
        dsResls.Tables(0).Columns.Add("DataAdozione", GetType(System.DateTime))

        dsResls.Tables.Add("tblContainers")
        dsResls.Tables(1).Columns.Add("Contenitori", GetType(System.String))

        Dim resl As Resolution = resls(0)
        Dim dr As DataRow = dsResls.Tables(1).NewRow()
        dr("Contenitori") = resl.Container.Name
        dsResls.Tables(1).Rows.Add(dr)

        For i As Integer = 0 To resls.Count - 1
            If (Finder.NotNumber Is Nothing OrElse resls(i).Number > 0) Then
                createProtocolRow(dsResls.Tables(0), resls(i), 0)
            End If
        Next

        TablePrint.LocalReport.ReportPath = RdlcPrint
        TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(0).TableName, dsResls.Tables(0)))
        TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(1).TableName, dsResls.Tables(1)))

    End Sub

    Private Sub createProtocolRow(ByRef tbl As DataTable, ByVal resl As Resolution, ByVal Index As Integer)
        Dim ProvNumber As String = String.Empty
        Dim FullNumber As String = String.Empty
        Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, ProvNumber, FullNumber)

        Dim dr As DataRow = tbl.NewRow()
        dr("Numero") = ProvNumber
        dr("Numero2") = getDetermina(resl)

        Dim subject As New StringBuilder(ResolutionUtil.OggettoPrivacy(resl, _omissis))
        If resl.Status.Id <> ResolutionStatusId.Annullato Then
            If resl.ImmediatelyExecutive.GetValueOrDefault(False) Then
                subject.AppendLine()
                subject.Append("IMMEDIATAMENTE ESEGUIBILE")
            End If
        Else
            subject.AppendLine()
            subject.AppendLine()
            subject.Append("ERRATA REGISTRAZIONE")
            If Not IsNothing(resl.LastChangedReason) AndAlso Not IsDBNull(resl.LastChangedReason) AndAlso resl.LastChangedReason.Length > 0 Then
                subject.AppendFormat(": {0}", resl.LastChangedReason)
            End If
        End If
        dr("Struttura") = GetStruttura(resl)
        dr("Oggetto") = subject.ToString()
        dr("DataAdozione") = resl.AdoptionDate
        tbl.Rows.Add(dr)

    End Sub

    Private Function getDetermina(ByVal resl As Resolution) As String
        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                proposerCode = String.Join(", ", resl.ResolutionContactProposers.Select(Function(s) s.Contact.Code))
            End If
            Return proposerCode
        Else
            Return String.Empty
        End If
    End Function

    Private Function getStruttura(ByVal resl As Resolution) As String
        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                proposerCode = String.Join(", ", resl.ResolutionContactProposers.Select(Function(s) s.Contact.Description))
            End If
            Return proposerCode
        Else
            Return String.Empty
        End If
    End Function

    Private Shared Function getControllo(ByRef resl As Resolution) As String
        Return ResolutionJournalPrinter.GetControllo(resl)
    End Function

#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        createPrint()
    End Sub
#End Region

End Class
