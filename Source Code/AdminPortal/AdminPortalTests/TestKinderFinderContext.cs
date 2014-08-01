using AdminPortal.Models;
using System;
using System.Data.Entity;

namespace AdminPortalTests {

	class TestKinderFinderContext : IKinderFinderContext {
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Patron> Patrons { get; set; }
		public DbSet<Map> Maps { get; set; }

		public TestKinderFinderContext() {
			Tags = new TestTagDbSet();
			Patrons = new TestPatronDbSet();
			Maps = new TestMapDbSet();

			Populate();
		}

		void Populate() {
			Patron test = new Patron();
			test.ID = 1;
			test.FirstName = "Test";
			test.Surname = "User";
			test.EmailAddress = "test@system.com";
			test.PasswordHash = "abcdefg";

			Patrons.Add(test);

			Tag tag1 = new Tag();
			Tag tag2 = new Tag();
			tag1.Label = "Tag1";
			tag2.Label = "Tag2";
			tag2.CurrentPatron = 1;

			Tags.Add(tag1);
			Tags.Add(tag2);
		}

		public int SaveChanges() {
			return 0;
		}

		public void MarkAsModified(Tag item) { }
		public void MarkAsModified(Patron item) { }
		public void MarkAsModified(Map item) { }
		public void Dispose() { }
	}
}
