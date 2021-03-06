
/* Getting User Custom Field Values*/

-- STEP 1: Obtain FieldIndex and Datatype for the custom field 
SELECT UFD.[FieldGUID]
      , UFD.[FieldName]
	  , UFD.DataType as DataTypeID		-- This is the datatype
      , UFDT.[DataTypeName]
	  ,EUF.UserFieldIndex				-- This is the field index
	  , EUF.EntityType as EntityTypeID
	  , ETT.EntityTypeName
  FROM   [dbo].[UserFieldDefinitions] UFD
  INNER JOIN  [dbo].[UserFieldDataTypes] UFDT on UFD.DataType = UFDT.DataTypeID
  INNER JOIN  [dbo].[EntityUserFields] EUF on EUF.FieldGUID = UFD.FieldGUID
  INNER JOIN  [dbo].[EntityTypesTable] ETT on EUF.EntityType = ETT.EntityTypeID
  WHERE DataTableID = 0
  -- 0=UserFields, 1=EntityReferences, 2=ContactAttachments, 3=LeadScoringFieldsTable, 4=SalesActivityFieldsTable
  /* FOR EXAMPLE
	FieldName				DataTypeName	UserFieldIndex
	Custom Code				Number			3
	TrainingDate			DateTime		3
	CustomOppFieldText		Text			1
	*/

  -- STEP 2: Determine the contact service ID
  -- FOR EXAMPLE
  SELECT [ContactServiceID], [FullName]  FROM [dbo].[AccountsFullView]  WHERE IsDeletedLocally=0 AND FullName = 'Best Bikes'
  SELECT [ContactServiceID], [FullName]  FROM [dbo].[ContactFullView]  WHERE IsDeletedLocally=0 AND FullName = 'Mr. Tony L. Allen'
  SELECT [ContactServiceID], OpportunityName  FROM [dbo].[OpportunityFullView]  WHERE IsDeletedLocally=0 AND OpportunityName = 'Follow-up with Oscar regarding Mountain bikes'
  /* RESULTS
  ContactServiceID	Name
  134				Best Bikes
  228				Mr. Tony Allen
  65				Follow-up with Oscar regarding Mountain Bikes
  */

  -- STE3: Obtain the varbinary value from the UserField column (using the index value) then convert from varbinary to the relevant data type (VARCHAR, NUMERIC, DATE)
  -- FOR EXAMPLE: 
   SELECT cast(cast(UserField3 as varbinary(512)) as NUMERIC) FROM [dbo].[UserFields] where contactserviceid = 134
   SELECT cast(cast(UserField3 as varbinary(512)) as DATETIME) FROM [dbo].[UserFields] where contactserviceid = 228
   SELECT cast(cast(UserField1 as varbinary(512)) as NVARCHAR(256)) FROM [dbo].[UserFields] where contactserviceid = 65
   
    

   
