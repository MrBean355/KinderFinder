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
        public Nullable<int> CurrentPatron { get; set; }
        public string Label { get; set; }
    
        public virtual Patron Patron { get; set; }
    }
}
