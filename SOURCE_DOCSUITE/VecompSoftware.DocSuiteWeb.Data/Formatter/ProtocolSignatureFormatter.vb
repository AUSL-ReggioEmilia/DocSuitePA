Imports System.Linq

Namespace Formatter

    Public Class ProtocolSignatureFormatter
        Implements IFormatProvider, ICustomFormatter

        Public Function Format(myFormat As String, arg As Object, formatProvider As IFormatProvider) As String Implements ICustomFormatter.Format
            Dim formatParameters As String() = myFormat.Split(":"c)

            Dim protocolEnv As ProtocolEnv = TryCast(arg, ProtocolEnv)
            If protocolEnv IsNot Nothing Then
                Select Case formatParameters(0)
                    Case "Short"
                        Return protocolEnv.CorporateAcronym
                    Case "Complete"
                        Return protocolEnv.CorporateName
                    Case "None"
                        Return String.Empty
                End Select
            End If

            Dim protocol As Protocol = TryCast(arg, Protocol)
            If protocol IsNot Nothing Then
                Select Case formatParameters(0)
                    Case "Number"
                        Return protocol.Number.ToString(If(formatParameters.Length > 1, formatParameters(1), String.Empty))
                    Case "Year"
                        Return protocol.Year.ToString()
                    Case "Date"
                        Return protocol.RegistrationDate.ToLocalTime().DateTime.ToString(formatParameters(1))
                    Case "Direction"
                        Select Case formatParameters(1)
                            Case "Id"
                                Return protocol.Type.Id.ToString()
                            Case "Short"
                                Return protocol.Type.ShortDescription
                            Case "Complete"
                                Return protocol.Type.Description
                        End Select
                    Case "Container"
                        Select Case formatParameters(1)
                            Case "Id"
                                Return protocol.Container.Id.ToString()
                            Case "Name"
                                Return protocol.Container.Name
                            Case "Note"
                                Return protocol.Container.Note
                        End Select
                    Case "Roles"
                        If protocol.Roles IsNot Nothing Then
                            Dim builder As IEnumerable(Of String) = protocol.Roles _
                                .Where(Function(f) f.Role IsNot Nothing AndAlso Not String.IsNullOrEmpty(f.Role.ServiceCode)) _
                                .Select(Function(f) f.Role.ServiceCode)
                            If (builder IsNot Nothing AndAlso builder.Any()) Then
                                Return String.Concat("[", String.Join("-", builder), "]")
                            End If
                            Return String.Empty
                        End If
                    Case Else
                        Return String.Empty
                End Select
            End If

            Dim info As ProtocolSignatureInfo = TryCast(arg, ProtocolSignatureInfo)
            If info IsNot Nothing Then
                Select Case formatParameters(0)
                    Case "DocumentType"
                        Select Case formatParameters(1)
                            Case "Short"
                                Select Case info.DocumentType
                                    Case ProtocolDocumentType.Main
                                        Return "P"
                                    Case ProtocolDocumentType.Attachment
                                        Return "A"
                                    Case ProtocolDocumentType.Annexed
                                        Return "X"
                                    Case ProtocolDocumentType.None
                                        Return String.Empty
                                End Select
                            Case "Long"
                                Select Case info.DocumentType
                                    Case ProtocolDocumentType.Main
                                        Return "Protocollo"
                                    Case ProtocolDocumentType.Attachment
                                        Return "Allegato"
                                    Case ProtocolDocumentType.Annexed
                                        Return "Annesso"
                                    Case ProtocolDocumentType.None
                                        Return String.Empty
                                End Select
                        End Select
                    Case "AttachmentsCount"
                        If info.AttachmentsCount.HasValue Then
                            Return info.AttachmentsCount.Value.ToString()
                        Else
                            Return String.Empty
                        End If
                    Case "DocumentNumber"
                        If info.DocumentNumber.HasValue Then
                            Return info.DocumentNumber.Value.ToString()
                        Else
                            Return String.Empty
                        End If
                End Select
            End If


            Return arg.ToString()
        End Function

        Public Function GetFormat(formatType As Type) As Object Implements IFormatProvider.GetFormat
            Return Me
        End Function

    End Class

End Namespace