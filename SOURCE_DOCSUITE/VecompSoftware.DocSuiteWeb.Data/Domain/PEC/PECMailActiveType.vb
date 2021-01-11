Public Structure ActiveType
    Public Enum PECMailActiveType As Short
        Delete = 0S
        Active = 1S
        Disabled = 2S
        Processing = 3S
        Processed = 4S
        [Error] = 255S 'tinyint in sql
    End Enum

    Public Shared Function Cast(type As PECMailActiveType) As Short
        Return type
    End Function

    Public Shared Function Message(type As Short) As String
        Return Message(CType(type, PECMailActiveType))
    End Function

    Public Shared Function Message(type As PECMailActiveType) As String
        Select Case type
            Case PECMailActiveType.Delete

                Return "Cancellata"
            Case PECMailActiveType.Active
                Return "Attiva"
            Case PECMailActiveType.Disabled
                Return "Nascosta"
            Case PECMailActiveType.Processing
                Return "In lavorazione"
            Case PECMailActiveType.Processed
                Return "(uso interno)"
            Case Else
                Return "Errore critico: PEC processata parzialmente. Contattare l'assistenza!"

        End Select

    End Function
End Structure