Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.SessionState
Imports System.Text
Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Xml

Public Class FilterHelper

    ''' <summary> Recupera il valore del filtro dato il controllo contenitore </summary>
    ''' <param name="container">Controllo contenitore</param>
    ''' <returns>Valore del filtro</returns>
    Public Shared Function GetFilterValue(ByVal container As Control, ByVal column As String) As Object
        Dim control As Control = DirectCast(container, Table).Rows(0).Cells(0).Controls(0)

        Select Case control.GetType()
            Case GetType(TextBox)
                SetSessionFilterClient(control.Page.Session, column, New FilterClient(column, "", DirectCast(control, TextBox).Text), True)
                Return DirectCast(control, TextBox).Text

            Case GetType(RadDatePicker)
                Dim datePicker As RadDatePicker = DirectCast(control, RadDatePicker)
                If datePicker.SelectedDate.HasValue Then
                    SetSessionFilterClient(control.Page.Session, column, New FilterClient(column, "", datePicker.SelectedDate.Value.ToString("dd/MM/yyyy")), True)
                    Return datePicker.SelectedDate
                End If

            Case GetType(RadComboBox)
                Dim combobox As RadComboBox = DirectCast(control, RadComboBox)
                SetSessionFilterClient(control.Page.Session, column, New FilterClient(column, "", combobox.SelectedIndex), True)
                Return combobox.SelectedValue

            Case GetType(DropDownList)
                Dim ddl As DropDownList = DirectCast(control, DropDownList)
                SetSessionFilterClient(control.Page.Session, ddl.DataTextField, New FilterClient(ddl.DataTextField, "", ddl.SelectedIndex), True)
                Return ddl.SelectedValue

            Case GetType(RadMaskedTextBox)
                Dim mask As RadMaskedTextBox = DirectCast(control, RadMaskedTextBox)
                SetSessionFilterClient(control.Page.Session, mask.ClientID, New FilterClient(mask.ClientID, "", mask.TextWithLiterals), True)
                Return mask.TextWithLiterals

        End Select

        Return Nothing
    End Function

    Public Shared Sub SetFilterValue(ByVal control As Control, ByVal value As String)
        Select Case control.GetType()
            Case GetType(TextBox)
                DirectCast(control, TextBox).Text = value
            Case GetType(RadDatePicker)
                DirectCast(control, RadDatePicker).SelectedDate = value
            Case GetType(DropDownList)
                DirectCast(control, DropDownList).SelectedValue = value
            Case GetType(RadMaskedTextBox)
                DirectCast(control, RadMaskedTextBox).TextWithLiterals = value
            Case GetType(RadComboBox)
                DirectCast(control, RadComboBox).SelectedValue = value
            Case GetType(RadTextBox)
                DirectCast(control, RadTextBox).Text = value
        End Select
    End Sub

    ''' <summary> Recupera il controllo che identifica il filtro nel controllo contenitore </summary>
    ''' <param name="container">Controllo contenitore</param>
    ''' <returns>Controllo filtro</returns>
    Public Shared Function GetFilterControl(Of T As Control)(ByVal container As Control) As T
        Dim table As Table = CType(container, Table)
        If table Is Nothing Then
            Return Nothing
        End If

        Return DirectCast(table.Rows(0).Cells(0).Controls(0), T)
    End Function

    ''' <summary>
    ''' Converte il valore di un filtro in una tipologia di filtro per il Finder
    ''' </summary>
    ''' <param name="filter">Valore del filtro</param>
    ''' <returns>Filtro per il Finder</returns>
    Public Shared Function ConvertFilterMenuValues(ByVal filter As String) As Data.FilterExpression.FilterType
        Select Case filter
            Case "Contains"
                Return Data.FilterExpression.FilterType.Contains
            Case "EqualTo"
                Return Data.FilterExpression.FilterType.EqualTo
            Case "GreaterThan"
                Return Data.FilterExpression.FilterType.GreaterThan
            Case "GreaterThanOrEqualTo"
                Return Data.FilterExpression.FilterType.GreaterThanOrEqualTo
            Case "LessThan"
                Return Data.FilterExpression.FilterType.LessThan
            Case "LessThanOrEqualTo"
                Return Data.FilterExpression.FilterType.LessThanOrEqualTo
            Case Else
                Return Data.FilterExpression.FilterType.NoFilter
        End Select
    End Function

    ''' <summary>
    ''' Converte una tipologia di filtro per il Finder in una funzione di filtro per la Griglia Telerik
    ''' </summary>
    ''' <param name="filterType">Tipologia filtro Finder</param>
    Public Shared Function ConvertFilterExpressionToGridKnownFunction(ByVal filterType As Data.FilterExpression.FilterType) As GridKnownFunction
        Select Case filterType
            Case Data.FilterExpression.FilterType.Contains
                Return GridKnownFunction.Contains
            Case Data.FilterExpression.FilterType.EqualTo
                Return GridKnownFunction.EqualTo
            Case Data.FilterExpression.FilterType.GreaterThan
                Return GridKnownFunction.GreaterThan
            Case Data.FilterExpression.FilterType.GreaterThanOrEqualTo
                Return GridKnownFunction.GreaterThanOrEqualTo
            Case Data.FilterExpression.FilterType.IsNotNull
                Return GridKnownFunction.NotIsNull
            Case Data.FilterExpression.FilterType.IsNull
                Return GridKnownFunction.IsNull
            Case Data.FilterExpression.FilterType.LessThan
                Return GridKnownFunction.LessThan
            Case Data.FilterExpression.FilterType.LessThanOrEqualTo
                Return GridKnownFunction.LessThanOrEqualTo
            Case Data.FilterExpression.FilterType.StartsWith
                Return GridKnownFunction.StartsWith
            Case Else
                Return GridKnownFunction.NoFilter
        End Select
    End Function

    ''' <summary>
    ''' Converte una funzione di filtro per la Griglia Telerik in una tipologia di filtro per il Finder
    ''' </summary>
    ''' <param name="gridFunction">funzione di filtro per la Griglia Telerik</param>
    Public Shared Function ConvertGridKnownFunctionToFilterExpression(ByVal gridFunction As GridKnownFunction) As Data.FilterExpression.FilterType
        Select Case gridFunction
            Case GridKnownFunction.Contains
                Return Data.FilterExpression.FilterType.Contains
            Case GridKnownFunction.EqualTo
                Return Data.FilterExpression.FilterType.EqualTo
            Case GridKnownFunction.GreaterThan
                Return Data.FilterExpression.FilterType.GreaterThan
            Case GridKnownFunction.GreaterThanOrEqualTo
                Return Data.FilterExpression.FilterType.GreaterThanOrEqualTo
            Case GridKnownFunction.NotIsNull
                Return Data.FilterExpression.FilterType.IsNotNull
            Case GridKnownFunction.IsNull
                Return Data.FilterExpression.FilterType.IsNull
            Case GridKnownFunction.LessThan
                Return Data.FilterExpression.FilterType.LessThan
            Case GridKnownFunction.LessThanOrEqualTo
                Return Data.FilterExpression.FilterType.LessThanOrEqualTo
            Case GridKnownFunction.StartsWith
                Return Data.FilterExpression.FilterType.StartsWith
            Case Else
                Return GridKnownFunction.NoFilter
        End Select
    End Function

    ''' <summary>
    ''' Evidenzia la voce del menu che identifica il filtro il cui valore è passato come parametro
    ''' </summary>
    ''' <param name="filter">Valore del filtro</param>
    ''' <param name="menu">Menu in cui si trova il filtro</param>
    Public Shared Sub HighlightFilterMenu(ByVal filter As String, ByRef menu As GridFilterMenu)
        For Each item As RadMenuItem In menu.Items
            If item.Value = filter Then
                item.CssClass = "rmFocused"
            Else
                item.CssClass = ""
            End If
        Next
    End Sub

    ''' <summary> Aggiorna/Aggiunge/Rimuove il filtro passato come parametro all'interno della collezione a seconda del valore del filtro </summary>
    ''' <param name="filterCollection">Collezione di filtri</param>
    ''' <param name="filter">Filtro da aggiornare/aggiungere/rimuove</param>
    Public Shared Sub UpdateFilter(ByRef filterCollection As IDictionary(Of String, IFilterExpression), ByRef filter As IFilterExpression)
        If Not filterCollection.ContainsKey(filter.PropertyName) Then
            filterCollection.Add(filter.PropertyName, filter)
            Exit Sub
        End If

        If filter.FilterExpression = Data.FilterExpression.FilterType.NoFilter OrElse String.IsNullOrEmpty(filter.FilterValue) Then
            filterCollection.Remove(filter.PropertyName)
        Else
            filterCollection(filter.PropertyName) = filter
        End If
    End Sub

    ''' <summary>
    ''' Metodo per la memorizzazione in Sessione dell stato dei filtri griglia risultati
    ''' </summary>
    ''' <param name="session">Sessione Corrente</param>
    ''' <param name="column">Colonna del filtro e chiave in Session</param>
    ''' <param name="clientFilter">Oggetto filtro cliente</param>
    ''' <param name="sessionName">Nome della sessione di default FilterClient</param>
    Public Shared Sub SetSessionFilterClient(ByVal session As HttpSessionState, ByVal column As String, ByVal clientFilter As FilterClient, Optional ByVal bUpdate As Boolean = False, Optional ByVal sessionName As String = "FilterClient")
        If session(sessionName) Is Nothing Then
            session.Add(sessionName, New Dictionary(Of String, FilterClient))
        End If
        'Salvo nella Session i valori del filtro
        Dim dic As Dictionary(Of String, FilterClient) = session(sessionName)
        If Not dic.ContainsKey(column) Then
            dic.Add(column, clientFilter)
            Exit Sub
        End If
        ' se a true aggiorno il valore del controllo presente
        If bUpdate Then
            Dim oldcf As FilterClient = dic(column)
            oldcf.Value = clientFilter.Value
            dic(column) = oldcf
        Else
            dic(column) = clientFilter
        End If
    End Sub

    ''' <summary>
    ''' Metodo per la scrittura delle informazioni nel file xml che gestisce lo stato cliente
    ''' </summary>
    ''' <param name="session">Oggetto Sessione</param>
    ''' <param name="tempPath">Path file temporanei</param>
    ''' <param name="pageValue">Valore pagina</param>
    ''' <param name="sessionName">Nome della sessione</param>
    Public Shared Sub WriteFilterClientState(ByVal session As HttpSessionState, ByVal tempPath As String, ByVal pageValue As Integer, Optional ByVal sessionName As String = "FilterClient")
        If session(sessionName) Is Nothing Then
            session.Add(sessionName, New Dictionary(Of String, FilterClient))
        End If

        Dim gridstate As New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>")
        gridstate.Append("<CLIENTSTATE>")
        gridstate.AppendFormat("<PAGESTATE value=""{0}"" />", pageValue.ToString())

        'Salvo nella Session i valori del filtro
        Dim dic As Dictionary(Of String, FilterClient) = session(sessionName)
        For Each fc As FilterClient In dic.Values
            gridstate.Append(fc.ToString())
        Next
        gridstate.Append("</CLIENTSTATE>")

        Using tmp As New StreamWriter(tempPath & session.SessionID & FileHelper.Xml)
            tmp.Write(gridstate.ToString())
            tmp.Close()
        End Using
    End Sub

    ''' <summary> Metodo per recuperare il valore da file xml di stato </summary>
    ''' <param name="session">Oggetto Session</param>
    ''' <param name="tempPath">Directory file temporanei</param>
    ''' <param name="column">Nome della colonna dove prelevare il valore</param>
    Public Shared Function GetFilterClientValue(ByVal session As HttpSessionState, ByVal tempPath As String, ByVal column As String) As String
        Dim xmlFilterClient As New XmlDocument
        Using textReader As New StreamReader(CommonUtil.GetInstance().AppTempPath & session.SessionID.ToString() & FileHelper.Xml)
            xmlFilterClient.LoadXml(textReader.ReadToEnd())
            textReader.Close()
        End Using
        ' Cerco il valore tra i nodi
        Dim nodes As XmlNodeList = xmlFilterClient.DocumentElement.GetElementsByTagName("FILTERSTATE")
        For Each node As XmlNode In nodes
            If node.Attributes("column").Value = column Then
                Return node.Attributes("value").Value
            End If
        Next
        Return String.Empty
    End Function

    ''' <summary> Metodo per la rimozione dello stato dei filtri client </summary>
    ''' <param name="session">Oggetto Session</param>
    ''' <param name="tempPath">Directory temporanea</param>
    ''' <param name="pageValue">Pagian selezionata</param>
    Public Shared Sub RemoveFilterClientState(ByVal session As HttpSessionState, ByVal tempPath As String, ByVal pageValue As Integer, Optional ByVal sessionName As String = "FilterClient")
        Dim gridstate As StringBuilder = New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>")
        gridstate.Append("<CLIENTSTATE>")
        gridstate.AppendFormat("<PAGESTATE value=""{0}"" />", pageValue.ToString())

        session.Add(sessionName, New Dictionary(Of String, FilterClient))
        Using tmp As New StreamWriter(tempPath & session.SessionID & FileHelper.Xml)
            tmp.Write(gridstate.ToString())
            tmp.Close()
        End Using
    End Sub

    ''' <summary> Crea la tabella che contiene un filtro </summary>
    ''' <param name="controlMain">controllo principale che identifica il filtro</param>
    ''' <param name="controlButton">pulsante per visualizzare il menu del filtro</param>
    Public Shared Function CreateTable(ByVal controlMain As Control, ByVal controlButton As Control) As DSTable
        Dim table As New DSTable(False)
        table.CSSClass = "filterControlTable"
        table.CreateEmptyRow(False)
        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.AddCellControl(controlMain)
        If controlButton IsNot Nothing Then
            table.CurrentRow.CurrentCell.AddCellControl(controlButton)
        End If
        Return table
    End Function

End Class
