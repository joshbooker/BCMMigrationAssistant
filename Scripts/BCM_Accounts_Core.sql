/* This query is to get the list of contact and its out of the box BCM values
The suggested mapping should be used to auto map the field to OCM field
THe remaining should be additional fields (along with custom field that the user can optionally migrate)
*/
SELECT
	 ContactServiceID,		 
	EntryGUID
	,FullName				-- [OCM] Company Name
	,[WorkAddressStreet]	-- [OCM] Business Address Street
	,[WorkAddressCity]		-- [OCM] Business Address City
	,[WorkAddressState]		-- [OCM] Business Address State
	,[WorkAddressZip]		-- [OCM] Business Address Postal Code
	,[WorkAddressCountry]	-- [OCM] Business Address Country / Region
	,[WorkPhoneNum]		
	,[CompanyMainPhoneNum]	-- [OCM] Business Phone
	,[WebAddress]			-- [OCM] Web Page Address
	,[Email1Address]		-- [OCM] Email Address
	,[ContactNotes]			-- [OCM] Notes
	,LastAccessTime		
	,PrefContactMethod
	,DoNotCall
	,DoNotEmail
	,DoNotFax
	,DoNotSendLetter
	,AccountPaymentStatus
	,AccountRating
	,AccountTerritory
	,LeadSource
  FROM [dbo].[AccountsFullView]
  WHERE IsDeletedLocally = 0 and AccountActive = 1