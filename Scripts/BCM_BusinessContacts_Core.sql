/*
Use this query to get the list of business contact, its OOB field values and link to company
*/
SELECT 
	    CFV.[ContactServiceID]
      , CFV.[Type]
      --,[ParentContactServiceID]
      , CFV.[EntryGUID]
      --,[ParentEntryID]
	  , CFV.[FirstName]					-- [OCM] First Name
      , CFV.[LastName]					-- [OCM] Last Name
      , CFV.[MiddleName]				-- [OCM] Middle Name
      , CFV.[FullName]					-- [OCM] Full Name
      , CFV.[JobTitle]					-- [OCM] Job Title
      , CFV.[Profession]				-- <type=text>
      --, CFV.[CompanyName]				-- [OCM] Link to Company Name
	   , AFV.[FullName]	As CompanyName	-- [OCM] Link to Company Name *** (changed mapping)
      , CFV.[Department]				-- <type=text>
      , CFV.[Birthday]					-- [OCM] Birthday
      , CFV.[WeddingAnniversary]		-- <type=date>
      , CFV.[ManagerName]				-- <type=text>
      , CFV.[AssistantName]				-- <type=text>
      , CFV.[PrefContactMethod]			-- <type=text>
      , CFV.[DoNotEmail]				-- <type=text>
      , CFV.[DoNotFax]					-- <type=text>
      , CFV.[DoNotCall]					-- <type=text>
      , CFV.[DoNotSendLetter]			-- <type=text>
      , CFV.[PaymentStatus]				-- <type=text>
      , CFV.[Rating]					-- <type=text>
      , CFV.[Active]					-- <type=text>
      , CFV.[AssignedTo]				-- <type=text>
      , CFV.[OfficeLocation]			-- <type=text>
      , CFV.[AreaOfInterest]			-- <type=text>
      , CFV.[ReferredBy]				-- <type=text>
      , CFV.[LeadSource]				-- <type=text>
      , CFV.[AreaOfInterest]			-- <type=text>
      --, CFV.[LeadScore]					-- <type=text> Not in older versions of DB
      , CFV.[WorkAddressStreet]			-- [OCM] Business Address Street
      , CFV.[WorkAddressPOB]			-- <type=text>
      , CFV.[WorkAddressCity]			-- [OCM] Business Address City
      , CFV.[WorkAddressState]			-- [OCM] Business Address State
      , CFV.[WorkAddressZip]			-- [OCM] Business Address Postal Code
      , CFV.[WorkAddressCountry]		-- [OCM] Business Address Country
      , CFV.[Email1Address]				-- [OCM] Email Address
      , CFV.[HomePhoneNum]				-- [OCM] Home Phone
      , CFV.[WorkPhoneNum]				-- [OCM] Business Phone *** (changed mapping)
      , CFV.[MobilePhoneNum]			-- [OCM] Mobile Phone
      , CFV.[ContactNotes]				-- [OCM] Notes
      , CFV.[Children]					-- <type=text>
      , CFV.[Hobby]						-- <type=text>
      , CFV.[Spouse]					-- <type=text>
  FROM [dbo].[ContactFullView] CFV
  LEFT JOIN dbo.AccountsFullView AFV on AFV.EntryGUID = CFV.ParentEntryID
  WHERE CFV.IsDeletedLocally = 0
  AND CFV.Type=1 -- 1= Business Contact, 2 = Accounts, 3 = Opportunities