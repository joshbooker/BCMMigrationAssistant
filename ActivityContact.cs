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
    
    public partial class ActivityContact
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }
        public int ContactID { get; set; }
        public Nullable<long> LinkVersion { get; set; }
        public Nullable<long> LinkContentVersion { get; set; }
        public int ContactType { get; set; }
        public System.Guid ContactEntryID { get; set; }
        public Nullable<int> GrandParentContactServiceID { get; set; }
        public Nullable<int> GrandParentType { get; set; }
        public Nullable<System.Guid> GrandParentEntryID { get; set; }
        public Nullable<int> GreatGrandParentContactServiceID { get; set; }
        public Nullable<int> GreatGrandParentType { get; set; }
        public Nullable<System.Guid> GreatGrandParentEntryID { get; set; }
        public System.Guid LinkGUID { get; set; }
        public bool IsDeletedLocally { get; set; }
    
        public virtual ActivitiesTable ActivitiesTable { get; set; }
        public virtual ContactMainTable ContactMainTable { get; set; }
        public virtual ContactMainTable ContactMainTable1 { get; set; }
        public virtual ContactMainTable ContactMainTable2 { get; set; }
    }
}
