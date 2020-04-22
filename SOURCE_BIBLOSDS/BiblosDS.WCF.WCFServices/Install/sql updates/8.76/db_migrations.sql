SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
--#############################################################################
PRINT N'Aggiunta vincolo unicità DefaultUser alla tabella [PreservationUser]';
GO

CREATE UNIQUE INDEX IX_PreservationUser_DefaultUser 
ON [dbo].[PreservationUser]([DefaultUser]) WHERE [DefaultUser] = 1
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Aggiunta indice [IX_Document_IsLatestVersion] alla tabella [Document]';
GO

CREATE NONCLUSTERED INDEX [IX_Document_IsLatestVersion] ON [dbo].[Document]
(
	[IsLatestVersion] ASC,
	[IsDetached] ASC
)
INCLUDE ([IdDocument],[IdParentVersion]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Creazione SQL function [AttributeValues_FX_FullDocumentAttributeValues]';
GO

CREATE FUNCTION [dbo].[AttributeValues_FX_FullDocumentAttributeValues] 
(	
	@IdDocument uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [AttributeValues_IdDocument],[AttributeValues_IdAttribute],[AttributeValues_ValueInt],[AttributeValues_ValueFloat],[AttributeValues_ValueDateTime],
	[AttributeValues_ValueString],

	[Attributes_IdAttribute],[Attributes_Name],[Attributes_IdArchive],[Attributes_IsRequired],[Attributes_KeyOrder],[Attributes_IdMode],
	[Attributes_IsMainDate],[Attributes_IsEnumerator],[Attributes_IsAutoInc],[Attributes_IsUnique],[Attributes_AttributeType],[Attributes_ConservationPosition],
	[Attributes_DefaultValue],[Attributes_MaxLenght],[Attributes_KeyFilter],[Attributes_KeyFormat],[Attributes_Validation],[Attributes_Format],[Attributes_IsChainAttribute],
	[Attributes_IdAttributeGroup],[Attributes_IsVisible],[Attributes_IsSectional],[Attributes_IsRequiredForPreservation],[Attributes_IsVisibleForUser],[Attributes_Description]	
	FROM 
	(
		SELECT [AttributesValue].[IdDocument] AS [AttributeValues_IdDocument],[AttributesValue].[IdAttribute] AS [AttributeValues_IdAttribute],
		[AttributesValue].[ValueInt] AS [AttributeValues_ValueInt],[AttributesValue].[ValueFloat] AS [AttributeValues_ValueFloat],
		[AttributesValue].[ValueDateTime] AS [AttributeValues_ValueDateTime],[AttributesValue].[ValueString] AS [AttributeValues_ValueString],

		[Attributes].[IdAttribute] AS [Attributes_IdAttribute],[Attributes].[Name] AS [Attributes_Name],[Attributes].[IdArchive] AS [Attributes_IdArchive]
       ,[Attributes].[IsRequired] AS [Attributes_IsRequired],[Attributes].[KeyOrder] AS [Attributes_KeyOrder],[Attributes].[IdMode] AS [Attributes_IdMode]
       ,[Attributes].[IsMainDate] AS [Attributes_IsMainDate],[Attributes].[IsEnumerator] AS [Attributes_IsEnumerator],[Attributes].[IsAutoInc] AS [Attributes_IsAutoInc]
       ,[Attributes].[IsUnique] AS [Attributes_IsUnique],[Attributes].[AttributeType] AS [Attributes_AttributeType],[Attributes].[ConservationPosition] AS [Attributes_ConservationPosition]
       ,[Attributes].[DefaultValue] AS [Attributes_DefaultValue],[Attributes].[MaxLenght] AS [Attributes_MaxLenght],[Attributes].[KeyFilter] AS [Attributes_KeyFilter]
       ,[Attributes].[KeyFormat] AS [Attributes_KeyFormat],[Attributes].[Validation] AS [Attributes_Validation],[Attributes].[Format] AS [Attributes_Format]
       ,[Attributes].[IsChainAttribute] AS [Attributes_IsChainAttribute],[Attributes].[IdAttributeGroup] AS [Attributes_IdAttributeGroup],[Attributes].[IsVisible] AS [Attributes_IsVisible]
       ,[Attributes].[IsSectional] AS [Attributes_IsSectional],[Attributes].[IsRequiredForPreservation] AS [Attributes_IsRequiredForPreservation],[Attributes].[IsVisibleForUser] AS [Attributes_IsVisibleForUser]
       ,[Attributes].[Description] AS [Attributes_Description]
	   FROM [AttributesValue]
	   INNER JOIN [Attributes] ON [Attributes].[IdAttribute] = [AttributesValue].[IdAttribute]
	   INNER JOIN [Document] ON [Document].[IdDocument] = [AttributesValue].[IdDocument]
	   WHERE [Attributes].[IsChainAttribute] = 1
	   AND [Document].[IsLatestVersion] = 1 AND ([Document].[IsDetached] IS NULL OR [Document].[IsDetached] = 0)
	   AND ([Document].[IdDocument] = @IdDocument OR [Document].[IdParentVersion] = @IdDocument)			
	) tbl
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
	PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update FAILED.'
GO
DROP TABLE #tmpErrors
GO