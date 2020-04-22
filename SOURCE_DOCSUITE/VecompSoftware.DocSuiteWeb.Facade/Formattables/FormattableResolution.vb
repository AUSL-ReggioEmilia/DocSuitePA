Imports VecompSoftware.DocSuiteWeb.Data

Namespace Formattables
    Public Class FormattableResolution
        Implements IFormattable

        Public Property Resolution As Resolution
        Public Property Factory As FacadeFactory

        Public Sub New(resl As Resolution)
            Resolution = resl
            Factory = New FacadeFactory("Resl")
        End Sub


        Public Overrides Function ToString() As String
            Return ToString("g", Nothing)
        End Function

        Public Overridable Overloads Function ToString(format As String) As String
            Return ToString(format, Nothing)
        End Function

        Public Overridable Overloads Function ToString(formatProvider As IFormatProvider) As String
            Return ToString(Nothing, formatProvider)
        End Function

        Public Overridable Overloads Function ToString(ByVal format As String, ByVal formatProvider As IFormatProvider) As String Implements IFormattable.ToString
            If format Is Nothing Then format = "g"

            If formatProvider IsNot Nothing Then
                Dim formatter As ICustomFormatter = TryCast(formatProvider.GetFormat(Me.[GetType]()), ICustomFormatter)
                If formatter IsNot Nothing Then
                    Return formatter.Format(format, Me, formatProvider)
                End If
            End If
            Dim f As String = format.ToLowerInvariant()

            Select Case f
                Case "adoptiondate"
                    Return GetDateString(Resolution.AdoptionDate, f)
                Case "adoptiondate-short"
                    Return GetDateString(Resolution.AdoptionDate, f)
                Case "adoptiondate-long"
                    Return GetDateString(Resolution.AdoptionDate, f)
                    
                Case "proposedate"
                    Return GetDateString(Resolution.ProposeDate, f)
                Case "proposedate-short"
                    Return GetDateString(Resolution.ProposeDate, f)
                Case "proposedate-long"
                    Return GetDateString(Resolution.ProposeDate, f)

                Case "publishingdate"
                    Return GetDateString(Resolution.PublishingDate, f)
                Case "publishingdate-short"
                    Return GetDateString(Resolution.PublishingDate, f)
                Case "publishingdate-long"
                    Return GetDateString(Resolution.PublishingDate, f)

                Case "effectivenessdate"
                    Return GetDateString(Resolution.EffectivenessDate, f)
                Case "effectivenessdate-short"
                    Return GetDateString(Resolution.EffectivenessDate, f)
                Case "effectivenessdate-long"
                    Return GetDateString(Resolution.EffectivenessDate, f)

                Case "leavedate"
                    Return GetDateString(Resolution.LeaveDate, f)
                Case "leavedate-short"
                    Return GetDateString(Resolution.LeaveDate, f)
                Case "leavedate-long"
                    Return GetDateString(Resolution.LeaveDate, f)
                    
                Case "subject"
                    Return Resolution.ResolutionObject
                Case "subject-privacy"
                    Return Resolution.ResolutionObjectPrivacy
                Case "subject-public"
                    If String.IsNullOrEmpty(Resolution.ResolutionObjectPrivacy) Then
                        Return Resolution.ResolutionObject
                    Else
                        Return Resolution.ResolutionObjectPrivacy
                    End If
                Case "label"
                    Return Factory.ResolutionFacade.GetResolutionLabel(Resolution)
                Case "number"
                    Return Factory.ResolutionFacade.GetResolutionNumber(Resolution)
                Case "categoryid"
                    Dim category As Category = GetCategory(Resolution)
                    If category Is Nothing Then
                        Return String.Empty
                    End If
                    Return category.Id.ToString()
                Case "categoryname"
                    Dim category As Category = GetCategory(Resolution)
                    If category Is Nothing Then
                        Return String.Empty
                    End If
                    Return GetCategory(Resolution).Name
                Case "g"
                    Return Factory.ResolutionFacade.GetResolutionLabel(Resolution)

            End Select

            Return ToString("g")
        End Function

        Private Function GetCategory(resl As Resolution) As Category
            If resl.Category Is Nothing Then
                Return Nothing
            End If

            If resl.SubCategory Is Nothing Then
                Return resl.Category
            Else
                Return resl.SubCategory
            End If
        End Function


        Private Function GetDateString(d As DateTime?, format As String) As String
            If Not d.HasValue Then
                Return String.Empty
            End If

            If format.EndsWith("-long") Then
                Return d.Value.ToLongDateString()
            Else
                Return d.Value.ToShortDateString()
            End If
        End Function


    End Class
End Namespace