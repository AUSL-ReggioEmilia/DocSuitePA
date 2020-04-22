Imports VecompSoftware.DocSuiteWeb.Data

Public Class ServiceCodeDescriptorFacade
    Inherits BaseResolutionFacade(Of ServiceCodeDescriptor, Integer, NHibernateServiceCodeDescriptorDao)

    ''' <summary>
    ''' Recupera l'elenco di descrittori attivi per la data di riferimento.
    ''' </summary>
    ''' <param name="reference">Data di riferimento</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescriptorsByDate(reference As DateTime) As IList(Of ServiceCodeDescriptor)
        Return _dao.GetDescriptorsByDate(reference)
    End Function

End Class
