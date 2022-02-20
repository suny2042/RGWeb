namespace RGWeb.Shared
{
    public class ServerInfo
    {
        public static int UserConnectCount { get; set; } = 0;

        public static int UserConnectCount_LitePC { get; set; } = 0;
        public static int UserConnectCount_LiteMobile { get; set; } = 0;
        public static int UserConnectCount_ProPC { get; set; } = 0;
        public static int UserConnectCount_ProMobile { get; set; } = 0;

        public class PageType
        {
            public int Value { get; set; }
            public string Text { get; set; }
            /// <summary>
            /// 1: 디시인사이드 / 2: 아카라이브
            /// </summary>
            public int SiteType { get; set; }
            /// <summary>
            /// 0: 자동 / -1: 순차 / 1: 최우선 / 2: 2번중한번 / 3: 3번중한번 / ~
            /// </summary>
            public int RefreshLevel { get; set; }
            public string Url { get; set; }
        }
        public static List<PageType> PageTypeList = new List<PageType>
        {
            new PageType() { Value = 0, SiteType = 1, RefreshLevel = 3, Text = "국내야구", Url="https://m.dcinside.com/board/baseball_new10" },
            new PageType() { Value = 1, SiteType = 1, RefreshLevel = 3, Text = "만화", Url="https://m.dcinside.com/board/comic_new3" },
            new PageType() { Value = 2, SiteType = 1, RefreshLevel = 3, Text = "스트리머", Url="https://m.dcinside.com/board/stream_new1" },
        };
    }
}
