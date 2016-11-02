using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace ChatApp.Common
{
    public class MySession : IReadOnlySessionState 
    {
        private MySession()
        {
            UserID = 0;
            Name = "";
            ProfilePicture = "";
            Gender = "";
        }

        // Gets the current session.
        public static MySession Current
        {
            get
            {
                MySession session =
                  (MySession)HttpContext.Current.Session["__MySession__"];
                if (session == null)
                {
                    session = new MySession();
                    HttpContext.Current.Session["__MySession__"] = session;
                }
                return session;
            }
        }

        // **** add your session properties here, e.g like this:
        public int UserID { get; set; }
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public string Gender { get; set; }
    }
}