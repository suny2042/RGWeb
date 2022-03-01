using RGWeb.Shared.Models;
using Syncfusion.Blazor.Popups;

namespace RGWeb.Shared.Models.AA.ADM
{
    public class ProHomeModel
    {
        public class ProWindowLayout : BaseModel
        {
            /// <summary>
            /// 싱크퓨전 컨트롤러 Dialog (ref)
            /// </summary>
            public SfDialog Dialog { get; set; }
            /// <summary>
            /// 해당 Page/Component가 MDI모드로 열리는지 유무 (ProWindow로 열리는 화면은 무조건 True로 세팅)
            /// </summary>
            public bool IsMDI { get; set; } = false;
            /// <summary>
            /// MDI창을 닫은 상태인지 연 상태인지
            /// </summary>
            public bool Visible { get; set; }
            /// <summary>
            /// 컴포넌트 위치
            /// </summary>
            public string Uri { get; set; }
            /// <summary>
            /// 동적 컴포넌트 파라매터 (해당 컴포넌트가 필요한 경우 사용)
            /// </summary>
            public Dictionary<string, object> Param { get; set; } = new Dictionary<string, object>();

            public double Left { get; set; } = 0;

            public double Top { get; set; } = 0;

            public double Width { get; set; } = 0;

            public double Height { get; set; } = 0;
            public bool FullSize { get; set; }
            /// <summary>
            /// 창을 오픈 했을때 처음 값
            /// </summary>
            public double Left_Original { get; set; } = 0;

            public double Top_Original { get; set; } = 0;

            public double Width_Original { get; set; } = 0;

            public double Height_Original { get; set; } = 0;

            public double ZIndex { get; set; }

            public string HOSPID { get; set; }

            public string USERID { get; set; }
        }


    }
}
