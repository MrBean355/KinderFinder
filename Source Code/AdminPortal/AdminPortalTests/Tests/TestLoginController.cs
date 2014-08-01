using AdminPortal.Controllers.Web_API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;

namespace AdminPortalTests.Tests {

	[TestClass]
	public class TestLoginController {

		[TestMethod]
		public void LoginTest_Success() {
			var controller = new LoginController(new TestKinderFinderContext());

			var details = new LoginController.LoginDetails();
			details.EmailAddress = "test@system.com";
			details.PasswordHash = "abcdefg";

			var result = controller.Login(details);

			Assert.IsInstanceOfType(result, typeof(OkResult));
		}

		[TestMethod]
		public void LoginTest_Fail() {
			var controller = new LoginController(new TestKinderFinderContext());
			var details = new LoginController.LoginDetails();

			details.EmailAddress = "test@system.com";
			details.PasswordHash = "badhash";

			var result = controller.Login(details);
			Assert.IsInstanceOfType(result, typeof(BadRequestResult));
		}
	}
}
