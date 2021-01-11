''' <summary> Classe contenitore per le tipologie di pec (lato server PEC). </summary>
Public NotInheritable Class PECMailTypes

    Public Shared ReadOnly Property Invio As String
        Get
            Return "invio"
        End Get
    End Property

    Public Shared ReadOnly Property Accettazione As String
        Get
            Return "accettazione"
        End Get
    End Property

    Public Shared ReadOnly Property NonAccettazione As String
        Get
            Return "non-accettazione"
        End Get
    End Property

    Public Shared ReadOnly Property AvvenutaConsegna As String
        Get
            Return "avvenuta-consegna"
        End Get
    End Property

    Public Shared ReadOnly Property PreavvisoErroreConsegna As String
        Get
            Return "preavviso-errore-consegna"
        End Get
    End Property

    Public Shared ReadOnly Property ErroreConsegna As String
        Get
            Return "errore-consegna"
        End Get
    End Property

    Public Shared ReadOnly Property ReinvioDaErrore As String
        Get
            Return "reinvio-da-errore"
        End Get
    End Property

End Class