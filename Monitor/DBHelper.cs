using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;




namespace Monitor
{
    /// <summary>
    /// 类，用于数据访问的类。
    /// </summary>
    public class Database : IDisposable 
    {


        /// <summary>
        /// 保护变量，数据库连接。
        /// </summary>
        protected SqlConnection Connection;
        public SqlConnection mConnection;




        /// <summary>
        /// 保护变量，数据库连接串。
        /// </summary>
        protected String ConnectionString;
        public String mConnectionString;




        /// <summary>
        /// 数据库操作中异常信息反馈，有助于排错
        /// </summary>
        public string errMessage="";
        




        /// <summary>
        /// 构造函数，不同的参数代表不同的数据库访问连接和权限："administerConnection"为管理员权限，"userConnection"为库（圃）用户权限，"expertConn"为专家权限
        /// "otherConn"为登陆时验证权限。
        /// </summary>
        /// <param name="DBAccessRole">数据库访问权限："administerConnection"为管理员权限，"userConnection"为库（圃）用户权限，"expertConn"为专家权限
        /// "otherConn"为登陆时验证权限。</param>
        public Database(string DBAccessRole)
        {
            if (DBAccessRole == "administerConnection")
                ConnectionString=ConfigurationManager.AppSettings["administerConn"];
            else if (DBAccessRole == "userConnection")
                ConnectionString = ConfigurationManager.AppSettings["userConn"];
            else if (DBAccessRole == "expertConnection")
                ConnectionString = ConfigurationManager.AppSettings["expertConn"];
            else
                ConnectionString = ConfigurationManager.AppSettings["other"];
            mConnectionString = ConnectionString;
        }





        /// <summary>
        /// 析构函数，释放非托管资源
        /// </summary>
        ~Database()
        {
            try
            {
                if (Connection != null)
                    Connection.Close();
            }
            catch
            {

            }
            finally
            {
                GC.Collect();
                Dispose();
            }
        }




        /// <summary>
        /// 保护方法，打开数据库连接。
        /// </summary>
        protected void Open() 
        {

            if (Connection == null)
            {
                Connection = new SqlConnection(ConnectionString);
            }
            if (Connection.State.Equals(ConnectionState.Closed))
            {
                Connection.Open();
            }

        }




        /// <summary>
        /// 公有方法，关闭数据库连接。
        /// </summary>
        public void Close() 
        {
            if (Connection != null)
                Connection.Close();
        }





        /// <summary>
        /// 公有方法，释放资源。
        /// </summary>
        public void Dispose() 
        {
            // 确保连接被关闭
            if (Connection != null) 
            {
                Connection.Dispose();
                Connection = null;
            }                
        }
        



        /// <summary>
        /// 公有方法，获取数据，返回一个SqlDataReader （调用后主意调用SqlDataReader.Close()）。
        /// </summary>
        /// <param name="SqlString">Sql语句</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader GetDataReader(String SqlString)
        {
            Open();
            SqlCommand cmd = new SqlCommand(SqlString,Connection);
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        



        /// <summary>
        /// 公有方法，获取数据，返回一个DataSet。
        /// </summary>
        /// <param name="SqlString">Sql语句</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(String SqlString,String Sqlobj)
        {
            Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SqlString,Connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset,Sqlobj);
            Close();
            return dataset;
        }
        public DataSet GetDataSet(String SqlString)
        {
            Open();
            SqlDataAdapter adapter = new SqlDataAdapter(SqlString, Connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            Close();
            return dataset;
        }





        /// <summary>
        /// 公有方法，获取数据，返回一个DataRow。
        /// </summary>
        /// <param name="SqlString">Sql语句</param>
        /// <returns>DataRow</returns>
        public DataRow GetDataRow(String SqlString,String Sqlobj)
        {
            DataSet dataset = GetDataSet(SqlString,Sqlobj);
            dataset.CaseSensitive = false;
            if (dataset.Tables[0].Rows.Count>0)
            {
                return dataset.Tables[0].Rows[0];
            }
            else
            {
                return null;
            }
        }






        /// <summary>
        /// 公有方法，执行Sql语句。
        /// </summary>
        /// <param name="SqlString">Sql语句</param>
        /// <returns>对Update、Insert、Delete为影响到的行数，其他情况为-1</returns>
        public int ExecuteSQL(String SqlString)
        {
            int count = -1;
            Open();
            try
            {
                SqlCommand cmd = new SqlCommand(SqlString,Connection);
                count = cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                count = -1;
                errMessage = e.Message.ToString();
            }
            finally
            {
                Close();
            }
            return count;
        }





        /// <summary>
        /// 公有方法，执行一组Sql语句。
        /// </summary>
        /// <param name="SqlStrings">Sql语句组</param>
        /// <returns>是否成功</returns>
        public bool ExecuteSQL(ArrayList SqlStrings)
        {
            bool success = true;
            Open();
            SqlCommand cmd = new SqlCommand();
            SqlTransaction trans = Connection.BeginTransaction();
            cmd.Connection = Connection;
            cmd.Transaction = trans;
            try
            {
                foreach (String str in SqlStrings)
                {
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch
            {
                success = false;
                trans.Rollback();                
            }
            finally
            {
                Close();
            }
            return success;
        }





        /// <summary>
        /// 公有方法，在一个数据表中插入一条记录。
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Cols">哈西表，键值为字段名，值为字段值</param>
        /// <returns>是否成功</returns>
        public bool Insert(String TableName,Hashtable Cols)
        {
            int Count = 0;

            if (Cols.Count<=0)            
            {
                return true;
            }

            String Fields = " (";
            String Values = " Values(";            
            foreach(DictionaryEntry item in Cols)
            {
                if (Count!=0)
                {
                    Fields += ",";
                    Values += ",";
                }
                Fields += item.Key.ToString();
                Values += item.Value.ToString();
                Count ++;
            }
            Fields += ")";
            Values += ")";

            String SqlString = "Insert into "+TableName+Fields+Values;

            return Convert.ToBoolean(ExecuteSQL(SqlString));
        }

        




        /// <summary>
        /// 公有方法，更新一个数据表。
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="Cols">哈西表，键值为字段名，值为字段值</param>
        /// <param name="Where">Where子句</param>
        /// <returns>是否成功</returns>
        public bool Update(String TableName,Hashtable Cols,String Where)
        {
            int Count = 0;
            if (Cols.Count<=0)            
            {
                return true;
            }
            String Fields = " ";
            foreach(DictionaryEntry item in Cols)
            {
                if (Count!=0)
                {
                    Fields += ",";
                }
                Fields += item.Key.ToString();
                Fields += "=";
                Fields += item.Value.ToString();
                Count ++;
            }
            Fields += " ";

            String SqlString = "Update "+TableName+" Set "+Fields+Where;

            return Convert.ToBoolean(ExecuteSQL(SqlString));
        }





        /// <summary>
        /// 把 sqlStr 查询数据绑定到 gridView
        /// </summary>
        /// <param name="sqlStr">SQL查询语句</param>
        /// <param name="gridView">GirdView Web 控件</param>
        /*public void bind2GridView(string sqlStr, System.Web.UI.WebControls.GridView gridView)
        { 
            DataSet ds = GetDataSet(sqlStr, "baseInforTable");
            gridView.DataSource = ds.Tables["baseInforTable"].DefaultView;
            //gridView.DataKeyNames = new string[] { "baseID" };
            gridView.DataBind();        
        }*/






        /// <summary>
        /// 文本框输入安全性检查
        /// </summary>
        /// <param name="mTextBox">文本输入框</param>
        /// <param name="minLength">最小字符长度</param>
        /// <returns></returns>
        /*public bool isSecure(System.Web.UI.WebControls.TextBox mTextBox,int minLength)
        {
            string s = mTextBox.Text.Trim();
            s = s.Replace("<","");  s = s.Replace(">","");
            s = s.Replace("{", ""); s = s.Replace("}", "");
            s = s.Replace("[", ""); s = s.Replace("]", "");
            s = s.Replace("%", ""); s = s.Replace("?", "");
            s = s.Replace("&", ""); s = s.Replace(";", "");
            s = s.Replace("'", ""); s = s.Replace("\"", "");
            if (s.Length < minLength)
            {
                return false;
            }
            return true;
        }*/
    }
}
