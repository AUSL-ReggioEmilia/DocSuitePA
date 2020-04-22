Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslLetteraTrasmAvvenutaAdozione
    Inherits ReportViewerPdfExporter


#Region "Fields"
    Private _dataSource As IList(Of Resolution)
    Private _internalDS As New dsLetteraTrasmAvvenutaAdozione()
#End Region

#Region "Ctor"
    Public Sub New()

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
        MyBase.PrimaryTableName = Me._internalDS.LetteraTrasmAvvenutaAdozione.TableName
        MyBase.DoPrint()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub AppendRowFromResolution(ByVal resl As Resolution)
        Dim row As dsLetteraTrasmAvvenutaAdozione.LetteraTrasmAvvenutaAdozioneRow = _internalDS.LetteraTrasmAvvenutaAdozione.NewLetteraTrasmAvvenutaAdozioneRow()

        row.Numero = String.Format("{0:0000000}", resl.Number) & "/" & UCase("" & resl.ResolutionContactProposers.ElementAt(0).Contact.Code) & "/" & resl.Year.Value
        row.Data = Format(resl.AdoptionDate, "dd/MM/yyyy")
        row.Oggetto = resl.ResolutionObject

        Dim sControllo As String = ""
        If (resl.OCSupervisoryBoard.GetValueOrDefault(False) = True) Then
            sControllo = "CS"
        End If
        If (resl.OCRegion.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= " - "
            sControllo &= "R"
        End If
        If (resl.OCManagement.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= " - "
            sControllo &= "CG"
        End If
        If (resl.OCOther.GetValueOrDefault(False) = True) Then
            If sControllo <> "" Then sControllo &= " - "
            sControllo &= resl.OtherDescription
        End If
        row.Controllo = sControllo

        Me._internalDS.LetteraTrasmAvvenutaAdozione.AddLetteraTrasmAvvenutaAdozioneRow(row)

    End Sub
#End Region



End Class
