namespace RGWeb.Models
{
    public class ContentModel
    {
        public class Content : BaseModel
        {
            /// <summary>
            /// 게시글 번호
            /// </summary>
            public string num { get; set; }
            /// <summary>
            /// 게시글 제목
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 게시글 링크
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 게시글 작성자 닉네임
            /// </summary>
            public string userName { get; set; }
            /// <summary>
            /// 게시글 작성자 아이피
            /// </summary>
            public string userIp { get; set; }
            /// <summary>
            /// 게시글 작성자 아이콘 Url
            /// </summary>
            public string userIconUrl { get; set; }
            /// <summary>
            /// 게시글 추천 수
            /// </summary>
            public int goodCount { get; set; }
            /// <summary>
            /// 게시글 코멘트 수
            /// </summary>
            public int comment { get; set; }
            /// <summary>
            /// 게시글 작성일/시간
            /// </summary>
            public DateTime date { get; set; }
        }
    }
}
