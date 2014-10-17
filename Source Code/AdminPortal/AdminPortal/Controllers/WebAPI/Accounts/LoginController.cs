using AdminPortal.Models;

using System;
using System.Linq;
using System.Web.Http;

namespace AdminPortal.Controllers.WebAPI.Accounts {

	/**
	 * Attempts to log an app user in.
	 */
	public class LoginController : ApiController {
		private KinderFinderEntities db = new KinderFinderEntities();

		[HttpPost]
		public IHttpActionResult Login(LoginDetails details) {
			/* Find first user with matching email address. */
			var password = (from item in db.AppUsers
							where item.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)
							select item.PasswordHash).FirstOrDefault();

			/* Check whether passwords match. */
            if (password != null && password.Equals(details.PasswordHash))
            {
                AppUser appuser = db.AppUsers.Where(i => i.EmailAddress.Equals(details.EmailAddress, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                UpdateAppUserStats(appuser);
                return Ok();
            }

			return BadRequest();
		}

		public struct LoginDetails {
			public string EmailAddress, PasswordHash;
		}

        private void UpdateAppUserStats(AppUser appUserSelected) {
            AppUserStat stat = db.AppUserStats.Find(appUserSelected.ID);

            //when the AppUserStats database doesn't have the user
            if (stat == null)
            {
                stat = new AppUserStat();
                stat.AppUserID = appUserSelected.ID;
                stat.FullName = appUserSelected.FirstName + " " + appUserSelected.Surname;
                stat.LastRestaurant = appUserSelected.CurrentRestaurant;
                stat.LastVisit = DateTime.Now;
                stat.VisitCount = 1;
                db.AppUserStats.Add(stat);
            }
            else
            {
                //when the AppUserStat already has a record for the user
                if (stat.LastRestaurant.Equals(appUserSelected.CurrentRestaurant)) {
                    //increase VisitCount if the user goes to the same restaurant consecutively
                    stat.VisitCount = stat.VisitCount + 1;
                }
                else
                {
                    stat.VisitCount = 1;
					stat.LastRestaurant = appUserSelected.CurrentRestaurant;
                }
                stat.LastVisit = DateTime.Now;
                db.Entry(stat).State = System.Data.Entity.EntityState.Modified;
            }

			db.SaveChanges();
        }
	}
}