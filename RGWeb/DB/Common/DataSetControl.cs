using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.Reflection;
using Syncfusion.Blazor.Grids;
using RGWeb.ViewModels;
using RGWeb.Models;
using Microsoft.AspNetCore.Components;
using RGWeb.Shared;
using Microsoft.JSInterop;

namespace RGWeb.DB.Common
{
    public class DataSetControl
    {
        /// <summary>
        /// 데이터셋을 (깊은)복사하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pDataSet">복사할 데이터셋</param>
        /// <returns>복사받을 데이터셋</returns>
        public static ObservableCollection<T> Copy<T>(ObservableCollection<T> pDataSet) where T : BaseModel, new()
        {
            ObservableCollection<T> oNew = new ObservableCollection<T>();

            for (int i = 0; i < pDataSet.Count; i++)
            {
                T tNew = new T();
                foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                    {
                        object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            propertyInfoInner.SetValue(instance, (propertyInfoInner.GetValue(propertyInfo.GetValue(pDataSet[i])) == null ? null : propertyInfoInner.GetValue(propertyInfo.GetValue(pDataSet[i])).ToString()));
                        }
                        propertyInfo.SetValue(tNew, instance);
                    }
                    else
                        propertyInfo.SetValue(tNew, (propertyInfo.GetValue(pDataSet[i]) == null ? null : propertyInfo.GetValue(pDataSet[i]).ToString()));
                }

                oNew.Add(tNew);
            }

            return oNew;
        }


        /// <summary>
        /// 현재 포커싱되어 있는 row 번호를 반환합니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pGrid">대상되는 그리드</param>
        /// <returns>row 번호</returns>
        public static async Task<int> GetSelectedRowNumber<T>(SfGrid<T> pGrid)
        {
            //List<(double, double)> li = await pGrid.GetSelectedRowCellIndexesAsync();
            //if (li == null || li.Count <= 0) return -1;
            //(double, double)[] d = li.ToArray(); // (rowNumber, columnNumber)
            //int row = (int)d[0].Item1;

            //return row;

            List<double> li = await pGrid.GetSelectedRowIndexes();
            double[] d = li.ToArray();
            foreach (var row in d) // 값이 다중일 수 있는데, 가장 첫번째로 반환
                return (int)row;
            return -1;
        }

        /// <summary>
        /// 현재 포커싱되어 있는 column 명을 반환합니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pGrid">대상되는 그리드</param>
        /// <returns>column 명칭</returns>
        public static async Task<string> GetSelectedColumnName<T>(SfGrid<T> pGrid)
        {
            List<(double, double)> li = await pGrid.GetSelectedRowCellIndexesAsync();
            if (li == null || li.Count <= 0) return "";
            (double, double)[] d = li.ToArray();
            int row = (int)d[0].Item2;

            return pGrid.Columns[row].Field;
        }

        public static async Task grid_CellSaved<T>(ModelDataSet<T> pDataSet, SfGrid<T> pGrid, CellSaveArgs<T> args) where T : BaseModel, new()
        {
            List<double> li = await pGrid.GetSelectedRowIndexes();
            double[] d = li.ToArray();
            foreach (var row in d) // row를 다중으로 선택할수도 있어서.
            {
                foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    string name = propertyInfo.Name;
                    if (string.IsNullOrEmpty(name) && args.ColumnName != name) continue;
                    if (args.Value == args.PreviousValue) continue;    // 변경사항 없는 경우 넘어가기

                    if (name == "IUD" && ComLib.fnNVL(propertyInfo.GetValue(pDataSet.dataSet[(int)row]), "") != "I") // INSERT row만 아니면 직접 수정의 경우 UPDATE row로.
                        propertyInfo.SetValue(pDataSet.dataSet[(int)row], "U");
                    else
                        propertyInfo.SetValue(pDataSet.dataSet[(int)row], propertyInfo.GetValue(args.Data));
                }

                // 변동사항이 없는 경우 IUD를 "N"값으로 처리
                pDataSet.SetChangedDataRowState((int)row);
            }

            pDataSet.OnPropertyChanged("grid_CellSaved()");
        }

        // 2022.01.13 윤기선 삭제해야함
        public static void grd_RowDataBound<T>(RowDataBoundEventArgs<T> args) where T : BaseModel
        {
            BaseModel lModel = args.Data;

            if (lModel.IUD == "I" || lModel.IUD == "U")
                args.Row.AddClass(new string[] { "InsertUpdateColor" });
        }



        // 단일 데이터(모델)
        public static T GetBindDataModel<T>(DataTable pDataTable) where T : BaseModel, new ()
        {
            T model = new T();

            if (pDataTable.Rows.Count <= 0)
                return null;

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = propertyInfo.Name;
                if (string.IsNullOrEmpty(name)) continue;

                if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.27 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                {
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                    foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        name = propertyInfoInner.Name;

                        string value = "";
                        foreach (DataColumn col in pDataTable.Rows[0].Table.Columns)
                            if (name == col.ColumnName)
                                value = pDataTable.Rows[0][name].ToString();

                        if (string.IsNullOrEmpty(value)) continue;

                        propertyInfoInner.SetValue(instance, value);
                    }
                    propertyInfo.SetValue(model, instance);
                }
                else
                {
                    if (name == "GUID")
                        propertyInfo.SetValue(model, Guid.NewGuid().ToString()); // 2021.11.18 윤기선 GUID(데이터셋의 레코드 고유번호) 세팅
                    else if (name == "IUD")
                        propertyInfo.SetValue(model, "N");
                    else
                    {
                        string value = "";
                        foreach (DataColumn col in pDataTable.Rows[0].Table.Columns)
                            if (name == col.ColumnName)
                                value = pDataTable.Rows[0][name].ToString();

                        if (string.IsNullOrEmpty(value)) continue;

                        propertyInfo.SetValue(model, value);
                    }
                }
            }

            return model;
        }




        
    }

    // 그리드 이벤트 관련
    public class BaseGridEventControl<T> where T : BaseModel, new()
    {
        ModelDataSet<T> _dataSet;
        SfGrid<T> _grid;
        System.Action InvokeRefreshEvent;   // 각 .razor 페이지에 있는 그리드 Refresh 이벤트 호출
        double focusPreviousRowIndex = -1;

        Func<Task>[] DetailLoadEvents = null;

        /// <summary>
        /// 그리드 조작과 관련하여 기본적인 이벤트를 자동으로 세팅합니다.
        /// </summary>
        /// <param name="pDataSet">그리드와 바인드되는 DataSet</param>
        /// <param name="pGrid">그리드 대상 (SfGrid에서 @ref)</param>
        /// <param name="pInvokeRefreshEvent">각 .razor페이지에 존재하는 InvokeRefreshEvent (그리드 리플레쉬 이벤트)</param>
        public BaseGridEventControl(ModelDataSet<T> pDataSet, SfGrid<T> pGrid, System.Action pInvokeRefreshEvent)
        {
            InitBaseGridEventControl(pDataSet, pGrid, pInvokeRefreshEvent);
        }
        /// <summary>
        /// 그리드 조작과 관련하여 기본적인 이벤트를 자동으로 세팅합니다. 추가로 마스터-디테일 관계의 그리드인 경우 연관되는 디테일 조회 메소드를 넣을 수 있습니다.
        /// </summary>
        /// <param name="pDataSet">그리드와 바인드되는 DataSet</param>
        /// <param name="pGrid">그리드 대상 (SfGrid에서 @ref)</param>
        /// <param name="pInvokeRefreshEvent">각 .razor페이지에 존재하는 InvokeRefreshEvent (그리드 리플레쉬 이벤트)</param>
        /// <param name="pDetailLoadEvents">디테일 관계에 있는 그리드 조회 메소드 (1개 이상 가능)</param>
        public BaseGridEventControl(ModelDataSet<T> pDataSet, SfGrid<T> pGrid, System.Action pInvokeRefreshEvent, params Func<Task>[] pDetailLoadEvents)
        {
            InitBaseGridEventControl(pDataSet, pGrid, pInvokeRefreshEvent);
            DetailLoadEvents = pDetailLoadEvents;
        }

        private void InitBaseGridEventControl(ModelDataSet<T> pDataSet, SfGrid<T> pGrid, System.Action pInvokeRefreshEvent)
        {
            _dataSet = pDataSet;
            _grid = pGrid;
            InvokeRefreshEvent = pInvokeRefreshEvent;

            // 그리드 IUD관련 이벤트 세팅
            _grid.GridEvents.CellSaved = new EventCallbackFactory().Create<CellSaveArgs<T>>(_grid, grid_CellSaved);
            _grid.GridEvents.OnBatchAdd = new EventCallbackFactory().Create<BeforeBatchAddArgs<T>>(_grid, grid_OnBatchAdd);
            _grid.GridEvents.OnBatchDelete = new EventCallbackFactory().Create<BeforeBatchDeleteArgs<T>>(_grid, grid_OnBatchDelete);
            _grid.GridEvents.OnBatchSave = new EventCallbackFactory().Create<BeforeBatchSaveArgs<T>>(_grid, grid_OnBatchSave);
            _grid.GridEvents.RowDataBound = new EventCallbackFactory().Create<RowDataBoundEventArgs<T>>(_grid, grid_RowDataBound);
            _grid.GridEvents.OnCellEdit = new EventCallbackFactory().Create<CellEditArgs<T>>(_grid, grid_OnCellEdit);

            // 그리드 포커싱관련 이벤트 세팅. 순서 RowSelecting -> RowSelected -> CellSelecting -> CellSelected
            _grid.GridEvents.CellSelecting = new EventCallbackFactory().Create<CellSelectingEventArgs<T>>(_grid, grid_CellSelecting);
            _grid.GridEvents.CellSelected = new EventCallbackFactory().Create<CellSelectEventArgs<T>>(_grid, grid_CellSelected);
            _grid.GridEvents.RowSelecting = new EventCallbackFactory().Create<RowSelectingEventArgs<T>>(_grid, grid_RowSelecting);
            _grid.GridEvents.RowSelected = new EventCallbackFactory().Create<RowSelectEventArgs<T>>(_grid, grid_RowSelected);
        }

        private void grid_RowSelecting(RowSelectingEventArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_RowSelecting " + args.PreviousRowIndex + " " + args.RowIndex);
            
            _dataSet.focusRowIndex = args.RowIndex;
            if (DetailLoadEvents is not null && (args.RowIndex != focusPreviousRowIndex))
            {
                foreach(Func<Task> DetailLoadEvent in DetailLoadEvents) // 관련된 디테일 그리드들 조회
                    DetailLoadEvent();
            }
        }

        private void grid_RowSelected(RowSelectEventArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_RowSelected " + args.PreviousRowIndex + " " + args.RowIndex);
        }

        public async Task grid_CellSelecting(CellSelectingEventArgs<T> args) 
        {
            Console.WriteLine("[DataSetControl] grid_CellSelecting Row: " + args.RowIndex + " Column: " + args.CellIndex);
        }

        public async Task grid_CellSelected(CellSelectEventArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_CellSelected Row: " + args.RowIndex + " Column: " + args.CellIndex);

            focusPreviousRowIndex = args.RowIndex;
            _dataSet.focusRowIndex = args.RowIndex;
            _dataSet.focusColumnIndex = args.CellIndex;
            _dataSet.focusColumnIndexName = _grid.Columns[(int)args.CellIndex].Field;
        }

        public async Task grid_CellSaved(CellSaveArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_CellSaved");
            await DataSetControl.grid_CellSaved<T>(_dataSet, _grid, args);
            //InvokeRefreshEvent();
        }

        public async Task grid_OnBatchAdd(BeforeBatchAddArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_OnBatchAdd  " + args.Index + " 추가 X");
            // 데이터셋을 직접 조작하지 않고 아래 메소드로 사용하는 경우
            //  await 그리드.AddRecordAsync()
            // 2022.01.21 윤기선 실제 해당 화면에서 기본으로 세팅해줘야 하는 값이 있을 수 있으므로 OnBatchAdd 이벤트는 추가하지 않기로.
            args.Cancel = true;
            //_dataSet.InsertRow(args.DefaultData);
            //InvokeRefreshEvent();
        }

        public async Task grid_OnBatchDelete(BeforeBatchDeleteArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_OnBatchDelete  " + args.RowIndex);
            // 데이터셋을 직접 조작하지 않고 아래 메소드로 사용하는 경우
            // await grdPIB1030sf.DeleteRecordAsync("", ViewModel.oPIB1030[await DataSetControl.GetSelectedRowNumber<PIB1030>(grdPIB1030sf)]);
            _dataSet.DeleteRow(await DataSetControl.GetSelectedRowNumber(_grid));
            //InvokeRefreshEvent();
        }

        public async Task grid_OnBatchSave(BeforeBatchSaveArgs<T> args)
        {
            Console.WriteLine("[DataSetControl] grid_OnBatchSave");
            //InvokeRefreshEvent();
        }

        public void grid_RowDataBound(RowDataBoundEventArgs<T> args)
        {
            BaseModel lModel = args.Data;

            if (lModel.IUD == "I" || lModel.IUD == "U")
                args.Row.AddClass(new string[] { "InsertUpdateColor" });
        }

        public async Task grid_OnCellEdit(CellEditArgs<T> args) // PK인 경우 INSERT때만 수정 가능하게
        {
            if (_dataSet.primaryKey == null || _dataSet.primaryKey.Length <= 0) return;

            string lColumnName = args.ColumnName;
            int lRowNumber = (int)_dataSet.focusRowIndex;

            for (int i = 0; i < _dataSet.primaryKey.Length; i++)
            {
                if (_dataSet.primaryKey[i] == lColumnName && _dataSet.dataSet[lRowNumber].IUD != "I")
                {
                    Console.WriteLine("[DataSetControl] 이미 정의된 PK 컬럼값을 수정하려고 하고 있습니다.");
                    args.Cancel = true;
                    return;
                }
            }
        }




    }


    public class BasePageComponentControl
    {
        private IJSRuntime _JS;
        private UserConnectionInfo _UCI;
        private Models.AA.ADM.ProHomeModel.ProWindowLayout _ProWindowLayout;

        public BasePageComponentControl()
        {

        }
        /// <summary>
        /// 해당 페이지/컴포넌트/컨트롤의 기본 속성입니다. 파라매터를 잘넣어주세요.
        /// </summary>
        /// <param name="pJS">자바스크립트 런타임(IJSRuntime)</param>
        /// <param name="pUCI">UserConnectionInfo</param>
        /// <param name="pProWindowLayout">ProWindowLayout</param>
        /// <param name="pMenuBar">True : 해당 페이지의 하단 메뉴바를 활성화합니다.</param>
        public BasePageComponentControl(IJSRuntime pJS, UserConnectionInfo pUCI, Models.AA.ADM.ProHomeModel.ProWindowLayout pProWindowLayout, bool pMenuBar)
        {
            _JS = pJS;
            _UCI = pUCI;
            _ProWindowLayout = pProWindowLayout;
            pageMenuBar = pMenuBar;
        }

        // 현재 포커싱된 그리드
        public string focusGrid { get; set; } = "";
        public EventCallback grid_onfocusin(string pGridName)
        {
            focusGrid = pGridName;
            Console.WriteLine(focusGrid);
            return EventCallback.Empty;
        }

        // 페이지 시작 유무 OnAfterRenderAsync의 PageFirstStart 작동 이후 인지
        public bool pageStart { get; set; } = false;
        // 해당 페이지에서 하단 메뉴바 사용 유무
        public bool pageMenuBar { get; set; } = false;
        // 하단 메뉴바의 높이
        public double pageMenuBarHeight { get; set; } = 56;

        // MDI 모드로부터 화면이 호출된건지
        private bool _pageMDI = false;
        public bool pageMDI
        { 
            get
            {
                if (_ProWindowLayout is not null)
                    return _ProWindowLayout.IsMDI;
                else
                    return false;
            }
        }

        // 페이지/컴포넌트/컨트롤마다 고유 번호 부여
        private string _guid = Guid.NewGuid().ToString(); 
        public string guid
        {
            get { return _guid + "_"; }
        }

        private double _width = 0;
        public double width
        {
            get
            {
                if (_ProWindowLayout is not null && _ProWindowLayout.IsMDI == true)   // MDI 모드의 화면인경우
                {
                    _width = _ProWindowLayout.Width - 12;
                }
                else if(_UCI is not null)                                             // 아닌 경우
                {
                    _width = _UCI.ClientWidth;
                }

                return _width;
            }
        }

        private double _height = 0;
        public double height
        {
            get
            {
                if (_ProWindowLayout is not null && _ProWindowLayout.IsMDI == true)     // MDI 모드의 화면인경우
                {
                    if (pageMenuBar)
                        _height = _ProWindowLayout.Height - (45 + 8 + pageMenuBarHeight); // 하단메뉴바 높이까지
                    else
                        _height = _ProWindowLayout.Height - (45 + 8 + 4 );
                }
                else if (_UCI is not null)                  // MDI 모드가 아닌 경우
                {
                    if (pageMenuBar)                        // 하단메뉴바 유무
                        _height = _UCI.ClientHeight - (56 + pageMenuBarHeight - 4);     // 상단메뉴바 + 하단메뉴바 높이만큼 빼기
                    else
                        _height = _UCI.ClientHeight - 56;   // 상단메뉴바 높이만큼 빼기
                }

                return _height;
            }
        }

    }
}
