Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary>
''' Gestisce la funzionalità classica/smart del menù a tendina di selezione contenitore.
''' </summary>
Public Class ContainerControl

#Region " Fields "

    Private _classicControl As DropDownList

    Private _smartControl As RadComboBox

    Private _selectedContainer As Container

#End Region

#Region " Constructor "
    Public Sub New(p_classic As DropDownList, p_smart As RadComboBox)
        _classicControl = p_classic
        _smartControl = p_smart
        UseClassic = Not DocSuiteContext.Current.ProtocolEnv.AutocompleteContainer
    End Sub
#End Region

#Region " Properties "

    Public ReadOnly Property ClassicControl As DropDownList
        Get
            If _classicControl Is Nothing Then
                Throw New Exception("ContainerControl.ClassicControl non inizializzato.")
            End If
            Return _classicControl
        End Get
    End Property

    Public ReadOnly Property SmartControl As RadComboBox
        Get
            If _smartControl Is Nothing Then
                Throw New Exception("ContainerControl.SmartControl non inizializzato.")
            End If
            Return _smartControl
        End Get
    End Property

    Public Property UseClassic As Boolean
        Get
            Return ClassicControl.Visible
        End Get
        Set(value As Boolean)
            ClassicControl.Visible = value
            SmartControl.Visible = Not ClassicControl.Visible
        End Set
    End Property

    ''' <summary>
    ''' Torna il controllo attualmente utilizzato
    ''' </summary>
    Public ReadOnly Property ActiveControl As Control
        Get
            If UseClassic Then
                Return ClassicControl
            Else
                Return SmartControl
            End If
        End Get
    End Property

    Public Property Visible As Boolean
        Get
            Return ActiveControl.Visible
        End Get
        Set(value As Boolean)
            ActiveControl.Visible = value
        End Set
    End Property

    Public Property Enabled As Boolean
        Get
            Dim retval As Boolean = False
            If UseClassic Then
                retval = ClassicControl.Enabled
            Else
                retval = SmartControl.Enabled
            End If
            Return retval
        End Get
        Set(value As Boolean)
            If UseClassic Then
                ClassicControl.Enabled = value
            Else
                SmartControl.Enabled = value
            End If
        End Set
    End Property

    Public Property AutoPostBack As Boolean
        Get
            Dim retval As Boolean = False
            If UseClassic Then
                retval = ClassicControl.AutoPostBack
            Else
                retval = SmartControl.AutoPostBack
            End If
            Return retval
        End Get
        Set(value As Boolean)
            If UseClassic Then
                ClassicControl.AutoPostBack = value
            Else
                SmartControl.AutoPostBack = value
            End If
        End Set
    End Property

    Public Property SelectedText As String
        Get
            Dim retval As String = String.Empty
            If UseClassic Then
                retval = IIf(ClassicControl.SelectedItem IsNot Nothing, ClassicControl.SelectedItem.Text, String.Empty)
            Else
                retval = IIf(SmartControl.SelectedItem IsNot Nothing, SmartControl.SelectedItem.Text, String.Empty)
            End If
            Return retval
        End Get
        Set(value As String)
            If UseClassic Then
                If ClassicControl.SelectedItem IsNot Nothing Then
                    ClassicControl.SelectedItem.Text = value
                End If
            Else
                If SmartControl.SelectedItem IsNot Nothing Then
                    SmartControl.SelectedItem.Text = value
                End If

            End If
        End Set
    End Property

    Public Property SelectedValue As String
        Get
            Dim retval As String = String.Empty
            If UseClassic Then
                retval = ClassicControl.SelectedValue
            Else
                retval = SmartControl.SelectedValue
            End If
            Return retval
        End Get
        Set(value As String)
            _selectedContainer = Nothing
            If UseClassic Then
                Dim result As ListItem = ClassicControl.Items.FindByValue(value)
                If result IsNot Nothing Then
                    ClassicControl.SelectedValue = value
                End If
            Else
                Dim result As RadComboBoxItem = SmartControl.Items.FindItemByValue(value)
                If result IsNot Nothing Then
                    SmartControl.SelectedValue = value
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property SelectedContainer(p_dbName As String) As Container
        Get
            If _selectedContainer Is Nothing Then
                If Not String.IsNullOrEmpty(SelectedValue) Then
                    Dim idContainer As Integer
                    If Integer.TryParse(SelectedValue, idContainer) Then
                        _selectedContainer = New ContainerFacade(p_dbName).GetById(idContainer, False, p_dbName)
                    End If
                End If
            End If
            Return _selectedContainer
        End Get
    End Property

#End Region

#Region " Methods "
    ''' <summary>
    ''' Associa ad un <see>RadAjaxManager</see> una lista di controlli
    ''' </summary>
    ''' <param name="manager"><see>RadAjaxManager</see></param>
    ''' <param name="controls"><see>Control</see></param>
    Public Sub AddAjaxSettings(manager As RadAjaxManager, controls As IList(Of Control))
        For Each c As Object In controls
            AddAjaxSettings(manager, c)
        Next
    End Sub

    ''' <summary>
    ''' Associa ad un <see>RadAjaxManager</see> un controllo
    ''' </summary>
    ''' <param name="manager"><see>RadAjaxManager</see></param>
    ''' <param name="control"><see>Control</see></param>
    Public Sub AddAjaxSettings(manager As RadAjaxManager, control As Control)
        If UseClassic Then
            manager.AjaxSettings.AddAjaxSetting(ClassicControl, control)
        Else
            manager.AjaxSettings.AddAjaxSetting(SmartControl, control)
        End If
    End Sub

    Public Sub ClearItems()
        If UseClassic Then
            ClassicControl.Items.Clear()
        Else
            SmartControl.Items.Clear()
        End If
    End Sub

    Public Sub AddItem(p_text As String, p_value As String)
        If UseClassic Then
            If ClassicControl.Items.FindByValue(p_value) Is Nothing Then
                ClassicControl.Items.Add(New ListItem(p_text, p_value))
            End If
        Else
            If SmartControl.Items.FindItemByValue(p_value) Is Nothing Then
                SmartControl.Items.Add(New RadComboBoxItem(p_text, p_value))
            End If
        End If
    End Sub

    Public Function HasItemWithValue(p_value As String) As Boolean
        Dim retval As Boolean = False
        If UseClassic Then
            retval = Not ClassicControl.Items.FindByValue(p_value) Is Nothing
        Else
            retval = Not SmartControl.Items.FindItemByValue(p_value) Is Nothing
        End If
        Return retval
    End Function

#End Region

End Class