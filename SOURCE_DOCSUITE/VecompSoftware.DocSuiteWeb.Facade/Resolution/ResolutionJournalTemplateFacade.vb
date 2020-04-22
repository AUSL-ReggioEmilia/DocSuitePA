
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionJournalTemplateFacade
    Inherits BaseResolutionFacade(Of ResolutionJournalTemplate, Integer, NHibernateResolutionJournalTemplateDao)

    Public Sub New()
        MyBase.New()
    End Sub


    Public Function GetTemplatesByGroup(templateGroup As String, enabledOnly As Boolean?) As IList(Of ResolutionJournalTemplate)
        Return _dao.GetTemplatesByGroup(templateGroup, enabledOnly)
    End Function
    Public Function GetTemplatesByGroup(templateGroup As String) As IList(Of ResolutionJournalTemplate)
        Return _dao.GetTemplatesByGroup(templateGroup, False)
    End Function
    Public Function GetEnabledTemplates() As IList(Of ResolutionJournalTemplate)
        Return _dao.GetTemplatesByGroup(Nothing, True)
    End Function
    Public Function GetAllTemplates() As IList(Of ResolutionJournalTemplate)
        Return _dao.GetTemplatesByGroup(Nothing, Nothing)
    End Function

    Public Shared Function GetContainerAttributeBySpecifications(specifications As IList(Of ResolutionJournalTemplateSpecification)) As String
        Dim list As New List(Of Integer)
        For Each spec As ResolutionJournalTemplateSpecification In specifications
            If spec.Container Is Nothing Then
                Return "*"
            ElseIf Not list.Contains(spec.Container.Id) Then
                list.Add(spec.Container.Id)
            End If
        Next
        Return String.Join("|", list)
    End Function
    Public Shared Function GetContainerAttributeByTemplate(template As ResolutionJournalTemplate) As String
        Return GetContainerAttributeBySpecifications(template.Specifications)
    End Function

    Public Shared Function GetResolutionTypeAttributeBySpecifications(specifications As IList(Of ResolutionJournalTemplateSpecification)) As String
        Dim list As New List(Of Short)
        For Each spec As ResolutionJournalTemplateSpecification In specifications
            If spec.ReslType Is Nothing Then
                Return "*"
            ElseIf Not list.Contains(spec.ReslType.Id) Then
                list.Add(spec.ReslType.Id)
            End If
        Next
        Return String.Join("|", list)
    End Function
    Public Shared Function GetResolutionTypeAttributeByTemplate(template As ResolutionJournalTemplate) As String
        Return GetResolutionTypeAttributeBySpecifications(template.Specifications)
    End Function

    Public Shared Function GetServiceCodeAttributeBySpecifications(specifications As IList(Of ResolutionJournalTemplateSpecification)) As String
        Dim list As New List(Of String)
        For Each spec As ResolutionJournalTemplateSpecification In specifications
            If spec.ServiceCode Is Nothing Then
                Return "*"
            ElseIf Not list.Contains(spec.ServiceCode) Then
                list.Add(spec.ServiceCode)
            End If
        Next
        Return String.Join("|", list)
    End Function
    Public Shared Function GetServiceCodeAttributeByTemplate(template As ResolutionJournalTemplate) As String
        Return GetServiceCodeAttributeBySpecifications(template.Specifications)
    End Function

End Class
