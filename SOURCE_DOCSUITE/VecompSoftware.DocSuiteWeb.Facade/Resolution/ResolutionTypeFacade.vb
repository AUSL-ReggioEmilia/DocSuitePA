Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ResolutionTypeFacade
    Inherits BaseResolutionFacade(Of ResolutionType, Short, NHibernateResolutionTypeDao)

#Region " Fields "

    Private _deliberaCaption As String
    Private _determinaCaption As String

#End Region

#Region " Properties"

    Public ReadOnly Property DeliberaCaption As String
        Get
            If String.IsNullOrEmpty(_deliberaCaption) Then
                _deliberaCaption = Factory.ResolutionTypeFacade.GetDescription(ResolutionType.IdentifierDelibera)
            End If
            Return _deliberaCaption
        End Get
    End Property

    Public ReadOnly Property DeterminaCaption As String
        Get
            If String.IsNullOrEmpty(_determinaCaption) Then
                _determinaCaption = Factory.ResolutionTypeFacade.GetDescription(ResolutionType.IdentifierDetermina)
            End If
            Return _determinaCaption
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Descrizione user friedly. </summary>
    Public Function GetDescription(ByVal type As ResolutionType) As String
        Return GetDescription(type.Id)
    End Function

    ''' <summary> Descrizione user friedly. </summary>
    Public Function GetDescription(resolutionTypeId As Short) As String
        Return Factory.TabMasterFacade.GetFieldValue(TabMasterFacade.DescriptionField, DocSuiteContext.Current.ResolutionEnv.Configuration, resolutionTypeId)
    End Function

    ''' <summary> Elenco con descrizioni personalizzate per la configurazione corrente. </summary>
    Public Function GetResolutionTypes() As IList(Of ResolutionType)
        Return _dao.GetResolutionTypeDictionary()
    End Function

#End Region

End Class