Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports Microsoft.Reporting.WebForms

Public Class ReslRegPub
    Inherits BasePrintRpt

#Region " Fields "

    Private _idContainers As String = String.Empty
    Private _containersName As String = String.Empty
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

    Private Sub CreatePrint()
        Dim resolutions As IList(Of Resolution) = Finder.DoSearch()
        If resolutions.IsNullOrEmpty Then
            Throw New DocSuiteException("Errore stampa", "Nessuna pubblicazione trovata.")
        End If

        TitlePrint = String.Format("Registro delle Pubblicazioni")

        Dim dsResls As New DataSet("Resolutions")
        dsResls.Tables.Add("tblResolution")
        dsResls.Tables(0).Columns.Add("NumeroPubblicazione", GetType(System.String))
        dsResls.Tables(0).Columns.Add("ProvNumber", GetType(System.String))
        dsResls.Tables(0).Columns.Add("FullNumber", GetType(System.String))
        dsResls.Tables(0).Columns.Add("Oggetto", GetType(System.String))
        dsResls.Tables(0).Columns.Add("AnnoAdozione", GetType(System.String))
        dsResls.Tables(0).Columns.Add("DataAdozione", GetType(System.String))
        dsResls.Tables(0).Columns.Add("DataPubblicazione", GetType(System.String))
        dsResls.Tables(0).Columns.Add("ImmediatelyExecutive", GetType(System.Boolean))
        dsResls.Tables(0).Columns.Add("Tipo", GetType(System.String))

        dsResls.Tables.Add("tblContainers")
        dsResls.Tables(1).Columns.Add("Contenitori", GetType(System.String))

        Dim dr As DataRow = dsResls.Tables(1).NewRow()

        Dim containers As String() = ContainersName.Split(","c)
        If containers.Length = 1 Then
            dr("Contenitori") = containers(0)
            dsResls.Tables(1).Rows.Add(dr)
        End If

        For i As Integer = 0 To resolutions.Count - 1
            If (Finder.NotNumber Is Nothing OrElse resolutions(i).Number > 0) Then
                CreateRow(dsResls.Tables(0), resolutions(i))
            End If
        Next



        TablePrint.LocalReport.ReportPath = RdlcPrint
        TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(0).TableName, dsResls.Tables(0)))
        TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(1).TableName, dsResls.Tables(1)))

    End Sub

    Private Sub CreateRow(ByRef tbl As DataTable, ByVal resl As Resolution)

        Dim provNumber As String = ""
        Dim fullNumber As String = ""
        Facade.ResolutionFacade.ReslFullNumber(resl, resl.Type.Id, provNumber, fullNumber)

        Dim dr As DataRow = tbl.NewRow()

        Dim wp As IList(Of WebPublication) = Facade.WebPublicationFacade.GetByResolution(resl)
        If Not wp Is Nothing AndAlso wp.Count > 0 Then
            dr("NumeroPubblicazione") = wp(0).ExternalKey
        Else
            dr("NumeroPubblicazione") = "-"
        End If

        dr("ProvNumber") = provNumber
        dr("FullNumber") = fullNumber
        dr("DataAdozione") = resl.AdoptionDate.DefaultString()
        dr("AnnoAdozione") = resl.AdoptionDate.Value.Year

        dr("DataPubblicazione") = String.Empty
        If resl.PublishingDate.HasValue Then
            dr("DataPubblicazione") = resl.PublishingDate.DefaultString()
        End If

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
        dr("Oggetto") = subject.ToString()

        dr("ImmediatelyExecutive") = resl.ImmediatelyExecutive.GetValueOrDefault(False)

        dr("Tipo") = If(resl.Type.Id = 0, "Atto", "Delib")

        tbl.Rows.Add(dr)

    End Sub

#End Region

#Region "IPrint Implementation"

    Public Overrides Sub DoPrint()
        createPrint()
    End Sub

#End Region

End Class
