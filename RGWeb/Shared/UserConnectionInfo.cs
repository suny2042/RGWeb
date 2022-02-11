using RGWeb.DB.Common;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
    
namespace RGWeb.Shared
{
    public class UserConnectionInfo
    {
        /// <summary>
        /// The IP for the current session
        /// </summary>

        public string HOSPID { get; set; } = "";
        public string HOSPJM { get; set; } = "";
        public string USERID { get; set; } = "";
        public string USERNM { get; set; } = "";
        public string USERTP { get; set; } = "";
        public string USERIP { get; set; } = "";
        public string SERVTP { get; set; } = "";
        public string UserLanguage { get; set; } = "J"; // 기본은 일본어로.
        public bool Login { get; set; } = false;
        public bool MDIMode { get; set; } = false;      // 기본은 MDI모드 꺼져있는 것으로

        public int ClientWidth { get; set; } = 0;       // 클라이언트 브라우저의 너비와 높이 값
        public int ClientHeight { get; set; } = 0;

        //public DBService DBService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pLS">LocalStorage</param>
        public void UserLogout(ProtectedLocalStorage pLS)
        {
            pLS.DeleteAsync("cHOSPID");
            pLS.DeleteAsync("cHOSPJM");
            pLS.DeleteAsync("cUSERID");
            pLS.DeleteAsync("cUSERNM");
            pLS.DeleteAsync("cUSERTP");
            pLS.DeleteAsync("cUSERIP");
            pLS.DeleteAsync("cSERVTP");

            HOSPID = "";
            HOSPJM = "";
            USERID = "";
            USERNM = "";
            USERTP = "";
            USERIP = "";
            SERVTP = "";
        }

        //public string MSG(string pKey)
        //{
        //    if (UserLanguage == "J")
        //        return ComLib.MSG_J[pKey];
        //    else if (UserLanguage == "K")
        //        return ComLib.MSG_K[pKey];
        //    else
        //        return "";
        //}

        //public string MDI_MenuBar()
        //{
        //    if (MDIMode)
        //        return "";
        //    else
        //        return "fixed-bottom";
        //}
    }
}
