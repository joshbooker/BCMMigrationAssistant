/*
Use this query to get all activities (across all entities - business contact, accounts, opportunities) in BCM to be migrated as OCM activity post/phone log
*/
SELECT
		AT.[ActivityID]
	   ,AC.ContactID
	   ,CMT.EntryGUID
	   ,CMT.Subject as EntityName
      ,AT.[ActivityGUID]
      ,AT.[Subject]
      ,AT.[ActivityNote]
      ,AT.[ActivityDate] as CreatedOn
      ,AT.[ActivityType] 
  FROM   [dbo].[ActivitiesTable] AT
  INNER JOIN  [dbo].[ActivityContacts] AC ON AT.ActivityID = AC.ActivityID
  INNER JOIN  [dbo].[ContactMainTable] CMT ON CMT.ContactServiceID = AC.ContactID
  WHERE 
	AT.IsDeletedLocally = 0 AND
	(AT.ActivityType = 14		-- BusinessNotes
	OR AT.ActivityType = 15		-- PhoneLog
	OR AT.ActivityType = 6		-- MeetingLog
	)