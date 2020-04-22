Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Public Class ReslParerSearch
    Inherits CommonBasePage

#Region " Events "


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()

        If Not Page.IsPostBack Then
            FillResolutionType()
            DataBindDdlServizio()
            DataBindRadComboBoxStatus()
            FormLoad()
        End If
    End Sub

    Private Sub cmdSearch_Click(sender As Object, e As EventArgs) Handles cmdSearch.Click
        Dim finder As New NHibernateResolutionFinder("ReslDB")

        ''Definisco l'anno
        Dim year As Short = 0
        If Short.TryParse(txtYear.Text, year) Then
            finder.Year = year.ToString()
        End If

        ''Definisco il tipo di resolution
        Dim idReslType As Short = 0
        If Short.TryParse(ddlDocumentType.SelectedValue, idReslType) Then
            finder.ResolutionType = Facade.ResolutionTypeFacade.GetById(idReslType)
        End If

        ''Definisco l'inclusive number di partenza
        Dim numberFrom As Integer = 0
        If Integer.TryParse(txtNumberFrom.Text, numberFrom) OrElse (Not String.IsNullOrEmpty(ddlServizio.SelectedValue)) Then
            finder.InclusiveNumberFrom = getInclusiveNumber(
                year,
                numberFrom,
                finder.ResolutionType,
                ddlServizio.SelectedValue, "0000", "00000000")
        End If

        ''Definisco l'inclusive number di arrivo
        Dim numberTo As Integer = 0
        If Integer.TryParse(txtNumberTo.Text, numberTo) OrElse (Not String.IsNullOrEmpty(ddlServizio.SelectedValue)) Then
            finder.InclusiveNumberTo = getInclusiveNumber(
                year,
                numberTo,
                finder.ResolutionType,
                ddlServizio.SelectedValue, "9999", "99999999")
        End If

        ''Definisco la data di partenza
        finder.RegistrationDateFrom = dateParerFrom.SelectedDate

        ''Definisco la data di arrivo
        finder.RegistrationDateTo = dateParerTo.SelectedDate

        ''Definisco lo status del Parer
        For Each parerStatus As RadComboBoxItem In rcbParerStatus.CheckedItems
            finder.ResolutionParerConservationStatus.Add(CType(parerStatus.Value, Integer))
        Next

        ''Salvo il finder in sessione
        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.ResolutionParerFinderType)
        Response.Redirect("../PARER/ReslParerGrid.aspx?Type=Resl", True)
    End Sub

#End Region

#Region " Methods "

    Private Sub FillResolutionType()

        For Each item As ResolutionType In Facade.ResolutionTypeFacade.GetAll()
            WebUtils.ObjDropDownListAdd(ddlDocumentType, item.Description, item.Id.ToString())
        Next

    End Sub

    Private Sub DataBindDdlServizio()
        ddlServizio.Items.Clear()
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetRoles(DSWEnvironment.Resolution, 1, True, String.Empty, False, Nothing)

        For Each role As Role In From role1 In roles Where Not String.IsNullOrEmpty(role1.ServiceCode)
            ddlServizio.Items.Add(role.ServiceCode)
        Next

        ''Se non sono riuscito a caricare codici di servizio significa che non sono abilitati
        ''Posso pertanto disattivare il controllo
        If ddlServizio.Items.Count = 0 Then
            trServizio.Visible = False
        Else
            If ddlServizio.Items.Count > 1 Then
                ddlServizio.Items.Insert(0, New ListItem("Seleziona Servizio", String.Empty))
            End If
        End If
    End Sub

    Private Sub DataBindRadComboBoxStatus()
        'rcbParerStatus.Items.Add(New RadComboBoxItem(String.Empty))
        For Each conservationStatus As KeyValuePair(Of ResolutionParerFacade.ResolutionParerConservationStatus, String) In ResolutionParerFacade.ConservationsStatus
            '' Converto prima in Int32 per avere il valore numerico che poi vado a caricare come string nella ddl
            rcbParerStatus.Items.Add(New RadComboBoxItem(conservationStatus.Value, Convert.ToInt32(conservationStatus.Key).ToString()))
        Next
    End Sub

    Private Sub FormLoad()
        ''Carico eventuali valori mantenuti in finder
        Dim finder As NHibernateResolutionFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ResolutionParerFinderType), NHibernateResolutionFinder)
        If finder IsNot Nothing Then
            ''Carico la tipologia
            If finder.ResolutionType IsNot Nothing Then ddlDocumentType.Items.FindByValue(finder.ResolutionType.Id.ToString).Selected = True
            ''Carico l'anno
            txtYear.Text = finder.Year
            ''Carico il numero di partenza
            If Not String.IsNullOrEmpty(finder.InclusiveNumberFrom) Then
                ''Separo l'inclusiveNumber considerando che potrebbe essere composto da 1,2 o 3 parti
                Dim inclusiveNumberParts As String() = finder.InclusiveNumberFrom.Split("/"c)
                'Il primo campo è sempre la data quindi lo posso rimuovere
                inclusiveNumberParts = inclusiveNumberParts.Skip(1).ToArray()
                ''Qui ho al massimo 2 valori
                Select Case inclusiveNumberParts.Count
                    Case 2
                        ''Casistica completa (Text è gestito separatamente)
                        ddlServizio.Items.FindByValue(inclusiveNumberParts(0)).Selected = True
                        If Not inclusiveNumberParts(1).Equals("00000000") Then
                            txtNumberFrom.Text = Integer.Parse(inclusiveNumberParts(1)).ToString
                        End If
                    Case 1
                        ''Potrebbe essere alternativamente un codice di servizio oppure un numero
                        Dim number As Integer
                        If Integer.TryParse(inclusiveNumberParts(0), number) Then
                            ''Se riesco a convertirlo è la parte numerica
                            If Not inclusiveNumberParts(0).Equals("00000000") Then
                                txtNumberFrom.Text = number.ToString
                            End If
                        Else
                            ''Altrimenti è un codice di servizio
                            ddlServizio.Items.FindByValue(inclusiveNumberParts(0)).Selected = True
                        End If
                End Select
            End If
            ''Carico il numero di arrivo
            If Not String.IsNullOrEmpty(finder.InclusiveNumberTo) Then
                ''Separo l'inclusiveNumber considerando che potrebbe essere composto da 1,2 o 3 parti
                Dim inclusiveNumberParts As String() = finder.InclusiveNumberTo.Split("/"c)
                'Il primo campo è sempre la data quindi lo posso rimuovere
                inclusiveNumberParts = inclusiveNumberParts.Skip(1).ToArray()
                ''Qui ho al massimo 2 valori
                Select Case inclusiveNumberParts.Count
                    Case 2
                        ''Casistica completa (Text è gestito separatamente)
                        ddlServizio.Items.FindByValue(inclusiveNumberParts(0)).Selected = True
                        If Not inclusiveNumberParts(1).Equals("99999999") Then
                            txtNumberTo.Text = Integer.Parse(inclusiveNumberParts(1)).ToString
                        End If
                    Case 1
                        ''Potrebbe essere alternativamente un codice di servizio oppure un numero
                        Dim number As Integer
                        If Integer.TryParse(inclusiveNumberParts(0), number) Then
                            ''Se riesco a convertirlo è la parte numerica
                            If Not inclusiveNumberParts(0).Equals("99999999") Then
                                txtNumberTo.Text = number.ToString
                            End If
                        Else
                            ''Altrimenti è un codice di servizio
                            ddlServizio.Items.FindByValue(inclusiveNumberParts(0)).Selected = True
                        End If
                End Select
            End If
            ''Carico la data di partenza
            dateParerFrom.SelectedDate = finder.RegistrationDateFrom
            ''Carico la data di arrivo
            dateParerTo.SelectedDate = finder.RegistrationDateTo
            ''Carico la tipologia di status
            For Each resolutionParerConservationStatusItem As Integer In finder.ResolutionParerConservationStatus
                rcbParerStatus.Items.FindItemByValue(resolutionParerConservationStatusItem.ToString).Checked = True
            Next
        End If
    End Sub

    ''' <summary>
    ''' Calcola l'InclusiveNumber da utilizzarsi nei filtri di ricerca.
    ''' </summary>
    ''' <param name="year">Anno</param>
    ''' <param name="number">Numero</param>
    ''' <param name="reslType">Tipologia atto</param>
    ''' <param name="serviceCode">Codice di servizio</param>
    ''' <remarks>
    ''' Questa logica sarebbe da centralizzare all'interno del Facade e slegarla dal ComposeReslNumber...
    ''' Credo inoltre ci siano dei buchi di analisi legati al fatto che ComposeReslNumber prevede di ricevere come parametro la data di adozione che arriva in questo caso costante a Nothing.
    ''' Resta da verificarne inoltre la fruibilità poichè filtrando da numero 1 a 999 viene estratto anche "2012/333/123654" poichè incluso nell'intervallo di stringhe fra "2012/1" e "2012/999".
    ''' Risulta a mio avviso poco chiara chiara la risposta così ottenuta. - FG
    ''' </remarks>
    Private Function getInclusiveNumber(year As Short?, number As Integer?, reslType As ResolutionType, serviceCode As String, yearDefaultNumber As String, numberDefaultNumber As String) As String
        Dim inclusiveNumber As String = number.ToString()
        If Not String.IsNullOrEmpty(serviceCode) Then
            If number > 0 Then
                inclusiveNumber = String.Format("{0}/{1:0000}", serviceCode, number)
            Else
                inclusiveNumber = String.Format("{0}/{1}", serviceCode, numberDefaultNumber)
            End If
        Else
            If number = 0 Then
                number = Integer.Parse(numberDefaultNumber)
                inclusiveNumber = numberDefaultNumber
            End If
        End If

        ''Metodo bruttissimo per calcolare l'inclusiveNumber
        inclusiveNumber = Facade.ResolutionFacade.ComposeReslNumber(year, number, inclusiveNumber, number.GetValueOrDefault(-1), reslType, DateTime.Now, Nothing, False)
        If year = 0 Then inclusiveNumber = inclusiveNumber.Replace("0/", yearDefaultNumber & "/")

        Return inclusiveNumber
    End Function
#End Region

End Class