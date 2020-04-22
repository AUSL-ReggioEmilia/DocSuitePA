Imports System
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Classe per la gestione della ParameterEnv. </summary>
Public Class ParameterEnvFacade
    Inherits CommonFacade(Of ParameterEnv, String, NHibernateParameterEnvDao)

    ''' <summary> Ritorna il valore di un parametro. </summary>
    ''' <remarks>  se non è presente nel DB, la <see cref="String"/> se presente. </remarks>
    Public Function GetValue(environment As EnvironmentDataCode, key As String, ByVal useInstanceValue As Boolean) As String
        Dim trueKey As String = If(useInstanceValue, DocSuiteContext.CustomInstanceName & key, key)

        Dim list As IList(Of String) = _dao.SelectByKey(environment, trueKey)
        If Not list.IsNullOrEmpty() Then
            Return list(0)
        End If

        Return Nothing
    End Function

    ''' <summary> Aggiorna il valore di un parametro. </summary>
    Public Sub SetValue(environment As EnvironmentDataCode, key As String, value As String, ByVal useInstanceValue As Boolean)
        ' Aggiungo il parametro
        If GetValue(environment, key, useInstanceValue) Is Nothing Then
            AddValue(environment, key, value, useInstanceValue)
            Exit Sub
        End If
        ' Aggiorno il parametro
        Dim trueKey As String = If(useInstanceValue, DocSuiteContext.CustomInstanceName & key, key)
        _dao.SetByKey(environment, trueKey, value)
    End Sub

    ''' <summary> Cancella il valore di un parametro. </summary>
    Public Sub DeleteValue(environment As EnvironmentDataCode, key As String, ByVal useInstanceValue As Boolean)
        ' ne controllo la presenza
        If GetValue(environment, key, useInstanceValue) Is Nothing Then
            Exit Sub
        End If
        ' Cancello il parametro
        Dim trueKey As String = If(useInstanceValue, DocSuiteContext.CustomInstanceName & key, key)
        _dao.DeleteByKey(environment, trueKey)
    End Sub

    ''' <summary> Aggiunge un parametro alle configurazioni. </summary>
    Public Sub AddValue(environment As EnvironmentDataCode, key As String, value As String, ByVal useInstanceValue As Boolean)
        ' Se esiste già lo aggiorno
        If GetValue(environment, key, useInstanceValue) IsNot Nothing Then
            SetValue(environment, key, value, useInstanceValue)
            Exit Sub
        End If
        ' Aggiungo il parametro
        Dim trueKey As String = If(useInstanceValue, DocSuiteContext.CustomInstanceName & key, key)
        _dao.AddByKey(environment, trueKey, value)
    End Sub

    ''' <summary> Ritorna tutti i parametri del database non mappati negli xml. </summary>
    ''' <remarks> Non supporta il check sui valori d'istanza, non c'era tempo. </remarks>
    Public Function GetWrongParameters() As List(Of String)
        Dim missingParameters As New List(Of String)

        For Each code As EnvironmentDataCode In System.Enum.GetValues(GetType(EnvironmentDataCode))

            Dim env As XmlEnvironment = BaseEnvironment.GetXmlEnvironment(code)

            Dim parametersenv As IList(Of ParameterEnv) = GetContext(code).ParameterEnvList

            If parametersenv Is Nothing Then
                Continue For
            End If

            For Each dummyParam As ParameterEnv In parametersenv
                Dim notFound As Boolean = True
                For Each param As XmlParameter In env.Parameters
                    If dummyParam.Id.Eq(param.Key) Then
                        notFound = False
                        Exit For
                    End If
                Next

                If notFound Then
                    missingParameters.Add(String.Format("{0} - [{1}]", code.ToString(), dummyParam.Id))
                End If
            Next

        Next
        Return missingParameters
    End Function

    ''' <summary> Restituisce il contesto relativo al codice ambiente. </summary>
    Public Function GetContext(ByVal environment As EnvironmentDataCode) As BaseEnvironment
        Dim environmentContext As BaseEnvironment

        Select Case environment
            Case EnvironmentDataCode.DocmDB
                environmentContext = DocSuiteContext.Current.DocumentEnv
            Case EnvironmentDataCode.ProtDB
                environmentContext = DocSuiteContext.Current.ProtocolEnv
            Case EnvironmentDataCode.ReslDB
                environmentContext = DocSuiteContext.Current.ResolutionEnv
            Case Else
                ' Ambiente non esistente
                Throw New NotImplementedException(String.Format("Ambiente non esistente [{0}]", GetType(EnvironmentDataCode).Name))
        End Select

        Return environmentContext

    End Function

End Class
