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
    
    public partial class OrgTable
    {
        public short ID { get; set; }
        public string OrgName { get; set; }
        public int DBVersionMajor { get; set; }
        public int DBVersionMinor { get; set; }
        public int DBVersionBuildMajor { get; set; }
        public int DBVersionBuildMinor { get; set; }
        public long ClientDataVersion { get; set; }
        public long ClientEverythingChangedDataVersion { get; set; }
        public long ClientMapiSchemaVersion { get; set; }
        public Nullable<System.Guid> StoreUID { get; set; }
        public string LocaleID { get; set; }
        public byte DbType { get; set; }
        public int PublicDataVersion { get; set; }
        public Nullable<long> LastSyncVersion { get; set; }
        public long PicklistsVersion { get; set; }
        public bool BCMWasUpgraded { get; set; }
        public bool IsOfflineDB { get; set; }
        public bool IsExampleDB { get; set; }
        public string SharedServer { get; set; }
        public bool DisableBootCache { get; set; }
        public Nullable<System.DateTime> DateOfLastBackup { get; set; }
        public string UserWhoLastBackedUp { get; set; }
        public int SchemaCreationReplicaKey { get; set; }
        public long SchemaCreationTickCount { get; set; }
        public int SchemaChangeReplicaKey { get; set; }
        public long SchemaChangeTickCount { get; set; }
    }
}
