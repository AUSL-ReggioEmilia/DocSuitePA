Public Partial Class uscDocumentDati
    Inherits DocSuite2008BaseControl


#Region "Properties"

    Public Property HeaderText As String
        Get
            Return lblInfo.Text
        End Get
        Set(value As String)
            lblInfo.Text = value
        End Set
    End Property

#Region "Date"
    Public Property DateVisible() As Boolean
        Get
            Return tblRowDate.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowDate.Visible = value
        End Set
    End Property

    Public Property DateLabel() As String
        Get
            Return lblFieldDate.Text
        End Get
        Set(ByVal value As String)
            lblFieldDate.Text = value
        End Set
    End Property

    Public Property DateText() As Nullable(Of Date)
        Get
            Return datePickerDate.SelectedDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            datePickerDate.SelectedDate = value
        End Set
    End Property

    Public Property DateReadOnly() As Boolean
        Get
            Return datePickerDate.DateInput.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            datePickerDate.DateInput.ReadOnly = value
            datePickerDate.DatePopupButton.Visible = (Not value)
        End Set
    End Property
#End Region

#Region "Object"
    Public Property ObjectVisible() As Boolean
        Get
            Return tblRowObject.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowObject.Visible = value
        End Set
    End Property

    Public Property ObjectLabel() As String
        Get
            Return lblFieldObject.Text
        End Get
        Set(ByVal value As String)
            lblFieldObject.Text = value
        End Set
    End Property

    Public Property ObjectText() As String
        Get
            Return txtObject.Text
        End Get
        Set(ByVal value As String)
            txtObject.Text = value
        End Set
    End Property

    Public Property ObjectReadOnly() As Boolean
        Get
            Return txtObject.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            txtObject.ReadOnly = value
        End Set
    End Property
#End Region

#Region "Reason"
    Public Property ReasonVisible() As Boolean
        Get
            Return tblRowReason.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowReason.Visible = value
        End Set
    End Property

    Public Property ReasonLabel() As String
        Get
            Return lblFieldReason.Text
        End Get
        Set(ByVal value As String)
            lblFieldReason.Text = value
        End Set
    End Property

    Public Property ReasonText() As String
        Get
            Return txtReason.Text
        End Get
        Set(ByVal value As String)
            txtReason.Text = value
        End Set
    End Property

    Public Property ReasonReadOnly() As Boolean
        Get
            Return txtReason.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            txtReason.ReadOnly = value
        End Set
    End Property
#End Region

#Region "Note"
    Public Property NoteVisible() As Boolean
        Get
            Return tblRowNote.Visible
        End Get
        Set(ByVal value As Boolean)
            tblRowNote.Visible = value
        End Set
    End Property

    Public Property NoteLabel() As String
        Get
            Return lblFieldNote.Text
        End Get
        Set(ByVal value As String)
            lblFieldNote.Text = value
        End Set
    End Property

    Public Property NoteText() As String
        Get
            Return txtNote.Text
        End Get
        Set(ByVal value As String)
            txtNote.Text = value
        End Set
    End Property

    Public Property NoteReadOnly() As Boolean
        Get
            Return txtNote.ReadOnly
        End Get
        Set(ByVal value As Boolean)
            txtNote.ReadOnly = value
        End Set
    End Property
#End Region

#End Region
    
End Class