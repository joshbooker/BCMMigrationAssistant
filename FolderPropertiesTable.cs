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
    
    public partial class FolderPropertiesTable
    {
        public int FolderID { get; set; }
        public Nullable<System.Guid> FolderGUID { get; set; }
        public byte[] CurrentView { get; set; }
        public byte[] ExtendedFolderFlags { get; set; }
        public int FolderViewFlags { get; set; }
        public bool FolderViewInitialized { get; set; }
        public string FolderName { get; set; }
        public string FolderComments { get; set; }
        public string FolderQuery { get; set; }
        public string ContainerClass { get; set; }
        public Nullable<long> Version { get; set; }
        public byte[] WorkPaneData { get; set; }
    }
}