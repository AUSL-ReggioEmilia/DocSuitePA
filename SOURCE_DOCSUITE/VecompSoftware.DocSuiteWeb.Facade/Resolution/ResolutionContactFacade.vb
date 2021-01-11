Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionContactFacade
    Inherits BaseResolutionFacade(Of ResolutionContact, Integer, NHibernateResolutionContactDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce una lista di ResolutionContact filtrati per IdResolution e ComunicationType </summary>
    Public Function GetByComunicationType(ByVal idResolution As Integer, ByVal comunicationType As String) As IList(Of ResolutionContact)
        Return _dao.GetByComunicationType(idResolution, comunicationType)
    End Function

End Class