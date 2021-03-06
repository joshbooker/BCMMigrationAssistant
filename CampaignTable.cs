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
    
    public partial class CampaignTable
    {
        public int ActivityID { get; set; }
        public string MessageClass { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignType { get; set; }
        public Nullable<decimal> BudgetedCost { get; set; }
        public string DeliveryMethod { get; set; }
        public Nullable<bool> EmailTracking { get; set; }
        public Nullable<int> RecipientCount { get; set; }
        public string ContentFile { get; set; }
        public string ContentSubject { get; set; }
        public Nullable<byte> Executed { get; set; }
        public Nullable<decimal> ActualCost { get; set; }
        public Nullable<System.DateTime> CompletionDate { get; set; }
        public Nullable<int> FormQuerySelection { get; set; }
        public Nullable<System.DateTime> ExecutedOn { get; set; }
        public string CampaignContent { get; set; }
        public string CampaignStatus { get; set; }
        public string CampaignLabel { get; set; }
        public Nullable<int> NumberLeads { get; set; }
        public Nullable<int> NumberContacts { get; set; }
        public Nullable<int> NumberOpportunities { get; set; }
        public Nullable<int> NumberAccounts { get; set; }
        public Nullable<decimal> ExpectedRevenue { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> NumberPhoneLogs { get; set; }
        public string WorkPaneData { get; set; }
        public Nullable<int> IrisSubType { get; set; }
        public string CallListBody { get; set; }
    
        public virtual ActivitiesTable ActivitiesTable { get; set; }
    }
}
