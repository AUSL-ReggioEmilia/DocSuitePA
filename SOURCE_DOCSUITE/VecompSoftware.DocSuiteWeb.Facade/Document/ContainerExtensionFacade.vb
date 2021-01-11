Imports System
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data
Imports NHibernate
Imports VecompSoftware.Helpers.TreeHelper

<ComponentModel.DataObject()> _
Public Class ContainerExtensionFacade
    Inherits BaseDocumentFacade(Of ContainerExtension, ContainerExtensionCompositeKey, NHibernateContainerExtensionDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DBName As String)
        MyBase.New(DBName)
    End Sub


    ''' <summary> Esegue il salvataggio di un ContainerExtension. </summary>
    ''' <param name="contExtension">ContainerExtension da salvare</param>
    Public Overrides Sub Save(ByRef contExtension As ContainerExtension)
        Dim keyType As ContainerExtensionType
        If Not [Enum].TryParse(contExtension.Id.KeyType, keyType) Then
            Throw New ArgumentException("Tipo di container non previsto.", "")
        End If
        contExtension.Id.Incremental = _dao.GetMaxIdByContainerAndKey(contExtension.Id.idContainer, keyType) + 1S
        MyBase.SaveWithoutTransaction(contExtension)
    End Sub

    ''' <summary> Restituisce i containerExtension filtrati per Container e KeyType </summary>
    ''' <param name="pIdContainer">Id del Container</param>
    ''' <param name="pKey">KeyType</param>
    Public Function GetByContainerAndKey(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType) As IList(Of ContainerExtension)
        Return _dao.GetByContainerAndKey(pIdContainer, pKey)
    End Function

    Public Function GetAllAccountingSectionals() As IList(Of ContainerExtension)
        Return _dao.GetAllAccountingSectionals()
    End Function

    ''' <summary> Memorizza la gerarchia di ContainerExtension con IdContainer e KeyType passati. </summary>
    ''' <param name="pIdContainer">Id del contenitore</param>
    ''' <param name="pKey">KeyType</param>
    ''' <param name="pTree">Struttura ad albero da memorizzare</param>
    ''' <returns>True se il salvataggio avviene correttamente. False altrimenti.</returns>
    Public Function CreateContainerExtensionTree(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType, ByVal pTree As Tree(Of ContainerExtension)) As Boolean
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom(Me._dbName).BeginTransaction()
        Try
            'elimina tutte gli extension di quel contenitore e keytype
            _dao.DeleteByContainerAndKey(pIdContainer, pKey, transaction)
            'per ogni nodo dell'albero crea l'extension
            For Each node As TreeNode(Of ContainerExtension) In pTree.Root.Children
                InsertContainerExtensionFromTreeNode(pIdContainer, pKey, node)
            Next

            transaction.Commit()
            Return True

        Catch ex As Exception
            transaction.Rollback()
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Memorizza il containerExtension del container passato che identifica il settore di default
    ''' </summary>
    ''' <param name="pIdContainer">Id del container</param>
    ''' <param name="pKey">KeyType</param>
    ''' <param name="pRole">List di settori il cui primo indentifica quello di default</param>
    Public Function SaveContainerExtensionDefaultRole(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType, ByVal pRole As IList(Of Role)) As Boolean
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom(Me._dbName).BeginTransaction()
        Try
            'elimina tutti gli extension di quel contenitore e keytype
            _dao.DeleteByContainerAndKey(pIdContainer, pKey, transaction)
            'esegue l'inserimento di un containerExtension
            If (Not pRole Is Nothing) AndAlso (pRole.Count > 0) Then
                InsertContainerExtensionRole(pIdContainer, pKey, pRole(0).Id.ToString())
            Else
                InsertContainerExtensionRole(pIdContainer, pKey, "0")
            End If

            transaction.Commit()
            Return True

        Catch ex As Exception
            transaction.Rollback()
            Return False
        End Try
    End Function

    ''' <summary> Inserisce un containerExtension di tipo SD specificando il settore di default. </summary>
    ''' <param name="pIdContainer">Id del container</param>
    ''' <param name="pKey">KeyType</param>
    ''' <param name="pKeyValue">?</param>
    Private Sub InsertContainerExtensionRole(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType, ByVal pKeyValue As String)
        Dim contExt As ContainerExtension = New ContainerExtension()
        contExt.Id.idContainer = pIdContainer
        contExt.KeyType = pKey.ToString()
        contExt.IncrementalFather = 0
        contExt.KeyValue = pKeyValue

        Save(contExt)
    End Sub

    ''' <summary>  Inserisce un containerExtension popolato con i dati del nodo. </summary>
    ''' <param name="pIdContainer">Id del container</param>
    ''' <param name="pKey">KeyType</param>
    ''' <param name="pNode">Nodo da cui recuperare il containerExtension da inserire</param>
    Private Sub InsertContainerExtensionFromTreeNode(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType, ByVal pNode As TreeNode(Of ContainerExtension))
        Dim contExt As ContainerExtension = pNode.NodeObject
        contExt.Id.idContainer = pIdContainer
        contExt.KeyType = pKey.ToString()
        If pNode.Parent Is Nothing Then
            contExt.IncrementalFather = 0
        Else
            contExt.IncrementalFather = pNode.Parent.NodeObject.Incremental
        End If
        Save(contExt)

        If pNode.Children.Count > 0 Then
            For Each node As TreeNode(Of ContainerExtension) In pNode.Children
                InsertContainerExtensionFromTreeNode(pIdContainer, pKey, node)
            Next
        End If
    End Sub

End Class