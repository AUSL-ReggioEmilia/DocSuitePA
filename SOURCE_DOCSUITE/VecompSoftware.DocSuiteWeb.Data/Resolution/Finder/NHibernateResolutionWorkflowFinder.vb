Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports System.ComponentModel
Imports VecompSoftware.NHibernateManager.Transformer
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Dialect.Function
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq

<Serializable(), DataObject()>
Public Class NHibernateResolutionWorkflowFinder
    Inherits NHibernateResolutionFinder


    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#Region " Properties "

    Public Property WorkflowStepTo As TOWorkflow
    Public Property InDate As Date?
    Public Property ProtocolLink As String
    Public Property NumberFrom As Integer?
    Public Property NumberTo As Integer?
    Public Property CheckLastPageDate As Boolean
    Public Property WorkflowResponsibleStep As Tuple(Of String, Integer)

#End Region


    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "R")
        criteria.CreateAlias("R.ControllerStatus", "ControllerStatus", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("R.Location", "Location", JoinType.InnerJoin)
        criteria.CreateAliasIfNotExists("R.ResolutionWorkflows", "RW", JoinType.LeftOuterJoin)
        criteria.CreateAlias("RW.ResolutionWorkflowUsers", "RWU", JoinType.LeftOuterJoin)

        If Delibera Xor Determina Then
            'Filtro Delibera
            If Delibera Then
                criteria.Add(Restrictions.Eq("R.Type.Id", Convert.ToInt16(1)))
            End If

            'Filtro Determina
            If Determina Then
                criteria.Add(Restrictions.Eq("R.Type.Id", Convert.ToInt16(0)))
            End If
        End If

        Dim dcStepWorkflow As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("TW")
        dcStepWorkflow.Add(Restrictions.EqProperty("Id.WorkflowType", "R.WorkflowType"))
        dcStepWorkflow.SetProjection(Projections.SqlFunction(New VarArgsSQLFunction("(", "-", ")"),
                                                             NHibernateUtil.Int16,
                                                             Projections.Property("TW.Id.ResStep"),
                                                             Projections.Constant(1)))

        Dim dcResolutionWorkflow As DetachedCriteria = DetachedCriteria.For(Of ResolutionWorkflow)("RWW")
        dcResolutionWorkflow.Add(Subqueries.PropertyIn("ResStep", dcStepWorkflow))
        dcResolutionWorkflow.Add(Restrictions.Eq("IsActive", 1S))
        dcResolutionWorkflow.SetProjection(Projections.Property("Resolution.Id"))

        'Passo del Flusso
        Select Case WorkflowStepTo
            Case TOWorkflow.RicercaFlussoAdozione
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Expression.Conjunction()

                propConju.Add(Expression.IsNotNull("R.ProposeDate"))

                'Controllo sulle date
                If DateFrom.HasValue Then
                    CreateDateGeExpression(propConju, "ProposeDate", DateFrom.Value)
                End If
                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "ProposeDate", DateTo.Value)
                End If

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.AdoptionDate"))
                End If

                dcStepWorkflow.Add(Restrictions.Eq("Description", "Adozione"))

                criteria.Add(Subqueries.PropertyIn("Id", dcResolutionWorkflow))

                criteria.Add(propConju) 'Aggiungo il conjunction criteria al disjunction

            Case TOWorkflow.RicercaFlussoAssegnaAffariGenerali
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Restrictions.Conjunction()
                propConju.Add(Restrictions.IsNotNull("R.AdoptionDate"))

                If DateFrom.HasValue Then
                    CreateDateGeExpression(propConju, "AdoptionDate", DateFrom.Value)
                End If
                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "AdoptionDate", DateTo.Value)
                End If

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.SupervisoryBoardProtocolLink"))
                    propConju.Add(Restrictions.IsNull("R.SupervisoryBoardProtocolCollaboration"))
                    propConju.Add(Restrictions.IsNull("R.PublishingDate"))
                End If

                Dim detachedCriteria As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("TW2")
                detachedCriteria.Add(Restrictions.Eq("Description", DescriptionStep))
                detachedCriteria.Add(Restrictions.EqProperty("Id.WorkflowType", "R.WorkflowType"))
                detachedCriteria.Add(Restrictions.EqProperty("TW2.Id.ResStep", "RW.ResStep"))
                detachedCriteria.SetProjection(Projections.Constant(1))
                detachedCriteria.SetMaxResults(1)

                criteria.Add(Subqueries.Exists(detachedCriteria))

                criteria.Add(propConju)

            Case TOWorkflow.RicercaFlussoInvioAvvenutaAdozione
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Expression.Conjunction()

                propConju.Add(Expression.IsNotNull("R.AdoptionDate"))

                'Controllo sulle date
                If DateFrom.HasValue Then
                    If DateTo.HasValue Then
                        CreateDateGeExpression(propConju, "AdoptionDate", DateFrom.Value)
                    Else
                        CreateDateEqExpression(propConju, "AdoptionDate", DateFrom.Value)
                    End If
                End If
                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "AdoptionDate", DateTo.Value)
                End If

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.ProposerWarningDate"))
                End If

                criteria.Add(propConju) 'Aggiungo il conjunction criteria al disjunction

            Case TOWorkflow.RicercaFlussoInvioAdozioneOrganiControllo, TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Restrictions.Conjunction()

                propConju.Add(Expression.IsNotNull("R.AdoptionDate"))

                'Controllo sulle date
                If InDate.HasValue Then
                    CreateDateEqExpression(propConju, "AdoptionDate", InDate.Value)
                End If

                If DateFrom.HasValue Then
                    CreateDateGeExpression(propConju, "AdoptionDate", DateFrom.Value)
                End If
                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "AdoptionDate", DateTo.Value)
                End If

                'Controllo ProtocolLink
                If Not String.IsNullOrEmpty(ProtocolLink) Then
                    propConju.Add(Restrictions.Eq("R.ProposerProtocolLink", ProtocolLink))
                End If

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.SupervisoryBoardProtocolLink"))
                    propConju.Add(Restrictions.IsNull("R.SupervisoryBoardProtocolCollaboration"))
                End If

                criteria.Add(propConju) 'Aggiungo il conjunction criteria al disjunction

            Case TOWorkflow.RicercaFlussoEsecutivita
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Expression.Conjunction()

                propConju.Add(Expression.IsNotNull("R.PublishingDate"))

                'Controllo sulle date
                If InDate.HasValue Then
                    CreateDateEqExpression(propConju, "PublishingDate", InDate)
                End If

                'Controllo ProtoclLink
                propConju.Add(Restrictions.Eq("R.OCRegion", OCRegion))

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.EffectivenessDate"))
                End If

                criteria.Add(propConju) 'Aggiungo il conjunction criteria al disjunction

            Case TOWorkflow.RicercaFlussoUltimaPagina
                Dim propConju As Conjunction = Restrictions.Conjunction()

                'Filtro Anno
                If Not String.IsNullOrEmpty(Year) Then
                    criteria.Add(Restrictions.Eq("R.Year", _year))
                End If

                'Filtro per data adozione
                If DateFrom.HasValue Then
                    If DateTo.HasValue Then
                        CreateDateGeExpression(propConju, "AdoptionDate", DateFrom.Value)
                    Else
                        CreateDateEqExpression(propConju, "AdoptionDate", DateFrom.Value)
                    End If
                End If

                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "AdoptionDate", DateTo.Value)
                End If

                If NumberFrom.HasValue AndAlso Not Number.HasValue Then
                    criteria.Add(Restrictions.Ge("R.Number", NumberFrom.Value))
                End If

                If NumberTo.HasValue AndAlso Not Number.HasValue Then
                    criteria.Add(Restrictions.Le("R.Number", NumberTo.Value))
                End If

                propConju.Add(Restrictions.IsNotNull("R.EffectivenessDate"))

                If CheckLastPageDate Then
                    propConju.Add(Restrictions.IsNull("R.UltimaPaginaDate"))
                End If

                criteria.Add(propConju)

            Case TOWorkflow.RicercaFlussoPubblicazione
                'Creo un conjunction criteria per mettere in AND i criteria del passo
                Dim propConju As Conjunction = Expression.Conjunction()

                Select Case True
                    Case Delibera
                        propConju.Add(Expression.IsNotNull("R.SupervisoryBoardWarningDate"))

                        'Controllo sulle date
                        If InDate.HasValue Then
                            CreateDateEqExpression(propConju, "SupervisoryBoardWarningDate", InDate.Value)
                        End If
                    Case Determina

                        'Controllo ProtoclLink
                        If Not String.IsNullOrEmpty(ProtocolLink) Then
                            propConju.Add(Restrictions.Eq("R.SupervisoryBoardProtocolLink", ProtocolLink))
                        End If
                End Select

                'Controllo che non sia attivo lo step successivo
                If StepAttivo Then
                    propConju.Add(Restrictions.IsNull("R.PublishingDate"))
                End If

                criteria.Add(propConju) 'Aggiungo il conjunction criteria al disjunction

            Case TOWorkflow.RicercaRaccoltaFirmeAdozione
                Dim propConju As Conjunction = Restrictions.Conjunction()
                If DateFrom.HasValue Then
                    CreateDateGeExpression(propConju, "ProposeDate", DateFrom.Value)
                End If
                If DateTo.HasValue Then
                    CreateDateLeExpression(propConju, "ProposeDate", DateTo.Value)
                End If
                dcStepWorkflow.Add(Restrictions.Eq("Description", "ControlloOut"))
                criteria.Add(Subqueries.PropertyIn("Id", dcResolutionWorkflow))
                criteria.Add(propConju)
        End Select

        'Filtro Container
        Dim disjunctionRightsCriteria As Disjunction = New Disjunction()
        If Not (String.IsNullOrEmpty(ContainerIds)) Then
            disjunctionRightsCriteria.Add(Restrictions.In("R.Container.Id", ContainerIds.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()))
        End If

        If Not WorkflowStepsForceVisibility.IsNullOrEmpty() Then
            Dim stepDisj As Disjunction = Restrictions.Disjunction()
            Dim tmpConj As Conjunction
            For Each stepToExcelude As Tuple(Of String, Integer) In WorkflowStepsForceVisibility
                tmpConj = Restrictions.Conjunction()
                tmpConj.Add(Restrictions.Eq("RW.ResStep", Convert.ToInt16(stepToExcelude.Item2)))
                tmpConj.Add(Restrictions.Eq("WorkflowType", stepToExcelude.Item1))
                stepDisj.Add(tmpConj)
            Next
            disjunctionRightsCriteria.Add(stepDisj)
        End If
        criteria.Add(disjunctionRightsCriteria)

        If SelectedContainerId.HasValue Then
            criteria.Add(Restrictions.Eq("R.Container.Id", SelectedContainerId.Value))
        End If
        'Creo la join per la colonna di filtro
        MyBase.AddJoinAlias(criteria, "R.Container", "Container", JoinType.LeftOuterJoin)

        Dim isActDisju As Disjunction = Restrictions.Disjunction()
        isActDisju.Add(Restrictions.Eq("RW.IsActive", 1S))
        isActDisju.Add(Restrictions.IsNull("RW.IsActive"))
        criteria.Add(isActDisju)

        If Not WorkflowStepsExcluded.IsNullOrEmpty() Then
            Dim stepDisj As Disjunction
            Dim tmpConj As Conjunction
            For Each stepToExclude As Tuple(Of String, Integer) In WorkflowStepsExcluded
                stepDisj = Restrictions.Disjunction()
                tmpConj = Restrictions.Conjunction()
                tmpConj.Add(Restrictions.Not(Restrictions.Eq("RW.ResStep", Convert.ToInt16(stepToExclude.Item2))))
                tmpConj.Add(Restrictions.Eq("WorkflowType", stepToExclude.Item1))
                stepDisj.Add(tmpConj)
                stepDisj.Add(Restrictions.Not(Restrictions.Eq("WorkflowType", stepToExclude.Item1)))
                criteria.Add(stepDisj)
            Next
        End If

        If WorkflowResponsibleStep IsNot Nothing Then
            Dim stepDisj As Disjunction = Restrictions.Disjunction()
            Dim tmpConj As Conjunction = Restrictions.Conjunction()
            tmpConj.Add(Restrictions.Eq("RW.ResStep", Convert.ToInt16(WorkflowResponsibleStep.Item2)))
            tmpConj.Add(Restrictions.Eq("WorkflowType", WorkflowResponsibleStep.Item1))
            tmpConj.Add(Restrictions.Eq("RWU.AuthorizationType", AuthorizationRoleType.Responsible))
            stepDisj.Add(tmpConj)
            stepDisj.Add(Restrictions.Not(Restrictions.Eq("WorkflowType", WorkflowResponsibleStep.Item1)))
            criteria.Add(stepDisj)
        End If

        'InteropProposer
        If Not String.IsNullOrEmpty(InteropProposers) Then
            'Criteria per i ResolutionContact
            Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Proponenti")

            'Criteri per i Contact
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Prop")
            dcCon.Add(Restrictions.EqProperty("Proponenti.Id.IdContact", "Prop.Id"))
            dcCon.Add(Restrictions.Eq("Prop.FullIncrementalPath", InteropProposers))

            'Proiezione sull'id dei Contacts
            dcCon.SetProjection(Projections.Id())

            'Subquery sulla ResolutionContact
            reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
            reCon.Add(Restrictions.Eq("Proponenti.Id.ComunicationType", "P"))
            reCon.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

            'Subquery sulla Resolution
            criteria.Add(Subqueries.PropertyIn("Id", reCon))
        End If

        'Classificazione
        criteria.CreateAlias("R.Category", "Category")
        If Not String.IsNullOrEmpty(Categories) Then
            criteria.CreateAlias("R.CategoryAPI", "CategoryAPI")

            Dim rootDisjunction As Disjunction = Restrictions.Disjunction()
            For Each category As String In Categories.Split(","c)
                If IncludeChildCategories Then
                    Dim dis As Disjunction = Restrictions.Disjunction()
                    dis.Add(Restrictions.Eq("CategoryAPI.FullIncrementalPath", Categories))
                    dis.Add(Restrictions.Like("CategoryAPI.FullIncrementalPath", $"%{Categories}|%"))
                    rootDisjunction.Add(dis)
                Else
                    rootDisjunction.Add(Restrictions.Eq("CategoryAPI.FullIncrementalPath", Categories))
                End If
            Next
            criteria.Add(rootDisjunction)
        End If

        If IdStatus.HasValue Then
            criteria.Add(Restrictions.Eq("Status.Id", IdStatus))

        ElseIf EnableStatus Then '(idStatus=0 OR idStatus BETWEEN -4 AND -2)
            Dim statDisju As Disjunction = Expression.Disjunction()
            statDisju.Add(Restrictions.Eq("Status.Id", 0S))
            statDisju.Add(Expression.Between("Status.Id", -4S, -2S))
            criteria.Add(statDisju)
        End If

        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)

        Return criteria

    End Function

    Public Overrides Function DoSearchHeader() As IList(Of ResolutionHeader)
        Dim criteria As ICriteria = CreateCriteria()

        criteria.AddOrder(Order.Asc("R.Year"))
        criteria.AddOrder(Order.Asc("R.Number"))
        criteria.AddOrder(Order.Asc("R.Id"))

        If EnablePaging Then
            criteria.SetFirstResult(PageIndex)
            criteria.SetMaxResults(PageSize)
        End If

        ' Uso la stessa proiezione del ResolutionFinder di base.
        Dim proj As ProjectionList = GetProjectionForResolutionHeader(False)

        If (DocSuiteContext.Current.ResolutionEnv.IsLogEnabled) Then
            LoadFetchMode(criteria, "ResolutionLogs")
        End If

        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(ResolutionHeader)))

        If TopMaxRecords > 0 Then
            criteria.SetResultTransformer(New TopRecordsResultTransformer(TopMaxRecords))
        End If

        Return criteria.List(Of ResolutionHeader)()
    End Function

End Class