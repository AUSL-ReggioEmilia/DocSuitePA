DECLARE @DestinationDb	AS VARCHAR(100)
DECLARE @SourceDb		AS VARCHAR(100)
DECLARE @Collation		AS VARCHAR(256)
DECLARE @IdArchive		AS VARCHAR(38)
DECLARE @Sql			AS NVARCHAR(4000)
DECLARE @Simulation		AS BIT 
BEGIN

Set @DestinationDb	= 'BiblosDs2010'
Set @SourceDb		= 'TFBDFTAT'
Set @Collation		= 'Latin1_General_CI_AS'
Set @Simulation		= 1



	IF ISNULL(@DestinationDb, '') = '' OR ISNULL(@SourceDb, '') = ''
	BEGIN
		RAISERROR('Specificare un database d''origine ed uno di destinazione.', 18, 1)
		RETURN
	END

	BEGIN TRANSACTION TR_DataTransfer
	
		PRINT 'Inizio procedura d''importazione dati.'
		/*********************************************/
		
		PRINT 'Creazione ARCHIVIO con il seguente nome: ' + @SourceDb
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Archive] ('
					+ 'IdArchive,'
					+ 'Name,'
					+ 'IsLegal,'
					+ 'MaxCache,'
					+ 'UpperCache,'
					+ 'LowerCache,'
					+ 'LastIdBiblos,'
					+ 'EnableSecurity,'
					+ 'PathPreservation'
					+ ') '
					+ 'SELECT NEWID(), ''' + @SourceDb + ''', 1, 104857600, 83886080, 52428800, 0, 0, ''C:\BiblosDs\WorkingDir'''
					+ 'WHERE NOT EXISTS (SELECT IdArchive FROM [' + @DestinationDb + '].[dbo].[Archive] WHERE RTRIM(LTRIM(Name)) = RTRIM(LTRIM(''' + @SourceDb + ''')))'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		DECLARE @TmpArchive TABLE (IdArchive VARCHAR(36) NOT NULL)
		
		INSERT INTO @TmpArchive
		SELECT TOP 1 CAST(IdArchive AS VARCHAR(40))
		FROM BiblosDs2010.dbo.Archive 
		WHERE Name = @SourceDb
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		DECLARE Cur 
		CURSOR 
			LOCAL 
			READ_ONLY
			FAST_FORWARD 
		FOR
			SELECT TOP 1 IdArchive FROM @TmpArchive
			
		OPEN Cur
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		FETCH Cur INTO @IdArchive
		
		CLOSE Cur
		
		DEALLOCATE Cur
		
		DELETE @TmpArchive
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		SET @IdArchive = '''' + @IdArchive + ''''			
		
		--PRINT 'Creazione STORAGE e STORAGE AREA'
		
		--SET @Sql = 'EXEC [' + @DestinationDb + '].[dbo].[Migrate_CreateStorage] ''' + @SourceDb + ''''
		--EXEC (@Sql)
		
		PRINT 'Importazione ATTRIBUTI (Archivio)'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Attributes]('
					+ 'IdAttribute,'
					+ 'Name,'
					+ 'IdArchive,'
					+ 'IsRequired,'
					+ 'KeyOrder,'
					+ 'IdMode,'
					+ 'IsMainDate,'
					+ 'IsAutoInc,'
					+ 'AttributeType,'
					+ 'ConservationPosition,'
					+ 'KeyFormat,'
					+ 'Validation,'
					+ 'Format'
					+ ') '
				+ 'SELECT NEWID() AS IdAttribute, attOgg.Nome as Name, ' + @IdArchive + ', '
					+ 'CASE ISNULL(attOgg.Obbligatorio, 0) '
					+ '	WHEN 0 THEN 0 '
					+ '	ELSE 1 '
					+ '	END AS IsRequired, '
					+ 'attOgg.PosizioneInChiaveUnivoca AS KeyOrder, '
					+ '0 AS IdMode, '
					+ 'CASE ISNULL(attOgg.DataPrincipale, 0) '
							+ 'WHEN 0 THEN 0 '
							+ 'ELSE 1 '
							+ 'END AS IsMainDate, '
					+ 'CASE attOgg.ProgressivoIntero '
						+ 'WHEN 0 THEN 0 '
						+ 'ELSE 1 '
						+ 'END AS IsAutoInc, '
					+ 'CASE LTRIM(RTRIM(UPPER(attOgg.TipoCampo))) '
						+ 'WHEN ''DATETIME'' THEN ''System.DateTime'' '
						+ 'WHEN ''INT'' THEN ''System.Int64'' '
						+ 'WHEN ''BIGINT'' THEN ''System.Int64'' '
						+ 'WHEN ''SMALLINT'' THEN ''System.Int64'' '
						+ 'WHEN ''TINYINT'' THEN ''System.Int64'' '
						+ 'ELSE ''System.String'' '
						+ 'END AS AttributeType, '
					+ 'attOgg.PosizioneInFileChiusura AS ConservationPosition, '
					+ 'ISNULL(attOgg.PorzioneInChiaveUnivoca, attOgg.FormatoInChiaveUnivoca) AS KeyFormat, '
					+ 'attOgg.Validazione AS Validation, '
					+ 'attOgg.Formato AS Format '
				+ 'FROM [' + @SourceDb + '].[dbo].[AttributoOggetto] attOgg '
				+ 'LEFT OUTER JOIN [' + @DestinationDb + '].[dbo].[Attributes] att ON RTRIM(LTRIM(att.Name)) COLLATE ' + @Collation + ' = RTRIM(LTRIM(attOgg.Nome)) '
				+ 'WHERE NOT att.Name IS NULL'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		--PRINT 'Importazione / creazione DOCUMENTI'
		
		--SET @Sql = 'EXEC [' + @DestinationDb + '].[dbo].[Compatibility_MergeOggetto] ''' + @SourceDb + ''''
		--EXEC (@Sql)
		
		PRINT 'Importazione PARAMETRI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationParameters]('
					+ 'IdArchive,'
					+ 'Label,'
					+ 'Value'
					+ ') '
					+ 'SELECT ' + @IdArchive + ', Nome, Valore '
					+ 'FROM [' + @SourceDb +'].[dbo].[Parametro] '
					+ 'WHERE Nome NOT IN (SELECT Label FROM [' + @DestinationDb + '].[dbo].[PreservationParameters])'
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione RUOLI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationRole]('
					+ 'IdPreservationRole,'
					+ 'KeyCode,'
					+ 'Name,'
					+ 'Enable,'
					+ 'AlertEnable'
					+ ') '
					+ 'SELECT NEWID(), IdRuolo, Nome, Attivo, RiceveChiusureTask '
					+ 'FROM [' + @SourceDb + '].[dbo].[Ruolo]'
					+ 'WHERE IdRuolo NOT IN (SELECT KeyCode FROM [' + @DestinationDb + '].[dbo].[PreservationRole])'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione SOGGETTI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationUser]('
					+ 'IdPreservationUser,'
					+ 'Name,'
					+ 'Surname,'
					+ 'FiscalId,'
					+ 'Address,'
					+ 'Email,'
					+ 'Enable,'
					+ 'DomainUser'
					+ ') '
					+ 'SELECT NEWID(), ISNULL(Nome, ''''), ISNULL(Cognome, ''''), ISNULL(CodiceFiscale, ''0000000000000000''), ISNULL(Residenza, ''''), ISNULL(eMail, ''''), ISNULL(Attivo, 1), ISNULL(DomainUser, '''') '
					+ 'FROM [' + @SourceDb + '].[dbo].[Soggetto] '
					+ 'WHERE RTRIM(LTRIM(ISNULL(Nome, ''X''))) + RTRIM(LTRIM(ISNULL(Cognome,''X''))) + RTRIM(LTRIM(ISNULL(CodiceFiscale, ''X''))) NOT IN ('
						+ 'SELECT RTRIM(LTRIM(ISNULL(Name, ''X''))) + RTRIM(LTRIM(ISNULL(Surname, ''X''))) + RTRIM(LTRIM(ISNULL(FiscalId, ''X''))) FROM [' + @DestinationDb + '].[dbo].[PreservationUser]'
					+ ')'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione RUOLI SOGGETTI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationUserRole]('
					+ 'IdPreservationUserRole,'
					+ 'IdPreservationRole,'
					+ 'IdPreservationUser,'
					+ 'IdArchive'
					+ ') '
					+ 'SELECT NEWID() AS IdPreservationUserRole, rol.IdPreservationRole, usr.IdPreservationUser, ' + @IdArchive + ' '
					+ 'FROM [' + @SourceDb + '].[dbo].[Ruolo_Soggetto] rs '
					+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON rs.IdSoggetto = sogg.IdSoggetto '
					+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON RTRIM(LTRIM(ISNULL(sogg.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.Cognome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.CodiceFiscale, ''X''))) = RTRIM(LTRIM(ISNULL(usr.Name, ''X''))) + RTRIM(LTRIM(ISNULL(usr.Surname, ''X''))) + RTRIM(LTRIM(ISNULL(usr.FiscalId, ''X''))) '
					+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON rs.IdRuolo = ruo.IdRuolo '
					+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON RTRIM(LTRIM(ruo.Nome)) = RTRIM(LTRIM(rol.Name))'
					+ ' WHERE CAST(rol.IdPreservationRole AS VARCHAR(40)) + CAST(usr.IdPreservationUser AS VARCHAR(40)) NOT IN (SELECT CAST(IdPreservationRole AS VARCHAR(40)) + CAST(IdPreservationUser AS VARCHAR(40)) FROM [' + @DestinationDb + '].[dbo].[PreservationUserRole])'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione TIPO TASK'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskType]('
					+ 'IdPreservationTaskType,'
					+ 'Description,'
					+ 'Period'
					+ ')'
					+ ' SELECT NEWID(), Descrizione, 0'
					+ ' FROM [' + @SourceDb + '].[dbo].[TipoTask]'
					+ ' WHERE RTRIM(LTRIM(Descrizione)) NOT IN (SELECT RTRIM(LTRIM([Description])) FROM [' + @DestinationDb + '].[dbo].[PreservationTaskType])'
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		/*
		PRINT 'Importazione STATO TASK (TaskStatus)'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskStatus]('
					+ 'IdPreservationTaskStatus,'
					+ 'Status'
					+ ') '
					+ 'SELECT NEWID() AS IdTaskStatus, tsk.Nome AS Status '
					+ 'FROM [' + @SourceDb + '].[dbo].[Task] tsk '
					+ 'WHERE tsk.Nome NOT IN (SELECT Status FROM [' + @DestinationDb + '].[dbo].[PreservationTaskStatus])'
		
		EXEC (@Sql)
		*/
		
		PRINT 'Importazione RUOLI TASK'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskRole] ('
					+ 'IdPreservationTaskType,'
					+ 'IdPreservationRole,'
					+ 'CreationDate'
					+ ') '
					+ 'SELECT IdPreservationTaskType, IdPreservationRole, GETDATE() '
					+ 'FROM [' + @SourceDb + '].[dbo].[TipoTask] tipo '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruolo ON  ruolo.IdRuolo = tipo.IdRuoloEsecutore '
						+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON RTRIM(LTRIM(pTT.Description)) = RTRIM(LTRIM(tipo.Descrizione)) '
						+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON RTRIM(LTRIM(rol.Name)) = RTRIM(LTRIM(ruolo.Nome)) '
					+ 'WHERE NOT EXISTS ('
										+ 'SELECT IdPreservationTaskType, IdPreservationRole FROM [' + @DestinationDb + '].[dbo].[PreservationTaskRole] ptr '
										+ 'WHERE ptr.IdPreservationTaskType = pTT.IdPreservationTaskType '
											+ 'AND ptr.IdPreservationRole = rol.IdPreservationRole'
									 + ')'
		
		EXEC(@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione SCADENZIARIO'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationSchedule]('
					+ 'IdPreservationSchedule,'
					+ 'Name,'
					+ 'Period,'
					+ 'ValidWeekDays,'
					+ 'FrequencyType,'
					+ 'Active'
					+ ') '
					+ 'SELECT NEWID(), Nome, Periodicita, REPLACE(REPLACE(GiorniSettimanaValidi, Char(13), ''''), Char(10), ''''), TipologiaFrequenza, Attivo '
					+ 'FROM [' + @SourceDb + '].[dbo].[Scadenziario] '
					+ 'WHERE RTRIM(LTRIM(ISNULL(Nome, ''X''))) + RTRIM(LTRIM(ISNULL(REPLACE(REPLACE(GiorniSettimanaValidi, Char(13), ''''), Char(10), ''''), ''X''))) + CAST(ISNULL(TipologiaFrequenza, -1) AS VARCHAR(10)) NOT IN ('
						+ 'SELECT RTRIM(LTRIM(ISNULL(Name, ''X''))) + RTRIM(LTRIM(ISNULL(REPLACE(REPLACE(ValidWeekDays, Char(13), ''''), Char(10), ''''), ''X''))) + CAST(ISNULL(FrequencyType, -1) AS VARCHAR(10)) '
						+ 'FROM [' + @DestinationDb + '].[dbo].[PreservationSchedule]'
					+ ')'
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione SCADENZIARIO - TIPO TASK'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationSchedule_TaskType]('
					+ 'IdPreservationSchedule,'
					+ 'IdPreservationTaskType'
					+ ') '
					+ 'SELECT IdPreservationSchedule, IdPreservationTaskType '
					+ 'FROM [' + @SourceDb + '].[dbo].[Scadenziario_TipoTask] scadTT '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Scadenziario] scad ON scadTT.IdScadenziario = scad.IdScadenziario '
							+ ' INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationSchedule] sched ON RTRIM(LTRIM(ISNULL(scad.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(scad.Periodicita, ''X''))) = RTRIM(LTRIM(ISNULL(sched.Name, ''X''))) + RTRIM(LTRIM(ISNULL(sched.Period, ''X''))) '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON scadTT.IdTipoTask = tt.IdTipoTask '
							+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] ptt ON tt.Descrizione = ptt.Description '
					+ 'WHERE CAST(IdPreservationSchedule AS VARCHAR(40)) + CAST(IdPreservationTaskType AS VARCHAR(40)) NOT IN (SELECT CAST(IdPreservationSchedule AS VARCHAR(40)) + CAST(IdPreservationTaskType AS VARCHAR(40)) FROM [' + @DestinationDb + '].[dbo].[PreservationSchedule_TaskType])'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione FESTIVITA'''
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationHolidays]('
					+ 'IdPreservationHolidays,'
					+ 'HolidayDate,'
					+ 'Description'
					+ ') '
					+ 'SELECT NEWID() AS IdPreservationHolidays, DataFestivita, Descrizione '
					+ 'FROM [' + @SourceDb + '].[dbo].[Festivita] '
					+ 'WHERE Descrizione NOT IN (SELECT Description FROM [' + @DestinationDb + '].[dbo].[PreservationHolidays])'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione TIPO GRUPPO TASK'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType]('
					+ 'IdPreservationTaskGroupType,'
					+ 'Description'
					+ ') '
					+ 'SELECT NEWID(), Descrizione '
					+ 'FROM [' + @SourceDb + '].[dbo].[TipoGruppoTask] '
					+ 'WHERE Descrizione NOT IN (SELECT Description FROM [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType])'
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione GRUPPO TASK'
		/*
			*************** N.B. ***************
			Possono esistere più gruppi task avente il medesimo nome, il medesimo soggetto incaricato e lo stesso identico scadenziario.
			Attenzione ai doppi!
		*/
		/*SET @Sql = 'IF EXISTS(SELECT Nome, IdSoggettoResponsabile, IdScadenziario FROM [' + @SourceDb + '].[dbo].[GruppoTask] GROUP BY Nome, IdSoggettoResponsabile, IdScadenziario HAVING COUNT(*) > 1) '
					+ 'BEGIN '
						+ 'RAISERROR (''Esistono dei GRUPPI TASK aventi lo stesso nome, lo stesso soggetto incaricato e lo stesso scadenziario.'', 18, 1) '
						+ 'RETURN '
					+ 'END'
		EXEC (@Sql)
		*/
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTaskGroup]('
					+ 'IdPreservationTaskGroup,'
					+ 'IdPreservationTaskGroupType,'
					+ 'Name,'
					+ 'IdPreservationUser,'
					+ 'IdPreservationSchedule,'
					+ 'Expiry,'
					+ 'EstimatedExpiry,'
					+ 'Closed'
					+ ') '
					+ 'SELECT NEWID(), ptgt.IdPreservationTaskGroupType, cast(ptgt.IdPreservationTaskGroupType as varchar) +''_''+gt.Nome AS Name, usr.IdPreservationUser, '
						+ 'sched.IdPreservationSchedule, gt.Scadenza AS Expiry, gt.ScadenzaTeorica AS EstimatedExpiry, gt.Chiuso AS Closed '
					+ 'FROM [' + @SourceDb + '].[dbo].[GruppoTask] gt '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoGruppoTask] tgt ON gt.IdTipoGruppoTask = tgt.IdTipoGruppoTask '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroupType] ptgt ON ptgt.Description = tgt.Descrizione '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON gt.IdSoggettoResponsabile =  sogg.IdSoggetto '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON RTRIM(LTRIM(ISNULL(sogg.Nome, ''X''))) + RTRIM(LTRIM(ISNULL(sogg.Cognome,''X''))) + RTRIM(LTRIM(ISNULL(sogg.CodiceFiscale, ''X''))) = RTRIM(LTRIM(ISNULL(usr.Name, ''X''))) + RTRIM(LTRIM(ISNULL(usr.Surname,''X''))) + RTRIM(LTRIM(ISNULL(usr.FiscalId, ''X''))) '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Scadenziario] scad ON gt.IdScadenziario = scad.IdScadenziario '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationSchedule] sched ON sched.Name = scad.Nome '
					+ 'WHERE NOT EXISTS(SELECT IdPreservationTaskGroupType, Name, IdPreservationUser, IdPreservationSchedule '
										+ 'FROM [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] '
										+ 'WHERE IdPreservationTaskGroupType = ptgt.IdPreservationTaskGroupType '
											+ 'AND Name = gt.Nome '
											+ 'AND IdPreservationUser = usr.IdPreservationUser '
											+ 'AND IdPreservationSchedule = sched.IdPreservationSchedule'
											+ ')'
					
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione TASK'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationTask]('
					+ 'IdPreservationTask,'
					+ 'IdPreservationConsole,'
					+ 'EstimatedDate,'
					+ 'ExecutedDate,'
					+ 'IdPreservationTaskType,'
					+ 'IdArchive,'
					-- + 'IdPreservationTaskStatus,'
					+ 'IdPreservationUser,'
					+ 'IdPreservationTaskGroup,'
					+ 'Name'
					+ ')'
					+ 'SELECT NEWID() AS IdPreservationTask, tsk.IdTask AS IdPreservationConsole, tsk.DataScadenza AS EstimatedDate, '
						+ 'tsk.DataCompletamento AS ExecutedDate, pTT.IdPreservationTaskType, ' + @IdArchive + ', ' 
						+ /*'tskStatus.IdPreservationTaskStatus, */ 'usr.IdPreservationUser, pGrp.IdPreservationTaskGroup, tsk.Nome as Name '
					+ 'FROM [' + @SourceDb + '].[dbo].[Task] tsk '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[GruppoTask] grp ON tsk.IdGruppoTask = grp.IdGruppoTask '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] pGrp ON RTRIM(LTRIM(UPPER(pGrp.Name))) = RTRIM(LTRIM(UPPER(grp.Nome))) '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[Soggetto] sogg ON tsk.IdSoggettoEsecutore = sogg.IdSoggetto '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationUser] usr ON UPPER(RTRIM(LTRIM(sogg.Nome)) + RTRIM(LTRIM(sogg.Cognome)) + RTRIM(LTRIM(sogg.CodiceFiscale))) = UPPER(RTRIM(LTRIM(usr.Name)) + RTRIM(LTRIM(usr.Surname)) + RTRIM(LTRIM(usr.FiscalId))) '
					+ 'INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON tsk.IdTipoTask = tt.IdTipoTask '
					+ '	INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON UPPER(RTRIM(LTRIM(tt.Descrizione))) = UPPER(RTRIM(LTRIM(pTT.Description))) '
					-- + 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskStatus] tskStatus ON UPPER(RTRIM(LTRIM(tsk.Nome))) = UPPER(RTRIM(LTRIM(tskStatus.Status))) '
					+ 'WHERE NOT EXISTS(SELECT IdTask, EstimatedDate, IdPreservationTaskType, IdArchive, IdPreservationTaskStatus, IdPreservationTaskGroup FROM [' + @DestinationDb + '].[dbo].[PreservationTask] '
										+ 'WHERE IdPreservationConsole = tsk.IdTask )'
											--+ 'AND EstimatedDate = tsk.DataScadenza '
											--+ 'AND IdPreservationTaskType = pTT.IdPreservationTaskType '
											--+ 'AND IdArchive = ' + @IdArchive + ' '
											--+ 'AND IdPreservationTaskStatus = tskStatus.IdPreservationTaskStatus '
											--+ 'AND IdPreservationTaskGroup = pGrp.IdPreservationTaskGroup)'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione CONSERVAZIONE SOSTITUTIVA'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[Preservation]('
					+ 'IdPreservation,'
					+ 'IdArchive,'
					+ 'IdPreservationTaskGroup,'
					+ 'IdPreservationTask,'
					+ 'Path,'
					+ 'Label,'
					+ 'PreservationDate,'
					+ 'StartDate,'
					+ 'EndDate,'
					+ 'CloseDate,'
					+ 'IndexHash,'
					+ 'CloseContent,'
					+ 'LastVerifiedDate,'
					+ 'IdPreservationUser,'
					+ 'IdCompatibility) '
					+ 'SELECT NEWID() AS IdPreservation, ' + @IdArchive + ', pTskGrp.IdPreservationTaskGroup, pTsk.IdPreservationTask,'
						+ 'cons.Collocazione AS Path, Cons.NomeSupporto AS Label,cons.DataConservazione AS PreservationDate,'
						+ 'cons.DataInizio AS StartDate, cons.DataFine AS EndDate, cons.DataChiusura AS CloseDate,'
						+ 'cons.FileIndiceHashHSA1 AS IndexHash, cons.FileChiusura AS CloseContent, cons.DataFirma AS LastVerifiedDate,'
						+ '(SELECT TOP 1 IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[PreservationUser]), '
						+ 'IdConservazione ' 
					+ 'FROM [' + @SourceDb + '].[dbo].[ConservazioneSostitutiva] cons '
						+ 'INNER JOIN [' + @SourceDb + '].[dbo].[GruppoTask] tskGrp ON tskGrp.IdGruppoTask = cons.IdGruppoTask '
							+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskGroup] pTskGrp ON RTRIM(LTRIM(pTskGrp.Name)) = RTRIM(LTRIM(tskGrp.Nome)) '
						+ 'LEFT OUTER JOIN [' + @SourceDb + '].[dbo].[Task] tsk ON tsk.IdTask = cons.IdTaskConservazione '
							+ 'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTask] pTsk ON pTsk.IdPreservationConsole = tsk.IdTask '
					+ 'WHERE NOT EXISTS (SELECT IdArchive, IdPreservationTaskGroup, IdPreservationTask, IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[Preservation] '
										+ 'WHERE IdArchive = ' + @IdArchive + ' '
											+ 'AND IdPreservationTaskGroup = pTskGrp.IdPreservationTaskGroup '
											+ 'AND IdPreservationTask = pTsk.IdPreservationTask '
											+ 'AND IdPreservationUser = (SELECT TOP 1 IdPreservationUser FROM [' + @DestinationDb + '].[dbo].[PreservationUser]))'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione TIPOLOGIA AVVISI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlertType]('
					+ 'IdPreservationAlertType,'
					+ 'IdPreservationRole,'
					+ 'IdPreservationConsole,'
					+ 'AlertText,'
					+ 'Offset'
					+ ') '
					+ 'SELECT NEWID(), rol.IdPreservationRole, tipo.IdTipoAvviso, tipo.TestoAvviso, tipo.Offset '
					+ 'FROM [' + @SourceDb + '].[dbo].[TipoAvviso] tipo '
					+	'INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON tipo.IdRuoloCC = ruo.IdRuolo '
					+		'INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON UPPER(RTRIM(LTRIM(rol.Name))) = UPPER(RTRIM(LTRIM(ruo.Nome))) '
					+ 'WHERE NOT EXISTS (SELECT * FROM [' + @DestinationDb + '].[dbo].[PreservationAlertType] '
										+ 'WHERE IdPreservationRole = rol.IdPreservationRole '
											+ 'AND AlertText = tipo.TestoAvviso)'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione TIPOLOGIA AVVISI - TASK'
		
		SET @Sql =	'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlertTask]('
					+ 'IdPreservationAlertType,'
					+ 'IdPreservationTaskType'
					+ ') '
					+ 'SELECT alr.IdPreservationAlertType, pTT.IdPreservationTaskType '
					+ 'FROM [' + @SourceDb + '].[dbo].[TipoAvviso] tipo '
					+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Ruolo] ruo ON tipo.IdRuoloCC = ruo.IdRuolo '
					+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationRole] rol ON UPPER(RTRIM(LTRIM(rol.Name))) = UPPER(RTRIM(LTRIM(ruo.Nome))) '
					+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationAlertType] alr ON alr.IdPreservationRole = rol.IdPreservationRole '
					+ '			AND alr.AlertText = tipo.TestoAvviso AND alr.Offset = tipo.Offset '
					+ '	INNER JOIN [' + @SourceDb + '].[dbo].[TipoTask] tt ON tipo.IdTipoTask = tt.IdTipoTask '
					+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTaskType] pTT ON pTT.Description = tt.Descrizione '
					+ 'WHERE pTT.IdPreservationTaskType NOT IN(SELECT IdPreservationTaskType FROM [' + @DestinationDb + '].[dbo].[PreservationAlertTask])'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione AVVISI'
		
		SET @Sql = 'INSERT INTO [' + @DestinationDb + '].[dbo].[PreservationAlert]('
					+ 'IdPreservationAlert,'
					+ 'IdPreservationAlertType,'
					+ 'IdPreservationTask,'
					+ 'MadeDate,'
					+ 'AlertDate,'
					+ 'ForwardFrequency'
					+ ')'
		+ 'SELECT NEWID() AS IdPreservationAlert, alr.IdPreservationAlertType, pTsk.IdPreservationTask, avv.Effettuato AS MadeDate,'
			+ 'avv.DataAvviso AS AlertDate, avv.FrequenzaDiReinvio AS ForwardFrequency '
		+ 'FROM [' + @SourceDb + '].[dbo].[Avviso] avv '
		+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationAlertType] alr ON alr.IdPreservationConsole = avv.IdTipoAvviso '
		+ '	INNER JOIN [' + @SourceDb + '].[dbo].[Task] tsk ON tsk.IdTask = avv.IdTask '
		+ '		INNER JOIN [' + @DestinationDb + '].[dbo].[PreservationTask] pTsk ON pTsk.IdPreservationConsole = tsk.IdTask '
		+ 'WHERE NOT EXISTS(SELECT IdPreservationAlertType, IdPreservationTask FROM [' + @DestinationDb + '].[dbo].[PreservationAlert] x '
							+ 'WHERE x.IdPreservationAlertType = alr.IdPreservationAlertType '
								+ 'AND x.IdPreservationTask = pTsk.IdPreservationTask)'
		
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Importazione ECCEZIONI'
		
		SET @Sql = 'INSERT INTO [BiblosDs2010].[dbo].[PreservationException]('
					+ 'IdPreservationException,'
					+ 'KeyName,'
					+ 'Description,'
					+ 'IsBlocked,'
					+ 'IdCompatibility) '
				+ 'SELECT * FROM( '
					+ 'SELECT NEWID() AS IdPreservationException, '
						+ 'CASE ecc.IdEccezione '
							+ 'WHEN 0 THEN ''NessunErrore'' '
							+ 'WHEN 1 THEN ''MancaValoreChiaveUnivoca'' '
							+ 'WHEN 2 THEN ''ValidazioneFallita'' '
							+ 'WHEN 3 THEN ''ChiaveUnivocaDuplicata'' '
							+ 'WHEN 4 THEN ''NumerazioneProgressivaErrata'' '
							+ 'WHEN 5 THEN ''MancaValoreCampoObbligatorio'' '
						+ 'END AS KeyName, '
						+ 'ecc.Descrizione AS Description, '
						+ 'ecc.Bloccante AS IsBlocked, '
						+ 'ecc.IdEccezione '
					+ 'FROM [' + @SourceDb + '].[dbo].[Eccezione] ecc '
					+ ') RS '
				+ 'WHERE RS.KeyName NOT IN (SELECT KeyName FROM [BiblosDs2010].[dbo].[PreservationException])'
				
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
		PRINT 'Aggiornamento CONSERVAZIONE DOCUMENTI'
		
		SET @Sql = 'UPDATE [' + @DestinationDb + '].[dbo].[Document] '
					+ 'SET IdPreservation = pres.IdPreservation, '
						+ 'IdPreservationException = ex.IdPreservationException, '
						+ 'DateCreated = oggCons.DataOggetto, '
						+ 'PrimaryKeyValue = oggCons.ChiaveUnivoca, '
						+ 'DocumentHash = oggCons.Hash '
					+ 'FROM [' + @SourceDb + '].[dbo].[Oggetto_Conservazione] oggCons '
						+ 'left join [' + @SourceDb + '].[dbo].[ConservazioneSostitutiva] cons on cons.IdConservazione = oggCons.IdConservazione '
						+ 'left join [' + @SourceDb + '].[dbo].[Eccezione] ecc on ecc.IdEccezione = oggCons.IdEccezione '
						+ 'left join [' + @DestinationDb + '].[dbo].[PreservationException] ex on ex.IdCompatibility = ecc.IdEccezione '
						+ 'left join [' + @DestinationDb + '].[dbo].[Preservation] pres on pres.IdCompatibility = cons.IdConservazione '
					+ 'WHERE oggCons.IdOggetto = document.IdBiblos'
					
		EXEC (@Sql)
		
		IF @@ERROR > 0
		BEGIN
			PRINT 'Si e'' verificato un errore. Impossibile continuare.'
			ROLLBACK TRANSACTION TR_DataTransfer
			RETURN
		END
		
	
	PRINT 'La procedura e'' stata completata con successo.'
	
	IF @Simulation = 1
		ROLLBACK TRANSACTION TR_DataTransfer
	ELSE
		COMMIT TRANSACTION TR_DataTransfer 

END