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
    
    public partial class ChangeQueue
    {
        public int ID { get; set; }
        public Nullable<System.Guid> EntryGUID { get; set; }
        public int EntityID { get; set; }
        public int ViewRowID { get; set; }
        public byte FolderType { get; set; }
        public byte ItemType { get; set; }
        public byte Operation { get; set; }
        public long Version { get; set; }
        public string ChangeUser { get; set; }
        public System.DateTime ChangeTime { get; set; }
    }
}
