//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BcmMigrationTool
{
    using System;
    using System.Collections.Generic;
    
    public partial class ActivityComputedPropertiesView
    {
        public int ID { get; set; }
        public System.Guid LinkGUID { get; set; }
        public int ActivityID { get; set; }
        public Nullable<bool> DeletedItemsSummaryItem { get; set; }
        public string MessageClass { get; set; }
        public int EntityTypeCode { get; set; }
        public int CoreEntityType { get; set; }
        public int FolderTypeCode { get; set; }
        public System.Guid ParentEntryGUID { get; set; }
        public Nullable<System.Guid> GrandParentEntryGUID { get; set; }
        public Nullable<System.Guid> GreatGrandParentEntryGUID { get; set; }
        public string EntryTypeLocalized { get; set; }
        public string EntryTypeLocaleIndependent { get; set; }
        public Nullable<int> StoreEntryID { get; set; }
        public string PrimaryContactName { get; set; }
        public string ParentDisplayName { get; set; }
        public Nullable<decimal> ExpectedRevenue { get; set; }
        public bool IsDeletedLocally { get; set; }
        public string NormalizedSubject { get; set; }
        public Nullable<bool> HasAttach { get; set; }
        public int ParentType { get; set; }
        public Nullable<int> GrandParentType { get; set; }
        public Nullable<int> GreatGrandParentType { get; set; }
        public Nullable<int> ReferredByType { get; set; }
    }
}
