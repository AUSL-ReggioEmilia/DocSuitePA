Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Helpers

Namespace UserControl

    Partial Public Class UscResWorkflow
        Inherits DocSuite2008BaseControl

        Public Delegate Sub OnRefreshEventHandler(ByVal sender As Object, ByVal e As EventArgs)

        ''' <summary> Scatenato quando viene aggiornato la tabella del flusso. </summary>
        Public Event AjaxRefresh As OnRefreshEventHandler

#Region " Fields "

        Private _resolution As Resolution
        Private _currentResolutionRight As ResolutionRights
        Private _btnProposta As Button
        Private _hasProtocol As Boolean
        Private _hasMessages As Boolean

        Private _table As DSTable

#End Region

#Region " Properties "

        Public Property CurrentResolution() As Resolution
            Get
                Return _resolution
            End Get
            Set(ByVal value As Resolution)
                _resolution = value
            End Set
        End Property
        Public ReadOnly Property CurrentResolutionRight As ResolutionRights
            Get
                If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                    _currentResolutionRight = New ResolutionRights(CurrentResolution)
                End If
                Return _currentResolutionRight
            End Get
        End Property

        Public Property ButtonProposta() As Button
            Get
                Return _btnProposta
            End Get
            Set(ByVal value As Button)
                _btnProposta = value
            End Set
        End Property

        Public Property HasProtocol() As Boolean
            Get
                Return _hasProtocol
            End Get
            Set(ByVal value As Boolean)
                _hasProtocol = value
            End Set
        End Property

        Public Property HasMessages() As Boolean
            Get
                Return _hasMessages
            End Get
            Set(value As Boolean)
                _hasMessages = value
            End Set
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()
            Initialize()
        End Sub

        Private Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefresh.Click
            Initialize()
            RaiseEvent AjaxRefresh(Me, New EventArgs())
        End Sub

#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, phWorkflow)
        End Sub

        Private Sub Initialize()
            _table = New DSTable() With {.CSSClass = "tabella"}
            SetWorkflow()

            'Aggiungo la tabella ai controlli del placehoder
            phWorkflow.Controls.Clear()
            phWorkflow.Controls.Add(_table.Table)
        End Sub

        ''' <summary> disegna la tabella. </summary>
        Public Sub SetWorkflow()
            If (_resolution Is Nothing) OrElse (_resolution.ResolutionWorkflows Is Nothing) Then
                Throw New DocSuiteException("Errore nella visualizzazione del flusso di lavoro")
            End If

            'Le vecchie delibere/disposizioni non  hanno flusso quindi non deve sparare
            'eccezioni in caso non ci siano righe in resolutionworkflow
            If _resolution.ResolutionWorkflows.Count > 0 Then
                CreateHeader()
                CreateRows(Facade.ResolutionWorkflowFacade.GetAllByResolution(_resolution.Id))
            End If
        End Sub

        Private Sub CreateHeader()
            'Creo la prima riga
            _table.CreateEmptyRow("tabella")

            'Creo cella Flusso di lavoro
            _table.CurrentRow.CreateEmpytCell()
            CreateHeaderCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = "Flusso di Lavoro"

            'Creo la seconda riga
            _table.CreateEmptyRow("tabella")

            'Creo cella Tipologia
            _table.CurrentRow.CreateEmpytCell()
            CreateTipologiaCellStyle(_table.CurrentRow.CurrentCell, True)
            _table.CurrentRow.CurrentCell.Text = "Tipologia"

            'Creo cella Data
            _table.CurrentRow.CreateEmpytCell()
            CreateDataUtenteCellStyle(_table.CurrentRow.CurrentCell, True)
            _table.CurrentRow.CurrentCell.Text = "Data"

            'Creo cella Utente
            _table.CurrentRow.CreateEmpytCell()
            CreateDataUtenteCellStyle(_table.CurrentRow.CurrentCell, True)
            _table.CurrentRow.CurrentCell.Text = "Utente"

            'Creo cella Documenti
            _table.CurrentRow.CreateEmpytCell()
            CreateDocumentiCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = "Documenti"

            'Creo cella Gestione Flusso
            _table.CurrentRow.CreateEmpytCell()
            CreateFlussoCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = "Gestione flusso"

            'Creo cella Protocollo
            If _hasProtocol Then
                _table.CurrentRow.CreateEmpytCell()
                CreateProtocolloCellStyle(_table.CurrentRow.CurrentCell)
                _table.CurrentRow.CurrentCell.Text = "Protocollo"
            End If

            'Creo cella Messaggi
            If HasMessages() Then
                _table.CurrentRow.CreateEmpytCell()
                CreateMessaggiCellStyle(_table.CurrentRow.CurrentCell)
                _table.CurrentRow.CurrentCell.Text = "Messaggi"
            End If
        End Sub

        Private Sub CreateRows(ByVal resolutionWorkflows As IList(Of ResolutionWorkflow))
            'Pagina standard
            Dim cmdW As String = "§../Resl/ReslFlusso.aspx?"
            Dim tabWorkFlow As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(resolutionWorkflows(0).Resolution.Id)
            ''Pagina per la sola funzionalità Next
            Dim cmdW_Next As String = tabWorkFlow.PubblicaRevocaPage
            'Se è definita la pagina viene ritornata, altrimenti viene restituita la pagina standard
            If (cmdW_Next = Nothing) Then
                cmdW_Next = "§../Resl/ReslFlusso.aspx?"
            Else
                cmdW_Next = String.Format("§../Resl/{0}?", cmdW_Next)
            End If

            Dim cmdWPar As String = String.Format("Titolo=Gestione&Type=Resl&AddButton=btnRefresh&idResolution={0}&ReslType={1}&Action=", _resolution.Id, _resolution.Type.Id)
            Dim frLst As IList(Of FileResolution) = Facade.FileResolutionFacade.GetByResolution(_resolution)
            If frLst.Count <= 0 Then
                Exit Sub
            End If

            Dim fileRes As FileResolution = frLst(0)

            For Each workflow As ResolutionWorkflow In resolutionWorkflows

                Dim rights As New ResolutionRights(CurrentResolution)
                Dim tab As TabWorkflow = Facade.TabWorkflowFacade.GetTabWorkFlow(CurrentResolution, workflow)

                Dim values As New RowDataValues
                values.Description = tab.CustomDescription

                If Not tab.Description.Eq(WorkflowStep.DOCUMENTO_ADOZIONE) Then
                    If Not String.IsNullOrEmpty(tab.FieldDate) Then
                        values.Data = String.Format("{0:dd/MM/yyyy}", ReflectionHelper.GetPropertyCase(_resolution, tab.FieldDate))
                    End If

                    If Not String.IsNullOrEmpty(tab.FieldUser) Then
                        values.User = CommonAD.GetDisplayName(ReflectionHelper.GetPropertyCase(_resolution, tab.FieldUser).ToString())
                    End If
                Else

                    If workflow.LastChangedDate.HasValue Then
                        values.Data = String.Format("{0:dd/MM/yyyy}", workflow.LastChangedDate.Value)
                        values.User = CommonAD.GetDisplayName(workflow.LastChangedUser)
                    Else
                        values.Data = String.Format("{0:dd/MM/yyyy}", workflow.RegistrationDate)
                        values.User = CommonAD.GetDisplayName(workflow.RegistrationUser)
                    End If
                End If

                'Visualizzo se sempre visualizzabile o se è lo step attivo
                Dim showStep As Boolean = (tab.ViewOnlyActive.Eq("0") OrElse (tab.ViewOnlyActive.Eq("1") AndAlso workflow.IsActive = 1))

                'Dim showProposta As Boolean = CurrentResolutionRight.IsProposalViewable()

                If showStep OrElse tab.ViewCurrentDocument Then
                    ' Documento
                    Dim documentId As Integer? = ReflectionHelper.GetPropertyCase(fileRes, tab.FieldDocument)
                    If documentId.HasValue AndAlso documentId.Value <> 0 Then
                        ' verifico se devo vis. i doc. precedenti
                        If tab.ViewPreStep.Eq("1") AndAlso workflow.IncrementalFather.HasValue Then
                            ' Seleziono il workflow precedente
                            Dim prevTab As TabWorkflow = Facade.TabWorkflowFacade.GetTabWorkFlow(CurrentResolution, workflow.Parent)

                            If ReflectionHelper.GetPropertyCase(fileRes, prevTab.FieldDocument) IsNot Nothing Then
                                values.Doc2 = String.Format("../Resl/Images/{0}§{1}", prevTab.DocumentImageFile, prevTab.DocumentDescription)
                                If rights.IsDocumentViewable(tab) Then
                                    values.Doc2 &= String.Format("§document.location = '{0}/viewers/ResolutionViewer.aspx?{1}'",
                                                              DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                                             CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&incremental={1}&documents=false&previous=conditional", CurrentResolution.Id, workflow.Id.Incremental)))
                                End If
                            End If
                        End If

                        If Not String.IsNullOrEmpty(tab.DocumentImageFile) And Not String.IsNullOrEmpty(tab.DocumentDescription) Then
                            values.Doc1 = String.Format("../Resl/Images/{0}§{1}", tab.DocumentImageFile, tab.DocumentDescription)
                            If rights.IsDocumentViewable(tab) Then
                                values.Doc1 &= String.Format("§document.location = '{0}/viewers/ResolutionViewer.aspx?{1}'",
                                                              DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                                             CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&incremental={1}&documents=true&previous=none&documentsomissisfromstep={2}", CurrentResolution.Id, workflow.Id.Incremental, tab.ViewCurrentDocument)))
                            End If
                        Else
                            If values.Doc2 = "" Then       'Non ho il doc. precedente
                                values.Doc1 = "../Resl/Images/FileDocumento.gif§Documento"
                            Else
                                values.Doc1 &= "../Resl/Images/FileFrontespizio.gif§Frontespizio"
                            End If
                        End If
                    End If
                End If

                ' Allegati principali (FileResolution)
                If showStep AndAlso tab.Attachment = "1" Then
                    Dim attachmentId As Integer? = CType(ReflectionHelper.GetPropertyCase(fileRes, tab.FieldAttachment), Integer?)
                    If attachmentId.HasValue AndAlso attachmentId.Value <> 0 Then
                        values.Doc3 = "../Comm/Images/File/Allegati16.gif§Allegati"
                        If rights.IsAttachmentViewable(tab) Then
                            values.Doc3 &= String.Format("§document.location = '{0}/viewers/ResolutionViewer.aspx?{1}'",
                                                          DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                                         CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&incremental={1}&documents=false&attachments=true&annexes=true&previous=none", CurrentResolution.Id, workflow.Id.Incremental)))
                        End If
                    End If
                End If

                'Visualizzo l'allegato dello step se è differente da quello corrente
                If Not showStep AndAlso tab.ViewCurrentAttachment Then
                    Dim attachmentId As Integer? = CType(ReflectionHelper.GetPropertyCase(fileRes, tab.FieldAttachment), Integer?)
                    'Se l'allegato che voglio vedere ha valore e quello ufficiale no (oppure hanno valori diversi)
                    If workflow.Attachment.HasValue AndAlso (Not attachmentId.HasValue OrElse Not workflow.Attachment.Equals(attachmentId)) Then
                        values.Doc3 = "../Comm/Images/File/Allegati16.gif§Allegati Proposta"
                        If rights.IsAttachmentViewable(tab) Then
                            values.Doc3 &= String.Format("§document.location = '{0}/viewers/ResolutionViewer.aspx?{1}'",
                                                              DocSuiteContext.Current.CurrentTenant.DSWUrl,
                                                             CommonShared.AppendSecurityCheck(String.Format("IdResolution={0}&incremental={1}&attachmentsfromstep=true&previous=none&attachmentsomissisfromstep=true", CurrentResolution.Id, workflow.Id.Incremental)))
                        End If
                    End If
                End If

                'Se la resolution è stata annullata non devo poter fare altro
                If CurrentResolution.Status.Id = ResolutionStatusId.Attivo AndAlso (DocSuiteContext.IsFullApplication AndAlso workflow.IsActive = 1) Then
                    Dim sOp As String = tab.OperationStep
                    If Facade.ResolutionFacade.TestOperationStepProperty(sOp, "M", _resolution) Then
                        Dim params As String = String.Format("{0}Modify&Step={1}", cmdWPar, tab.Id.ResStep)
                        values.Flusso1 = String.Format("../Resl/Images/StepModify.gif§Modifica Passo{0}{1}", cmdW, CommonShared.AppendSecurityCheck(params))
                    End If
                    If Facade.ResolutionFacade.TestOperationStepProperty(sOp, "D", _resolution) Then
                        Dim params As String = String.Format("{0}Delete&Step={1}", cmdWPar, tab.Id.ResStep)
                        values.Flusso2 = String.Format("../Resl/Images/StepDelete.gif§Elimina Passo{0}{1}", cmdW, CommonShared.AppendSecurityCheck(params))
                    End If
                    If Facade.ResolutionFacade.TestOperationStepProperty(sOp, "N", _resolution) AndAlso Not (DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled AndAlso tab.CustomDescription.Equals("Pubblicazione")) Then
                        Dim params As String = String.Format("{0}Next&Step={1}", cmdWPar, tab.Id.ResStep)
                        values.Flusso3 = String.Format("../Resl/Images/StepNext.gif§Prossimo Passo{0}{1}", cmdW_Next, CommonShared.AppendSecurityCheck(params))
                    End If
                End If

                ' Protocollo
                values.ProtDesc = String.Empty
                values.ProtIco = String.Empty
                If Not String.IsNullOrEmpty(tab.FieldProtocol) Then
                    Dim fp As String = ReflectionHelper.GetPropertyCase(_resolution, tab.FieldProtocol)
                    If Not String.IsNullOrEmpty(fp) Then
                        Dim s() As String = fp.Split("|"c)
                        Dim params As String = String.Format("Year={0}&Number={1}", s(0), s(1))
                        values.ProtDesc = ProtocolFacade.ProtocolFullNumber(Short.Parse(s(0)), Integer.Parse(s(1)))
                        values.ProtIco = String.Format("../Comm/Images/DocSuite/Protocollo16.gif§Protocollo§document.location='../Prot/ProtVisualizza.aspx?{0}'", CommonShared.AppendSecurityCheck(params))
                    End If
                End If

                ' Messaggi
                values.Messaggi = String.Empty
                Dim nextStep As TabWorkflow = Nothing
                If (Facade.TabWorkflowFacade.GetByStep(_resolution.WorkflowType, workflow.ResStep.Value + 1S, nextStep)) AndAlso
                    Facade.ResolutionFacade.TestOperationStepProperty(nextStep.OperationStep, "D", _resolution) AndAlso (_resolution.DeclineNote IsNot Nothing) Then

                    Dim declineNote As String() = _resolution.DeclineNote.Split("§"c)
                    If declineNote.Length > 1 AndAlso (Short.Parse(declineNote(1)) - 1 = workflow.ResStep.Value) Then
                        values.Messaggi = _resolution.DeclineNote
                    End If
                End If

                CreateValueRow(values)
            Next

            ' Se devo, aggiungo gli step che mancano
            Dim viewAllStep As Boolean = Facade.TabMasterFacade.GetByConfigurationAndType(DocSuiteContext.Current.ResolutionEnv.Configuration, _resolution.Type.Id).ViewAllStep = "1"
            If viewAllStep Then
                Dim workflows As IList(Of TabWorkflow) = Nothing
                If Facade.TabWorkflowFacade.GetAllNextStep(_resolution.WorkflowType, Facade.ResolutionFacade.GetActiveStep(CurrentResolution).ResStep.Value, workflows) Then
                    For Each work As TabWorkflow In workflows
                        Dim emptyValues As New RowDataValues With {.Description = work.CustomDescription}
                        CreateValueRow(emptyValues)
                    Next
                End If
            End If
        End Sub

        Private Sub CreateValueRow(values As RowDataValues)
            CreateValueRow(values.Description, values.Data, values.User, values.Doc1, values.Doc2, values.Doc3,
                           values.Flusso1, values.Flusso2, values.Flusso3, values.ProtDesc, values.ProtIco, values.Messaggi)
        End Sub

        Private Sub CreateValueRow(ByVal description As String, ByVal data As String, ByVal user As String,
                                   ByVal doc1 As String, ByVal doc2 As String, ByVal doc3 As String,
                                   ByVal flusso1 As String, ByVal flusso2 As String, ByVal flusso3 As String,
                                   ByVal protDesc As String, ByVal protIco As String, ByVal messaggi As String)
            'Creo la riga
            _table.CreateEmptyRow()

            'Creo cella Tipologia
            _table.CurrentRow.CreateEmpytCell()
            CreateTipologiaCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = description

            'Creo cella Data
            _table.CurrentRow.CreateEmpytCell()
            CreateDataUtenteCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = data

            'Creo cella Utente
            _table.CurrentRow.CreateEmpytCell()
            CreateDataUtenteCellStyle(_table.CurrentRow.CurrentCell)
            _table.CurrentRow.CurrentCell.Text = user

            'Creo cella Doc1
            _table.CurrentRow.CreateEmpytCell()
            CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(doc1) Then
                _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(doc1))
            End If

            'Creo cella Doc2
            _table.CurrentRow.CreateEmpytCell()
            CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(doc2) Then
                _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(doc2))
            End If

            'Creo cella Doc3
            _table.CurrentRow.CreateEmpytCell()
            CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(doc3) Then
                _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(doc3))
            End If

            'Creo cella Flusso1
            _table.CurrentRow.CreateEmpytCell()
            CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(flusso1) Then
                _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(flusso1))
            End If

            'Creo cella Flusso2
            _table.CurrentRow.CreateEmpytCell()
            CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(flusso2) Then
                _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(flusso2))
            End If

            'Creo cella Flusso3
            _table.CurrentRow.CreateEmpytCell()
            CreateGF2CellStyle(_table.CurrentRow.CurrentCell)
            If Not String.IsNullOrEmpty(flusso3) Then
                Dim nextStepImageButton As ImageButton = GetImageButton(flusso3)
                nextStepImageButton.Attributes.Add("id", "forwardButton")
                _table.CurrentRow.CurrentCell.AddCellControl(nextStepImageButton)
            End If

            'Creo celle del protocollo
            If _hasProtocol Then
                'Descrizione
                _table.CurrentRow.CreateEmpytCell()
                CreateProtocolloDescCellStyle(_table.CurrentRow.CurrentCell)
                _table.CurrentRow.CurrentCell.Text = protDesc

                'Icona
                _table.CurrentRow.CreateEmpytCell()
                CreateIcoCellStyle(_table.CurrentRow.CurrentCell)
                If Not String.IsNullOrEmpty(protIco) Then
                    _table.CurrentRow.CurrentCell.AddCellControl(GetImageButton(protIco))
                End If
            End If

            'Creo messaggio in cella Messaggio
            If HasMessages() AndAlso Not String.IsNullOrEmpty(messaggi) Then
                _table.CurrentRow.CreateEmpytCell()
                CreateMessaggiDescCellStyle(_table.CurrentRow.CurrentCell)

                Dim arrayMessaggi As String() = messaggi.Split("§"c)
                _table.CurrentRow.CurrentCell.Text = String.Format("{0} : Annullato ""{1}"".", arrayMessaggi(3), arrayMessaggi(2))
            End If
        End Sub

        Private Function GetImageButton(ByVal argument As String) As ImageButton
            Dim splitted As String() = argument.Split("§"c)
            If Not splitted.Length > 0 Then
                ' Ha qualche tipo di logica questo comportamento? Mah... - FG
                ' Decisamente no - GC
                Return New ImageButton()
            End If

            Dim img As New ImageButton() With {.ImageUrl = splitted(0), .ToolTip = splitted(1)}

            If splitted.Length > 2 Then
                If splitted(2).Contains("document.location") Then
                    img.Attributes.Add("onclick", splitted(2) & "; return false;")
                Else
                    img.Attributes.Add("onclick", String.Format("return {0}_OpenWindow('windowWorkflow', '{1}');", ID, splitted(2)))
                End If
            End If
            Return img
        End Function

        ''' <summary> Header.  </summary>
        Private Sub CreateHeaderCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(100)
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            cellStyle.LineBox = True
            cellStyle.ColumnSpan = If(_hasProtocol Or _hasMessages, 11, 9)
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Tipologia. </summary>
        Private Sub CreateTipologiaCellStyle(ByRef cell As DSTableCell, Optional ByVal bold As Boolean = False)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(20)
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Right
            cellStyle.LineBox = bold
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Data/Utente. </summary>
        Private Sub CreateDataUtenteCellStyle(ByRef cell As DSTableCell, Optional ByVal bold As Boolean = False)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(17)
            cellStyle.Font.Bold = bold
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cellStyle.LineBox = bold
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Documenti. </summary>
        Private Sub CreateDocumentiCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(12)
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cellStyle.LineBox = True
            cellStyle.ColumnSpan = 3
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Gestione Flusso. </summary>
        Private Sub CreateFlussoCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = If(_hasProtocol Or _hasMessages, Unit.Percentage(12), Unit.Percentage(34))
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cellStyle.LineBox = True
            cellStyle.ColumnSpan = 3
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Protocollo. </summary>
        Private Sub CreateProtocolloCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(22)
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cellStyle.LineBox = True
            cellStyle.ColumnSpan = 2
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Messaggi. </summary>
        Private Sub CreateMessaggiCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(22)
            cellStyle.Font.Bold = True
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cellStyle.LineBox = True
            cellStyle.ColumnSpan = 2
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Documenti/Flusso Icone. </summary>
        Private Sub CreateIcoCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(4)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Gestione Flusso 2. </summary>
        Private Sub CreateGF2CellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = If(_hasProtocol Or _hasMessages, Unit.Percentage(4), Unit.Percentage(26))
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Protocollo Descrizione. </summary>
        Private Sub CreateProtocolloDescCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(18)
            cellStyle.HorizontalAlignment = HorizontalAlign.Right
            cell.ApplyStyle(cellStyle)
        End Sub

        ''' <summary> Messaggi Descrizione. </summary>
        Private Sub CreateMessaggiDescCellStyle(ByRef cell As DSTableCell)
            Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
            cellStyle.Width = Unit.Percentage(18)
            cellStyle.HorizontalAlignment = HorizontalAlign.Center
            cell.ApplyStyle(cellStyle)
        End Sub

#End Region

    End Class
End Namespace