using System.ComponentModel.DataAnnotations;

namespace AdminPortal.Models {
	/*public class ExternalLoginConfirmationViewModel {
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class ExternalLoginListViewModel {
		public string Action { get; set; }
		public string ReturnUrl { get; set; }
	}*/

	public class ManageUserViewModel {
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginViewModel {
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9-_]*$")]
		/* Usernames may only contain letters, numbers, dashes and underscores. */
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel {
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9-_]*$")]
		/* Usernames may only contain letters, numbers, dashes and underscores. */
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	/**
	 * TODO: Implement password resetting.
	 */

	public class ResetPasswordViewModel {
		/*[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }*/
	}

	public class ForgotPasswordViewModel {
		/*[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9-_]*$")]
		/* Usernames may only contain letters, numbers, dashes and underscores. *
		[Required]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		[RegularExpression(@"^[a-zA-Z0-9-_]*$")]
		/* Usernames may only contain letters, numbers, dashes and underscores. *
		public string Username { get; set; }
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }*/
	}
}
