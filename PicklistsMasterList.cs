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
    
    public partial class PicklistsMasterList
    {
        public System.Guid PicklistValueGUID { get; set; }
        public System.Guid PicklistID { get; set; }
        public short OrderID { get; set; }
        public string StringValue { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Picklist Picklist { get; set; }
    }
}
