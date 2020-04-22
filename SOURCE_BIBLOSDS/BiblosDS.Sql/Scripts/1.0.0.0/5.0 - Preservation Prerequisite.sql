-- =============================================
-- Script Template
-- =============================================

--Generare script di import di PreservationJournalingActivity

INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])     
values('00000000-0000-0000-0000-000000000001',	'SostituitoSupporto',	'Sostituito supporto',	1)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000002',	'CopiaSupporti',	'Copia supporti',	1)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000003',	'StampaManuale',	'Stampa manuale della conservazione',	0)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000004',	'ChiusuraConservazione',	'Chiusura conservazione',	0)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000005',	'Masterizzazione',	'Masterizzazione',	1)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000006',	'RiaperturaConservazione',	'Riapertura conservazione',	0)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000007',	'CreazioneConservazione',	'Creazione conservazione',	0)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000008',	'VerificaSupporto',	'Verifica supporto conservazione',	1)
INSERT INTO [BiblosDS2010].[dbo].[PreservationJournalingActivity]([IdPreservationJournalingActivity],[KeyCode],[Description],[IsUserActivity])
values('00000000-0000-0000-0000-000000000009',	'VerificaConservazione',	'Verifica conservazione',	1)

/*TODO Inserire Preservation Role
36549bb0-125a-4807-a46a-56ae2d892068	Responsabile Archiviazione	True	False	3
8c013d17-13d3-427f-9334-6533b4150ba0	Amministratore	True	False	1
ef54f5cb-b43d-42fb-8430-70d49c7b7c61	Utente	True	False	4
1c8fe8e7-e6e8-4687-a839-a24d619ad8d1	Responsabile Conservazione	True	False	2
*/
--Gestire IsUserDeleteEnable -> all'utente non deve essere permesso di eliminare tutte le attività di journaling (es. la creazione della conservazione non va eliminata mai)


-- =============================================
-- Script Template
-- =============================================

INSERT INTO [BiblosDS2010].[dbo].[PreservationStorageDeviceStatus]
           ([IdPreservationStorageDeviceStatus]
           ,[KeyCode]
           ,[Value])
VALUES
('00000000-0000-0000-0000-000000000001',	'Masterizzato',	'Masterizzato')
INSERT INTO [BiblosDS2010].[dbo].[PreservationStorageDeviceStatus]
           ([IdPreservationStorageDeviceStatus]
           ,[KeyCode]
           ,[Value])
VALUES
('00000000-0000-0000-0000-000000000002',	'Copiato',	'Copiato')
INSERT INTO [BiblosDS2010].[dbo].[PreservationStorageDeviceStatus]
           ([IdPreservationStorageDeviceStatus]
           ,[KeyCode]
           ,[Value])
VALUES
('94aaffc9-0f90-48fe-b930-c2f8f47563f8',	'StatoNullo',	'StatoNullo')

INSERT INTO [BiblosDS2010].[dbo].[PreservationTaskType]
           ([IdPreservationTaskType]
           ,[Description]
           ,[Period]
           ,[IdPreservationPeriod]
           ,[KeyCode])
		   values
('10adf819-c32a-4255-9e37-90a8a4268f05',	'Conservazione Sostitutiva',	'0',	NULL,	'1')

INSERT INTO [BiblosDS2010].[dbo].[PreservationTaskGroupType]
           ([IdPreservationTaskGroupType]
           ,[Description])
     VALUES
('5e9d7bc8-3a11-4f6a-8e0f-1b453eabdaaa',	'Conservazione')


INSERT INTO [BiblosDS2010].[dbo].[PreservationAlertType]
           ([IdPreservationAlertType]
           ,[IdPreservationRole]
           ,[IdPreservationConsole]
           ,[AlertText]
           ,[Offset])
     VALUES(
'd8b71dd2-f6d5-476b-8c29-5a768b9cc95e',	'8c013d17-13d3-427f-9334-6533b4150ba0',	'1',	'Conservazione Passiva',	'-2'
)
INSERT INTO [BiblosDS2010].[dbo].[PreservationAlertType]
           ([IdPreservationAlertType]
           ,[IdPreservationRole]
           ,[IdPreservationConsole]
           ,[AlertText]
           ,[Offset])
     VALUES('08ea262d-fb28-405f-ba0e-ec6eb736a298',	'8c013d17-13d3-427f-9334-6533b4150ba0',	'1',	'Conservazione Sostitutiva',	'-2')

	 INSERT INTO [BiblosDS2010].[dbo].[PreservationTaskRole]
           ([IdPreservationTaskType]
           ,[IdPreservationRole]
           ,[CreationDate])
     VALUES(
	 '10adf819-c32a-4255-9e37-90a8a4268f05',	'1c8fe8e7-e6e8-4687-a839-a24d619ad8d1',	'2011-11-02 10:46:17.490')


	 INSERT INTO [BiblosDS2010].[dbo].[PreservationException]
           ([IdPreservationException]
           ,[IdPreservationExceptionType]
           ,[IdPreservationExceptionCorrelated]
           ,[KeyName]
           ,[Description]
           ,[IsBlocked]
           ,[IdCompatibility])
     VALUES
	 ('ffa28866-9149-4637-b36b-08b31b76a56b',	NULL,	NULL,	'ChiaveUnivocaDuplicata',	'ChiaveUnivocaDuplicata',	'False',	'3')
	 INSERT INTO [BiblosDS2010].[dbo].[PreservationException]
           ([IdPreservationException]
           ,[IdPreservationExceptionType]
           ,[IdPreservationExceptionCorrelated]
           ,[KeyName]
           ,[Description]
           ,[IsBlocked]
           ,[IdCompatibility])
     VALUES
('24248e45-001d-4fa8-81cf-168ab3044681',	NULL,	NULL,	'NessunErrore',	'OK',	'True',	'0')
INSERT INTO [BiblosDS2010].[dbo].[PreservationException]
           ([IdPreservationException]
           ,[IdPreservationExceptionType]
           ,[IdPreservationExceptionCorrelated]
           ,[KeyName]
           ,[Description]
           ,[IsBlocked]
           ,[IdCompatibility])
     VALUES
('6c7088a5-f07f-4936-86f2-7aa16a388c84',	NULL,	NULL,	'MancaValoreChiaveUnivoca',	'MancaValoreChiaveUnivoca',	'False',	'1')
INSERT INTO [BiblosDS2010].[dbo].[PreservationException]
           ([IdPreservationException]
           ,[IdPreservationExceptionType]
           ,[IdPreservationExceptionCorrelated]
           ,[KeyName]
           ,[Description]
           ,[IsBlocked]
           ,[IdCompatibility])
     VALUES
('975cae22-89ad-4d4f-a446-bc0b4f248e31',	NULL,	NULL,	'NumerazioneProgressivaErrata',	'NumerazioneProgressivaErrata',	'True',	'4')
INSERT INTO [BiblosDS2010].[dbo].[PreservationException]
           ([IdPreservationException]
           ,[IdPreservationExceptionType]
           ,[IdPreservationExceptionCorrelated]
           ,[KeyName]
           ,[Description]
           ,[IsBlocked]
           ,[IdCompatibility])
     VALUES
('335e40a8-4896-46c8-9232-d258782ca02a',	NULL,	NULL,	'ValidazioneFallita',	'ValidazioneFallita',	'False',	'2')