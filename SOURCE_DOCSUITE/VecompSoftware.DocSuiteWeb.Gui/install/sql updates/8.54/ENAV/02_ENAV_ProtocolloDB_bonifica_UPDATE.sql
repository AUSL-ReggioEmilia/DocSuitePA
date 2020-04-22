
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