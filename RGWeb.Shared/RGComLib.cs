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
