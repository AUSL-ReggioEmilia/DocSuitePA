Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class PrintLetterRoleFacade
    Inherits BaseResolutionFacade(Of PrintLetterRole, String, NHibernatePrintLetterRoleDao)

#Region "Constructor"
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Ritorna tutti gli elementi in tabella in un dizionario
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodesDictionary() As IDictionary(Of String, String)
        Dim codes As IList(Of PrintLetterRole) = _dao.GetAll()
        If Not codes.Any() Then
            Return New Dictionary(Of String, String)
        End If
        Return codes.ToDictionary(Function(k) k.Id.IdCode, Function(k) k.Id.Description)
    End Function
#End Region
End Class
