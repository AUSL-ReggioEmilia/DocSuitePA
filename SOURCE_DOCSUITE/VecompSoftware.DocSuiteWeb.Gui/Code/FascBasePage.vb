Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class FascBasePage
    Inherits CommonBasePage

#Region "Fields"
    Private _idFascicle As Guid?
    Private _currentFascicle As Fascicle
    Private _currentFascicleWebAPI As Entity.Fascicles.Fascicle
    Private _categoryFascicleFacade As CategoryFascicleFacade
    Private _currentFascicleFinder As Data.WebAPI.Finder.Fascicles.FascicleFinder = Nothing

#End Region

#Region "Properties"
    Protected ReadOnly Property IdFascicle As Guid
        Get
            If _idFascicle Is Nothing Then
                _idFascicle = GetKeyValueOrDefault(Of Guid?)("IdFascicle", Nothing)
                If _idFascicle Is Nothing AndAlso Year.HasValue AndAlso IdCategory.HasValue AndAlso Incremental.HasValue Then
                    _currentFascicle = Facade.FascicleFacade.GetByYearNumberCategory(Year.Value, IdCategory.Value, Incremental.Value)
                    _idFascicle = _currentFascicle.Id
                End If
            End If
            If _idFascicle.HasValue Then
                Return _idFascicle.Value
            Else
                Return Guid.Empty
            End If
        End Get
    End Property

    Public ReadOnly Property CurrentFascicle As Fascicle
        Get
            If _currentFascicle Is Nothing Then
                _currentFascicle = Facade.FascicleFacade.GetById(IdFascicle)
            End If
            Return _currentFascicle
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleFinder() As Data.WebAPI.Finder.Fascicles.FascicleFinder
        Get
            If _currentFascicleFinder Is Nothing Then
                _currentFascicleFinder = New Data.WebAPI.Finder.Fascicles.FascicleFinder(DocSuiteContext.Current.CurrentTenant)
                _currentFascicleFinder.EnablePaging = False
                Return _currentFascicleFinder
            Else
                Return _currentFascicleFinder
            End If
        End Get
    End Property

    Public ReadOnly Property CurrentFascicleWebAPI As Entity.Fascicles.Fascicle
        Get
            If _currentFascicleWebAPI Is Nothing AndAlso Not IdFascicle = Guid.Empty Then
                Dim result As ICollection(Of WebAPIDto(Of Entity.Fascicles.Fascicle)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFascicleFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.UniqueId = IdFascicle
                        finder.ExpandRoles = True
                        Return finder.DoSearch()
                    End Function)

                Dim fascicleDto As WebAPIDto(Of Entity.Fascicles.Fascicle) = result.FirstOrDefault()
                If result IsNot Nothing Then
                    _currentFascicleWebAPI = fascicleDto.Entity
                End If
            End If

            Return _currentFascicleWebAPI
        End Get
    End Property

    Public ReadOnly Property CurrentCategoryFascicleFacade As CategoryFascicleFacade
        Get
            If _categoryFascicleFacade Is Nothing Then
                _categoryFascicleFacade = New CategoryFascicleFacade()
            End If
            Return _categoryFascicleFacade
        End Get
    End Property

    Protected ReadOnly Property CurrentUser As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Protected ReadOnly Property Year As Short?
        Get
            Return GetKeyValueOrDefault(Of Short?)("Year", Nothing)
        End Get
    End Property

    Protected ReadOnly Property IdCategory As Integer?
        Get
            Return GetKeyValueOrDefault(Of Integer?)("IdCategory", Nothing)
        End Get
    End Property

    Protected ReadOnly Property Incremental As Integer?
        Get
            Return GetKeyValueOrDefault(Of Integer?)("Incremental", Nothing)
        End Get
    End Property

    Protected ReadOnly Property IdFascicleFolder As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("IdFascicleFolder", Nothing)
        End Get
    End Property
#End Region

#Region "Methods"
    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, FascBasePage)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function
#End Region

End Class