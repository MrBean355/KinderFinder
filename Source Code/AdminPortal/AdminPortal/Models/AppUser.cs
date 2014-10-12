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
    
    public partial class AppUser
    {
        public AppUser()
        {
            this.Tags = new HashSet<Tag>();
        }
    
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public Nullable<int> CurrentRestaurant { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
    
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual AppUserStat AppUserStat { get; set; }
    }
}
