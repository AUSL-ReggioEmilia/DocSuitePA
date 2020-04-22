<Serializable()>
Public Class CollaborationVersioningCompositeKey

#Region " Properties "

    Public Overridable Property IdCollaboration As Integer
    Public Overridable Property CollaborationIncremental As Short
    Public Overridable Property Incremental As Short

#End Region


    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        ' Ho rivisto la metodologia di cast del parametro obj
        ' perchè creava problemi in fase di serializzazione con Newtonsoft.Json.JsonConvert - FG
        Dim compareTo As CollaborationVersioningCompositeKey = TryCast(obj, CollaborationVersioningCompositeKey)
        If compareTo Is Nothing Then
            Return False
        End If
        Return IdCollaboration = compareTo.IdCollaboration _
            AndAlso CollaborationIncremental = compareTo.CollaborationIncremental _
            AndAlso Incremental = compareTo.Incremental
    End Function
    Public Overrides Function ToString() As String
        ' L'ordinamento della chiave avrebbe dovuto essere questo:
        ' Return String.Format("{0}/{1}/{2}", IdCollaboration, CollaborationIncremental, Incremental)
        ' Tuttavia non sapendo dove tale modifica avrebbe potuto andare ad impattare e non avendo voglia di scoprirlo,
        ' ho mantenuto il comportamento precedente... - FG
        Return String.Format("{0}/{1}/{2}", IdCollaboration, Incremental, CollaborationIncremental)
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return ToString().GetHashCode()
    End Function

End Class