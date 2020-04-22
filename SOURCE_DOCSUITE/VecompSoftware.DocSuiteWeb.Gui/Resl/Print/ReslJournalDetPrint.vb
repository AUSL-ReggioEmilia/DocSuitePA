Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Microsoft.Reporting.WebForms
Imports System.Linq
''' <summary>
''' Stampa del registro giornaliero 
''' </summary>
Public Class ReslJournalDetPrint
    Inherits BasePrintRpt

#Region "Fields"
    Private _omissis As Boolean
    Private _tipologia As Short
    Private _containersName As String = String.Empty
#End Region

#Region "Properties"

    Public Property Omissis() As Boolean
        Get
            Return _omissis
        End Get
        Set(ByVal value As Boolean)
            _omissis = value
        End Set
    End Property

    Public Property Tipologia() As Short
        Get
            Return _tipologia
        End Get
        Set(ByVal value As Short)
            _tipologia = value
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

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Registro Determinazioni"

        CreatePrint()
    End Sub
#End Region

#Region "Private Functions"

    Private Sub CreatePrint()
        Dim resls As IList(Of Resolution)
        resls = Finder.DoSearch()
        Dim dsResls As DataSet

        If (resls.Count > 0) Then

            dsResls = New DataSet("Resolutions")
            dsResls.Tables.Add("tblResolution")
            dsResls.Tables(0).Columns.Add("Numero", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Numero2", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Controllo", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Oggetto", GetType(System.String))
            dsResls.Tables(0).Columns.Add("DataAdozione", GetType(System.DateTime))
            dsResls.Tables(0).Columns.Add("PublishingDate", GetType(System.DateTime))
            dsResls.Tables(0).Columns.Add("EffectivenessDate", GetType(System.DateTime))
            dsResls.Tables(0).Columns.Add("ImmediatelyExecutive", GetType(System.Boolean))

            dsResls.Tables.Add("tblContainers")
            dsResls.Tables(1).Columns.Add("Contenitori", GetType(System.String))
            Dim dr As DataRow = dsResls.Tables(1).NewRow()
            dr("Contenitori") = ContainersName
            dsResls.Tables(1).Rows.Add(dr)

            For i As Integer = 0 To resls.Count - 1
                If (Finder.NotNumber Is Nothing OrElse resls(i).Number > 0) Then
                    CreateProtocolRow(dsResls.Tables(0), resls(i), 0)
                End If
            Next

            TablePrint.LocalReport.ReportPath = RdlcPrint
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(0).TableName, dsResls.Tables(0)))
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(1).TableName, dsResls.Tables(1)))

        Else
            Throw New Exception("Ricerca Nulla")
        End If

    End Sub

    Private Sub CreateProtocolRow(ByRef tbl As DataTable, ByVal resl As Resolution, ByVal Index As Integer)

        Dim dr As DataRow = tbl.NewRow()
        dr("Numero") = resl.Number.Value
        dr("Numero2") = GetDetermina(resl)
        dr("Controllo") = GetControllo(resl)
        dr("Oggetto") = ResolutionUtil.OggettoPrivacy(resl, _omissis) & _
            If(resl.ImmediatelyExecutive.GetValueOrDefault(False) = True, vbCrLf & "IMMEDIATAMENTE ESEGUIBILE", "")
        dr("ImmediatelyExecutive") = False
        If resl.AdoptionDate.HasValue Then
            dr("DataAdozione") = resl.AdoptionDate
        End If
        If resl.PublishingDate.HasValue Then
            dr("PublishingDate") = resl.PublishingDate
        End If
        If resl.EffectivenessDate.HasValue Then
            dr("EffectivenessDate") = resl.EffectivenessDate
        End If

        tbl.Rows.Add(dr)

    End Sub

    Private Function GetDetermina(ByVal resl As Resolution) As String

        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then proposerCode = resl.ResolutionContactProposers.ElementAt(0).Contact.Code
            Return "/" & proposerCode & "/" & resl.Year.ToString
        Else
            Return String.Empty
        End If

    End Function

    Private Function GetControllo(ByVal resl As Resolution) As String
        Dim sControllo As String = ""
        If resl.OCSupervisoryBoard.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "art.14"
        End If
        If resl.OCRegion.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Regione"
        End If
        If resl.OCManagement.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Controllo Gestione"
        End If
        If resl.OCCorteConti.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Corte dei Conti"
        End If
        If resl.OCOther.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= resl.OtherDescription
        End If

        Return sControllo
    End Function
#End Region

End Class
