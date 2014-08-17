using AdminPortal.Controllers.Web_API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AdminPortalTests.Tests {

	[TestClass]
	public class TestTagListController {

		[TestMethod]
		public void GetTagsTest() {
			var controller = new TagListController(new TestKinderFinderContext());
			var details = new TagListController.RequestDetails();

			details.EmailAddress = "test@system.com";
			List<string> result = controller.GetTags(details) as List<string>;
			List<string> expected = new List<string>();
			expected.Add("Tag2");

			CollectionAssert.AreEqual(expected, result);
		}
	}
}
