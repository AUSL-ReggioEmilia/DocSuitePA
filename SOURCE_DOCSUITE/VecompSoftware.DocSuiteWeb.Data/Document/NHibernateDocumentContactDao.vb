Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Criterion

Public Class NHibernateDocumentContactDao
    Inherits BaseNHibernateDao(Of DocumentContact)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Restituisce una lista di DocumentContact filtrati per Year, Number e ComunicationType
    ''' </summary>
    ''' <param name="year">Anno Pratica</param>
    ''' <param name="number">Numero Pratica</param>
    ''' <returns>Lista di DocumentContact</returns>
    ''' <remarks></remarks>
    Public Function GetByDocument(ByVal year As Short, ByVal number As Integer) As IList(Of DocumentContact)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Contact", "Contact")

        criteria.Add(Restrictions.Eq("Id.Year", year))
        criteria.Add(Restrictions.Eq("Id.Number", number))

        criteria.AddOrder(Order.Asc("Contact.Description"))

        Return criteria.List(Of DocumentContact)()
    End Function
End Class
