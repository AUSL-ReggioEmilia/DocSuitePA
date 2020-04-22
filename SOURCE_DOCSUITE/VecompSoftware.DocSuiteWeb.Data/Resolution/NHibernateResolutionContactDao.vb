Imports System.Collections.Generic
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionContactDao
    Inherits BaseNHibernateDao(Of ResolutionContact)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce una lista di ResolutionContact filtrati per IdResolution e ComunicationType </summary>
    ''' <param name="idResolution">Id dell'Atto</param>
    ''' <param name="comunicationType">ComunicationType</param>
    Public Function GetByComunicationType(ByVal idResolution As Integer, ByVal comunicationType As String) As IList(Of ResolutionContact)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Contact", "Contact")

        criteria.Add(Restrictions.Eq("Id.IdResolution", idResolution))

        If (Not String.IsNullOrEmpty(comunicationType)) Then
            criteria.Add(Restrictions.Eq("Id.ComunicationType", comunicationType))
        End If

        criteria.AddOrder(Order.Asc("Contact.Description"))

        Return criteria.List(Of ResolutionContact)()
    End Function

End Class
