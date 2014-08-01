using AdminPortal.Controllers.Web_API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

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

		[TestMethod]
		public void GetFreeTagsTest() {
			var controller = new FreeTagListController(new TestKinderFinderContext());

			List<string> result = controller.GetFreeTags() as List<string>;
			List<string> expected = new List<string>();
			expected.Add("Tag1");

			CollectionAssert.AreEqual(expected, result);
		}

		[TestMethod]
		public void LinkTagTest_Success() {
			var controller = new LinkTagController(new TestKinderFinderContext());
			var details = new LinkTagController.RequestDetails();

			details.EmailAddress = "test@system.com";
			details.TagLabel = "Tag1";

			var result = controller.LinkTag(details);

			Assert.IsInstanceOfType(result, typeof(OkResult));
		}

		[TestMethod]
		public void LinkTagTest_Fail() {
			var controller = new LinkTagController(new TestKinderFinderContext());
			var details = new LinkTagController.RequestDetails();

			details.EmailAddress = "test@system.com";
			details.TagLabel = "Tag2";

			var result = controller.LinkTag(details);

			Assert.IsInstanceOfType(result, typeof(BadRequestResult));
		}

		[TestMethod]
		public void UnlinkTagTest_Success() {
			var controller = new UnlinkTagController(new TestKinderFinderContext());
			var details = new UnlinkTagController.RequestDetails();

			details.EmailAddress = "test@system.com";
			details.TagLabel = "Tag2";

			var result = controller.UnlinkTag(details);

			Assert.IsInstanceOfType(result, typeof(OkResult));
		}

		[TestMethod]
		public void UnlinkTagTest_Fail() {
			var controller = new UnlinkTagController(new TestKinderFinderContext());
			var details = new UnlinkTagController.RequestDetails();

			details.EmailAddress = "test@system.com";
			details.TagLabel = "Tag1";

			var result = controller.UnlinkTag(details);

			Assert.IsInstanceOfType(result, typeof(BadRequestResult));
		}
	}
}
