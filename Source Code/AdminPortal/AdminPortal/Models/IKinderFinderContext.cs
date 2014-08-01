using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Models {

	public interface IKinderFinderContext : IDisposable {
		DbSet<Tag> Tags { get; }
		DbSet<Patron> Patrons { get; }
		DbSet<Map> Maps { get; }

		int SaveChanges();

		void MarkAsModified(Tag item);
		void MarkAsModified(Patron item);
		void MarkAsModified(Map item);
	}
}
