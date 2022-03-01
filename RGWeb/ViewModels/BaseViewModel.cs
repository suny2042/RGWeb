using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RGWeb.DB.Common;
using RGWeb.Shared.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using RGWeb.Shared;

namespace RGWeb.ViewModels
{
    // 뷰모델들의 기본이 되는 뷰모델로써(상속) 뷰모델이 반드시 가지고 있어야할 요소들을 적재함
    // 뷰모델은 INotify를 구현해야함 (모델의 데이터가 변경되면 바뀐 사항을 뷰에서 바뀐 사항으로 보여주어야 하므로)
    public class BaseViewModel : INotifyPropertyChanged
    {
        // 데이터 뮤텍스 (락)
        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                Console.WriteLine("busy");
                SetValue(ref isBusy, value);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // 바뀐 사항이 있는지 감지.  = null 은 바뀐 사항이 없는 경우 트리거 안되게.
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Console.WriteLine("OnPropertyChanged : " + propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
        {
            Console.WriteLine("SetValue : " + propertyName);
            if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return;
            backingFiled = value;
            OnPropertyChanged(propertyName);
        }

        
        private BaseViewModelInfo _baseViewModelInfo = new();
        public BaseViewModelInfo baseViewModelInfo
        {
            get => _baseViewModelInfo;
        }
        
    }

    // BaseViewModel에서 추가하고 싶은 사용자 정의 클래스/메소드
    public class BaseViewModelInfo
    {
        // SfGrid RefreshEvent 그리드 상태 다시 그리기 (타컴포넌트간 이벤트 발생용)
        //private int? _refreshEventCount = 0; // 2022.01.13 윤기선 테스트용 사용 x
        //public int? refreshEventCount
        //{
        //    get
        //    {
        //        if (_refreshEventCount > 1000000)
        //            _refreshEventCount = 1;
        //        return _refreshEventCount;
        //    }
        //    set
        //    {
        //        _refreshEventCount = value;
        //    }
        //}
    }

    /// <summary>
    /// 그리드에 바인딩 하기 위한 모델기반 데이터셋입니다.
    /// </summary>
    /// <typeparam name="T">기준되는 모델</typeparam>
    public class ModelDataSet<T> : BaseViewModel where T : BaseModel, new()
    {
        public ModelDataSet()
        {

        }
        /// <summary>
        /// 특정 컬럼들에게 기본키(PK)를 부여합니다. 해당되는 컬럼들을 INSERT시에만 수정가능하며 UPDATE시에는 수정 불가능합니다.
        /// </summary>
        /// <param name="pPrimaryKey">해당 테이블의 기본키. 구분자는 ';'입니다.</param>
        public ModelDataSet(string pPrimaryKey)
        {
            _primaryKey = pPrimaryKey;
        }

        private string _primaryKey = null; // 해당 데이터셋의 기본키. "AAAAAA;BBBBBB;CCCCCC;" 등으로 PK 컬럼이 담김
        public string[] primaryKey
        {
            get
            {
                if (_primaryKey == null)
                    return null;
                else
                    return _primaryKey.Split(';');
            }
        }

        public double focusRowIndex { get; set; }
        public double focusColumnIndex { get; set; }
        public string focusColumnIndexName { get; set; }

        private ObservableCollection<T> _dataSet = new ObservableCollection<T>();
        public ObservableCollection<T> dataSet
        {
            get => _dataSet;
            set
            {   // 새로 조회 했을 때
                SetValue(ref _dataSet, value);
                // 삭제대기 리스트 비우기
                _dataSet_Delete.Clear();
                // (조회할 당시에)원본 리스트 새로 만들기
                _dataSet_Original = Copy(_dataSet);

                Console.WriteLine("_dataSet seted");
            }
        }

        private ObservableCollection<T> _dataSet_Delete = new ObservableCollection<T>();
        /// <summary>
        /// IUD 처리시 삭제할 데이터들을 임시 저장하는 리스트입니다. IUD가 'D'로 세팅되며 DB에 저장시 해당 레코드들은 삭제가 진행됩니다.
        /// 가능하면 직접 조작하지 않고 메소드를 사용해주세요.
        /// </summary>
        public ObservableCollection<T> dataSet_Delete
        {
            get => _dataSet_Delete;
            set => SetValue(ref _dataSet_Delete, value);
        }

        private ObservableCollection<T> _dataSet_Original = new ObservableCollection<T>();
        /// <summary>
        /// 데이터베이스에서 조회할 당시의 원본 데이터셋 입니다. 조회와 비교용으로 사용됩니다.
        /// </summary>
        public ObservableCollection<T> dataSet_Original
        {
            get => _dataSet_Original;
        }


        #region [ 데이터셋 IUD 로직 ]

        /// <summary>
        /// 데이터셋을 (깊은)복사하여 반환합니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pDataSet">복사할 데이터셋</param>
        /// <returns>복사받을 데이터셋</returns>
        public static ObservableCollection<T> Copy(ObservableCollection<T> pDataSet)
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
        /// 데이터셋의 특정 row를 삭제하고 삭제대기 리스트에 특정 row를 추가합니다. 인덱스가 모호한경우 삭제를 하지 않습니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pIndex">삭제할 위치</param>
        public void DeleteRow(int pIndex)
        {
            if (_dataSet.Count == 0) return;

            if (pIndex > _dataSet.Count) return;
            else if (pIndex < 0) return; //pIndex = pModel.Count;

            T target = _dataSet[pIndex];
            if (_dataSet[pIndex].IUD == "I")
            {
                // 추가중이었던 row인 경우 데이터셋에서 삭제만 하고 종료
                _dataSet.RemoveAt(pIndex);
                return;
            }
            else
                target.IUD = "D"; // IUD 컬럼을 D 로 세팅

            // 삭제리스트에 IUD를 'D'로 세팅된 레코드를 추가
            _dataSet_Delete.Add(target);
            // 데이터셋에 데이터삭제
            _dataSet.RemoveAt(pIndex);
        }

        /// <summary>
        /// 특정 row를 데이터셋의 원하는 위치에 추가합니다. 인덱스가 없는 경우 제일 끝에 추가합니다.
        /// 동시에 연속으로 사용할 경우 그리드 갱신 이벤트로 인하여 표시가 불안정해질 수 있습니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pRow">추가할 row(모델)</param>
        /// <param name="pIndex">추가할 위치</param>
        /// <returns>추가된 row 인덱스 번호</returns>
        public int InsertRow(T pRow, int? pIndex = null)
        {
            if (pIndex != null)
            {
                if (0 >= _dataSet.Count) pIndex = 0;
                else if (pIndex > _dataSet.Count) pIndex = 0;
                else if (pIndex < 0) pIndex = _dataSet.Count;
            }

            // IUD 컬럼을 I 로 세팅
            pRow.GUID = Guid.NewGuid().ToString();
            pRow.IUD = "I";

            if (pIndex == null) // 인덱스 파라매터를 안넣은 경우 맨 끝으로 추가
            {
                pIndex = _dataSet.Count;
                _dataSet.Add(pRow);
            }
            else                // 인덱스 파라매터를 넣은 경우 해당 위치에 추가
                _dataSet.Insert((int)pIndex, pRow);

            return (int)pIndex;
        }

        /// <summary>
        /// row 리스트를 특정한 위치에 한 번에 삽입합니다. 인덱스가 없는 경우 제일 끝에 추가합니다.
        /// InsertRow를 동시에 여러번 사용할 경우 그리드 갱신 이벤트가 연속으로 발생하므로 해당 메소드가 고안되었습니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <param name="pRowList">해당 모델 타입의 List</param>
        /// <param name="pIndex">추가할 위치</param>
        /// <returns></returns>
        public void InsertRowList(List<T> pRowList, int? pIndex = null)
        {
            ObservableCollection<T> newModel = new ObservableCollection<T>();
            ObservableCollection<T> newModel_Delete = new ObservableCollection<T>();

            if (pIndex != null)
            {
                if (0 >= _dataSet.Count) pIndex = 0;
                else if (pIndex > _dataSet.Count) pIndex = 0;
                else if (pIndex < 0) pIndex = _dataSet.Count;
            }
            // IUD 컬럼을 I 로 세팅
            for (int i = 0; i < pRowList.Count; i++)
            {
                pRowList[i].GUID = Guid.NewGuid().ToString();
                pRowList[i].IUD = "I";
            }

            if (pIndex == null) // 인덱스 파라매터를 안넣은 경우 맨 끝으로 추가
            {
                for (int i = 0; i < _dataSet.Count; i++)
                    newModel.Add(_dataSet[i]);
                for (int i = 0; i < pRowList.Count; i++)
                    newModel.Add(pRowList[i]);
            }
            else                // 인덱스 파라매터를 넣은 경우 해당 위치에 추가
            {
                for (int i = 0; i < pIndex; i++)
                    newModel.Add(_dataSet[i]);
                for (int i = pRowList.Count - 1; i >= 0; i--)
                    newModel.Insert((int)pIndex, pRowList[i]);
                for (int i = (int)pIndex; i < _dataSet.Count; i++)
                    newModel.Add(_dataSet[i]);
            }

            // 삭제목록 옮기기
            for (int i = 0; i < _dataSet_Delete.Count; i++)
                newModel_Delete.Add(_dataSet_Delete[i]);

            _dataSet = newModel;
            _dataSet_Delete = newModel_Delete;
        }

        // 2021.08.24 윤기선 데이터테이블을 해당 모델에 있는 프로퍼티를 전부 방문하여 그(해당 그리드의 Data 속성 등)에 맞게 동적세팅
        /// <summary>
        /// 그리드 바인딩을 위해 데이터 테이블을 모델에 바인딩합니다.
        /// </summary>
        /// <typeparam name="T">모델 타입</typeparam>
        /// <param name="pDataTable">DB로 부터 읽어온 데이터테이블</param>
        /// <returns>바인딩된 결과가 Null인 경우 False를 반환합니다.</returns>
        public bool BindDataSet(DataTable pDataTable)
        {
            ObservableCollection<T> lModel = new ObservableCollection<T>();

            if (pDataTable == null || pDataTable.Rows.Count == 0)
            {
                dataSet.Clear();
                dataSet_Delete.Clear();
                dataSet_Original.Clear();
                return false;
            }


            for (int i = 0; i < pDataTable.Rows.Count; i++)
            {
                T model = new T();

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

                            object value = null;
                            foreach (DataColumn col in pDataTable.Rows[i].Table.Columns)
                                if (name == col.ColumnName)
                                    value = pDataTable.Rows[i][name].ToString();

                            //if (string.IsNullOrEmpty(value)) continue;
                            if (value == null) continue;

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
                            object value = null;
                            foreach (DataColumn col in pDataTable.Rows[i].Table.Columns)
                                if (name == col.ColumnName)
                                    value = pDataTable.Rows[i][name].ToString();

                            //if (string.IsNullOrEmpty(value)) continue;
                            if (value == null) continue;

                            propertyInfo.SetValue(model, value);
                        }
                    }
                }

                lModel.Add(model);
            }

            dataSet = lModel; // 직접 접근이 아니라 프로퍼티를 통하므로 PropertyChanged 이벤트를 타게 됨.
            return true;

            #region [ 정적바인딩 예시 ]
            // 위는 동적바인딩이고 아래는 정적바인딩의 예시임
            //for (int i = 0; i < pDataTable.Rows.Count; i++)
            //{
            //    lModel.Add(new OMB1010()
            //    {
            //        DISP_RGSTTM = pDataTable.Rows[i]["DISP_RGSTTM"].ToString(),
            //        DISP_PATINFO = pDataTable.Rows[i]["DISP_PATINFO"].ToString(),
            //        DIS_PRHOPNM = pDataTable.Rows[i]["DIS_PRHOPNM"].ToString(),
            //        DISP_DOCTNM = pDataTable.Rows[i]["DISP_DOCTNM"].ToString(),
            //        DISP_PHONNO = pDataTable.Rows[i]["DISP_PHONNO"].ToString(),
            //        DISP_ADDRNM = pDataTable.Rows[i]["DISP_ADDRNM"].ToString(),
            //        REGKEY = pDataTable.Rows[i]["REGKEY"].ToString(),
            //    });
            //}
            #endregion
        }

        // 2021.11.10 윤기선 저장할 데이터셋(모델)을 받아서 DataTable 타입으로 만들어줌
        /// <summary>
        /// 데이터셋의 데이터들을 데이터테이블로 만들어 반환합니다. 저장 프로시저를 위하여 DELETE 그리고 INSERT, UPDATE 상태별로 테이블을 만듭니다.
        /// (다른 모델 참조 등으로) 중복되는 컬럼이 있을 경우, Null이 아닌 컬럼의 것으로 중복되는 모든 컬럼을 같은 값으로 덮어씁니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        /// <returns>저장 프로시저를 위한 데이터 테이블</returns>
        public DataTable GetDataTable() // 2021.12.28 윤기선 다른 모델 지원으로, 다른모델의 같은이름 컬럼이 있는 경우 값이 있으면 덮어씀
        {
            DataTable Dt = new DataTable();
            HashSet<string> lColumnList = new HashSet<string>();

            // 데이터 테이블 정의  (모델에 정의되어있는 컬럼대로 데이터 테이블 컬럼 추가)
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = propertyInfo.Name;
                if (string.IsNullOrEmpty(name)) continue;
                if (name == "GUID") continue; // GUID 고유번호 컬럼은 스킵

                if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                {
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                    foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        lColumnList.Add(propertyInfoInner.Name); // 중복걸러내고 추가
                    }
                }
                else
                    lColumnList.Add(name); // 중복걸러내고 추가
            }
            for (int i = 0; i < lColumnList.Count; i++)
                Dt.Columns.Add(lColumnList.ElementAt(i), typeof(string));

            // 모델에 있는 데이터셋을 (Delete 먼저) 데이터 테이블에 세팅
            for (int i = 0; i < _dataSet_Delete.Count; i++)
            {
                // IUD 컬럼이 D 말고 다른 것은 무시
                if (_dataSet_Delete[i].IUD != "D") continue;

                // 모델에 정의되어 있는 컬럼대로 데이터 row 추가
                DataRow lDataRow = Dt.NewRow();
                foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (propertyInfo.Name == "GUID") continue; // GUID 고유번호 컬럼은 스킵

                    if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                    {
                        object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (lDataRow[propertyInfoInner.Name] == System.DBNull.Value) // 현재 해당 컬럼이 Null이면 세팅
                                lDataRow[propertyInfoInner.Name] = (propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet_Delete[i])) == null ? null : propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet_Delete[i])).ToString());
                        }
                    }
                    else
                        lDataRow[propertyInfo.Name] = (propertyInfo.GetValue(_dataSet_Delete[i]) == null ? null : propertyInfo.GetValue(_dataSet_Delete[i]));
                    // memo) DateTime 등 타입에 따라 변환을 해줘야 할 수도 있음
                }

                Dt.Rows.Add(lDataRow);
            }

            // 이어서 Insert 와 Update 데이터를 테이블에 세팅
            for (int i = 0; i < _dataSet.Count; i++)
            {
                // IUD 컬럼이 I , U 말고 다른 것은 무시
                if (!(_dataSet[i].IUD == "U" || _dataSet[i].IUD == "I")) continue;

                DataRow lDataRow = Dt.NewRow();
                // 모델에 정의되어 있는 컬럼대로 데이터 row 추가
                foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (propertyInfo.Name == "GUID") continue; // GUID 고유번호 컬럼은 스킵

                    if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                    {
                        object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (lDataRow[propertyInfoInner.Name] == System.DBNull.Value) // 현재 해당 컬럼이 Null이면 세팅
                                lDataRow[propertyInfoInner.Name] = (propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet[i])) == null ? null : propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet[i])).ToString());
                        }
                    }
                    else
                        lDataRow[propertyInfo.Name] = (propertyInfo.GetValue(_dataSet[i]) == null ? null : propertyInfo.GetValue(_dataSet[i]));
                    // memo) DateTime 등 타입에 따라 변환을 해줘야 할 수도 있음
                }

                Dt.Rows.Add(lDataRow);
            }

            return Dt;
        }

        /// <summary>
        /// 데이터셋을 모두 클리어 합니다.
        /// </summary>
        public void ClearDataset()
        {
            _dataSet.Clear();
            _dataSet_Delete.Clear();
            _dataSet_Original.Clear();
        }

        /// <summary>
        /// 데이터셋의 변경사항(I/U/D)을 초기화합니다. 그리드의 변경사항을 데이터베이스에 반영 후, 재조회를 원치않을 경우에 사용합니다.
        /// </summary>
        /// <typeparam name="T">해당 데이터셋의 모델</typeparam>
        public void ResetUpdatedDataset()
        {
            for (int i = 0; i < _dataSet.Count; i++)
                _dataSet[i].IUD = "N";

            // 원본 리스트 새로 만들기
            _dataSet_Original = Copy(_dataSet);
            _dataSet_Delete.Clear();
        }

        /// <summary>
        /// 데이터셋의 특정 row와 Column의 Cell을 수정합니다. 수정시 IUD값을 'U'로 세팅(INSERT 제외)합니다.
        /// 그리드와 바인딩 또는 DB에 업데이트시킬 데이터인 경우 이 메소드로 값을 수정해야 합니다.
        /// </summary>
        /// <param name="pRowNumber">해당 인덱스 번호</param>
        /// <param name="pColumnName">컬럼명</param>
        /// <param name="pNewData">바꿀 데이터 값</param>
        /// <param name="pOldData">이전(Previous) 값을 파라매터에 넣을 경우, 바꿀 데이터 값과 비교하여 수정사항이 없으면 아무것도 하지 않습니다.</param>
        /// <returns>바뀐 경우(IUD가 'U'로 세팅된 경우) True를 반환합니다.</returns>
        public bool SetDataCell(int pRowNumber, string pColumnName, object pNewData, object? pOldData = null)
        {
            bool lResult = false;
            if (pRowNumber < 0 || string.IsNullOrEmpty(pColumnName)) return false;
            if (pOldData != null && (pNewData == pOldData)) return false;

            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = propertyInfo.Name;
                if (string.IsNullOrEmpty(name)) continue;

                if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                {
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                    foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        string innername = propertyInfoInner.Name;
                        if (pColumnName == innername)
                            propertyInfoInner.SetValue(propertyInfo.GetValue(dataSet[(int)pRowNumber]), pNewData);
                    }
                }
                else
                {
                    if (pColumnName == name)
                        propertyInfo.SetValue(dataSet[(int)pRowNumber], pNewData);
                }
            }

            if (dataSet[(int)pRowNumber].IUD != "I" && IsChangedDataCell(pRowNumber, pColumnName))
            {
                dataSet[(int)pRowNumber].IUD = "U";
                lResult = true;
            }

            OnPropertyChanged(pColumnName);

            return lResult;
        }

        /// <summary>
        /// 해당 Cell에 변동사항이 있는 경우 True를 반환합니다.
        /// </summary>
        /// <param name="pRowNumber"></param>
        /// <param name="pColumnName"></param>
        /// <returns></returns>
        public bool IsChangedDataCell(int pRowNumber, string pColumnName)
        {
            if (pRowNumber < 0 || string.IsNullOrEmpty(pColumnName)) return false;

            int tRow = -1;
            for (int i = 0; i < _dataSet_Original.Count; i++)
            {
                // IUD 상태가 있는 것들은 무시
                if (_dataSet[pRowNumber].IUD == "I" || _dataSet[pRowNumber].IUD == "U" || _dataSet[pRowNumber].IUD == "D")
                    continue;

                // 일치하는 레코드 찾음
                if (_dataSet[pRowNumber].GUID == _dataSet_Original[pRowNumber].GUID)
                    tRow = i;
            }

            // 원본에서 일치하는 row를 못찾은 경우 추가되었다는 뜻이므로 True 반환
            if (tRow == -1) return true;

            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                {
                    object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                    foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        // 원본과 비교하여 변경된 경우 (문자열로 변환하여 비교함)
                        if (ComLib.fnNVL(propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet[pRowNumber])), "") != ComLib.fnNVL(propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet_Original[tRow])), ""))
                            return true;
                    }
                }
                else
                {
                    if (propertyInfo.Name == pColumnName)
                    {
                        // 원본과 비교하여 변경된 경우 (문자열로 변환하여 비교함)
                        if (ComLib.fnNVL(propertyInfo.GetValue(_dataSet[pRowNumber]), "") != ComLib.fnNVL(propertyInfo.GetValue(_dataSet_Original[tRow]), ""))
                            return true;
                    }
                    else
                        continue;
                }
            }

            return false;
        }

        /// <summary>
        /// 해당 Row의 변동사항이 (오리지널과 비교하여) 없을 시 IUD를 "N"값으로 세팅합니다.
        /// </summary>
        /// <param name="pRow">해당 인덱스 번호</param>
        /// <returns>변동사항이 없는 경우 True를 반환합니다.</returns>
        public bool SetChangedDataRowState(int pRow)
        {
            // 일치하는 오리지널 찾기
            int tRow = -1;
            for (int k = 0; k < _dataSet_Original.Count; k++)
                if (_dataSet[pRow].GUID == _dataSet_Original[k].GUID)
                    tRow = k;

            bool lEq = true;
            if (tRow >= 0) // 일치하는 오리지널 레코드가 있는 경우
            {
                foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (propertyInfo.Name == "IUD") continue;

                    if (propertyInfo.PropertyType.BaseType.Name == "BaseModel") // 2021.12.28 윤기선 모델 내에 다른 (AA/ 등)모델이 정의되어 있는 경우 
                    {
                        object instance = Activator.CreateInstance(propertyInfo.PropertyType);
                        foreach (PropertyInfo propertyInfoInner in propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            // 해당 Row의 Column중 하나라도 원본과 안맞으면 불일치
                            if (ComLib.fnNVL(propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet[pRow])), "") != ComLib.fnNVL(propertyInfoInner.GetValue(propertyInfo.GetValue(_dataSet_Original[tRow])), ""))
                                lEq = false;
                        }
                    }
                    else
                    {
                        // 해당 Row의 Column중 하나라도 원본과 안맞으면 불일치
                        if (ComLib.fnNVL(propertyInfo.GetValue(_dataSet[pRow]), "") != ComLib.fnNVL(propertyInfo.GetValue(_dataSet_Original[tRow]), ""))
                            lEq = false;
                    }
                    
                }

                if (lEq)   // 오리지널과 똑같다면 IUD를 변동사항 없음으로 설정
                    _dataSet[pRow].IUD = "N";
            }
            else           // 일치하는 레코드가 없는 경우 추가이므로
                lEq = false;
            
            return lEq;
        }

        #endregion

        #region [ 그리드 관련 ]

        #endregion

    }
}
