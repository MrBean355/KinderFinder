using AdminPortal.Models;
using System.Collections.Generic;

namespace AdminPortal.Models
{
    public class AdminIndexData
    {
        public IEnumerable<AspNetUser> AspNetUsers { get; set; }
        public IEnumerable<Restaurant> Restaurants { get; set; }        
    }

    public class AssignedRestaurantData
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}