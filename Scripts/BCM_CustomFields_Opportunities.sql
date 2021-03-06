
/* Getting User Custom Field Values*/

-- Datatype Mapping table (BCM datatype and SQL Data type)
  DECLARE @DataTypeMap TABLE (DateTypeID TINYINT, DataTypeName NVARCHAR(32))
  INSERT INTO @DataTypeMap	VALUES	(1, 'NVARCHAR(MAX)'),	-- Text
									(2, 'NUMERIC(10,5)'),	-- Number
									(3, 'DECIMAL(5,2)'),	-- Percent
									(4, 'MONEY'),			-- Currency
									(5, 'BIT'),				-- Y/N
									(6, 'DATETIME'),		-- Date/time
									(7, 'BIGINT'),			-- Integer
									(8, 'NVARCHAR(MAX)'),	-- Picklist (text)
									(9, 'NVARCHAR(MAX)')	-- URL
-- Out of scope: (10) Reference, (11) Image, (12) Attachement

-- Prepare the variable to store the dynamic sql statement
DECLARE @SQLSTATEMENTS AS  NVARCHAR(4000) = N''

-- Obtain FieldIndex and Datatype for the custom field 
SELECT  @SQLSTATEMENTS = @SQLSTATEMENTS + N',cast(cast(UserField' + 
			CAST(EUF.UserFieldIndex AS NVARCHAR(8)) +
			N' as varbinary(512)) as ' +
			DM.DataTypeName +
			N') as [' +
			UFD.[FieldName] +
			N']'
  FROM   [dbo].[UserFieldDefinitions] UFD
  INNER JOIN  [dbo].[UserFieldDataTypes] UFDT on UFD.DataType = UFDT.DataTypeID
  INNER JOIN  [dbo].[EntityUserFields] EUF on EUF.FieldGUID = UFD.FieldGUID 
  INNER JOIN  @DataTypeMap DM on DM.DateTypeID = UFD.DataType
  WHERE DataTableID = 0		-- 0=UserFields, 1=EntityReferences, 2=ContactAttachments, 3=LeadScoringFieldsTable, 4=SalesActivityFieldsTable
	AND EUF.EntityType = 3	--  1=contact, 2=account, 3=opportunity
	 
-- Create SQL statement based on the list of the custom field for the entity
SET @SQLSTATEMENTS =	CASE WHEN 	@SQLSTATEMENTS <> N'' THEN
							N'SELECT FV.EntryGUID, FV.OpportunityName '+ 
							CASE WHEN @SQLSTATEMENTS IS NOT NULL THEN N', '+ RIGHT(@SQLSTATEMENTS,LEN(@SQLSTATEMENTS)-1) ELSE N'' END +
							N' FROM [dbo].[UserFields] UF JOIN [dbo].[OpportunityFullView] FV ON FV.[ContactServiceID]=UF.[ContactServiceID]' +
							N' WHERE FV.IsDeletedLocally=0'
						ELSE N'SELECT NULL as EntryGUID, NULL as OpportunityName' END
-- Execute the SQL Statement
EXECUTE sp_executesql @SQLSTATEMENTS
 