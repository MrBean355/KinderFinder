using AdminPortal.Controllers;
using AdminPortal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;

namespace AdminPortalTests.Tests {

	[TestClass]
	public class TestRegisterController {

		[TestMethod]
		public void RegisterTest_Success() {
			var controller = new RegisterController(new TestKinderFinderContext());
			var patron = new Patron();

			patron.ID = 2;
			patron.FirstName = "Another";
			patron.Surname = "User";
			patron.EmailAddress = "something@system.com";
			patron.PasswordHash = "hijklmno";

			var result = controller.Register(patron);

			Assert.IsInstanceOfType(result, typeof(OkResult));
		}

		[TestMethod]
		public void RegisterTest_Fail() {
			var controller = new RegisterController(new TestKinderFinderContext());
			var patron = new Patron();

			patron.ID = 2;
			patron.FirstName = "Another";
			patron.Surname = "User";
			patron.EmailAddress = "test@system.com"; // email in use.
			patron.PasswordHash = "hijklmno";

			var result = controller.Register(patron);

			Assert.IsInstanceOfType(result, typeof(ConflictResult));
		}
	}
}
