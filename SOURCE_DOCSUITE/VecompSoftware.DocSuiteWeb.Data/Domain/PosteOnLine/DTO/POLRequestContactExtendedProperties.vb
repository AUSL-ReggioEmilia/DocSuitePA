Imports Newtonsoft.Json

Public Class POLRequestContactExtendedProperties
    <JsonProperty("SercExtra")>
    Public Property SercExtra As POLRequestContactSercExtendedProperties

    Public Shared Function Serialize(ByRef extendedProperties As POLRequestContactExtendedProperties) As String
        Return JsonConvert.SerializeObject(extendedProperties)
    End Function

    Public Shared Function Deserialize(ByRef extendedProperties As String) As POLRequestContactExtendedProperties
        Try
            If (extendedProperties IsNot Nothing) Then
                Return JsonConvert.DeserializeObject(Of POLRequestContactExtendedProperties)(extendedProperties)
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
