//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdminPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tag
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public Nullable<int> Restaurant { get; set; }
        public Nullable<int> CurrentUser { get; set; }
        public bool OutOfOrder { get; set; }
        public Nullable<System.DateTime> LastAccessed { get; set; }
        public string BeaconID { get; set; }
    
        public virtual AppUser AppUser { get; set; }
        public virtual Restaurant Restaurant1 { get; set; }
    }
}
