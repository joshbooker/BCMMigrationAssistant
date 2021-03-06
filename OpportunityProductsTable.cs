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
    
    public partial class OpportunityProductsTable
    {
        public long ID { get; set; }
        public int ContactServiceID { get; set; }
        public System.Guid ProductLineGUID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> AdjustedPrice { get; set; }
        public Nullable<double> AdjustedPercent { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> ExtendedAmount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<double> DiscountPercent { get; set; }
        public Nullable<bool> Taxable { get; set; }
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<double> Markup { get; set; }
    
        public virtual ContactMainTable ContactMainTable { get; set; }
    }
}
