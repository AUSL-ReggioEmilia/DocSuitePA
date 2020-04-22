Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtPackageGes
    Inherits ProtBasePage

#Region " Fields "

    Dim _package As String = String.Empty
    Dim _origin As String = String.Empty
    Dim _currentPackage As Package

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not DocSuiteContext.Current.ProtocolEnv.IsPackageEnabled Then
            Throw New DocSuiteException("Impossibile aprire pagina", "Funzionalità non abilitata")
        End If

        MasterDocSuite.TitleVisible = False
        _currentPackage = New Package()
        _package = Request.QueryString("PK")
        _origin = Request.QueryString("OR")
        ' Inizializza gli oggetti di pagina
        If Not IsPostBack Then
            Initialize()
        Else
            If Action.Eq("modifica") Then
                ' Package selezionato
                _currentPackage = Facade.PackageFacade.GetById(Convert.ToChar(_origin), Convert.ToInt32(_package))
            End If
        End If

    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim vOrigin As Char = Convert.ToChar(txtOrigin.Text.Trim())
        _currentPackage.Account = txtAccount.Text.Trim()
        _currentPackage.MaxDocuments = Integer.Parse(txtMaxDocuments.Text)
        _currentPackage.State = CType(cmbState.SelectedValue, Char)

        ' Se è un package gia esistente aggiorno i dati
        If String.Compare(Action, "modifica", StringComparison.OrdinalIgnoreCase) = 0 Then
            Dim vPackage As Integer = Convert.ToInt32(txtPackage.Text.Trim())
            If IsNumeric(vPackage) And Not String.IsNullOrEmpty(vOrigin.ToString()) Then
                ' Ottiene la chiave composta del package
                _currentPackage.Id = New PackageCompositeKey(vOrigin, vPackage)
                ' Aggiorna i dati
                Facade.PackageFacade.Update(_currentPackage)
            End If

        ElseIf Action.Eq("nuovo") Then
            ' Se è un nuovo package salvo i dati
            ' Inserisce un nuovo package
            If Not String.IsNullOrEmpty(vOrigin.ToString()) Then
                ' Crea una chiave composta per il nuovo package
                _currentPackage.Id = New PackageCompositeKey(vOrigin, Facade.PackageFacade.GetMaxID(vOrigin))
                _currentPackage.Lot = 1
                ' Salva il package
                Facade.PackageFacade.Save(_currentPackage)
            End If
        End If

        MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow();")
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Inizializza gli oggetti di pagina </summary>
    Private Sub Initialize()
        cmbState.Items.Clear()
        'Modifica di un package
        If Action.Eq("modifica") And Not String.IsNullOrEmpty(_origin) And Not String.IsNullOrEmpty(_package) Then
            ' Imposta il titolo della pagina
            Title = String.Format("Modifica Scatolone: {0} - {1}", txtOrigin.Text, txtPackage.Text.PadLeft(7, "0"c))
            ' Package selezionato
            _currentPackage = Facade.PackageFacade.GetById(Convert.ToChar(_origin), Convert.ToInt32(_package))
            ' Popola i campi di testo
            txtAccount.Text = _currentPackage.Account.ToString()
            txtOrigin.Text = _currentPackage.Origin.ToString()
            txtPackage.Text = _currentPackage.Package.ToString()
            txtMaxDocuments.Text = _currentPackage.MaxDocuments.ToString()
            ' Popola combo box degli stati
            If _currentPackage.State.ToString().ToUpper() = "A" Then
                cmbState.Items.Add(New ListItem("A", "A"))
                cmbState.Items.Add(New ListItem("C", "C"))
                cmbState.Enabled = True
            ElseIf _currentPackage.State.ToString().ToUpper() = "D" Then
                cmbState.Items.Add(New ListItem("D", "D"))
                cmbState.Items.Add(New ListItem("A", "A"))
                cmbState.Enabled = True
            Else
                ' Se lo stato è diverso da A e D disabilito la combo
                cmbState.Enabled = False
            End If
            ' Disabilita i campi di testo
            txtAccount.Enabled = False
            txtOrigin.Enabled = False
            rowPackage.Visible = True
            txtPackage.Enabled = False
            txtMaxDocuments.Enabled = True
        Else
            'Inserimento di un nuovo package
            ' Imposta titolo pagina
            Title = "Inserimento scatolone"
            ' Popola combo boc
            cmbState.Items.Add(New ListItem("A", "A"))
            cmbState.Items.Add(New ListItem("D", "D"))
            ' Attiva campi di testo e combo box
            cmbState.Enabled = True
            txtAccount.Enabled = True
            txtOrigin.Enabled = True
            rowPackage.Visible = False
            txtMaxDocuments.Enabled = True
            txtAccount.Focus()
        End If

    End Sub

#End Region

End Class
