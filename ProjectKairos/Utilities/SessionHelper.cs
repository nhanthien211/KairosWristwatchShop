using System.Web;
using Newtonsoft.Json.Linq;

namespace ProjectKairos.Utilities
{
    public static class SessionHelper
    {
        public static string GetCurrentUserInfo(this HttpSessionStateBase session, string key)
        {
            string currentUser = (string)session["CURRENT_USER_ID"];
            if (currentUser == null)
            {
                return null;
            }
            JObject user = JObject.Parse(currentUser);
            string info = (string)user[key];
            return info;
        }
    }
}