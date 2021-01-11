Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()> _
Public Class DocumentContactFacade
    Inherits BaseDocumentFacade(Of DocumentContact, Integer, NHibernateDocumentContactDao)

    Public Sub New()
        MyBase.New()
    End Sub
    ''' <summary>
    ''' Restituisce una lista di ProtocolContact filtrati per Year, Number e ComunicationType
    ''' </summary>
    ''' <param name="year">Anno Protocollo</param>
    ''' <param name="number">Numero Protocollo</param>
    ''' <param name="comunicationType">ComunicationType</param>
    ''' <returns>Lista di ProtocolContact</returns>
    ''' <remarks></remarks>
    Public Function GetByComunicationType(ByVal year As Short, ByVal number As Integer, ByVal comunicationType As String) As IList(Of DocumentContact)
        Return _dao.GetByDocument(year, number)
    End Function

End Class