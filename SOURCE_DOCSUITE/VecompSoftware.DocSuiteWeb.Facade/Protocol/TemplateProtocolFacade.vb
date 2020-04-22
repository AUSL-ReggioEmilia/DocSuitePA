Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class TemplateProtocolFacade
    Inherits FacadeNHibernateBase(Of TemplateProtocol, Int32, NHibernateTemplateProtocolDao)

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        _dbName = ProtDB
        _unitOfWork = New NHibernateUnitOfWork(_dbName)
    End Sub

    Public Sub New(factory As FacadeFactory)
        MyBase.New(factory)
    End Sub

    Public Sub New(ByVal dbName As String, factory As FacadeFactory)
        MyBase.New(dbName, factory)
    End Sub
#End Region

#Region "Methods"
    Private Function GetDefaultTemplates() As IList(Of TemplateProtocol)
        Return _dao.GetDefaultTemplates()
    End Function

    Public Function GetDefaultTemplate() As TemplateProtocol
        Dim defaults As IList(Of TemplateProtocol) = GetDefaultTemplates()
        If Not defaults.Any() Then
            Return Nothing
        End If

        Return defaults.First()
    End Function

    Public Sub SetDefaultTemplate(ByVal templateProtocol As TemplateProtocol)
        Dim defaultTemplates As IList(Of TemplateProtocol) = GetDefaultTemplates()
        If defaultTemplates.Any() Then
            For Each template As TemplateProtocol In defaultTemplates
                template.IsDefault = False
                UpdateOnly(template)
            Next
        End If

        templateProtocol.IsDefault = True
        UpdateOnly(templateProtocol)
    End Sub

    Public Sub RemoveDefaultTemplate(ByVal idTemplate As Integer)
        Dim defaultTemplate As TemplateProtocol = GetById(idTemplate)
        If defaultTemplate IsNot Nothing Then
            defaultTemplate.IsDefault = False
            UpdateOnly(defaultTemplate)
        End If
    End Sub

    Private Function GetTemplatesList() As IList(Of TemplateProtocol)
        Return _dao.GetTemplatesList()
    End Function

    Public Function GetTemplates() As IList(Of TemplateProtocol)
        Return GetTemplatesList()
    End Function

#End Region

End Class
