using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RGWeb.Shared;
using Microsoft.AspNetCore.Components;

namespace RGWeb.DB.Common
{
    public class ComLib
    {
        private const string mLangMode = "J";// 일본어모드

        private static JObject serversettingsJObject = JObject.Parse(File.ReadAllText("serversettings.json"));

        // System Error Log 작성, 12.23KDH추가
        public static void WriteErrorLog(UserConnectionInfo pUCI, string pLogMessage)
        {
            string lLogPath = null; // System.Configuration.ConfigurationManager.AppSettings["LogPath"];

            string lPath = lLogPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_Error.txt";
            string lHeader = string.Empty;
            if (pUCI.HOSPID != null && pUCI.USERID != null && pUCI.USERTP != null)
            {
                lHeader = "[" + pUCI.HOSPID + "|" + pUCI.USERID
                + "|" + pUCI.USERTP + "]";
            }

            using (StreamWriter sw = new StreamWriter(lPath, true))
            {
                sw.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + lHeader + pLogMessage);
            }

            // 공통Log
            lPath = lLogPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            using (StreamWriter sw = new StreamWriter(lPath, true))
            {
                sw.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":Error] " + lHeader + pLogMessage);
            }
        }

        // Log 작성12.23KDH추가
        public static void WriteLog(UserConnectionInfo pUCI, string pLogMessage)
        {
            string lLogWrite = null; // System.Configuration.ConfigurationManager.AppSettings["LogWrite"];
            string lLogPath = null; //System.Configuration.ConfigurationManager.AppSettings["LogPath"];

            string lHeader = string.Empty;

            if (lLogWrite == "Y")
            {
                if (pUCI.HOSPID != null && pUCI.USERID != null && pUCI.HOSPID != null)
                {
                    lHeader = "[" + pUCI.HOSPID + "|" + pUCI.USERID
                                  + "|" + pUCI.USERTP + "]";
                }

                string lPath = lLogPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_Log.txt";

                using (StreamWriter sw = new StreamWriter(lPath, true))
                {
                    sw.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "] " + lHeader + pLogMessage);
                }

                // 공통Log
                lPath = lLogPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                using (StreamWriter sw = new StreamWriter(lPath, true))
                {
                    sw.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":Log] " + lHeader + pLogMessage);
                }
            }
        }

        public static string fnNVL(object pObj, string pNullValue)
        {
            string lRtnValue = string.Empty;
            if (pObj == null || pObj.ToString() == "")
            {
                lRtnValue = pNullValue;
            }
            else
            {
                lRtnValue = pObj.ToString();
            }

            return lRtnValue;
        }

        public static string GetSYSDATE()
        {
            return DateTime.Today.ToString("yyyyMMdd");
        }
        public static string fnDateToStr(DateTime pDate)
        {
            string lDate = string.Empty;

            lDate = pDate.ToString("yyyyMMdd");

            return lDate;
        }
        public static string fnDateToTimeStr(object pDate)
        {
            if (pDate == null) return "";

            string lDate = string.Empty;

            lDate = ((DateTime)pDate).ToString("HHmm");

            return lDate;
        }

        public static DateTime fnStrToDate(string pDate)
        {
            DateTime lDate;

            lDate = DateTime.ParseExact(pDate, "yyyyMMdd", null);

            return lDate;
        }

        public static string fnStrToDispDate(string pDate)
        {
            string lDate;
            try
            {
                lDate = DateTime.ParseExact(pDate.Substring(0, 8), "yyyyMMdd", null).ToString("yyyy/MM/dd");
                return lDate;
            }
            catch
            {
                return pDate;
            }
        }

        // DateTime 시각을 날짜+시간으로 바꿔서 string 반환
        public static String fnDateTimeToStr(Object pDate)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", (DateTime)pDate);
        }

        // 2021.08.31 윤기선 시각을 yyyyMMddHHmmss 로 바꿔서 string 반환 (오라클 DB 컬럼에 적절)
        public static String fnDateTimeToStryyyyMMddHHmmss(Object pDate)
        {
            return string.Format("{0:yyyyMMddHHmmss}", (DateTime)pDate);
        }

        public static string GetPATNUM(string pPATNUM)
        {
            string lPATNUM = string.Empty;

            if (string.IsNullOrEmpty(pPATNUM) != true)
            {
                lPATNUM = pPATNUM;

                if (lPATNUM.Length != 9)
                {
                    lPATNUM = lPATNUM.PadLeft(9, '0');
                }
            }

            return lPATNUM;
        }


        // 클라이언트 IP
        //public static string GetUserIP(HttpRequest pRequest)
        //{
        //    string ipList = pRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ipList))
        //    {
        //        return ipList.Split(',')[0];
        //    }

        //    return pRequest.ServerVariables["REMOTE_ADDR"];
        //}




        /*
        //
        public static DataTable ConvertJsonToDataTableForSave(string pJsonStr)
        {
            // object[]로 리턴됨.
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            object[] lJSONData = (object[])jsSerializer.DeserializeObject(pJsonStr);

            // DataTable 정의
            object[] lColumnList = (object[])lJSONData[0];
            DataTable ldtTemp = new DataTable();
            for (int i = 0; i < lColumnList.Length; i++)
                ldtTemp.Columns.Add(new DataColumn(lColumnList[i].ToString(), typeof(string)));

            // DataRow 삽입[0번째 Row는 Colunm 목록이므로, Skip]
            for (int i = 1; i < lJSONData.Length; i++)
            {
                object[] lRowData = (object[])lJSONData[i];

                DataRow lRow = ldtTemp.NewRow();
                for (int j = 0; j < lColumnList.Length; j++)
                {
                    string lColumn = lColumnList[j].ToString();
                    lRow[lColumn] = fnNVL(lRowData[j], string.Empty);
                }
                //ldtTemp.ImportRow(lRow);
                ldtTemp.Rows.Add(lRow);
            }

            return ldtTemp;
        }
        
        */

        public static Dictionary<string, string> MSG_J = new Dictionary<string, string>();
        public static Dictionary<string, string> MSG_K = new Dictionary<string, string>();
        // XML메세지
        public static void GetMessageInfo()
        {
            MSG_J.Clear();
            MSG_K.Clear();

            // XML Load
            string lXmlPath = System.IO.Directory.GetCurrentDirectory() + @"\Shared\String.xml";

            StreamReader lStream = null;
            if (!File.Exists(lXmlPath))
            {
                //WriteLog("GetMessageInfo - File Not Found [" + lXmlPath + "]");
                return;
            }

            try
            {
                lStream = new StreamReader(lXmlPath);
                if (lStream == null)
                    throw new Exception("GetMessageInfo - Stream Open Failed");

                // xml 데이터를 읽음.
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(lStream);

                XmlAttribute idAttr = null, koAttr = null, jpAttr = null;

                foreach (XmlNode node in xmlDoc.DocumentElement)
                {
                    //field Load
                    if (node.Name == "msgs")  //Field Load
                    {
                        foreach (XmlNode sNode in node.ChildNodes)
                        {
                            idAttr = sNode.Attributes["ID"];  //ID
                            jpAttr = sNode.Attributes["J"];  //일본어
                            koAttr = sNode.Attributes["K"];  //한글

                            if (string.IsNullOrEmpty(jpAttr.Value) != true)
                            {
                                MSG_J.Add(idAttr.Value, jpAttr.Value);
                            }
                            if (string.IsNullOrEmpty(koAttr.Value) != true)
                            {
                                MSG_K.Add(idAttr.Value, koAttr.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //WriteLog("GetMessageInfo - Error [" + ex.Message + "]");
                return;
            }
            finally
            {
                if (lStream != null)
                    lStream.Close();
            }

            return;
        }

        
        

        /// <summary>
        /// 년,월,시간,분 Combo
        /// </summary>
        /// <param name="pCODETP">년:YEAR / 월:MONTH / 시간:HOUR / 분:MINUTE</param>
        /// <returns></returns>
        public static DataTable GetDateComboList(string pCODETP)
        {
            DataTable lCombo = new DataTable();

            lCombo.Columns.Add("DTCODE", System.Type.GetType("System.String"));
            lCombo.Columns.Add("CODENM", System.Type.GetType("System.String"));

            DataRow lNewRow;

            switch (pCODETP)
            {
                case "YEAR":
                    // 올해부터 10년뒤까지 // 2022.01.10 윤기선 2020년부터 올해의 +10년까지로
                    int lYEARstart = 2020;
                    int lTEARend = int.Parse(DateTime.Today.AddYears(-1).ToString("yyyy")) + 10;
                    for (int i = lYEARstart; i < lTEARend; i++)
                    {
                        lNewRow = lCombo.NewRow();
                        lNewRow["DTCODE"] = i.ToString();
                        lNewRow["CODENM"] = i.ToString();

                        lCombo.Rows.Add(lNewRow);
                    }
                    break;
                case "MONTH":
                    // 01월 부터 12월까지
                    for (int i = 1; i <= 12; i++)
                    {
                        lNewRow = lCombo.NewRow();
                        lNewRow["DTCODE"] = i.ToString().PadLeft(2, '0');
                        lNewRow["CODENM"] = i.ToString().PadLeft(2, '0');

                        lCombo.Rows.Add(lNewRow);
                    }
                    break;
                case "HOUR":
                    // 00부터 23까지
                    for (int i = 0; i < 24; i++)
                    {
                        lNewRow = lCombo.NewRow();
                        lNewRow["DTCODE"] = i.ToString().PadLeft(2, '0');
                        lNewRow["CODENM"] = i.ToString().PadLeft(2, '0');

                        lCombo.Rows.Add(lNewRow);
                    }
                    break;

                case "MINUTE":
                    // 00부터 59까지
                    for (int i = 0; i < 60; i++)
                    {
                        lNewRow = lCombo.NewRow();
                        lNewRow["DTCODE"] = i.ToString().PadLeft(2, '0');
                        lNewRow["CODENM"] = i.ToString().PadLeft(2, '0');

                        lCombo.Rows.Add(lNewRow);
                    }
                    break;
            }

            return lCombo;
        }

        /*
        // 2021.04.22 윤기선 캘린더 내 모든 공휴일은 우리 DB의 공휴일을 따르게 설정.
        // BootstrapDateEdit 에서 CalendarDayCellPrepared 이벤트를 아래로 연결해야함. 아래 설정 추가
        // OnCalendarDayCellPrepared="dtp_Modal_CalendarDayCellPrepared" CalendarProperties-HighlightWeekends="False"
        public static void SetBootstrapDateEditHoliday_CalendarDayCellPrepared(object sender, DevExpress.Web.CalendarDayCellPreparedEventArgs e)
        {
            // 휴일이면 빨간색 표시
            if (Holiday.isHoliday(e.Date.ToString("yyyyMMdd")) == true)
            {
                e.Cell.ForeColor = System.Drawing.Color.Red;
            }
        }
        */

        //public static async Task<bool> UserVerify(IJSRuntime pJS, ProtectedLocalStorage pLS, NavigationManager pNM, UserConnectionInfo pUCI)
        //{
        //    if (pUCI == null || pUCI.Login == false || pUCI.DBService == null)
        //        if(!await UserLogin(pJS, pLS, pNM, pUCI))
        //        {
        //            string PreviousPage = pNM.Uri.Substring(pNM.BaseUri.Length, pNM.Uri.Length - pNM.BaseUri.Length);
        //            pNM.NavigateTo("Login?pre=" + PreviousPage); // 로그인 실패 및 로그인창 강제이동
        //            return false;
        //        }

        //    return true;
        //}

        // memo) 파라매터 마지막에 하나 더 받아서 로그인시 원래있던 페이지로 이동url ?
        //public static async Task<bool> UserLogin(IJSRuntime pJS, ProtectedLocalStorage pLS, NavigationManager pNM, UserConnectionInfo pUCI)
        //{
        //    var lHOSPID = await pLS.GetAsync<string>("cHOSPID");
        //    var lHOSPJM = await pLS.GetAsync<string>("cHOSPJM");
        //    var lUSERID = await pLS.GetAsync<string>("cUSERID");
        //    var lUSERNM = await pLS.GetAsync<string>("cUSERNM");
        //    var lUSERTP = await pLS.GetAsync<string>("cUSERTP");
        //    var lUSERIP = await pLS.GetAsync<string>("cUSERIP");
        //    var lSERVTP = await pLS.GetAsync<string>("cSERVTP");
        //    var lUserLanguage = await pLS.GetAsync<string>("cUserLanguage");

        //    if (string.IsNullOrEmpty(fnNVL(lHOSPID.Value, ""))
        //        || string.IsNullOrEmpty(fnNVL(lUSERID.Value, ""))
        //        || string.IsNullOrEmpty(fnNVL(lUSERTP.Value, ""))
        //        || string.IsNullOrEmpty(fnNVL(lUSERIP.Value, ""))
        //        || string.IsNullOrEmpty(fnNVL(lSERVTP.Value, "")))
        //    {
        //        return false;
        //    }

        //    // 로그인 성공
        //    pUCI.HOSPID = lHOSPID.Value;
        //    pUCI.HOSPJM = lHOSPJM.Value;
        //    pUCI.USERID = lUSERID.Value;
        //    pUCI.USERNM = lUSERNM.Value;
        //    pUCI.USERTP = lUSERTP.Value;
        //    pUCI.SERVTP = lSERVTP.Value;
        //    pUCI.UserLanguage = lUserLanguage.Value;
        //    pUCI.Login = true;
        //    pUCI.DBService = new DB.Common.DBService(pUCI);

        //    //pJS.InvokeVoidAsync("JS_alert", "로그인 성공 " + pUCI.USERID);
        //    return true;
        //}

        //public static async Task UserLogout(IJSRuntime pJS, ProtectedLocalStorage pLS, NavigationManager pNM, UserConnectionInfo pUCI)
        //{
        //    pLS.DeleteAsync("cHOSPID");
        //    pLS.DeleteAsync("cHOSPJM");
        //    pLS.DeleteAsync("cUSERID");
        //    pLS.DeleteAsync("cUSERNM");
        //    pLS.DeleteAsync("cUSERTP");
        //    pLS.DeleteAsync("cUSERIP");
        //    pLS.DeleteAsync("cSERVTP");
        //    pUCI.Login = false;
        //    pUCI.HOSPID = "";
        //    pUCI.HOSPJM = "";
        //    pUCI.USERID = "";
        //    pUCI.USERNM = "";
        //    pUCI.USERTP = "";
        //    pUCI.USERIP = "";
        //    pUCI.SERVTP = "";
        //    // UserLanguage 는 초기화 안함

        //    pNM.NavigateTo("Login");
        //}
    }
}
