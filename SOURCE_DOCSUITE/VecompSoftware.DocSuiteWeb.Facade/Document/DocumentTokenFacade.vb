Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentTokenFacade
    Inherits BaseDocumentFacade(Of DocumentToken, YearNumberIncrCompositeKey, NHibernateDocumentTokenDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetMaxIncremental(ByVal year As Short, ByVal number As Integer) As Short
        Return _dao.GetMaxIncremental(year, number)
    End Function

    Public Sub UpdateIsActive(ByRef documentToken As DocumentToken, ByVal isActive As Short)
        documentToken.IsActive = isActive
    End Sub

    Public Sub UpdateResponse(ByRef documentToken As DocumentToken, ByVal response As String)
        documentToken.Response = response
    End Sub

    Public Function CreateDocumentToken(ByVal year As Short, ByVal number As Integer) As DocumentToken
        Dim _documentToken As New DocumentToken()
        _documentToken.Year = year
        _documentToken.Number = number
        _documentToken.Incremental = _dao.GetMaxIncremental(year, number) + 1

        Return _documentToken
    End Function

    Public Overloads Function GetById(ByVal year As Short, ByVal number As Integer, ByVal incremental As Integer, Optional ByVal shoudLock As Boolean = False) As DocumentToken
        Dim id As YearNumberIncrCompositeKey = New YearNumberIncrCompositeKey(year, number, incremental)
        Return MyBase.GetById(id, shoudLock)
    End Function

    Public Function GetMaxStep(ByVal documentTokens As IList(Of DocumentToken)) As Short
        Dim _maxStep As Short = Short.MinValue
        If documentTokens IsNot Nothing Then
            For Each documentToken As DocumentToken In documentTokens
                If documentToken.DocStep > _maxStep Then
                    _maxStep = documentToken.DocStep
                End If
            Next
        Else
            _maxStep = 0
        End If
        Return _maxStep
    End Function

    Function GetDocumentTokenRoleR(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentToken)
        Return GetTokenList(year, number, {"RC", "RP", "RR"})
    End Function

    Public Function GetTokenList(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), Optional ByVal bAddResponse As Boolean = False, Optional ByVal iStep As Integer = 0, Optional ByVal bIncludeIsActive As Boolean = True) As List(Of DocumentToken)
        Return _dao.GetTokenList(year, number, idTokenTypes, bAddResponse, iStep, bIncludeIsActive)
    End Function

    Public Function GetTokenList(ByVal year As Short, ByVal number As Integer) As List(Of Object)
        Return _dao.GetTokenList(year, number)
    End Function

    Function GetTokenListForAllTokenTypes(ByVal year As Short, ByVal number As Integer, Optional ByVal bAddResponse As Boolean = False, Optional ByVal Incremental As Int16 = 0, Optional ByVal Role As Role = Nothing) As IList(Of DocumentToken)
        Return _dao.GetTokenListForAllTokenTypes(year, number, bAddResponse, Incremental, Role)
    End Function

    Function GetTokenOperationDateList(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), ByVal roles As String()) As IList(Of DocumentToken)
        Return _dao.GetTokenOperationDateList(year, number, idTokenTypes, roles)
    End Function

    Function GetDocumentTokenByTokenType(ByVal year As Short, ByVal number As Integer, ByVal idTokenTypes As String(), ByVal roles As String(), Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal isActive As Boolean = False) As IList(Of DocumentToken)
        Return _dao.GetDocumentTokenByTokenType(year, number, idTokenTypes, roles, bAddResponseCriteria, isActive)
    End Function

    Function GetByYearNumber(ByVal year As Short, ByVal number As Integer, ByVal isActive As Integer) As IList(Of DocumentToken)
        Return _dao.GetByYearNumber(year, number, isActive)
    End Function

    Function GetTokenListCC(ByVal year As Short, ByVal number As Integer, Optional ByVal bAddResponseCriteria As Boolean = False, Optional ByVal Incremental As Int16 = 0, Optional ByVal Role As Integer = 0) As IList(Of DocumentToken)
        Return _dao.GetTokenListCC(year, number, bAddResponseCriteria, Incremental, Role)
    End Function

    Public Function DocumentTokenRoleCC(ByVal Year As Short, ByVal Number As Integer, Optional ByVal iStep As Integer = 0) As IList(Of DocumentToken)
        Return _dao.DocumentTokenRoleCC(Year, Number, iStep)
    End Function

    Public Function GetTokenSuspend(ByVal Year As Short, ByVal Number As Integer, ByVal [Step] As Integer, ByVal IdTokenType As String()) As IList(Of DocumentToken)
        Return _dao.GetTokenSuspend(Year, Number, [Step], IdTokenType)
    End Function

    Public Function DocumentTokenStep(ByVal year As Short, ByVal number As Integer, ByVal idRole As Integer) As IList(Of DocumentToken)
        Return _dao.DocumentTokenStep(year, number, idRole)
    End Function

    Function GetDocumentTokenRoleP(ByVal year As Short, ByVal number As Integer, Optional ByVal idRoleDestination As Integer = 0) As IList(Of DocumentToken)
        Return _dao.GetDocumentTokenRoleP(year, number, idRoleDestination)
    End Function

    Function VerifyDocumentTokenRoleP(ByVal year As Short, ByVal number As Integer, ByVal idRoleDestination As Integer) As Boolean
        Dim _documentTokens As IList(Of DocumentToken) = _dao.GetDocumentTokenRoleP(year, number, idRoleDestination)
        If _documentTokens.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
