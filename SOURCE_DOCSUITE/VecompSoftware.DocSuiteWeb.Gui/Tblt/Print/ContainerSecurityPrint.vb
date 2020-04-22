Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Stampa dei contenitori con visualizzazione sicurezza </summary>
Public Class ContainerSecurityPrint
    Inherits SecurityPrint

#Region "Fields"
    Private _containers As IList(Of Integer)
#End Region

#Region "Properties"
    Public Property ContainersID() As IList(Of Integer)
        Get
            If _containers Is Nothing Then
                _containers = New List(Of Integer)
            End If
            Return _containers
        End Get
        Set(ByVal value As IList(Of Integer))
            _containers = value
        End Set
    End Property
#End Region
    Public Sub New()
        TraslitteraDiritti = New TraslitteraDelegate(AddressOf ContainerGroupFacade.TraslitteraDiritti)
    End Sub

    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa dei Contenitori con Sicurezza"
        StampaContenitoriSecurity()
    End Sub

#Region "Creazione Righe"
    Protected Sub CreateLocationSecurityRow(ByRef tbl As DSTable, ByVal location As String)
        MyBase.CreateGroupSecurityRow(tbl, location)
    End Sub

    Protected Sub CreateLocationRow(ByRef tbl As DSTable, ByVal type As String, ByVal id As String, ByVal name As String, ByVal server As String, ByVal archive As String, ByVal lineBox As Boolean, ByVal textBold As Boolean)
        Dim cellStyle As New DSTableCellStyle()
        'stili comuni a tutte le celle
        cellStyle.Font.Bold = textBold
        cellStyle.LineBox = lineBox

        'Crea riga
        tbl.CreateEmptyRow()

        'crea prima cella (spazio)
        tbl.CurrentRow.CreateEmpytCell()
        cellStyle.Width = Unit.Percentage(5)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'cella tipologia
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = type
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'cella id
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = id
        cellStyle.Width = Unit.Percentage(5)
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'cella nome
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = name
        cellStyle.Width = Unit.Percentage(25)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'cella server
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = server
        cellStyle.Width = Unit.Percentage(25)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

        'cella archivio
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = archive
        cellStyle.Width = Unit.Percentage(25)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "Funzioni di Stampa"
    Private Sub StampaContenitoriSecurity()
        Dim lblResl As String = String.Empty
        Dim _containers As IList(Of Container) = New List(Of Container)()

        If DocSuiteContext.Current.IsResolutionEnabled Then
            lblResl = Facade.TabMasterFacade.TreeViewCaption
        End If

        For Each id As Integer In ContainersID
            _containers.Add(Facade.ContainerFacade.GetById(id))
        Next

        If _containers.Count > 0 Then
            For Each container As Container In _containers
                PrintContainer(container, lblResl)
            Next
        End If
    End Sub

    Protected Sub PrintContainer(ByVal container As Container, ByVal lblResl As String)
        Dim location As Location = Nothing
        Dim rights As String = String.Empty

        'aggiungi spazio
        CreateSpaceRow(TablePrint)
        'nome
        CreateSecurityRow(TablePrint, "Contenitore: " & container.Name)
        'aggiungi spazio
        CreateSpaceRow(TablePrint)
        'locazioni
        CreateLocationSecurityRow(TablePrint, "Locazioni")
        'intestazione locazioni
        CreateLocationRow(TablePrint, "Tipo", "Id", "Nome", "Server", "Archivio", True, True)
        'recupera la location attravErso i facade perchè su un altro DB
        If DocSuiteContext.Current.IsDocumentEnabled AndAlso container.DocmLocation IsNot Nothing Then
            location = Facade.LocationFacade.GetById(container.DocmLocation.Id, False, "DocmDB")
            If location IsNot Nothing Then
                CreateLocationRow(TablePrint, DocSuiteContext.Current.DossierAndPraticheLabel, location.Id.ToString(), location.Name, location.DocumentServer, location.DocmBiblosDSDB, False, False)
            End If
        End If
        If DocSuiteContext.Current.IsProtocolEnabled AndAlso container.ProtLocation IsNot Nothing Then
            location = Facade.LocationFacade.GetById(container.ProtLocation.Id, False, "ProtDB")
            If location IsNot Nothing Then
                CreateLocationRow(TablePrint, "Protocollo", location.Id.ToString(), location.Name, location.DocumentServer, location.ProtBiblosDSDB, False, False)

            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled AndAlso container.ReslLocation IsNot Nothing Then
            location = Facade.LocationFacade.GetById(container.ReslLocation.Id, False, "ReslDB")
            If location IsNot Nothing Then
                CreateLocationRow(TablePrint, lblResl, location.Id.ToString(), location.Name, location.DocumentServer, location.ReslBiblosDSDB, False, False)
            End If
        End If
        'aggiungi spazio
        CreateSpaceRow(TablePrint)
        'Gruppi
        For Each group As ContainerGroup In container.ContainerGroups
            CreateGroupsSection(TablePrint, group)
        Next
    End Sub
#End Region
End Class
