using AdminPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortalTests {

	class TestPatronDbSet : TestDbSet<Patron> {
		public override Patron Find(params object[] keyValues) {
			return this.SingleOrDefault(item => item.ID == (int)keyValues.Single());
		}
	}
}
