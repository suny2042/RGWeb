using static RGWeb.Models.ContentModel;

namespace RGWeb.ViewModels
{
    public class ContentViewModel : BaseViewModel, IContentViewModel
    {
        // 이곳을 static으로 선언시 모든 유저와 공유됨
        private static ModelDataSet<Content> _oContentList = new ModelDataSet<Content>();
        public ModelDataSet<Content> oContentList
        {
            get => _oContentList;
            set => SetValue(ref _oContentList, value);
        }
    }
}
