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
    
    public partial class Transmitter
    {
        public int ID { get; set; }
        public Nullable<int> Restaurant { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<double> PosX { get; set; }
        public Nullable<double> PosY { get; set; }
    
        public virtual Restaurant Restaurant1 { get; set; }
    }
}
