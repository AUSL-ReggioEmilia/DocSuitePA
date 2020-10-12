Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging

Public Class DocmInserimento
    Inherits DocmBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        InitializePage()
        InitializeAjax()
        uscResponsabile.ContactRoot = DocumentEnv.ContactRoot
        uscResponsabile.ButtonADContactVisible = True
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub ddlContainer_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlContainer.SelectedIndexChanged
        SelectContainer()
    End Sub

    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        'verifica lunghezza minima oggetto
        If DocSuiteContext.Current.DocumentEnv.ObjectMinLength <> 1 Then
            If Len(uscOggetto.Text) < CInt(DocSuiteContext.Current.DocumentEnv.ObjectMinLength) Then
                AjaxAlert("Impossibile salvare.\n\nIl campo Oggetto non ha raggiunto i caratteri minimi (" & DocSuiteContext.Current.DocumentEnv.ObjectMinLength & " caratteri).")
                Exit Sub
            End If
        End If

        Dim document As New Document()
        'Rilascio numero progressivo
        Dim yearNumberKey As YearNumberCompositeKey = Facade.ParameterFacade.GetDocumentYearNumber()
        If yearNumberKey Is Nothing Then
            AjaxAlert("Il Server non ha assegnato correttamente il numero di Pratica progressivo.{0}Ripetere l\'operazione di Inserimento", Environment.NewLine)
            Exit Sub
        End If
        'Imposta Anno e Numero (chiavi primarie) della pratica
        document.Year = yearNumberKey.Year.Value
        document.Number = yearNumberKey.Number.Value

        'Settore
        If uscSettori.GetRoles().Count > 0 Then
            document.Role = uscSettori.GetRoles(0)
        End If

        'Contenitore
        If Not String.IsNullOrEmpty(ddlContainer.SelectedValue) Then
            document.Container = Facade.ContainerFacade.GetById(Integer.Parse(ddlContainer.SelectedValue), False, "DocmDB")
            document.Location = document.Container.DocmLocation
        End If

        'Classificatore
        'se il classifcatore selezionato è un sottoclassificatore allora viene impostata la 
        'proprietà SubCategory con il classificatore selezionato e la proprietà Category con la root
        If uscClassificatore.HasSelectedCategories Then
            Dim selectedCategory As Category = uscClassificatore.SelectedCategories.First()
            Dim root As Category = selectedCategory.Root

            If root.Equals(selectedCategory) Then
                document.Category = selectedCategory
            Else
                document.Category = root
                document.SubCategory = selectedCategory
            End If
        End If

        'Dati Pratica
        document.StartDate = rdpDataInizio.SelectedDate
        document.ExpiryDate = rdpDataScadenza.SelectedDate
        document.ServiceNumber = ServiceNumber.Text
        document.Name = Name.Text
        document.DocumentObject = uscOggetto.Text
        document.Manager = uscResponsabile.GetContactText()
        document.Note = Note.Text

        'Apertura Pratica
        document.Status = Facade.DocumentTabStatusFacade.GetById("AP")

        'Salvataggio pratica
        Try
            Facade.DocumentFacade.Save(document)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in inserimento pratica.", ex)
            AjaxAlert("Errore in inserimento pratica. Ripetere operazione")
            Exit Sub
        End Try

        'Salvataggio Riferimenti
        If uscContatti.GetContacts(False).Count > 0 Then
            For Each contact As ContactDTO In uscContatti.GetContacts(False)
                Facade.DocumentFacade.AddDocumentContact(document, contact.Contact)
            Next
        End If

        'Inserimento Log
        Facade.DocumentLogFacade.Insert(document.Year, document.Number, "DI", String.Empty)

        'Token
        Dim documentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(document.Year, document.Number)
        With documentToken
            .IncrementalOrigin = 0
            .IsActive = 1
            .Response = String.Empty
            .DocStep = 1
            .SubStep = 0
            .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("PA")
            .RoleDestination = document.Role
            .RoleSource = document.Role
            .OperationDate = DateTime.Now
        End With
        Facade.DocumentTokenFacade.Save(documentToken)

        'Folder Settore
        Facade.DocumentFolderFacade.InsertFoldersRole(document.Year, document.Number, document.Role.FullIncrementalPath, Nothing)

        'Folder Cartella
        Dim containerExtension As IList(Of ContainerExtension) = Facade.ContainerExtensionFacade.GetByContainerAndKey(document.Container.Id, ContainerExtensionType.FL)
        If containerExtension IsNot Nothing Then
            If Not (Facade.DocumentFolderFacade.CreateFromContainerExtension(document, containerExtension)) Then
                AjaxAlert("Errore nella creazione delle cartelle")
            End If
        End If

        '--
        Session("DocmContainer") = document.Container.Id

        'Visualizzazione pratica
        Dim script As String = "Type=Docm&Year=" & document.Year & "&Number=" & document.Number
        Response.Redirect("DocmVisualizza.aspx?" & CommonShared.AppendSecurityCheck(script))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainer, uscClassificatore)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(ddlContainer, uscSettori)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
    End Sub

    Private Sub InitializePage()
        '--Riferimento
        tblInterop.Visible = DocumentEnv.IsInteropEnabled
    End Sub

    Private Sub Initialize()
        uscContatti.ButtonSelectDomainVisible = False
        'Put user code to initialize the page here
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, 1, True)
        If roles.Count = 1 Then
            uscSettori.SourceRoles = roles.ToList()
            uscSettori.DataBind()
        End If

        'impostazione contenitori: se diritti di inserimento allora combo inizializzata...altrimenti errore di inserimento
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Document, DocumentContainerRightPositions.Insert, True)
        If containers.IsNullOrEmpty() Then
            Throw New DocSuiteException("Inserimento pratica", "Utente senza diritti di Inserimento nella pratica.")
        End If

        For Each container As Container In containers
            If ddlContainer.Items.FindByValue(container.Id.ToString()) Is Nothing Then
                ddlContainer.Items.Add(New ListItem(container.Name, container.Id.ToString()))
            End If
        Next
        SelectContainer()
    End Sub

    ''' <summary> In base ai diritti sul contenitore visualizza o meno il controllo per i riferimenti </summary>
    Private Sub SelectContainer()
        Dim containerExtensionAp As IList(Of ContainerExtension)
        Dim containerExtensionAd As IList(Of ContainerExtension)
        Dim containerExtensionSd As IList(Of ContainerExtension)

        Dim ap As String = "12000000000000000000"
        Dim ad As String = "00000000000000000000" ' TODO: questa non viene usata o sotto c'è un errore di copia/incolla?
        If Not String.IsNullOrEmpty(ddlContainer.SelectedValue) Then
            Dim idContainer As Integer = Integer.Parse(ddlContainer.SelectedValue)
            containerExtensionAp = Facade.ContainerExtensionFacade.GetByContainerAndKey(idContainer, ContainerExtensionType.AP)
            If containerExtensionAp.Count > 0 Then
                ap = containerExtensionAp(0).KeyValue
            End If
            containerExtensionAd = Facade.ContainerExtensionFacade.GetByContainerAndKey(idContainer, ContainerExtensionType.AD)
            If containerExtensionAd.Count > 0 Then
                ad = containerExtensionAd(0).KeyValue
            End If
            containerExtensionSd = Facade.ContainerExtensionFacade.GetByContainerAndKey(idContainer, ContainerExtensionType.SD)
            If containerExtensionSd.Count > 0 Then
                Dim idRole As String = containerExtensionSd(0).KeyValue
                If Not String.IsNullOrEmpty(idRole) Then
                    Dim userRoles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Document, 1, True)

                    Dim added As Boolean = False
                    For Each rightRole As Role In userRoles
                        If rightRole.Id = idRole Then
                            uscSettori.SourceRoles.Add(rightRole)
                            added = True
                            Exit For
                        End If
                    Next
                    If added Then
                        uscSettori.DataBind()
                    End If
                End If
            End If
        End If
        Select Case Mid(ap, DocumentEnv.ChkAbilitazioni.Contact, 1)
            Case "0"
                tblInterop.Visible = False
            Case "1"
                tblInterop.Visible = True
                uscContatti.IsRequired = False
            Case "2"
                tblInterop.Visible = True
                uscContatti.IsRequired = True
        End Select
        Select Case Mid(ap, DocumentEnv.ChkAbilitazioni.Category, 1)
            Case "0"
                uscClassificatore.Visible = False
            Case "1"
                uscClassificatore.Visible = True
                uscClassificatore.Required = False
            Case "2"
                uscClassificatore.Visible = True
                uscClassificatore.Required = True
        End Select
    End Sub

#End Region

End Class

