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
    
    public partial class AccountsUpdatesTable
    {
        public System.Guid EntryGUID { get; set; }
        public Nullable<System.DateTime> UpdateStart { get; set; }
        public bool IsDeletedLocally { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string FullName { get; set; }
        public string WebPage { get; set; }
        public Nullable<bool> Active { get; set; }
        public string BusinessAddressStreet { get; set; }
        public string BusinessAddressCity { get; set; }
        public string BusinessAddressState { get; set; }
        public string BusinessAddressPostalCode { get; set; }
        public string BusinessAddressCountry { get; set; }
        public string OtherAddressStreet { get; set; }
        public string OtherAddressCity { get; set; }
        public string OtherAddressState { get; set; }
        public string OtherAddressPostalCode { get; set; }
        public string OtherAddressCountry { get; set; }
        public string Email1Address { get; set; }
        public string Email2Address { get; set; }
        public string Email3Address { get; set; }
        public string BusinessTelephoneNumber { get; set; }
        public string BusinessFaxNumber { get; set; }
        public string OtherTelephoneNumber { get; set; }
        public string OtherFaxNumber { get; set; }
        public Nullable<System.Guid> PrimaryContactGUID { get; set; }
    }
}