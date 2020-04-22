Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierBasePage
    Inherits CommonBasePage

#Region "Fields"

    Private _idDossier As Guid?
    Private _currentDossierFinder As Data.WebAPI.Finder.Dossiers.DossierFinder = Nothing
    Private _currentdossier As Entity.Dossiers.Dossier = Nothing
    Private _dossierTitle As String

#End Region

#Region "Properties"

    Protected ReadOnly Property IdDossier As Guid
        Get
            If _idDossier Is Nothing Then
                _idDossier = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdDossier", Nothing)
            End If
            If _idDossier.HasValue Then
                Return _idDossier.Value
            Else
                Return Guid.Empty
            End If
        End Get
    End Property

    Protected ReadOnly Property DossierTitle As String
        Get
            If String.IsNullOrEmpty(_dossierTitle) Then
                _dossierTitle = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("DossierTitle", Nothing)
            End If
            Return _dossierTitle
        End Get
    End Property

    Protected ReadOnly Property CurrentDossier As Entity.Dossiers.Dossier
        Get
            If _currentdossier Is Nothing AndAlso Not IdDossier = Guid.Empty Then
                Dim dossierDto As WebAPIDto(Of Entity.Dossiers.Dossier) = New WebAPIDto(Of Entity.Dossiers.Dossier)
                CurrentDossierFinder.ExpandProperties = True
                CurrentDossierFinder.EnablePaging = False
                CurrentDossierFinder.DossierId = IdDossier

                dossierDto = CurrentDossierFinder.DoSearch().FirstOrDefault()
                If Not dossierDto Is Nothing Then
                    _currentdossier = dossierDto.Entity
                End If
            End If
                Return _currentdossier
        End Get
    End Property

    Public ReadOnly Property CurrentDossierFinder() As Data.WebAPI.Finder.Dossiers.DossierFinder
        Get
            If _currentDossierFinder Is Nothing Then
                _currentDossierFinder = New Data.WebAPI.Finder.Dossiers.DossierFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _currentDossierFinder
        End Get
    End Property

    Protected ReadOnly Property CurrentUser As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

#End Region

#Region "Methods"
    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, DossierBasePage)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function
#End Region

End Class