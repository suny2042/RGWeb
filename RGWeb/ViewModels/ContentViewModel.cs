using static RGWeb.Models.ContentModel;

namespace RGWeb.ViewModels
{
    public class ContentViewModel : BaseViewModel, IContentViewModel
    {
        // 이곳을 static으로 선언시 모든 유저와 공유됨
        private static List<ModelDataSet<Content>> _oContentList = new List<ModelDataSet<Content>>();
        public List<ModelDataSet<Content>> oContentList
        {
            get => _oContentList;
            set => SetValue(ref _oContentList, value);
        }

        public static bool _serverOnoff = false;
        public bool serverOnoff
        {
            get => _serverOnoff;
            set => SetValue(ref _serverOnoff, value);
        }
    }
}
