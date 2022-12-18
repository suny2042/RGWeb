namespace RGWeb.Shared
{
    public class ServerInfo
    {
        public static string RGWebServerUrl { get; set; } = "";
        public static int UserConnectCount_Index { get; set; } = 0;
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
            /// 1: 디시인사이드 2: 디씨마갤 3: 디씨미니갤 4: 분류있는갤 / 11: 아카라이브
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

        /// <summary>
        /// 게시판 링크
        /// int : PageTypeList.SiteType / string : PageTypeList.Url
        /// </summary>
        public static Dictionary<int, string> UrlPageDictionary = new Dictionary<int, string>()
        {
            {1, "https://m.dcinside.com/board/" },
            {2, "https://m.dcinside.com/board/" },
            {3, "https://m.dcinside.com/mini/" },
            {4, "https://m.dcinside.com/board/" },
            {11, "https://arca.live/b/" },
        };
        /// <summary>
        /// 게시판 글쓰기 링크 접두사
        /// </summary>
        public static Dictionary<int, string> UrlWritePrefixDictionary = new Dictionary<int, string>()
        {
            {1, "https://gall.dcinside.com/board/write/?id=" },
            {2, "https://gall.dcinside.com/mgallery/board/write/?id=" },
            {3, "https://gall.dcinside.com/mini/board/write/?id=" },
            {4, "https://gall.dcinside.com/board/write/?id=" },
            {11, "https://arca.live/b/" },
        };
        /// <summary>
        /// 게시판 글쓰기 링크 접미사
        /// </summary>
        public static Dictionary<int, string> UrlWriteSuffixDictionary = new Dictionary<int, string>()
        {
            {11, "/write" }, // 아카라이브
        };
        /// <summary>
        /// 게시판 태블릿/폰 글쓰기 링크 접두사
        /// </summary>
        public static Dictionary<int, string> UrlMobileWritePrefixDictionary = new Dictionary<int, string>()
        {
            {1, "https://m.dcinside.com/write/" },
            {2, "https://m.dcinside.com/write/" },
            {3, "https://m.dcinside.com/writemini/" },
            {4, "https://m.dcinside.com/write/" },
            {11, "https://arca.live/b/" },
        };
        /// <summary>
        /// 게시판 태블릿/폰 글쓰기 링크 접미사
        /// </summary>
        public static Dictionary<int, string> UrlMobileWriteSuffixDictionary = new Dictionary<int, string>()
        {
            {11, "/write" }, // 아카라이브
        };

        public static List<PageType> PageTypeList = new List<PageType>
        {
            new PageType() { Value =  0, SiteType = 1, RefreshLevel = 1, Text = "국내야구", FixedRefreshLevel = true, Url="baseball_new11" },
            new PageType() { Value =  1, SiteType = 1, RefreshLevel = 0, Text = "만화", Url="comic_new3" },
            new PageType() { Value =  2, SiteType = 1, RefreshLevel = 0, Text = "스트리머", Url="stream_new1" },
            new PageType() { Value =  3, SiteType = 1, RefreshLevel = 0, Text = "인터넷방송", Url="ib_new2" },
            new PageType() { Value =  4, SiteType = 1, RefreshLevel = 0, Text = "해외축구", Url="football_new7" },

            new PageType() { Value =  5, SiteType = 1, RefreshLevel = 0, Text = "리그오브레전드", Url="leagueoflegends4" },
            new PageType() { Value =  6, SiteType = 4, RefreshLevel = 0, Text = "로스트아크", Url="lostark" },
            new PageType() { Value =  7, SiteType = 1, RefreshLevel = 0, Text = "기타국내드라마", Url="drama_new3" },
            new PageType() { Value =  8, SiteType = 1, RefreshLevel = 0, Text = "여자연예인", Url="w_entertainer" },
            new PageType() { Value =  9, SiteType = 1, RefreshLevel = 0, Text = "남자연예인", Url="m_entertainer_new1" },
            new PageType() { Value = 10, SiteType = 1, RefreshLevel = 0, Text = "비트코인", Url="bitcoins_new1" },
            new PageType() { Value = 11, SiteType = 2, RefreshLevel = 0, Text = "키즈나아이", Url="kizunaai" },
            new PageType() { Value = 12, SiteType = 3, RefreshLevel = 0, Text = "버츄얼스트리머", Url="virtual_streamer" },
            new PageType() { Value = 13, SiteType = 3, RefreshLevel = 0, Text = "이세계아이돌", Url="isekaidol" },
            new PageType() { Value = 14, SiteType = 2, RefreshLevel = 0, Text = "프롬(엘든링)", Url="fromsoftware" },
            new PageType() { Value = 15, SiteType = 2, RefreshLevel = 0, Text = "원신", Url="onshinproject" },

            //new PageType() { Value = 14, SiteType = 11, RefreshLevel = 0, Text = "원신", Url="https://arca.live/b/genshin" },
            //new PageType() { Value = 15, SiteType = 11, RefreshLevel = 0, Text = "에픽세븐", Url="https://arca.live/b/epic7" },

        };
    }
}
