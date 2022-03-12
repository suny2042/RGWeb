using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RGWeb.Shared.Models.ContentModel;

namespace RGWeb.Shared
{
    public static class RGComLib
    {
        /// <summary>
        /// 크롤러가 콘솔앱에서 작동하는지 서버사이드앱에서 작동하는지
        /// </summary>
        public enum CrawlerType
        {
            ConsoleApp,
            ServerSide
        }

        public static object? ContentToJson(List<List<Content>> pContent)
        {
            var payload = new Payload
            {
                crawlingRefreshTime = (int)ServerInfo.ServerRefreshTime,
                content = pContent
            };

            return payload;
        }

        public static string GetWritePageUrl(int pSelectPageValue)
        {
            int lSiteType = ServerInfo.PageTypeList[pSelectPageValue].SiteType;
            return (ServerInfo.UrlWritePrefixDictionary.GetValueOrDefault(lSiteType, "")
                + ServerInfo.PageTypeList[pSelectPageValue].Url
                + ServerInfo.UrlWriteSuffixDictionary.GetValueOrDefault(lSiteType, ""));
        }

        public static string GetMobileWritePageUrl(int pSelectPageValue)
        {
            int lSiteType = ServerInfo.PageTypeList[pSelectPageValue].SiteType;
            return (ServerInfo.UrlMobileWritePrefixDictionary.GetValueOrDefault(lSiteType, "")
                + ServerInfo.PageTypeList[pSelectPageValue].Url
                + ServerInfo.UrlMobileWriteSuffixDictionary.GetValueOrDefault(lSiteType, ""));
        }
    }
}
