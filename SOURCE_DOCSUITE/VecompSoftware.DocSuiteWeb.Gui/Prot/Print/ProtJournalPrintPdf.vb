Imports System.Collections.Generic
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Microsoft.Reporting.WebForms

'' <summary>
'' Stampa del registro giornaliero 
'' </summary>
'' <remarks></remarks>
Public Class ProtJournalPrintPdf
    Inherits BasePrintRpt

#Region "Fields"
    Private _finder As NHibernateProtocolFinder
    Private _containersName As String
#End Region

#Region "Properties"
    Public Property Finder() As NHibernateProtocolFinder
        Get
            Return _finder
        End Get
        Set(ByVal value As NHibernateProtocolFinder)
            _finder = value
            _finder.LoadFetchModeFascicleEnabled = False
            _finder.LoadFetchModeProtocolLogs = False
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

#Region "Create Rows in Dataset"

    Private Sub CreateProtocolRow(ByRef tbl As DataSet, ByVal protocol As Protocol, ByVal Index As Integer)

        Dim rd As DataRow = tbl.Tables(Index).NewRow()
        Dim text As String = String.Empty

        'Contenitore
        _containersName = _containersName & "," & protocol.Container.Name

        'Data registrazione
        rd("DataRegistrazione") = protocol.RegistrationDate.ToLocalTime()

        'Protocollo
        text = ProtocolFacade.ProtocolFullNumber(protocol.Year, protocol.Number) & vbCrLf & String.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime())
        rd("Protocollo") = text

        'Tipo
        text = ProtocolTypeFacade.CalcolaTipoProtocollo(protocol.Type.Id)
        rd("Tipo") = text

        'Protocollo Origine
        text = Replace("" & protocol.DocumentProtocol, "|", "/") & protocol.DocumentDate.ToString()
        rd("POrigin") = text

        'Categoria
        rd("Classificatore") = protocol.Category.Name

        'Mittenti/Destinatari
        'Contatti
        text = String.Empty
        Dim cTipo As Char
        Select Case protocol.Type.Id
            Case -1
                cTipo = ProtocolContactCommunicationType.Sender
            Case 1
                cTipo = ProtocolContactCommunicationType.Recipient
        End Select

        Dim contacts As IList(Of ProtocolContact) = Facade.ProtocolContactFacade.GetByComunicationType(protocol, cTipo)
        If contacts.Count > 0 Then
            ContactFacade.FormatContacts(contacts, text)
        End If

        Dim mcontacts As IList(Of ProtocolContactManual) = Facade.ProtocolContactManualFacade.GetByComunicationType(protocol, cTipo)
        If mcontacts.Count > 0 Then
            ContactFacade.FormatContacts(mcontacts, text)
        End If
        If text <> "" Then
            text &= StringHelper.ReplaceCrLf(protocol.AlternativeRecipient)
            If text <> "" Then text = vbCrLf & text
        Else
            text &= StringHelper.ReplaceCrLf(protocol.AlternativeRecipient)
        End If
        rd("MittDest") = text

        If protocol.IdStatus.HasValue Then rd("Stato") = ProtocolFacade.GetStatusDescription(protocol.IdStatus.Value)

        'Oggetto
        rd("Oggetto") = StringHelper.ReplaceCrLf(protocol.ProtocolObject)

        'Annullamento
        If protocol.IdStatus.HasValue AndAlso protocol.IdStatus.Value = ProtocolStatusId.Annullato Then
            text = "Estremi Annullamento: " & StringHelper.ReplaceCrLf(protocol.LastChangedReason)
            rd("Annullamento") = text
        End If
        tbl.Tables(Index).Rows.Add(rd)

    End Sub
    Private Sub CreateContainersRow(ByRef tbl As DataSet, ByVal Index As Integer, ByVal text As String)
        tbl.Tables(Index).Columns.Add("Contenitori", GetType(System.String))
        Dim rd As DataRow = tbl.Tables(Index).NewRow()
        rd("Contenitori") = ContainersName
        tbl.Tables(Index).Rows.Add(rd)
    End Sub

    Private Sub CreateProtocolHeader(ByRef tbl As DataSet, ByVal Index As Integer)
        tbl.Tables(Index).Columns.Add("DataRegistrazione", GetType(DateTimeOffset))
        tbl.Tables(Index).Columns.Add("Protocollo", GetType(String))
        tbl.Tables(Index).Columns.Add("Tipo", GetType(String))
        tbl.Tables(Index).Columns.Add("POrigin", GetType(String))
        tbl.Tables(Index).Columns.Add("Classificatore", GetType(String))
        tbl.Tables(Index).Columns.Add("MittDest", GetType(String))
        tbl.Tables(Index).Columns.Add("Stato", GetType(String))
        tbl.Tables(Index).Columns.Add("Oggetto", GetType(String))
        tbl.Tables(Index).Columns.Add("Annullamento", GetType(String))
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()

        Dim LastDate As Date = Nothing
        Dim protocols As IList(Of Protocol)
        Dim dsProtocols As DataSet
        protocols = Finder.DoSearch()

        'Converto il Protocolli trovati in un Dataset da passare la ReportViewer

        If (protocols.Count > 0) Then
            dsProtocols = New DataSet("Protocols")
            dsProtocols.Tables.Add("tblProtocols")
            dsProtocols.Tables.Add("tblContainers")

            ' Definisco la struttura del Dataset per il Stampa Registro
            CreateProtocolHeader(dsProtocols, 0)
            CreateContainersRow(dsProtocols, 1, ContainersName)
            '_table.Table.WriteXmlSchema("c:\Protcols.xsd")

            For i As Integer = 0 To protocols.Count - 1
                CreateProtocolRow(dsProtocols, protocols(i), 0)
            Next

            TablePrint.LocalReport.ReportPath = RdlcPrint
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsProtocols.DataSetName & "_" & dsProtocols.Tables(0).TableName, dsProtocols.Tables(0)))
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsProtocols.DataSetName & "_" & dsProtocols.Tables(1).TableName, dsProtocols.Tables(1)))

        Else
            Throw New DocSuiteException("Ricerca Nulla")
        End If

    End Sub
#End Region

End Class
