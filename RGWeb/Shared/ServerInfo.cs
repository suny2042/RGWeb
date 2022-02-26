﻿namespace RGWeb.Shared
{
    public class ServerInfo
    {
        public static int UserConnectCount_LitePC { get; set; } = 0;
        public static int UserConnectCount_LiteMobile { get; set; } = 0;
        public static int UserConnectCount_ProPC { get; set; } = 0;
        public static int UserConnectCount_ProMobile { get; set; } = 0;
        public static int UserConnectCount_ProAir { get; set; } = 0;
        /// <summary>
        /// 서버가 레프레시되는 주기(ms)
        /// </summary>
        public static long ServerRefreshTime { get; set; } = 0;
        /// <summary>
        /// 현재 스케줄러에 의한 쓰레드
        /// </summary>
        public static List<Thread> threadList { get; set; } = new List<Thread>();

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
            /// 1: 디시인사이드 / 2: 디씨마갤(분류있는갤) 3: 디씨미니갤 / 11: 아카라이브
            /// </summary>
            public int SiteType { get; set; }
            /// <summary>
            /// 사이트 URL
            /// </summary>
            public string Url { get; set; }

            // 스케줄 관련
            /// <summary>
            /// 0: 자동 / -1: 정지 / 1: 최우선 / 2: 2번중한번 / 3: 3번중한번 / ~
            /// </summary>
            public int RefreshLevel { get; set; }
            /// <summary>
            /// 레벨을 스케줄러에 의한 변동없이 고정할 여부 (True시 고정)
            /// </summary>
            public bool FixedRefreshLevel { get; set; } = false;
            /// <summary>
            /// 연산을 담당할 서버명
            /// </summary>
            public string AllocationServer { get; set; } = "";
            /// <summary>
            /// 스케줄용 마지막항목 게시시간
            /// </summary>
            public DateTime LastContentDateTime { get; set; }
        }

        public static Dictionary<int, string> UrlPageDictionary = new Dictionary<int, string>()
        {
            {1, "https://m.dcinside.com/board/" },
            {2, "https://m.dcinside.com/board/" },
            {3, "https://m.dcinside.com/mini/" },
            {11, "https://arca.live/b/" },
        };
        public static Dictionary<int, string> UrlWritePrefixDictionary = new Dictionary<int, string>()
        {
            {1, "https://m.dcinside.com/write/" },
            {2, "https://m.dcinside.com/write/" },
            {3, "https://m.dcinside.com/writemini/" },
            {11, "https://arca.live/b/" },
        };
        public static Dictionary<int, string> UrlWriteSuffixDictionary = new Dictionary<int, string>()
        {
            {11, "/write" },
        };

        public static List<PageType> PageTypeList = new List<PageType>
        {
            new PageType() { Value =  0, SiteType = 1, RefreshLevel = 1, Text = "국내야구", FixedRefreshLevel = true, Url="https://m.dcinside.com/board/baseball_new10" },
            new PageType() { Value =  1, SiteType = 1, RefreshLevel = 0, Text = "만화", Url="https://m.dcinside.com/board/comic_new3" },
            new PageType() { Value =  2, SiteType = 1, RefreshLevel = 0, Text = "스트리머", Url="https://m.dcinside.com/board/stream_new1" },
            new PageType() { Value =  3, SiteType = 1, RefreshLevel = 0, Text = "인터넷방송", Url="https://m.dcinside.com/board/ib_new2" },
            new PageType() { Value =  4, SiteType = 3, RefreshLevel = 0, Text = "이세계아이돌", Url="https://m.dcinside.com/mini/isekaidol" },

            new PageType() { Value =  5, SiteType = 1, RefreshLevel = 0, Text = "리그오브레전드", Url="https://m.dcinside.com/board/leagueoflegends4" },
            new PageType() { Value =  6, SiteType = 2, RefreshLevel = 0, Text = "로스트아크", Url="https://m.dcinside.com/board/lostark" },
            new PageType() { Value =  7, SiteType = 1, RefreshLevel = 0, Text = "기타국내드라마", Url="https://m.dcinside.com/board/drama_new3" },
            new PageType() { Value =  8, SiteType = 1, RefreshLevel = 0, Text = "여자연예인", Url="https://m.dcinside.com/board/w_entertainer" },
            new PageType() { Value =  9, SiteType = 1, RefreshLevel = 0, Text = "남자연예인", Url="https://m.dcinside.com/board/m_entertainer_new1" },
            new PageType() { Value = 10, SiteType = 1, RefreshLevel = 0, Text = "비트코인", Url="https://m.dcinside.com/board/bitcoins_new1" },
            new PageType() { Value = 11, SiteType = 2, RefreshLevel = 0, Text = "키즈나아이", Url="https://m.dcinside.com/board/kizunaai" },
            new PageType() { Value = 12, SiteType = 2, RefreshLevel = 0, Text = "원신", Url="https://m.dcinside.com/board/onshinproject" },

            //new PageType() { Value = 13, SiteType = 11, RefreshLevel = 0, Text = "원신", Url="https://arca.live/b/genshin" },
            //new PageType() { Value = 14, SiteType = 11, RefreshLevel = 0, Text = "에픽세븐", Url="https://arca.live/b/epic7" },
        };
    }
}
