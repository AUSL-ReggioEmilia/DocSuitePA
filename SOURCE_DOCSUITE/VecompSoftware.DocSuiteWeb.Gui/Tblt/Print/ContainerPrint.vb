Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary>
''' Stampa dei contenitori (Normale)
''' </summary>
''' <remarks></remarks>
Public Class ContainerPrint
    Inherits BasePrint

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
#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa dei Contenitori"
        StampaContenitori()
    End Sub
#End Region

#Region "Creazione Righe"
    Private Sub CreaRigaContenitore(ByRef tbl As DSTable, ByVal text As String, ByVal isActive As Integer)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        Select Case isActive
            Case 0
                tbl.CreateEmptyRow("Prnt-Grigio")
            Case Else
                tbl.CreateEmptyRow("Prnt-Tabella")
        End Select
        'crea cella
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = text
        'crea stile
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.ColumnSpan = 3
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreaRigaDiritti(ByRef tbl As DSTable, ByVal tipologia As String, ByVal gruppo As String, ByVal diritti As String, ByVal textBold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'crea riga
        tbl.CreateEmptyRow()
        'crea cella tipologia
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = tipologia
        cellStyle.Width = Unit.Percentage(20)
        cellStyle.Font.Bold = textBold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = True
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella gruppo
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = gruppo
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'crea cella diritti
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = diritti
        cellStyle.Width = Unit.Percentage(60)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "Funzioni di Stampa"
    Public Sub StampaContenitori()

        Dim containers As IList(Of Container) = New List(Of Container)()

        For Each id As Integer In ContainersID
            containers.Add(Facade.ContainerFacade.GetById(id))
        Next

        For Each container As Container In containers
            'Contenitore
            CreaRigaContenitore(TablePrint, String.Format("{0} ({1})", container.Name, container.Id), container.IsActive)
            'Intestazione Diritti
            CreaRigaDiritti(TablePrint, "Tipologia", "Gruppo", "Diritti", True)

            If DocSuiteContext.Current.IsProtocolEnabled Then
                For Each containerGroup As ContainerGroup In container.ContainerGroups
                    Dim rights As String = ContainerGroupFacade.TraslitteraDiritti(DSWEnvironment.Protocol, containerGroup)
                    If Not String.IsNullOrEmpty(rights) Then
                        CreaRigaDiritti(TablePrint, "Protocollo", containerGroup.Name, rights, False)
                    End If
                Next
            End If
            If DocSuiteContext.Current.IsResolutionEnabled Then
                For Each containerGroup As ContainerGroup In container.ContainerGroups
                    Dim rights As String = ContainerGroupFacade.TraslitteraDiritti(DSWEnvironment.Resolution, containerGroup)
                    If Not String.IsNullOrEmpty(rights) Then
                        CreaRigaDiritti(TablePrint, Facade.TabMasterFacade.TreeViewCaption, containerGroup.Name, rights, False)
                    End If
                Next
            End If
            If DocSuiteContext.Current.IsDocumentEnabled Then
                For Each containerGroup As ContainerGroup In container.ContainerGroups
                    Dim rights As String = ContainerGroupFacade.TraslitteraDiritti(DSWEnvironment.Document, containerGroup)
                    If Not String.IsNullOrEmpty(rights) Then
                        CreaRigaDiritti(TablePrint, "Pratica", containerGroup.Name, rights, False)
                    End If
                Next
            End If
        Next
    End Sub
#End Region

End Class
