//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdminPortal.Models {
	using System.ComponentModel.DataAnnotations;

	public partial class Map {
		public int ID { get; set; }

		[Display(Name = "Name")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		public string Name { get; set; }

		[Display(Name = "Data")]
		public byte[] Data { get; set; }

		[Display(Name = "Is Active")]
		public bool Active { get; set; }
	}
}
