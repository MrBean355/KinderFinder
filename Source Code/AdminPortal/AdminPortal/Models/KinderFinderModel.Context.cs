﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdminPortal.Models {
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Infrastructure;

	public partial class KinderFinderEntities : DbContext, IKinderFinderContext {
		public KinderFinderEntities()
			: base("name=KinderFinderEntities") {
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			throw new UnintentionalCodeFirstException();
		}

		public virtual DbSet<Map> Maps { get; set; }
		public virtual DbSet<Patron> Patrons { get; set; }
		public virtual DbSet<Tag> Tags { get; set; }

		public void MarkAsModified(Tag item) {
			Entry(item).State = EntityState.Modified;
		}

		public void MarkAsModified(Patron item) {
			Entry(item).State = EntityState.Modified;
		}

		public void MarkAsModified(Map item) {
			Entry(item).State = EntityState.Modified;
		}
	}
}
