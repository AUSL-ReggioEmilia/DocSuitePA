Imports Newtonsoft.Json

Public Class POLAccountExtendedProperties

    <JsonProperty("SercExtra")>
    Public Property SercExtra As PolAccountSercExtendedProperties

    Public Shared Function Serialize(ByRef extendedProperties As POLAccountExtendedProperties) As String
        Return JsonConvert.SerializeObject(extendedProperties)
    End Function

    Public Shared Function Deserialize(ByRef extendedProperties As String) As POLAccountExtendedProperties
        Try
            If (extendedProperties IsNot Nothing) Then
                Return JsonConvert.DeserializeObject(Of POLAccountExtendedProperties)(extendedProperties)
            End If
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class