
--#############################################################################
UPDATE [dbo].[ProtocolContact] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PC SET PC.UniqueIdProtocol = P.UniqueId,
			  PC.RegistrationDate = P.RegistrationDate,
			  PC.RegistrationUser = P.RegistrationUser,
			  PC.LastChangedDate = P.LastChangedDate,
			  PC.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolContact] AS PC
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PC.[Year] and P.[Number] = PC.[Number]
WHERE PC.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolRole] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PR SET PR.UniqueIdProtocol = P.UniqueId,
			  PR.RegistrationDate = P.RegistrationDate,
			  PR.RegistrationUser = P.RegistrationUser,
			  PR.LastChangedDate = P.LastChangedDate,
			  PR.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolRole] AS PR
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PR.[Year] and P.[Number] = PR.[Number]
WHERE PR.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolParer] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PP SET PP.UniqueIdProtocol = P.UniqueId,
			  PP.RegistrationDate = P.RegistrationDate,
			  PP.RegistrationUser = P.RegistrationUser,
			  PP.LastChangedDate = P.LastChangedDate,
			  PP.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolParer] AS PP
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PP.[Year] and P.[Number] = PP.[Number]
WHERE PP.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolContactManual] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PCM SET PCM.UniqueIdProtocol = P.UniqueId,
			   PCM.RegistrationDate = P.RegistrationDate,
			   PCM.RegistrationUser = P.RegistrationUser,
			   PCM.LastChangedDate = P.LastChangedDate,
			   PCM.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolContactManual] AS PCM
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PCM.[Year] and P.[Number] = PCM.[Number]
WHERE PCM.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolLog] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PL SET PL.[UniqueIdProtocol] = P.UniqueId
FROM [dbo].[ProtocolLog] AS PL
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PL.[Year] and P.[Number] = PL.[Number]
WHERE PL.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolRoleUser] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PRU SET PRU.UniqueIdProtocol = P.UniqueId,
			   PRU.RegistrationDate = P.RegistrationDate,
			   PRU.RegistrationUser = P.RegistrationUser,
			   PRU.LastChangedDate = P.LastChangedDate,
			   PRU.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolRoleUser] AS PRU
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PRU.[Year] and P.[Number] = PRU.[Number]
WHERE PRU.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolMessage] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PM SET PM.UniqueIdProtocol = P.UniqueId,
			  PM.RegistrationDate = P.RegistrationDate,
			  PM.RegistrationUser = P.RegistrationUser,
			  PM.LastChangedDate = P.LastChangedDate,
			  PM.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolMessage] AS PM
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PM.[Year] and P.[Number] = PM.[Number]
WHERE PM.UniqueIdProtocol IS NULL

--#############################################################################
UPDATE [dbo].[ProtocolLinks] SET [UniqueId] = NEWID() WHERE [UniqueId] IS NULL

UPDATE PL SET PL.UniqueIdProtocolParent = P.UniqueId,
			  PL.RegistrationDate = P.RegistrationDate,
			  PL.RegistrationUser = P.RegistrationUser,
			  PL.LastChangedDate = P.LastChangedDate,
			  PL.LastChangedUser = P.LastChangedUser
FROM [dbo].[ProtocolLinks] AS PL
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PL.[Year] and P.[Number] = PL.[Number]
WHERE PL.UniqueIdProtocolParent IS NULL

UPDATE PS SET PS.UniqueIdProtocolSon = P.UniqueId
FROM [dbo].[ProtocolLinks] AS PS
INNER JOIN [dbo].[Protocol] P ON P.[Year] = PS.[YearSon] and P.[Number] = PS.[NumberSon]
WHERE PS.UniqueIdProtocolSon IS NULL

--#############################################################################
ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolNumberOut] 

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolNumberIn] 

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolYearOut] 

ALTER TABLE [dbo].[PECMail] DROP COLUMN [ProtocolYearIn] 

--ALTER TABLE [dbo].[ProtocolContact] DROP COLUMN [Incremental]

--ALTER TABLE [dbo].[Container] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

--ALTER TABLE [dbo].[Container] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL

--ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

--ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL

--ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL

--ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL

ALTER TABLE [dbo].[Protocol] ADD [Timestamp] TIMESTAMP not null

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [UniqueIdProtocol] uniqueidentifier not null

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolContact] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolContact] ADD [Timestamp] TIMESTAMP not null

CREATE UNIQUE INDEX [IX_ProtocolContact_UniqueId] ON [dbo].[ProtocolContact]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolRole] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolRole] ADD [Timestamp] TIMESTAMP not null

CREATE UNIQUE INDEX [IX_ProtocolRole_UniqueId] ON [dbo].[ProtocolRole]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolParer] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolParer] ADD [Timestamp] TIMESTAMP not null

CREATE UNIQUE INDEX [IX_ProtocolParer_UniqueId] ON [dbo].[ProtocolParer]([UniqueId] ASC);


ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolContactManual] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolContactManual] ADD [Timestamp] TIMESTAMP not null


CREATE UNIQUE INDEX [IX_ProtocolContactManual_UniqueId] ON [dbo].[ProtocolContactManual]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [UniqueId] [uniqueidentifier] not null


ALTER TABLE [dbo].[ProtocolLog] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

CREATE UNIQUE INDEX [IX_ProtocolLog_UniqueId] ON [dbo].[ProtocolLog]([UniqueId] ASC);


ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolRoleUser] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolRoleUser] ADD [Timestamp] TIMESTAMP not null

CREATE UNIQUE INDEX [IX_ProtocolRoleUser_UniqueId] ON [dbo].[ProtocolRoleUser]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [UniqueIdProtocol] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolMessage] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolMessage] ADD [Timestamp] TIMESTAMP not null

CREATE UNIQUE INDEX [IX_ProtocolMessage_UniqueId] ON [dbo].[ProtocolMessage]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolHighlightUsers] ADD [Timestamp] TIMESTAMP not null

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueId] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueIdProtocolParent] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [UniqueIdProtocolSon] [uniqueidentifier] not null

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [RegistrationDate] datetimeoffset(7) not null

ALTER TABLE [dbo].[ProtocolLinks] ALTER COLUMN [RegistrationUser] nvarchar(256) not null

ALTER TABLE [dbo].[ProtocolLinks] ADD [Timestamp] TIMESTAMP not null


CREATE UNIQUE INDEX [IX_ProtocolLinks_UniqueId] ON [dbo].[ProtocolLinks]([UniqueId] ASC);

ALTER TABLE [dbo].[ProtocolContact] WITH CHECK ADD CONSTRAINT [FK_ProtocolContact_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])


ALTER TABLE [dbo].[ProtocolContact] CHECK CONSTRAINT [FK_ProtocolContact_Protocol_UniqueId]


ALTER TABLE [dbo].[ProtocolContactManual] WITH CHECK ADD CONSTRAINT [FK_ProtocolContactManual_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])


ALTER TABLE [dbo].[ProtocolContactManual] CHECK CONSTRAINT [FK_ProtocolContactManual_Protocol_UniqueId]


ALTER TABLE [dbo].[ProtocolRole] WITH CHECK ADD CONSTRAINT [FK_ProtocolRole_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])


ALTER TABLE [dbo].[ProtocolRole] CHECK CONSTRAINT [FK_ProtocolRole_Protocol_UniqueId]

ALTER TABLE [dbo].[ProtocolRoleUser] WITH CHECK ADD CONSTRAINT [FK_ProtocolRoleUser_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])


ALTER TABLE [dbo].[ProtocolRoleUser] CHECK CONSTRAINT [FK_ProtocolRoleUser_Protocol_UniqueId]

ALTER TABLE [dbo].[ProtocolParer] WITH CHECK ADD CONSTRAINT [FK_ProtocolParer_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol] ([UniqueId])


ALTER TABLE [dbo].[ProtocolParer] CHECK CONSTRAINT [FK_ProtocolParer_Protocol_UniqueId]


ALTER TABLE [dbo].[ProtocolLinks] WITH CHECK ADD CONSTRAINT [FK_ProtocolLinks_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocolParent]) 
REFERENCES [dbo].[Protocol] ([UniqueId])

ALTER TABLE [dbo].[ProtocolLinks] CHECK CONSTRAINT [FK_ProtocolLinks_Protocol_UniqueId]

ALTER TABLE [dbo].[ProtocolLinks] WITH CHECK ADD CONSTRAINT [FK_ProtocolLinks_Protocol_Son_UniqueId] FOREIGN KEY ([UniqueIdProtocolSon]) 
REFERENCES [dbo].[Protocol] ([UniqueId])

ALTER TABLE [dbo].[ProtocolLinks] CHECK CONSTRAINT [FK_ProtocolLinks_Protocol_Son_UniqueId]

ALTER TABLE [dbo].[ProtocolMessage] WITH CHECK ADD CONSTRAINT [FK_ProtocolMessage_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol] ([UniqueId])

ALTER TABLE [dbo].[ProtocolMessage] CHECK CONSTRAINT [FK_ProtocolMessage_Protocol_UniqueId]

ALTER TABLE [dbo].[ProtocolLog] WITH CHECK ADD CONSTRAINT [FK_ProtocolLog_Protocol_UniqueId] FOREIGN KEY ([UniqueIdProtocol]) 
REFERENCES [dbo].[Protocol]([UniqueId])

ALTER TABLE [dbo].[ProtocolLog] CHECK CONSTRAINT [FK_ProtocolLog_Protocol_UniqueId]
