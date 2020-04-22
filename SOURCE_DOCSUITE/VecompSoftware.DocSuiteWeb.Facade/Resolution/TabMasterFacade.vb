Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class TabMasterFacade
    Inherits BaseResolutionFacade(Of TabMaster, Integer, NHibernateTabMasterDao)

#Region " Fields "

    Private _treeViewCaption As String


    Public Const DescriptionField As String = "Description"
    Public Const TitleField As String = "Title"
    Public Const ManagedDataField As String = "ManagedData"

#End Region

#Region " Properties "

    Public ReadOnly Property TreeViewCaption As String
        Get
            If String.IsNullOrEmpty(_treeViewCaption) Then
                _treeViewCaption = GetFieldValue(TitleField, DocSuiteContext.Current.ResolutionEnv.Configuration, ResolutionType.IdentifierDelibera)
            End If
            Return _treeViewCaption
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Restituisce il valore del campo specificato com parametro. </summary>
    ''' <param name="fieldName">nome del campo da recuperare</param>
    ''' <param name="type">Identificativo del tipo di atto (Delibera/Determina)</param>
    Public Function GetFieldValue(ByVal fieldName As String, ByVal configuration As String, ByVal type As Short) As String
        Return _dao.GetFieldValue(fieldName, configuration, type)
    End Function

    Public Function GetFieldValue(ByVal fieldName As String, ByVal configuration As String) As String
        Return GetFieldValue(fieldName, configuration, ResolutionType.IdentifierDelibera)
    End Function

    ''' <summary>
    ''' Restituisce una riga specifica di TabMaster a partire dalla combinazione di configurazione e tipo di resolution
    ''' </summary>
    ''' <param name="configuration"></param>
    ''' <param name="resolutionType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetByConfigurationAndType(ByVal configuration As String, ByVal resolutionType As Short) As TabMaster
        Return _dao.GetByConfigurationAndType(configuration, resolutionType)
    End Function

#End Region

End Class