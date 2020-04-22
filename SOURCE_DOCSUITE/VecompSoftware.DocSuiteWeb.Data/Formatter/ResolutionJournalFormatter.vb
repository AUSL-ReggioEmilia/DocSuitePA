Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers

Public Class ResolutionJournalFormatter
    Implements IFormatProvider, ICustomFormatter

    Public Function Format(myFormat As String, arg As Object, formatProvider As IFormatProvider) As String Implements ICustomFormatter.Format
        Dim reslJournal As ResolutionJournal = TryCast(arg, ResolutionJournal)
        If reslJournal IsNot Nothing Then
            Select Case myFormat
                Case "Id"
                    Return reslJournal.Id.ToString()
                Case "Date"
                    Return reslJournal.RegistrationDate.DefaultString()
                Case "Description"
                    Return String.Format("{0} {1} {2}", reslJournal.Template.Description, StringHelper.UppercaseFirst(MonthName(reslJournal.Month)), reslJournal.Year)
                Case Else
                    Return String.Empty
            End Select
        End If

        If TypeOf (arg) Is DateTime Then
            Return DirectCast(arg, DateTime).ToString(myFormat)
        End If

        Return arg.ToString()
    End Function

    Public Function GetFormat(formatType As Type) As Object Implements IFormatProvider.GetFormat
        Return Me
    End Function

End Class
