using System;
using System.Collections.Generic;

using KinderFinder_Droid;

using NUnit.Framework;

namespace KinderFinder_DroidTests {

	[TestFixture]
	public class TestUtility {
		const int PASSWORD_HASH_LENGTH = 44;

		[Test]
		public void ParseJSONTest1() {
			string json = "[\"Tag1\",\"Tag2\",\"Tag3\"]";
			var result = Utility.ParseJSON(json);
			var expected = new List<string>();
			expected.Add("Tag1");
			expected.Add("Tag2");
			expected.Add("Tag3");

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseJSONTest2() {
			string json = "[]";
			var result = Utility.ParseJSON(json);
			Assert.AreEqual(new List<string>(), result);
		}

		[Test]
		public void IsValidEmailAddressTest1() {
			string email = "mrbean355@gmail.com";
			var result = Utility.IsValidEmailAddress(email);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void IsValidEmailAddressTest2() {
			string email = "mrbean355@com";
			var result = Utility.IsValidEmailAddress(email);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void HashPasswordTest() {
			string password = "password";
			var result = Utility.HashPassword(password);
			Assert.AreEqual(PASSWORD_HASH_LENGTH, result.Length);
		}
	}
}

