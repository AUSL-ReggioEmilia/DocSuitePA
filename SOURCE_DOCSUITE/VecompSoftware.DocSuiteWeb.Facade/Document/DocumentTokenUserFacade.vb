Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class DocumentTokenUserFacade
    Inherits BaseDocumentFacade(Of DocumentTokenUser, YearNumberIncrCompositeKey, NHibernateDocumentTokenUserDao)

    Public Sub New()
        MyBase.New()
    End Sub


    Public Function CreateDocumentTokenUser(ByVal year As Short, ByVal number As Integer) As DocumentTokenUser
        Dim _documentTokenUser As New DocumentTokenUser()
        _documentTokenUser.Year = year
        _documentTokenUser.Number = number
        _documentTokenUser.Incremental = _dao.GetMaxIncremental(year, number) + 1S

        Return _documentTokenUser
    End Function


    Public Function GetDocumentTokenUserList(ByVal year As Short, ByVal number As Integer, ByVal DocumentToken As DocumentToken, Optional ByVal Account As String = "", _
                                                Optional ByVal bAddIncremental As Boolean = False, Optional ByVal bAddStepNumber As Boolean = False, _
                                                Optional ByVal bAddSubStep As Boolean = False, Optional ByVal bAddIdRoleDestination As Boolean = False, _
                                                Optional ByVal bAddIsActive As Boolean = False _
                                            ) As IList(Of DocumentTokenUser)

        Return _dao.GetDocumentTokenUserList(year, number, DocumentToken, Account, bAddIncremental, bAddStepNumber, bAddSubStep, bAddIdRoleDestination, bAddIsActive)
    End Function

    Public Function GetByYearNumber(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentTokenUser)
        Return _dao.GetByYearNumber(year, number)
    End Function

    Public Function DocumentTokenUserSearch( _
    ByVal Year As Short, ByVal Number As Integer, _
    Optional ByVal StepNumber As Integer = 0, Optional ByVal SubStep As Integer = 0, _
    Optional ByVal IdRoleDestination As Integer = 0, Optional ByVal IsActive As Boolean = False, _
    Optional ByVal Account As String = "", Optional ByVal Incremental As Boolean = False) As IList(Of DocumentTokenUser)


        Return _dao.DocumentTokenUserSearch(Year, Number, StepNumber, SubStep, IdRoleDestination, IsActive, Account, Incremental)

    End Function


End Class