using System.Collections.ObjectModel;
using RGWeb.Models;
using static RGWeb.Models.ContentModel;
using System.ComponentModel;

namespace RGWeb.ViewModels
{
    public interface IContentViewModel
    {
        #region [ 〈공통〉 BaseViewModel 에서 정의되어 있는 필수 메소드 ]
        bool IsBusy { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModelInfo baseViewModelInfo { get; }

        #endregion

        List<ModelDataSet<Content>> oContentList { get; set; }
    }
}
