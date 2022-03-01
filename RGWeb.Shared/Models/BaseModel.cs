namespace RGWeb.Shared.Models
{
    public class BaseModel
    {
        /// <summary>
        /// 해당 레코드의 IUD(INSERT, UPDATE, DELETE)값을 나타냅니다.
        /// </summary>
        public string IUD { get; set; }
        /// <summary>
        /// 레코드(Row)끼리의 식별되는 고유번호입니다.
        /// </summary>
        public string GUID { get; set; }
    }
}
