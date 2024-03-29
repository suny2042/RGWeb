﻿using static RGWeb.Shared.Models.ContentModel;

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

        private static bool _serverOnoff = false;
        public bool serverOnoff
        {
            get => _serverOnoff;
            set => SetValue(ref _serverOnoff, value);
        }

        private static bool _serverCrawlerOnoff = false;
        public bool serverCrawlerOnoff
        {
            get => _serverCrawlerOnoff;
            set => SetValue(ref _serverCrawlerOnoff, value);
        }

        private string _userAgent = "";     // 클라이언트 유저 에이전트값
        public string userAgent
        {
            get => _userAgent;
            set => SetValue(ref _userAgent, value);
        }
    }
}
