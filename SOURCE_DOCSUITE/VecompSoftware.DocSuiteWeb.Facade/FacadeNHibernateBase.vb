Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports System.Linq
Imports VecompSoftware.NHibernateManager.Config
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants

''' <summary> Base class for Facade implementation </summary>
''' <typeparam name="T">Domain Object</typeparam>
''' <typeparam name="TIdT">Type of Domain Object Key</typeparam>
''' <typeparam name="TDaoType">Dao Interface</typeparam>
<Serializable()>
Public MustInherit Class FacadeNHibernateBase(Of T, TIdT, TDaoType As INHibernateDao(Of T))
    Implements IFacadeBase(Of T, TIdT, T)

    Shared typeExitList As New List(Of Type)(New Type() {GetType(Role), GetType(Contact), GetType(Category)})

#Region " Fields "

    Public Const ProtDB As String = "ProtDB"
    Public Const DocmDB As String = "DocmDB"
    Public Const ReslDB As String = "ReslDB"

    Public Const BIBLOS_ATTRIBUTE_UniqueId As String = "UniqueId"
    Public Const BIBLOS_ATTRIBUTE_Environment As String = "Environment"

    Private _factory As FacadeFactory
    Protected _dao As TDaoType = Nothing
    Protected _dbName As String
    Protected _unitOfWork As IUnitOfWork

#End Region

#Region " Properties "
    Public ReadOnly Property CurrentTenant As Tenant
        Get
            Dim tenant As Tenant = Nothing
            If FacadeUtil.NeedTenantAction IsNot Nothing Then
                FacadeUtil.NeedTenantAction(Sub(t As Tenant)
                                                tenant = t
                                            End Sub)
            End If
            Return tenant
        End Get
    End Property

    Public Property Factory As FacadeFactory
        Get
            If _factory Is Nothing Then
                _factory = New FacadeFactory(_dbName)
            End If
            Return _factory
        End Get
        Set(value As FacadeFactory)
            _factory = value
        End Set
    End Property

    Public ReadOnly Property CurrentCriteria() As NHibernate.ICriteria
        Get
            Return _dao.HCriteria
        End Get
    End Property

    Public Overridable ReadOnly Property LoggerName As String
        Get
            Return LogName.FileLog
        End Get
    End Property

    Protected ReadOnly Property DataBaseConfig() As OpenSessionInViewSection
        Get
            Dim openSessionInViewSection As OpenSessionInViewSection = TryCast(ConfigurationManager.GetSection("nhibernateSettings"), OpenSessionInViewSection)
            If openSessionInViewSection Is Nothing Then
                Throw New DocSuiteException("Configurazione NHibernate") With {.Descrizione = "Impossibile trovare la sezione nhibernateSettings nel ConfigurationManager.", .User = DocSuiteContext.Current.User.FullUserName}
            End If
            Return openSessionInViewSection
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        Me.New(ProtDB)
    End Sub

    Public Sub New(ByVal dbName As String)
        Me.New(dbName, Nothing)
    End Sub

    Public Sub New(factory As FacadeFactory)
        Me.New(ProtDB, factory)
    End Sub

    Public Sub New(ByVal dbName As String, factory As FacadeFactory)
        _dbName = dbName
        _factory = factory
        ResolveDao()
    End Sub


#End Region

#Region " Methods "

    Public Overridable Function Count() As Integer Implements IFacadeBase(Of T, TIdT, T).Count
        Return _dao.Count()
    End Function

    Public Overridable Overloads Function GetById(ByVal id As TIdT) As T Implements IFacadeBase(Of T, TIdT, T).GetById
        Return GetById(id, False)
    End Function

    Public Overridable Overloads Function GetById(ByVal id As TIdT, shoudLock As Boolean) As T
        Return GetById(id, False, _dbName)
    End Function

    Public Overridable Overloads Function GetAll() As IList(Of T) Implements IFacadeBase(Of T, TIdT, T).GetAll
        Return GetAll(_dbName)
    End Function

    Public Overridable Overloads Sub Save(ByRef obj As T)
        Save(obj, _dbName)
    End Sub

    Public Overridable Overloads Sub SaveWithoutTransaction(ByRef obj As T)
        Save(obj, _dbName, needTransaction:=False)
    End Sub

    Public Overridable Overloads Sub UpdateNeedAuditable(ByRef obj As T, needAuditable As Boolean)
        UpdateNeedAuditable(obj, _dbName, needAuditable:=needAuditable)
    End Sub
    Public Overridable Overloads Sub UpdateWithoutTransaction(ByRef obj As T)
        Update(obj, _dbName, needTransaction:=False)
    End Sub
    Public Overridable Overloads Sub Update(ByRef obj As T)
        Update(obj, _dbName)
    End Sub
    Public Overridable Overloads Sub UpdateNoLastChange(ByRef obj As T)
        UpdateNoLastChange(obj, _dbName)
    End Sub

    Public Overridable Overloads Sub UpdateOnly(ByRef obj As T)
        UpdateOnly(obj, _dbName)
    End Sub

    Public Overridable Overloads Sub UpdateOnlyWithoutTransaction(ByRef obj As T)
        UpdateOnly(obj, _dbName, needTransaction:=False)
    End Sub

    Public Overridable Overloads Function Delete(ByRef obj As T) As Boolean
        Return Delete(obj, _dbName)
    End Function

    Public Overridable Overloads Function DeleteWithoutTransaction(ByRef obj As T) As Boolean
        Return Delete(obj, _dbName, needTransaction:=False)
    End Function


    Public Overridable Overloads Sub Evict(ByRef obj As T)
        Evict(obj, _dbName)
    End Sub

    Public Overridable Overloads Sub Recover(ByRef obj As T)
        Recover(obj, _dbName)
    End Sub

    Public Overridable Overloads Function IsUsed(ByRef obj As T) As Boolean
        Return False
    End Function

    Public Overridable Overloads Function GetById(ByVal id As TIdT, ByVal shoudLock As Boolean, ByVal dbName As String) As T
        _dao.ConnectionName = dbName
        Return _dao.GetById(id, False)
    End Function

    Public Overridable Overloads Function GetAll(ByVal dbName As String) As IList(Of T)
        _dao.ConnectionName = dbName
        Return _dao.GetAll()
    End Function

    ''' <summary>
    ''' Restituisce tutti gli elementi ordinati per il criterio selezionato
    ''' TODO: Rivedere con una espressione appropriata.
    ''' </summary>
    ''' <param name="orderExpression">Espressione del tipo "Property ASC|DESC"</param>
    Public Overridable Overloads Function GetAllOrdered(ByVal orderExpression As String) As IList(Of T)
        Return _dao.GetAll(orderExpression)
    End Function

    ''' <summary>
    ''' Restituisce tutti gli elementi di un database ordinati per il criterio selezionato
    ''' </summary>
    ''' <param name="orderExpression">Espressione del tipo "Property ASC|DESC"</param>
    ''' <param name="dbName">Nome della stringa di connessione da chiamare</param>
    ''' <returns>Tutti gli elementi ordinati</returns>
    Public Overridable Overloads Function GetAllOrdered(ByVal orderExpression As String, ByVal dbName As String) As IList(Of T)
        _dao.ConnectionName = dbName
        Return _dao.GetAll(orderExpression)
    End Function

    Public Shared Sub SetAuditableOnEntity(ByRef obj As IAuditable)
        Try
            If obj Is Nothing OrElse obj.RegistrationUser IsNot Nothing OrElse Not String.IsNullOrWhiteSpace(obj.RegistrationUser) Then
                Return
            End If
            obj.RegistrationDate = DateTimeOffset.UtcNow
            obj.RegistrationUser = DocSuiteContext.Current.User.FullUserName
            obj.LastChangedDate = Nothing
            obj.LastChangedUser = Nothing
        Catch ex As Exception

        End Try
    End Sub

    Protected Overridable Sub SetAuditableOnRelatedEntities(ByRef obj As Object)

        Dim objType As Type = obj.GetType()

        If typeExitList.Contains(objType) Then
            Exit Sub
        End If

        Dim propertyInfos As PropertyInfo()
        Try
            propertyInfos = obj.GetType().GetProperties()
        Catch ex As NHibernate.LazyInitializationException
            Return
        End Try

        Dim evalutate As ICollection(Of IAuditable) = New List(Of IAuditable)()
        Dim entities As IEnumerable(Of IAuditable)
        Dim entity As IAuditable

        For Each propertyInfo As PropertyInfo In propertyInfos
            entities = Nothing
            If GetType(IAuditable).IsAssignableFrom(propertyInfo.PropertyType) Then
                Try
                    evalutate.Add(DirectCast(propertyInfo.GetValue(obj, Nothing), IAuditable))
                Catch ex As NHibernate.LazyInitializationException
                End Try
            End If
            If GetType(IEnumerable(Of IAuditable)).IsAssignableFrom(propertyInfo.PropertyType) OrElse
                GetType(ICollection(Of IAuditable)).IsAssignableFrom(propertyInfo.PropertyType) Then
                entities = DirectCast(propertyInfo.GetValue(obj, Nothing), IEnumerable(Of IAuditable))
                If (entities IsNot Nothing) Then
                    Try
                        For Each item As IAuditable In entities.Where(Function(f) String.IsNullOrEmpty(f.RegistrationUser))
                            evalutate.Add(item)
                        Next
                    Catch ex As NHibernate.LazyInitializationException
                    End Try
                End If
            End If
        Next

        For index As Integer = 0 To evalutate.Count - 1 Step 1
            Try
                entity = evalutate.ElementAt(index)
                If (entity IsNot Nothing) Then
                    SetAuditableOnEntity(entity)
                End If
            Catch ex As NHibernate.LazyInitializationException
            End Try
        Next
    End Sub

    Public Overridable Overloads Sub Save(ByRef obj As T, ByVal dbName As String, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = dbName

        If GetType(T).GetInterface("IAuditable") IsNot Nothing Then
            Dim objToSave As IAuditable = DirectCast(obj, IAuditable)
            SetAuditableOnEntity(objToSave)
            obj = DirectCast(objToSave, T)
        End If

        Try
            SetAuditableOnRelatedEntities(obj)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("NOT IMPORTANT EXCEPTION on FacadeEntityBase.Save->SetAuditableOnRelatedEntities ", ex.Message), ex)
        End Try

        If (needTransaction) Then
            _dao.Save(obj)
        Else
            _dao.SaveWithoutTransaction(obj)
        End If

    End Sub

    Public Overridable Overloads Sub UpdateNeedAuditable(ByRef obj As T, ByVal dbName As String, Optional needAuditable As Boolean = True, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = dbName

        Try
            SetAuditableOnRelatedEntities(obj)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("NOT IMPORTANT EXCEPTION on FacadeEntityBase.Save->SetAuditableOnRelatedEntities ", ex.Message), ex)
        End Try

        If GetType(T).GetInterface("IAuditable") IsNot Nothing AndAlso needAuditable Then
            Dim objToUpdate As IAuditable = DirectCast(obj, IAuditable)
            objToUpdate.LastChangedDate = DateTimeOffset.UtcNow
            objToUpdate.LastChangedUser = DocSuiteContext.Current.User.FullUserName
            If needTransaction Then
                _dao.Update(objToUpdate)
            Else
                _dao.UpdateWithoutTransaction(objToUpdate)
            End If

        Else
            _dao.Update(obj)
        End If
    End Sub

    Public Overridable Overloads Sub Update(ByRef obj As T, ByVal dbName As String, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = dbName

        Try
            SetAuditableOnRelatedEntities(obj)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("NOT IMPORTANT EXCEPTION on FacadeEntityBase.Save->SetAuditableOnRelatedEntities ", ex.Message), ex)
        End Try

        If GetType(T).GetInterface("IAuditable") IsNot Nothing Then
            Dim objToUpdate As IAuditable = DirectCast(obj, IAuditable)
            objToUpdate.LastChangedDate = DateTimeOffset.UtcNow
            objToUpdate.LastChangedUser = DocSuiteContext.Current.User.FullUserName
            obj = objToUpdate
        End If
        If (needTransaction) Then
            _dao.Update(obj)
        Else
            _dao.UpdateWithoutTransaction(obj)
        End If
    End Sub

    ''' <summary>Salva gli aggiornamenti dul DataBase senza tracciare i dati di Auditable</summary>
    Public Overridable Overloads Sub UpdateNoLastChange(ByRef obj As T, ByVal dbName As String)
        _dao.ConnectionName = dbName

        Try
            SetAuditableOnRelatedEntities(obj)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("NOT IMPORTANT EXCEPTION on FacadeEntityBase.Save->SetAuditableOnRelatedEntities ", ex.Message), ex)
        End Try

        If GetType(T).GetInterface("IAuditable") IsNot Nothing Then
            Dim objToUpdate As IAuditable = DirectCast(obj, IAuditable)
            objToUpdate.LastChangedDate = Nothing
            objToUpdate.LastChangedUser = Nothing
            _dao.UpdateOnly(objToUpdate)
        Else
            _dao.UpdateOnly(obj)
        End If
    End Sub

    Public Overridable Overloads Sub UpdateOnly(ByRef obj As T, ByVal dbName As String, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = dbName

        Try
            SetAuditableOnRelatedEntities(obj)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, String.Concat("NOT IMPORTANT EXCEPTION on FacadeEntityBase.Save->SetAuditableOnRelatedEntities ", ex.Message), ex)
        End Try

        If GetType(T).GetInterface("IAuditable") IsNot Nothing Then
            Dim objToUpdate As IAuditable = DirectCast(obj, IAuditable)
            objToUpdate.LastChangedDate = DateTimeOffset.UtcNow
            objToUpdate.LastChangedUser = DocSuiteContext.Current.User.FullUserName
            obj = objToUpdate
        End If
        If (needTransaction) Then
            _dao.UpdateOnly(obj)
        Else
            _dao.UpdateOnlyWithoutTransaction(obj)
        End If
    End Sub

    Public Overridable Overloads Function IsUsed(ByRef obj As T, ByVal dbName As String) As Boolean
        Return False
    End Function

    Public Overridable Overloads Function Delete(ByRef obj As T, ByVal dbName As String, Optional needTransaction As Boolean = True) As Boolean
        _dao.ConnectionName = dbName

        Dim shortLogicalDeleteType As Type = GetType(T).GetInterface("ISupportShortLogicDelete")
        Dim booleanLogicalDeleteType As Type = GetType(T).GetInterface("ISupportBooleanLogicDelete")

        Dim isLogicalDelete As Boolean = shortLogicalDeleteType IsNot Nothing OrElse booleanLogicalDeleteType IsNot Nothing

        If Not isLogicalDelete Then
            If Not (_dao.ConnectionName = dbName) Then
                _dao.ConnectionName = dbName
            End If

            If (needTransaction) Then
                _dao.Delete(obj)
            Else
                _dao.DeleteWithoutTransaction(obj)
            End If

            Return True
        End If

        If shortLogicalDeleteType IsNot Nothing Then
            Dim objectToDelete As ISupportShortLogicDelete = DirectCast(obj, ISupportShortLogicDelete)
            objectToDelete.IsActive = 0S
            Update(objectToDelete, dbName, needTransaction)
        End If

        If booleanLogicalDeleteType IsNot Nothing Then
            Dim objectToDelete As ISupportBooleanLogicDelete = DirectCast(obj, ISupportBooleanLogicDelete)
            objectToDelete.IsActive = False
            Update(objectToDelete, dbName, needTransaction)
        End If

        Return False
    End Function

    Protected Overridable Overloads Sub Evict(ByVal obj As T, ByVal dbName As String)
        _dao.ConnectionName = dbName
        If Not (_dao.ConnectionName = dbName) Then
            _dao.ConnectionName = dbName
        End If

        _dao.Evict(obj)
    End Sub

    Public Overridable Overloads Sub Recover(ByRef obj As T, ByVal dbName As String)
        _dao.ConnectionName = dbName

        If GetType(T).GetInterface("ISupportShortLogicDelete") IsNot Nothing Then
            Dim objToRecover As ISupportShortLogicDelete = DirectCast(obj, ISupportShortLogicDelete)
            objToRecover.IsActive = 1S
            Update(objToRecover)
        End If

        If GetType(T).GetInterface("ISupportBooleanLogicDelete") IsNot Nothing Then
            Dim objToRecover As ISupportBooleanLogicDelete = DirectCast(obj, ISupportBooleanLogicDelete)
            objToRecover.IsActive = True
            Update(objToRecover)
        End If
    End Sub

    ''' <summary>
    ''' Inizializza il dao col tipo generico passato
    ''' </summary>
    ''' <remarks>Se il metodo va in errore assicurarsi che la dao sia stata messa nel costruttore della classe.</remarks>
    Private Sub ResolveDao()
        Try
            _dao = GetType(TDaoType).GetConstructor(New System.Type() {}).Invoke(New Object() {})
            If _dao Is Nothing Then
                Throw New Exception("Nessun DAO configurato nell'oggetto " & GetType(TDaoType).Name)
            End If
            _dao.ConnectionName = _dbName
        Catch ex As Exception
            Throw New DocSuiteException("Errore nel ritirare il costruttore di " & GetType(TDaoType).Name, ex) With {.User = DocSuiteContext.Current.User.FullUserName}
        End Try
    End Sub


#End Region

    Public Overridable Overloads Function GetListByIds(ids As IList(Of TIdT), factoryName As String) As IList(Of T)
        _dao.ConnectionName = factoryName
        Dim items As Array = DirectCast(ids, List(Of TIdT)).ToArray()
        Return _dao.GetListByIds(items)
    End Function

    Public Overridable Overloads Function GetListByIds(ids As IList(Of TIdT)) As IList(Of T)
        Return GetListByIds(ids, _dbName)
    End Function

    Protected Shared Function SetProtocolUniqueIdAttribute(doc As BiblosDocumentInfo, uniqueId As Guid, environment As Integer) As BiblosDocumentInfo
        If (doc IsNot Nothing) Then
            doc.AddAttribute(BIBLOS_ATTRIBUTE_UniqueId, uniqueId.ToString())
            doc.AddAttribute(BIBLOS_ATTRIBUTE_Environment, environment.ToString())
        End If
        Return doc
    End Function

End Class