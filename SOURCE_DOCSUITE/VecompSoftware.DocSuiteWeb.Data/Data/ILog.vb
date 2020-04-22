Public Interface ILog
    Property Id() As Integer
    Property LogDate() As DateTime
    Property SystemComputer() As String
    Property SystemUser() As String
    Property Program() As String
    Property LogType() As String
    Property LogDescription() As String
    Property Year() As Short
    Property Number() As Integer
    Property Severity() As Short?
End Interface
