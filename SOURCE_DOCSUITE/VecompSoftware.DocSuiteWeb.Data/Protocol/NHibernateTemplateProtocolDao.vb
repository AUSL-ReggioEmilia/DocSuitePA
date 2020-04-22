Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateTemplateProtocolDao
    Inherits BaseNHibernateDao(Of TemplateProtocol)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region
    Public Function GetDefaultTemplates() As IList(Of TemplateProtocol)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("IsDefault", True))
        crit.Add(Restrictions.Eq("IdTemplateStatus", CType(TemplateStatus.Active, Short)))
        Return crit.List(Of TemplateProtocol)()
    End Function

    Public Function GetTemplatesList() As IList(Of TemplateProtocol)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("IdTemplateStatus", CType(TemplateStatus.Active, Short)))
        Return crit.List(Of TemplateProtocol)()
    End Function
End Class
