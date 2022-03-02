using HtmlAgilityPack;
using Newtonsoft.Json;
using RGWeb.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static RGWeb.Shared.Models.ContentModel;
using static RGWeb.Shared.ServerInfo;

namespace RGWeb.Crawler
{
    internal class RGCrawler
    {
        public bool serverOnoff { get; set; } = false;

        private static List<List<Content>> oContentList = new List<List<Content>>();

        Stopwatch stopwatch = new Stopwatch();
        List<Thread> threadList = new List<Thread>();

        // 리프레쉬 스케줄러
        private List<PageType> refreshList_Lv0 = new List<PageType>(); // 자동
        private List<Queue<PageType>> refreshQueue_Lv0 = new List<Queue<PageType>>();
        private List<PageType> refreshList_Lv1 = new List<PageType>();
        private List<Queue<PageType>> refreshQueue_Lv1 = new List<Queue<PageType>>();
        private List<PageType> refreshList_Lv2 = new List<PageType>();
        private List<Queue<PageType>> refreshQueue_Lv2 = new List<Queue<PageType>>();
        private List<PageType> refreshList_Lv3 = new List<PageType>();
        private List<Queue<PageType>> refreshQueue_Lv3 = new List<Queue<PageType>>();

        public async Task CrawlingStart()
        {
            serverOnoff = true;
            await CrawlerStart();
        }
        public async Task CrawlingEnd()
        {
            serverOnoff = false;
            Console.WriteLine("[Blazor TEST] Dispose");
        }

        // 페이지의 시작
        public async Task CrawlerStart() // 페이지가 처음 로드될때 (아래보다 주로 사용)
        {
            Console.WriteLine("[Blazor TEST] Start");

            InitRefreshSchedule();

            oContentList.Clear();
            for (int i = 0; i < PageTypeList.Count; i++)
                oContentList.Add(new List<Content>());

            while (serverOnoff)
            {
                stopwatch.Reset();
                stopwatch.Start();
                threadList.Clear();

                try
                {
                    LoadRefreshSchedule();

                    NewThread_LvN(ref threadList, refreshQueue_Lv1);
                    NewThread_LvN(ref threadList, refreshQueue_Lv2);
                    NewThread_LvN(ref threadList, refreshQueue_Lv3);
                    NewThread_LvN(ref threadList, refreshQueue_Lv0);

                    foreach (Thread t in threadList)
                    {
                        t.IsBackground = true;
                        t.Start();
                    }

                    foreach (Thread t in threadList)
                        t.Join();

                    ServerRefreshTime = stopwatch.ElapsedMilliseconds;
                    //Console.WriteLine("[While] 쓰레드수 : " + threadList.Count + " / 시간결과 : " + ServerRefreshTime + "ms");

                    // 스케줄러
                    PageSchedule(refreshList_Lv1);
                    PageSchedule(refreshList_Lv2);
                    PageSchedule(refreshList_Lv3);
                    PageSchedule(refreshList_Lv0);

                    if (ServerRefreshTime < 1000)       // 최소 1초 딜레이 보장
                        await Task.Delay(1000 - (int)ServerRefreshTime);
                    else
                        await Task.Delay(10);
                    stopwatch.Stop();
                    //Console.WriteLine("타임 : " + stopwatch.ElapsedMilliseconds);

                    await SendToRGWebServer();
                } catch { }
                
            }
        }

        private async Task SendToRGWebServer()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();

            var stringPayload = JsonConvert.SerializeObject(RGComLib.ContentToJson(oContentList));
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var stringTask = client.PostAsync("https://galllive.azurewebsites.net/api/Crawling/Crawler", httpContent);

            var msg = await stringTask;
            //Console.WriteLine(msg); // 응답결과
            var result = msg.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(result); // 응답에 포함된 Body 내용
        }

        private void PageSchedule(List<PageType> pRefreshList_Lv)
        {
            DateTime now = DateTime.Now;
            int DiffMin = 0;
            PageType temp = null;

            for (int i = 0; i < pRefreshList_Lv.Count; i++)
            {
                if (pRefreshList_Lv[i].FixedRefreshLevel == true) continue; // 고정레벨은 무시

                DiffMin = (now - pRefreshList_Lv[i].LastContentDateTime).Minutes;
                if (DiffMin <= 1)
                {
                    if (pRefreshList_Lv[i].RefreshLevel == 1) continue;
                    PageTypeList[pRefreshList_Lv[i].Value].RefreshLevel = 1;
                    temp = pRefreshList_Lv[i];
                    pRefreshList_Lv.RemoveAt(i);
                    i--;
                    refreshList_Lv1.Add(temp);
                }
                else if (DiffMin <= 2)
                {
                    if (pRefreshList_Lv[i].RefreshLevel == 2) continue;
                    PageTypeList[pRefreshList_Lv[i].Value].RefreshLevel = 2;
                    temp = pRefreshList_Lv[i];
                    pRefreshList_Lv.RemoveAt(i);
                    i--;
                    refreshList_Lv2.Add(temp);
                }
                else if (DiffMin <= 3)
                {
                    if (pRefreshList_Lv[i].RefreshLevel == 3) continue;
                    PageTypeList[pRefreshList_Lv[i].Value].RefreshLevel = 3;
                    temp = pRefreshList_Lv[i];
                    pRefreshList_Lv.RemoveAt(i);
                    i--;
                    refreshList_Lv3.Add(temp);
                }
                else
                {
                    if (pRefreshList_Lv[i].RefreshLevel == 0) continue;
                    PageTypeList[pRefreshList_Lv[i].Value].RefreshLevel = 0;
                    temp = pRefreshList_Lv[i];
                    pRefreshList_Lv.RemoveAt(i);
                    i--;
                    refreshList_Lv0.Add(temp);
                }
            }
        }

        private void NewThread_LvN(ref List<Thread> pThreadList, List<Queue<PageType>> pQueue)
        {
            foreach (Queue<PageType> item in pQueue)
                if (item.Any())
                {
                    PageType lIndex = item.Dequeue();
                    pThreadList.Add(new Thread(() => ListRefresh_dcMobile_Thread(lIndex)));
                    //Console.WriteLine("[NewThread_LvN lv/Th/val] " + lIndex.RefreshLevel + "|" + (pThreadList.Count - 1) + "|" + lIndex.Value + " : " + lIndex.Text);
                }
        }

        private void InitRefreshSchedule()
        {
            refreshList_Lv1.Clear();
            refreshList_Lv2.Clear();
            refreshList_Lv3.Clear();
            refreshList_Lv0.Clear();

            for (int i = 0; i < PageTypeList.Count; i++)
            {
                switch (PageTypeList[i].RefreshLevel)
                {
                    case 0:
                        refreshList_Lv0.Add(PageTypeList[i]);
                        break;
                    case 1:
                        refreshList_Lv1.Add(PageTypeList[i]);
                        break;
                    case 2:
                        refreshList_Lv2.Add(PageTypeList[i]);
                        break;
                    case 3:
                        refreshList_Lv3.Add(PageTypeList[i]);
                        break;
                    case -1:  // 무시리스트
                    default:
                        break;
                }
            }
        }

        private void AddRefreshQueue(int pLevel, List<PageType> pList, List<Queue<PageType>> pQueue)
        {
            if (pQueue.Count <= 0 || pQueue.ElementAt(0).Any() == false) // 큐가 없으면 추가
            {
                pQueue.Clear();
                for (int i = 0; i < pList.Count; i++)
                {
                    // 큐에 빈곳 없으면 리스트추가해서 큐 생성
                    if (pLevel == 0)
                    {
                        if (i == 0)
                            pQueue.Add(new Queue<PageType>());
                    }
                    else if (pLevel != 0 && i % pLevel == 0) // 레벨 0이 오면 에러남
                    {
                        pQueue.Add(new Queue<PageType>());
                    }

                    pQueue[pQueue.Count - 1].Enqueue(pList[i]);
                }
            }
        }

        private void LoadRefreshSchedule()
        {
            AddRefreshQueue(1, refreshList_Lv1, refreshQueue_Lv1);
            AddRefreshQueue(2, refreshList_Lv2, refreshQueue_Lv2);
            AddRefreshQueue(3, refreshList_Lv3, refreshQueue_Lv3);
            AddRefreshQueue(0, refreshList_Lv0, refreshQueue_Lv0);
        }

        private void ListRefresh_dcMobile_Thread(PageType pIndex)
        {
            //using System.Net; [사용되지 않음]이니 나중에 httpclient 등으로 변경하기
            HttpWebRequest wrGETURL;
            // 사이트 종류에 따라서 게시판 링크와 조합
            string targetUrl = UrlPageDictionary.GetValueOrDefault(pIndex.SiteType) + pIndex.Url;

            wrGETURL = (HttpWebRequest)WebRequest.Create(targetUrl);
            wrGETURL.Method = "GET";
            wrGETURL.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_5_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1.1 Mobile/15E148 Safari/604.1";
            wrGETURL.Timeout = 3000;

            Stream objStream = null;
            HtmlDocument doc = new HtmlDocument();
            try
            {
                objStream = wrGETURL.GetResponse().GetResponseStream();
                doc.Load(objStream);
            }
            catch
            {
                return;
            }
            finally
            {
                if (objStream is not null)
                    objStream.Close();
            }

            string siteXPath = "/html/body/div/div/div/section[3]/ul[2]/li";
            if (pIndex.SiteType == 3) siteXPath = "/html/body/div/div/div/div/section[3]/ul[2]/li";     // 미니갤
            HtmlNodeCollection article = doc.DocumentNode.SelectNodes(siteXPath);
            List<Content> list = new List<Content>();

            if (article is null)
            {
                Console.WriteLine("[ListRefresh_dcMobile_Thread] " + pIndex.Text + " " + "Error");
                return;
            }

            oContentList[pIndex.Value].Clear();

            for (int i = 0; i < article.Count; i++)
            {
                if (article[i].HasClass("adv-inner")) continue;

                HtmlNode lLeftLabel = article[i].SelectSingleNode("div/a[1]");
                HtmlNodeCollection lBottomLabel = lLeftLabel.SelectNodes("ul/li");

                // 디씨 마갤/미니갤 대응
                int lElementNumber = 0;
                if (pIndex.SiteType == 2 || pIndex.SiteType == 3 || pIndex.SiteType == 4)
                    lElementNumber++;

                string lUserIconUrl = "";
                try
                {
                    lUserIconUrl = lBottomLabel[lElementNumber + 0].SelectSingleNode("span").GetAttributeValue("class", "");
                }
                catch { }

                Content n = new Content()
                {
                    num = UrlToNumber(lLeftLabel.GetAttributeValue("href", "")),
                    url = lLeftLabel.GetAttributeValue("href", ""),
                    title = HttpUtility.HtmlDecode(lLeftLabel.SelectSingleNode("span/span[2]").InnerText),
                    userName = HttpUtility.HtmlDecode(lBottomLabel[lElementNumber + 0].InnerText),
                    viewCount = lBottomLabel[lElementNumber + 2].InnerText.Remove(0, 3),
                    goodCount = lBottomLabel[lElementNumber + 3].InnerText.Remove(0, 3),
                    contentDate = lBottomLabel[lElementNumber + 1].InnerText,
                    date = fnDcinsideStrToDate(lBottomLabel[lElementNumber + 1].InnerText),
                    userIconUrl = lUserIconUrl,
                    userIp = "",
                    contentIconUrl = lLeftLabel.SelectSingleNode("span/span[1]").GetAttributeValue("class", ""),
                    commentCount = article[i].SelectSingleNode("div/a[2]/span").InnerText,

                    // JSON으로 말때 공백이라도 있어야 함
                    IUD = "",
                    GUID = "",
                };

                oContentList[pIndex.Value].Add(n);

                // Last 게시물을 저장 (글 올린 시간)
                if (i == article.Count - 1)
                    pIndex.LastContentDateTime = n.date;
            }
        }

        // DcInsise 문자열을 DateTime으로 반환
        public DateTime fnDcinsideStrToDate(string pStr)
        {
            DateTime now = new DateTime();
            DateTime dt = new DateTime();

            if (pStr.Length == 5)
            {
                if (pStr[2] == ':') // hh:mm
                    dt = new DateTime(now.Year
                    , now.Month
                    , now.Day
                    , int.Parse(pStr.Substring(0, 2))
                    , int.Parse(pStr.Substring(3, 2))
                    , now.Second
                    );
                else                // mm.dd
                    dt = new DateTime(now.Year
                    , int.Parse(pStr.Substring(0, 2))
                    , int.Parse(pStr.Substring(3, 2))
                    , now.Hour
                    , now.Minute
                    , now.Second
                    );
            }
            else if (pStr.Length == 19) // xxxx-xx-xx xx:xx:xx
                dt = new DateTime(int.Parse(pStr.Substring(0, 4))
                , int.Parse(pStr.Substring(5, 2))
                , int.Parse(pStr.Substring(8, 2))
                , int.Parse(pStr.Substring(11, 2))
                , int.Parse(pStr.Substring(14, 2))
                , int.Parse(pStr.Substring(17, 2))
                );
            else if (pStr.Length == 8)  // yy.mm.dd
                dt = new DateTime(2000 + int.Parse(pStr.Substring(0, 2))
                , int.Parse(pStr.Substring(3, 2))
                , int.Parse(pStr.Substring(6, 2))
                , now.Hour
                , now.Minute
                , now.Second
                );

            return dt;
        }

        public int UrlToNumber(string pUrl)
        {
            if (string.IsNullOrEmpty(pUrl))
                return 0;
            string[] temp = pUrl.Split('/');
            return int.Parse(temp[temp.Length - 1]);
        }
    }
}
