namespace RGWeb.Shared
{
    public class ServerInfo
    {
        public static int UserConnectCount { get; set; } = 0;

        public static int UserConnectCount_LitePC { get; set; } = 0;
        public static int UserConnectCount_LiteMobile { get; set; } = 0;
        public static int UserConnectCount_ProPC { get; set; } = 0;
        public static int UserConnectCount_ProMobile { get; set; } = 0;

        public static long ServerRefreshTime { get; set; } = 0;

        public const int REFRESH_LEVEL_MAX = 3;

        public class PageType
        {
            /// <summary>
            /// 해당 갤러리 코드값
            /// </summary>
            public int Value { get; set; }
            /// <summary>
            /// 콤보박스에서 보이는 타이틀
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// 1: 디시인사이드 / 2: 디씨 마갤,미니갤(분류있는갤) / 3: 아카라이브
            /// </summary>
            public int SiteType { get; set; }
            public string Url { get; set; }
            /// <summary>
            /// 0: 자동 / -1: 정지 / 1: 최우선 / 2: 2번중한번 / 3: 3번중한번 / ~
            /// </summary>
            public int RefreshLevel { get; set; }
            public DateTime LastContentDateTime { get; set; }
        }
        public static List<PageType> PageTypeList = new List<PageType>
        {
            new PageType() { Value =  0, SiteType = 1, RefreshLevel = 1, Text = "국내야구", Url="https://m.dcinside.com/board/baseball_new10" },
            new PageType() { Value =  1, SiteType = 1, RefreshLevel = 0, Text = "만화", Url="https://m.dcinside.com/board/comic_new3" },
            new PageType() { Value =  2, SiteType = 1, RefreshLevel = 0, Text = "스트리머", Url="https://m.dcinside.com/board/stream_new1" },
            new PageType() { Value =  3, SiteType = 1, RefreshLevel = 0, Text = "인터넷방송", Url="https://m.dcinside.com/board/ib_new2" },
            new PageType() { Value =  4, SiteType = 2, RefreshLevel = 0, Text = "이세계아이돌", Url="https://m.dcinside.com/mini/isekaidol" },

            new PageType() { Value =  5, SiteType = 1, RefreshLevel = 0, Text = "리그오브레전드", Url="https://m.dcinside.com/board/leagueoflegends4" },
            new PageType() { Value =  6, SiteType = 2, RefreshLevel = 0, Text = "로스트아크", Url="https://m.dcinside.com/board/lostark" },
            new PageType() { Value =  7, SiteType = 1, RefreshLevel = 0, Text = "기타국내드라마", Url="https://m.dcinside.com/board/drama_new3" },
            new PageType() { Value =  8, SiteType = 1, RefreshLevel = 0, Text = "여자연예인", Url="https://m.dcinside.com/board/w_entertainer" },
            new PageType() { Value =  9, SiteType = 1, RefreshLevel = 0, Text = "남자연예인", Url="https://m.dcinside.com/board/m_entertainer_new1" },
            new PageType() { Value = 10, SiteType = 1, RefreshLevel = 0, Text = "비트코인", Url="https://m.dcinside.com/board/bitcoins_new1" },
            new PageType() { Value = 11, SiteType = 2, RefreshLevel = 0, Text = "키즈나아이", Url="https://m.dcinside.com/board/kizunaai" },
            new PageType() { Value = 12, SiteType = 2, RefreshLevel = 0, Text = "원신", Url="https://m.dcinside.com/board/onshinproject" },

            //new PageType() { Value = 13, SiteType = 3, RefreshLevel = 0, Text = "원신", Url="https://arca.live/b/genshin" },
            //new PageType() { Value = 14, SiteType = 3, RefreshLevel = 0, Text = "에픽세븐", Url="https://arca.live/b/epic7" },
        };
    }
}
