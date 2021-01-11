Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<ComponentModel.DataObject()>
Public Class ContainerPropertyFacade
    Inherits CommonFacade(Of ContainerProperty, Integer, NHibernateContainerPropertyDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region


#Region " Methods "

    Public Function GetProperty(key As String, container As Container, parameterType As ContainerPropertyType) As Object
        Dim val As Object = Nothing

        If container Is Nothing Then
            Throw New ArgumentNullException("container")
        End If

        Dim properties As IList(Of ContainerProperty) = _dao.GetPropertyByContainerId(container.Id)
        If properties.Any(Function(x) x.Name.Equals(key)) Then
            Dim containerProp As ContainerProperty = properties.Single(Function(x) x.Name.Equals(key))

            Select Case parameterType
                Case ContainerPropertyType.PropertyString
                    val = containerProp.ValueString
                Case ContainerPropertyType.PropertyInt
                    val = containerProp.ValueInt
                Case ContainerPropertyType.PropertyGuid
                    val = containerProp.ValueGuid
                Case ContainerPropertyType.PropertyDouble
                    val = containerProp.ValueDouble
                Case ContainerPropertyType.PropertyDate
                    val = containerProp.ValueDate
                Case ContainerPropertyType.PropertyBoolean
                    val = containerProp.ValueBoolean
            End Select
        End If
        Return val
    End Function

    Public Sub SetProperty(ByRef containerProp As ContainerProperty, value As Object, parameterType As ContainerPropertyType)
        Select Case parameterType
            Case ContainerPropertyType.PropertyString
                containerProp.ValueString = value.ToString()
            Case ContainerPropertyType.PropertyInt
                containerProp.ValueInt = Convert.ToInt64(value)
            Case ContainerPropertyType.PropertyGuid
                containerProp.ValueGuid = Guid.Parse(value.ToString())
            Case ContainerPropertyType.PropertyDouble
                containerProp.ValueDouble = Convert.ToDouble(value)
            Case ContainerPropertyType.PropertyDate
                containerProp.ValueDate = Convert.ToDateTime(value)
            Case ContainerPropertyType.PropertyBoolean
                containerProp.ValueBoolean = Convert.ToBoolean(value)
        End Select
    End Sub

    Public Sub InsertContainerProperty(key As String, value As Object, container As Container, parameterType As ContainerPropertyType)
        If String.IsNullOrEmpty(key) Then
            Throw New ArgumentNullException("key")
        End If

        If container Is Nothing Then
            Throw New ArgumentNullException("container")
        End If

        Dim properties As IList(Of ContainerProperty) = _dao.GetPropertyByContainerId(container.Id)

        If properties.Any(Function(x) x.Name.Equals(key)) Then
            Dim prop As ContainerProperty = _dao.GetPropertyByNameAndContainer(key, container.Id)
            SetProperty(prop, value, parameterType)
            Update(prop)
        Else
            Dim newProperty As ContainerProperty = New ContainerProperty()
            newProperty.Container = container
            newProperty.Name = key
            newProperty.ContainerType = parameterType
            SetProperty(newProperty, value, parameterType)
            Save(newProperty)
        End If
    End Sub

    Public Sub DeleteContainerProperty(key As String, container As Container)
        If String.IsNullOrEmpty(key) Then
            Throw New ArgumentNullException("key")
        End If

        If container Is Nothing Then
            Throw New ArgumentNullException("container")
        End If
        Dim properties As IList(Of ContainerProperty) = _dao.GetPropertyByContainerId(container.Id)
        If properties.Any(Function(x) x.Name.Equals(key)) Then
            Dim prop As ContainerProperty = _dao.GetPropertyByNameAndContainer(key, container.Id)
            Delete(prop)
        End If
    End Sub
#End Region

End Class