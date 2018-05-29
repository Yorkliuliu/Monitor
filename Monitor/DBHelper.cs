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
    /// �࣬�������ݷ��ʵ��ࡣ
    /// </summary>
    public class Database : IDisposable 
    {


        /// <summary>
        /// �������������ݿ����ӡ�
        /// </summary>
        protected SqlConnection Connection;
        public SqlConnection mConnection;




        /// <summary>
        /// �������������ݿ����Ӵ���
        /// </summary>
        protected String ConnectionString;
        public String mConnectionString;




        /// <summary>
        /// ���ݿ�������쳣��Ϣ�������������Ŵ�
        /// </summary>
        public string errMessage="";
        




        /// <summary>
        /// ���캯������ͬ�Ĳ�������ͬ�����ݿ�������Ӻ�Ȩ�ޣ�"administerConnection"Ϊ����ԱȨ�ޣ�"userConnection"Ϊ�⣨�ԣ��û�Ȩ�ޣ�"expertConn"Ϊר��Ȩ��
        /// "otherConn"Ϊ��½ʱ��֤Ȩ�ޡ�
        /// </summary>
        /// <param name="DBAccessRole">���ݿ����Ȩ�ޣ�"administerConnection"Ϊ����ԱȨ�ޣ�"userConnection"Ϊ�⣨�ԣ��û�Ȩ�ޣ�"expertConn"Ϊר��Ȩ��
        /// "otherConn"Ϊ��½ʱ��֤Ȩ�ޡ�</param>
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
        /// �����������ͷŷ��й���Դ
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
        /// ���������������ݿ����ӡ�
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
        /// ���з������ر����ݿ����ӡ�
        /// </summary>
        public void Close() 
        {
            if (Connection != null)
                Connection.Close();
        }





        /// <summary>
        /// ���з������ͷ���Դ��
        /// </summary>
        public void Dispose() 
        {
            // ȷ�����ӱ��ر�
            if (Connection != null) 
            {
                Connection.Dispose();
                Connection = null;
            }                
        }
        



        /// <summary>
        /// ���з�������ȡ���ݣ�����һ��SqlDataReader �����ú��������SqlDataReader.Close()����
        /// </summary>
        /// <param name="SqlString">Sql���</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader GetDataReader(String SqlString)
        {
            Open();
            SqlCommand cmd = new SqlCommand(SqlString,Connection);
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        



        /// <summary>
        /// ���з�������ȡ���ݣ�����һ��DataSet��
        /// </summary>
        /// <param name="SqlString">Sql���</param>
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
        /// ���з�������ȡ���ݣ�����һ��DataRow��
        /// </summary>
        /// <param name="SqlString">Sql���</param>
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
        /// ���з�����ִ��Sql��䡣
        /// </summary>
        /// <param name="SqlString">Sql���</param>
        /// <returns>��Update��Insert��DeleteΪӰ�쵽���������������Ϊ-1</returns>
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
        /// ���з�����ִ��һ��Sql��䡣
        /// </summary>
        /// <param name="SqlStrings">Sql�����</param>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// ���з�������һ�����ݱ��в���һ����¼��
        /// </summary>
        /// <param name="TableName">����</param>
        /// <param name="Cols">��������ֵΪ�ֶ�����ֵΪ�ֶ�ֵ</param>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// ���з���������һ�����ݱ�
        /// </summary>
        /// <param name="TableName">����</param>
        /// <param name="Cols">��������ֵΪ�ֶ�����ֵΪ�ֶ�ֵ</param>
        /// <param name="Where">Where�Ӿ�</param>
        /// <returns>�Ƿ�ɹ�</returns>
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
        /// �� sqlStr ��ѯ���ݰ󶨵� gridView
        /// </summary>
        /// <param name="sqlStr">SQL��ѯ���</param>
        /// <param name="gridView">GirdView Web �ؼ�</param>
        /*public void bind2GridView(string sqlStr, System.Web.UI.WebControls.GridView gridView)
        { 
            DataSet ds = GetDataSet(sqlStr, "baseInforTable");
            gridView.DataSource = ds.Tables["baseInforTable"].DefaultView;
            //gridView.DataKeyNames = new string[] { "baseID" };
            gridView.DataBind();        
        }*/






        /// <summary>
        /// �ı������밲ȫ�Լ��
        /// </summary>
        /// <param name="mTextBox">�ı������</param>
        /// <param name="minLength">��С�ַ�����</param>
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
