Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports System.Linq
Public Class ReslElencoDetPrint
    Inherits BasePrintRpt

#Region " Fields "

    Private _idContainers As String = String.Empty
    Private _tipologia As Short
    Private _adoptionDate As Date?
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

    Public Property AdoptionDate() As Date?
        Get
            Return _adoptionDate
        End Get
        Set(ByVal value As Date?)
            _adoptionDate = value
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

#End Region

#Region " Methods "

    Private Sub createPrint()
        Dim resls As IList(Of Resolution)
        Finder.EagerLog = False
        resls = Finder.DoSearch()

        TitlePrint = "ELENCO DETERMINAZIONI DIRIGENZIALI ADOTTATE"

        If (resls.Count = 0) Then
            Throw New Exception("Ricerca Nulla")
        End If

        Dim dsResls As New DataSet("Resolutions")
        dsResls.Tables.Add("tblResolution")
        dsResls.Tables(0).Columns.Add("Numero", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Numero2", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Container", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Controllo", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Oggetto", GetType(System.String))
        dsResls.Tables(0).Columns.Add("DataAdozione", GetType(System.DateTime))
        dsResls.Tables(0).Columns.Add("ImmediatelyExecutive", GetType(System.Boolean))


        For i As Integer = 0 To resls.Count - 1
            If (Finder.NotNumber Is Nothing OrElse resls(i).Number > 0) Then
                createProtocolRow(dsResls.Tables(0), resls(i), 0)
            End If
        Next

        TablePrint.LocalReport.ReportPath = RdlcPrint
        TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(0).TableName, dsResls.Tables(0)))


    End Sub

    Private Sub createProtocolRow(ByRef tbl As DataTable, ByVal resl As Resolution, ByVal Index As Integer)

        Dim dr As DataRow = tbl.NewRow()
        If resl.Number.HasValue Then
            dr("Numero") = resl.Number.Value
        End If
        dr("Numero2") = getDetermina(resl)

        Dim controllo As String = getControllo(resl)
        dr("Controllo") = controllo

        dr("Container") = resl.Container.Name

        Dim subject As New StringBuilder(ResolutionUtil.OggettoPrivacy(resl, _omissis))
        If resl.ImmediatelyExecutive.GetValueOrDefault(False) Then
            subject.AppendLine()
            subject.Append("IMMEDIATAMENTE ESEGUIBILE")
        End If
        If Not String.IsNullOrEmpty(controllo) Then
            subject.AppendLine()
            subject.Append(controllo)
        End If
        If resl.Status.Id = ResolutionStatusId.Annullato Then
            subject.AppendLine()
            subject.AppendLine()
            subject.Append("ERRATA REGISTRAZIONE")
        End If
        If Not IsNothing(resl.LastChangedReason) AndAlso Not IsDBNull(resl.LastChangedReason) AndAlso resl.LastChangedReason.Length > 0 Then
            subject.AppendFormat(": {0}", resl.LastChangedReason)
        End If
        dr("Oggetto") = subject.ToString()

        dr("ImmediatelyExecutive") = False
        If resl.AdoptionDate.HasValue Then
            dr("DataAdozione") = resl.AdoptionDate
        End If
        tbl.Rows.Add(dr)
    End Sub

    Private Function getDetermina(ByVal resl As Resolution) As String

        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                proposerCode = resl.ResolutionContactProposers.ElementAt(0).Contact.Code
            End If
            Return String.Format("/{0}/{1}", proposerCode, resl.Year.ToString)
        Else
            Return String.Empty
        End If

    End Function

    Private Function getControllo(ByVal resl As Resolution) As String
        Return ResolutionJournalPrinter.GetControllo(resl)
    End Function

#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        createPrint()
    End Sub
#End Region

End Class
