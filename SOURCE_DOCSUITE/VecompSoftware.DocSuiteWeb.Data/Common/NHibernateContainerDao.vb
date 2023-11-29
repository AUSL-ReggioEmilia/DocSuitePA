Imports System.Linq
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports NHibernate.Transform
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContainerDao
    Inherits BaseNHibernateDao(Of Container)

#Region " Constructors "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function AlreadyExists(name As String) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Container)()
        criteria.Add(Restrictions.Eq("Name", name))
        criteria.SetProjection(Projections.Constant(True))

        criteria.SetMaxResults(1)
        Return criteria.List(Of Boolean).Count > 0
    End Function

    Public Function ContainerUsedProtocol(ByVal container As Container) As Boolean
        Dim protDao As New NHibernateProtocolDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        If protDao.GetCountByContainer(container) > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Function ContainerUsedDocument(ByVal container As Container) As Boolean
        Dim docmDao As New NHibernateDocumentDao("DocmDB")
        If docmDao.GetCountByContainer(container) > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Function ContainerUsedResolution(ByVal container As Container) As Boolean
        Dim reslDao As New NHibernateResolutionDao("ReslDB")
        If reslDao.GetCountByContainer(container) > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function GetAll(ByVal orderedExpression As String) As IList(Of Container)
        criteria = CreateGetContainerCriteria(orderedExpression)

        Return criteria.List(Of Container)()
    End Function

    Public Function GetAllRights(ByVal type As String, ByVal isActive As Boolean?) As IList(Of ContainerRightsDto)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.CreateAlias("C.ContainerGroups", "ContainerGroups", SqlCommand.JoinType.LeftOuterJoin)

        Dim fields As String
        Select Case UCase$(type)
            Case "DOCM"
                fields = "_documentRights"
                criteria.CreateAlias("C.DocmLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "PROT"
                fields = "_protocolRights"
                criteria.CreateAlias("C.ProtLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case Else
                fields = "_resolutionRights"
                criteria.CreateAlias("C.ReslLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
        End Select

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("C.IsActive", isActive.Value))
        End If

        criteria.Add(Restrictions.Not(Restrictions.Eq("ContainerGroups." & fields, "00000000000000000000")))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("C.Id"), "ContainerId")
        proj.Add(Projections.GroupProperty("C.Name"), "Name")
        proj.Add(Projections.GroupProperty("Location.Id"), "LocationId")

        criteria.SetProjection(Projections.Distinct(proj))
        criteria.AddOrder(Order.Asc("C.Name"))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ContainerRightsDto))

        Return criteria.List(Of ContainerRightsDto)()

    End Function

    Public Function GetAllRightsDistinct(ByVal type As String, ByVal isActive As Boolean?) As IList(Of Container)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.CreateAlias("C.ContainerGroups", "ContainerGroups", SqlCommand.JoinType.LeftOuterJoin)

        Dim fields As String
        Select Case UCase$(type)
            Case "DOCM"
                fields = "_documentRights"
                criteria.CreateAlias("C.DocmLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "PROT"
                fields = "_protocolRights"
                criteria.CreateAlias("C.ProtLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "SERIES"
                fields = "DocumentSeriesRights"
                criteria.CreateAlias("C.DocumentSeriesLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "DESK"
                fields = "DeskRights"
                criteria.CreateAlias("C.DeskLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "UDS"
                fields = "UDSRights"
                criteria.CreateAlias("C.UDSLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
            Case "FASCICLE"
                fields = "FascicleRights"
            Case Else
                fields = "_resolutionRights"
                criteria.CreateAlias("C.ReslLocation", "Location", SqlCommand.JoinType.LeftOuterJoin)
        End Select

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("C.IsActive", isActive.Value))
        End If

        criteria.Add(Expression.Not(Restrictions.Eq("ContainerGroups." & fields, "00000000000000000000")))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        criteria.AddOrder(Order.Asc("C.Name"))

        Return criteria.List(Of Container)()
    End Function

    Public Function GetSecurityGroupsContainerRights(ByVal type As String, ByVal securityGroups As IList(Of SecurityGroups), ByVal isActive As Boolean, ByVal rightPosition As Integer?, ByVal idContainer As Integer?) As IList(Of Container)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.CreateAlias("C.ContainerGroups", "ContainerGroups", SqlCommand.JoinType.LeftOuterJoin)

        Dim fields As String
        Select Case type.ToLowerInvariant
            Case "docm"
                fields = "_documentRights"
            Case "prot"
                fields = "_protocolRights"
            Case "series"
                fields = "DocumentSeriesRights"
            Case Else
                fields = "_resolutionRights"
        End Select

        Dim idGruppi As String()
        ReDim idGruppi(securityGroups.Count - 1)

        For i As Integer = 0 To securityGroups.Count - 1
            idGruppi(i) = securityGroups(i).Id.ToString()
        Next

        criteria.Add(Expression.In("ContainerGroups.SecurityGroup.Id", idGruppi))

        criteria.Add(Restrictions.Eq("C.IsActive", isActive))

        Dim tmpRights As String = RIGHTS_LENGTH
        If rightPosition.HasValue Then
            Mid$(tmpRights, rightPosition.Value, 1) = "1"
            criteria.Add(Expression.Like("ContainerGroups." & fields, tmpRights))
        End If

        If idContainer.HasValue Then
            criteria.Add(Restrictions.Eq("Id", idContainer.Value))
        End If

        criteria.AddOrder(Order.Asc("C.Name"))

        criteria.SetResultTransformer(New DistinctRootEntityResultTransformer())

        Return criteria.List(Of Container)()
    End Function

    Public Function GetContainerRigths(ByVal type As String, ByVal groups As String, ByVal active As Short, ByVal rightPosition As Integer?, ByVal idContainer As Integer?) As IList(Of Container)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.CreateAlias("C.ContainerGroups", "ContainerGroups", SqlCommand.JoinType.LeftOuterJoin)

        Dim gruppi As List(Of String) = (From g In groups.Split(","c) Select g.Trim("'"c)).ToList()

        criteria.Add(Expression.In("ContainerGroups.Name", gruppi.ToArray()))

        Select Case active
            Case 0, 1
                criteria.Add(Restrictions.Eq("C.IsActive", active))
            Case 3
                criteria.Add(Expression.Lt("C.IsActive", active))
        End Select

        'Il gruppo dell'utente deve possedere diritti 
        If rightPosition.HasValue Then
            Dim sRight As String = RIGHTS_LENGTH
            Mid$(sRight, rightPosition.Value, 1) = "1"
            Select Case type.ToUpperInvariant()
                Case "DOCM"
                    criteria.Add(Expression.Like("ContainerGroups._documentRights", sRight))
                Case "PROT"
                    criteria.Add(Expression.Like("ContainerGroups._protocolRights", sRight))
                Case "SERIES"
                    criteria.Add(Expression.Like("ContainerGroups.DocumentSeriesRights", sRight))
                Case Else
                    criteria.Add(Expression.Like("ContainerGroups._resolutionRights", sRight))
            End Select
        End If

        'eventuale contenitore selezionato
        If idContainer.HasValue Then
            criteria.Add(Restrictions.Eq("Id", idContainer.Value))
        End If

        criteria.AddOrder(Order.Asc("C.Name"))
        criteria.AddOrder(Order.Asc("ContainerGroups.Name"))

        criteria.SetResultTransformer(New DistinctRootEntityResultTransformer())

        Return criteria.List(Of Container)()
    End Function

    Public Function GetContainerExtensionRigths(ByVal type As String, ByVal groups As String, ByVal active As Short, ByVal keyType As String, Optional ByVal rights As String = "") As IList(Of Container)
        Dim sRight As String = RIGHTS_LENGTH

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.CreateAlias("C.ContainerGroups", "ContainerGroups", SqlCommand.JoinType.LeftOuterJoin)

        criteria.CreateAlias("C.ContainerExtensions", "CExtensions", SqlCommand.JoinType.InnerJoin)
        Dim dc As DetachedCriteria = DetachedCriteria.For(GetType(ContainerExtension), "Ext")
        dc.Add(Restrictions.EqProperty("CExtensions.Id", "Ext.Id"))
        dc.Add(Restrictions.Eq("CExtensions.Id.KeyType", keyType))
        dc.Add(Restrictions.Eq("CExtensions.KeyValue", "1"))
        dc.SetProjection(Projections.Id())
        criteria.Add(Subqueries.Exists(dc))

        Dim fields As String
        Select Case UCase$(type)
            Case "DOCM"
                fields = "_documentRights"
            Case "PROT"
                fields = "_protocolRights"
            Case Else
                fields = "_resolutionRights"
        End Select

        Dim gruppi As String() = groups.Split(","c)
        For i As Integer = 0 To gruppi.Length - 1
            gruppi(i) = gruppi(i).Trim("'"c)
        Next

        criteria.Add(Expression.In("ContainerGroups.Name", gruppi))

        Select Case active
            Case 0, 1 : criteria.Add(Restrictions.Eq("C.IsActive", active))
            Case 3 : criteria.Add(Expression.Lt("C.IsActive", active))
        End Select

        'Il gruppo dell'utente deve possedere diritti 
        If Not String.IsNullOrEmpty(rights) Then
            Mid$(sRight, rights, 1) = "1"
            criteria.Add(Expression.Like("ContainerGroups." & fields, sRight))
        End If

        criteria.AddOrder(Order.Asc("C.Name"))
        criteria.AddOrder(Order.Asc("ContainerGroups.Name"))

        'criteria.SetResultTransformer(New Transform.DistinctRootEntityResultTransformer())
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Container)()
    End Function

    Public Function GetContainerByName(ByVal description As String, ByVal isActive As Boolean?) As IList(Of Container)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'Description
        criteria.Add(Restrictions.Eq("Name", description))

        'IsActive
        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("IsActive", isActive))
        End If

        Return criteria.List(Of Container)()
    End Function

    Public Function IsAlmostOneConservationEnabled() As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Conservation", CByte(1))).SetProjection(Projections.RowCount())

        Return (criteria.UniqueResult(Of Integer)() > 0)
    End Function

    Private Function CreateGetContainerCriteria(env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, getRowCount As Boolean) As ICriteria
        Dim rightPositions As List(Of Integer) = Nothing
        If rightPosition.HasValue Then
            rightPositions = New List(Of Integer) From {rightPosition.Value}
        End If
        Return CreateGetContainerCriteria(env, rightPositions, active, getRowCount)
    End Function

    Private Function CreateGetContainerCriteria(env As DSWEnvironment, rightPositions As IList(Of Integer), active As Boolean?) As ICriteria
        Return CreateGetContainerCriteria(env, rightPositions, active, False)
    End Function

    Private Function CreateGetContainerCriteria(env As DSWEnvironment, rightPositions As IList(Of Integer), isActive As Boolean?, getRowCount As Boolean) As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Container)("C")
        criteria.CreateAlias("C.ContainerGroups", "CG", SqlCommand.JoinType.LeftOuterJoin)

        If Not rightPositions.IsNullOrEmpty Then
            Dim disj As New Disjunction()
            For Each rightPosition As Integer In rightPositions
                Dim pattern As String = "1".PadLeft(rightPosition, "_"c)
                Select Case env
                    Case DSWEnvironment.Protocol
                        disj.Add(Restrictions.Like("CG._protocolRights", pattern, MatchMode.Start))
                    Case DSWEnvironment.Resolution
                        disj.Add(Restrictions.Like("CG._resolutionRights", pattern, MatchMode.Start))
                    Case DSWEnvironment.Document
                        disj.Add(Restrictions.Like("CG._documentRights", pattern, MatchMode.Start))
                    Case DSWEnvironment.DocumentSeries
                        disj.Add(Restrictions.Like("CG.DocumentSeriesRights", pattern, MatchMode.Start))
                    Case DSWEnvironment.Desk
                        disj.Add(Restrictions.Like("CG.DeskRights", pattern, MatchMode.Start))
                    Case DSWEnvironment.UDS
                        disj.Add(Restrictions.Like("CG.UDSRights", pattern, MatchMode.Start))
                    Case Else
                        Throw New InvalidOperationException("È necessario specificare un DSWEnvironment valido se si valorizza rightPosition.")
                End Select
            Next
            criteria.Add(disj)
        End If

        If isActive.HasValue Then
            criteria.Add(Restrictions.Eq("C.IsActive", isActive.Value))
        End If
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        If getRowCount Then
            criteria.SetProjection(Projections.RowCount())
        Else
            criteria.AddOrder(Order.Asc("C.Name"))
        End If

        Return criteria
    End Function

    Public Function GetContainersByAD(groupNameIn As IList(Of String), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, Optional idProtocolType As Short? = Nothing) As IList(Of Container)
        Dim rightPositions As List(Of Integer) = Nothing
        If rightPosition.HasValue Then
            rightPositions = New List(Of Integer) From {rightPosition.Value}
        End If
        Return GetContainersByAD(groupNameIn, env, rightPositions, active, idProtocolType)
    End Function

    Public Function GetContainersByAD(groupNameIn As IList(Of String), env As DSWEnvironment, rightPositions As List(Of Integer), active As Boolean?, Optional idProtocolType As Short? = Nothing) As IList(Of Container)
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPositions, active)

        criteria.Add(Restrictions.In("CG.Name", groupNameIn.ToArray()))

        If idProtocolType.HasValue Then
            criteria.Add(Restrictions.Or(Restrictions.Eq("C.IdProtocolType", idProtocolType.Value), Restrictions.IsNull("C.IdProtocolType")))
        End If

        Return criteria.List(Of Container)()
    End Function

    Public Function GetContainersBySG(idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?, Optional idProtocolType As Short? = Nothing) As IList(Of Container)
        Dim rightPositions As List(Of Integer) = Nothing
        If rightPosition.HasValue Then
            rightPositions = New List(Of Integer) From {rightPosition.Value}
        End If
        Return GetContainersBySG(idGroupIn, env, rightPositions, active, idProtocolType)
    End Function

    Public Function GetContainersBySG(idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPositions As List(Of Integer), active As Boolean?, Optional idProtocolType As Short? = Nothing, Optional accounting As Boolean? = Nothing) As IList(Of Container)
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPositions, active)
        criteria.Add(Restrictions.In("CG.SecurityGroup.Id", idGroupIn.ToArray()))
        If idProtocolType.HasValue Then
            criteria.Add(Restrictions.Or(Restrictions.Eq("C.IdProtocolType", idProtocolType.Value), Restrictions.IsNull("C.IdProtocolType")))
        End If

        If accounting IsNot Nothing Then
            Dim existContainerProp As DetachedCriteria = DetachedCriteria.For(GetType(ContainerProperty), "CP")
            existContainerProp.Add(Restrictions.EqProperty("CP.Container.Id", "C.Id"))
            existContainerProp.Add(Restrictions.Eq("CP.ValueBoolean", True))
            existContainerProp.Add(Restrictions.Eq("CP.Name", ContainerPropertiesName.ResolutionAccountingEnabled))
            existContainerProp.SetProjection(Projections.Constant(1))
            If accounting Then
                criteria.Add(Subqueries.Exists(existContainerProp))
            Else
                criteria.Add(Subqueries.NotExists(existContainerProp))
            End If
        End If

        Return criteria.List(Of Container)()
    End Function

    Public Function GetContainersCountByAD(groupNameIn As IList(Of String), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Integer
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPosition, active, True)

        criteria.Add(Restrictions.In("CG.Name", groupNameIn.ToArray()))

        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Function GetContainersCountBySG(idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Integer
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPosition, active, True)

        criteria.Add(Restrictions.In("CG.SecurityGroup.Id", idGroupIn.ToArray()))

        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Function CheckContainerRightByAD(idContainer As Integer, groupNameIn As IList(Of String), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Boolean
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPosition, active, True)

        criteria.Add(Restrictions.In("CG.Name", groupNameIn.ToArray()))
        criteria.Add(Restrictions.Eq("CG.Container.Id", idContainer))

        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function CheckContainerRightBySG(idContainer As Integer, idGroupIn As IList(Of Integer), env As DSWEnvironment, rightPosition As Integer?, active As Boolean?) As Boolean
        Dim criteria As ICriteria = CreateGetContainerCriteria(env, rightPosition, active, True)

        criteria.Add(Restrictions.In("CG.SecurityGroup.Id", idGroupIn.ToArray()))
        criteria.Add(Restrictions.Eq("CG.Container.Id", idContainer))

        Return criteria.UniqueResult(Of Integer) > 0
    End Function

    Public Function GetArchiveNames(idArchive As Integer) As List(Of String)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Container)("C")
        criteria.CreateAlias("C.DocumentSeriesLocation", "Location", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("C.Archive.Id", idArchive))
        criteria.Add(Restrictions.Eq("C.IsActive", True))
        criteria.SetProjection(Projections.Property("Location.ProtBiblosDSDB"))
        Return criteria.List(Of String)()
    End Function

    Public Function GetContainersBySecurity(securityGroups As IList(Of SecurityGroups)) As IList(Of Container)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("ContainerGroups", "CG", JoinType.InnerJoin)
        criteria.Add(Restrictions.In("CG.SecurityGroup.Id", securityGroups.Select(Function(f) f.Id).ToArray()))
        criteria.AddOrder(Order.Asc("Name"))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of Container)()
    End Function

    Public Function GetOrderedContainersByName(Optional orderedExpression As String = Nothing, Optional name As String = Nothing) As IList(Of Container)
        Dim criteria As ICriteria = CreateGetContainerCriteria(orderedExpression, name)

        Return criteria.List(Of Container)()
    End Function

    Private Function CreateGetContainerCriteria(Optional orderedExpression As String = Nothing, Optional name As String = Nothing) As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        If Not String.IsNullOrEmpty(name) Then
            criteria.Add(Restrictions.Like("Name", name, MatchMode.Anywhere))
        End If

        If Not String.IsNullOrEmpty(orderedExpression) Then
            Dim expressions As String() = orderedExpression.Split(" "c)
            Select Case expressions(1).ToUpper()
                Case "DESC"
                    criteria.AddOrder(Order.Desc(expressions(0)))
                Case Else
                    criteria.AddOrder(Order.Asc(expressions(0)))
            End Select
        End If

        Return criteria
    End Function

#End Region

End Class
