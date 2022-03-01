using Newtonsoft.Json;
using RGWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RGWeb.Shared.Models.ContentModel;

namespace RGWeb.Crawler
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
    }
}
