/* Use this query to get the list of BCM custom user fields*/
SELECT UFD.[FieldGUID]
      , UFD.[FieldName]
	  , UFD.DataType as DataTypeID
      , UFDT.[DataTypeName]
	  ,EUF.UserFieldIndex
	  , EUF.EntityType as EntityTypeID
	  , ETT.EntityTypeName
  FROM [dbo].[UserFieldDefinitions] UFD
  INNER JOIN [dbo].[UserFieldDataTypes] UFDT on UFD.DataType = UFDT.DataTypeID
  INNER JOIN [dbo].[EntityUserFields] EUF on EUF.FieldGUID = UFD.FieldGUID
  INNER JOIN [dbo].[EntityTypesTable] ETT on EUF.EntityType = ETT.EntityTypeID
  WHERE DataTableID = 0
  -- 0=UserFields, 1=EntityReferences, 2=ContactAttachments, 3=LeadScoringFieldsTable, 4=SalesActivityFieldsTable
