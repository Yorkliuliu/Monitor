using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Monitor
{

    /// <summary>
    /// 本地信息录入，包括城市信息、学校信息、教室信息以及电脑信息
    /// </summary>
    public partial class frmRegister : Form
    {
        // 定义必要的数据操作对象
        Process[] myProcess = Process.GetProcesses();
        int rowsDeleted = 9999;
        Database mDB = new Database("other");
        string sql = "myinsert";//要调用的存储过程名


        public frmRegister()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 当手动点击关闭按钮时，窗体最小化到托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmRegister_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)                //当用户点击窗体右上角X按钮或(Alt + F4)时 发生
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }




        //定义全局变量
        public class global
        {
            #region "变量定义区"
            public static string CityID = "";                     //城市编号
            public static string CityName = "";                   //城市名
            public static string CountyName = "";                 //区/县名
            public static string SchoolID = "";                   //学校编号
            public static string SchoolName = "";                 //学校名
            public static string SchoolKind = "";                 //学校类别
            public static string SchoolQuality = "";              //学校性质
            public static string ClassID = "";                    //教室编号
            public static string ClassAddress = "";               //教室地址
            public static string ComputerID = "";                 //电脑编号
            public static string CPU = "";                        //电脑CPU序列号
            public static int i = 0;
            #endregion     private global()        {            //            // TODO: 此处添加构造函数逻辑            //        }
        }




        /// <summary>
        /// 将已填入信息写入数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            //如果有的信息没有填入，则出现提示
            if(txtComputerID.Text=="" )
            {
                MessageBox.Show("所有信息不能为空！请再填入一次！");
            }


            //获取本机CPU序列号
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                String strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                global.CPU = strCpuID;
            }
            catch
            {
                global.CPU = "";
            }
            


            //信息如果全部填入，则将数据写入数据库，并传递给全局变量传给下一个窗体
            global.CityID = cmbCityID.Text;                            //城市编号
            global.CityName = cmbCityName.Text;                        //城市名
            global.CountyName = cmbCounty.Text;                        //区县名
            global.SchoolID = cmbSchoolID.Text;                        //学校编号
            global.SchoolName = cmbSchoolName.Text;                    //学校名
            global.SchoolKind = cmbSchoolKind.Text;                    //学校类型
            global.SchoolQuality = cmbSchoolQuality.Text;              //学校性质
            global.ClassID = cmbClassID.Text;                          //教室编号
            global.ClassAddress = cmbClassAddress.Text;                //教室地址
            global.ComputerID = txtComputerID.Text;                    //电脑编号


            /* 
            //调用存储过程
            //将城市信息写入数据库
            //String sqlString = "insert into tArea(cityID,cityName,countyName) values(" + global.CityID + "," + global.CityName + "," + global.CountyName + ")";
            String sqlString = "exec insert InsertArea global.CityID,global.CityName,global.CountyName";
            rowsDeleted = mDB.ExecuteSQL(sqlString);
          
            
            //将学校信息写入数据库
            //String sqlString1 = "insert into tSchool(SchoolID,SchoolName,SchoolKind,SchoolQuality) values(" + global.SchoolID + "," + global.SchoolName + "," + global.SchoolKind + "," + global.SchoolQuality + ")";
            sqlString = "exec insert InsertSchool global.SchoolID,global.SchoolName,global.SchoolKind,global.SchoolQuality";
            rowsDeleted = mDB.ExecuteSQL(sqlString);

            //将教室信息写入数据库
            sqlString = "exec insert InsertClass global.ClassID,global.ClassAddress";
           // String sqlString2 = "insert into tClass(ClassID,ClassAddress) values(" + global.ClassID + "," + global.ClassAddress + ")";
            rowsDeleted = mDB.ExecuteSQL(sqlString);

            //将电脑信息写入数据库
            sqlString = "exec insert InsertComputer global.ComputerID,global.CPU";
           // String sqlString3 = "insert into tComputer(ComputerID,CPU) values(" + global.ComputerID + "," + global.CPU + ")";
            rowsDeleted = mDB.ExecuteSQL(sqlString);
            
            // 操作完毕关闭数据库通道
            mDB.Close();*/

            string strsql = "Data Source=127.0.0.1;Initial Catalog=dbProcess;Persist Security Info=True;User ID=sa;Password=LY19971025";//数据库链接字符串
            string sql1 = "InsertArea";//要调用的存储过程名
            string sql2 = "InsertSchool";
            string sql3 = "InsertClass";           
            string sql4 = " InsertComputer";           
            SqlConnection conStr = new SqlConnection(strsql);//SQL数据库连接对象，以数据库链接字符串为参数
            SqlCommand comStr1 = new SqlCommand(sql1, conStr);//SQL语句执行对象，第一个参数是要执行的语句，第二个是数据库连接对象
            SqlCommand comStr2 = new SqlCommand(sql2, conStr);
            SqlCommand comStr3 = new SqlCommand(sql3, conStr);
            SqlCommand comStr4 = new SqlCommand(sql4, conStr);
            comStr1.CommandType = CommandType.StoredProcedure;//因为要使用的是存储过程，所以设置执行类型为存储过程
            comStr2.CommandType = CommandType.StoredProcedure;
            comStr3.CommandType = CommandType.StoredProcedure;
            comStr4.CommandType = CommandType.StoredProcedure;

           /* conStr.Open();//打开数据库连接

            //依次设定存储过程的参数
            comStr1.Parameters.Add("@cityID", SqlDbType.VarChar, 10).Value = global.CityID;
            comStr1.Parameters.Add("@cityName", SqlDbType.VarChar, 20).Value = global.CityName;
            comStr1.Parameters.Add("@countyName", SqlDbType.VarChar, 20).Value = global.CountyName;
            MessageBox.Show(comStr1.ExecuteNonQuery().ToString());//执行存储过程
            conStr.Close();//关闭连接*/

            conStr.Open();//打开数据库连接
            comStr2.Parameters.Add("@cityID", SqlDbType.VarChar, 10).Value = global.CityID;
            comStr2.Parameters.Add("@schoolID", SqlDbType.VarChar, 20).Value = global.SchoolID;
            comStr2.Parameters.Add("@countyName", SqlDbType.VarChar, 20).Value = global.CountyName;
            comStr2.Parameters.Add("@schoolName", SqlDbType.VarChar, 20).Value = global.SchoolName;
            comStr2.Parameters.Add("@schoolKind", SqlDbType.VarChar, 20).Value = global.SchoolKind;
            comStr2.Parameters.Add("@schoolQuality", SqlDbType.VarChar, 20).Value = global.ClassAddress;
            MessageBox.Show(comStr3.ExecuteNonQuery().ToString());//执行存储过程
            conStr.Close();//关闭连接

            /*comStr3.Parameters.Add("@schoolID", SqlDbType.VarChar, 20).Value = global.SchoolID;
            comStr3.Parameters.Add("@classID", SqlDbType.VarChar, 20).Value = global.ClassID;
            comStr3.Parameters.Add("@classAddress", SqlDbType.VarChar, 20).Value = cmbClassAddress.Text;
            MessageBox.Show(comStr2.ExecuteNonQuery().ToString());//执行存储过程
                      
            comStr3.Parameters.Add("@computerID", SqlDbType.VarChar, 20).Value = global.ComputerID;
            comStr3.Parameters.Add("@cpuID", SqlDbType.VarChar, 20).Value = global.CPU;
            MessageBox.Show(comStr4.ExecuteNonQuery().ToString());//执行存储过程
            */
            

         

            MessageBox.Show("对咯！");
        }




        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            frmSee form = new frmSee();
            form.Show();
        }




        /// <summary>
        /// 点击下拉菜单选择城市编号时，根据选择的城市编号自动匹配城市名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCityID_TextChanged(object sender, EventArgs e)
        {
            String cityid = cmbCityID.SelectedItem.ToString();
            switch (cityid)
            {
                case "A": cmbCityName.Text = "南京"; break;
                case "B": cmbCityName.Text = "无锡"; break;
                case "C": cmbCityName.Text = "徐州"; break;
                case "D": cmbCityName.Text = "常州"; break;
                case "E": cmbCityName.Text = "苏州"; break;
                case "F": cmbCityName.Text = "南通"; break;
                case "G": cmbCityName.Text = "连云港"; break;
                case "H": cmbCityName.Text = "淮安"; break;
                case "J": cmbCityName.Text = "盐城"; break;
                case "K": cmbCityName.Text = "扬州"; break;
                case "L": cmbCityName.Text = "镇江"; break;
                case "M": cmbCityName.Text = "泰州"; break;
                case "N": cmbCityName.Text = "宿迁"; break;
            }
            cmbCityName.Enabled = false;
        }



        /// <summary>
        /// 点击下拉菜单选择学校名称时，根据选择的学校名称自动匹配学校编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         private void cmbSchoolName_SelectionChangeCommitted(object sender, EventArgs e)
         {
             /*String schoolName = cmbSchoolName.SelectedItem.ToString();
             switch (schoolName)
             {
                 case "南京师范大学":cmbSchoolID.Text = "NJNU";break;                           //当选择“南京师范大学”时，学校编号为“NJNU”，下同
             }
             cmbSchoolID.Enabled = false;*/
         }
        private void cmbSchoolName_TextChanged(object sender, EventArgs e)
        {
            String schoolName = cmbSchoolName.SelectedItem.ToString();
            switch (schoolName)
            {
                case "南京师范大学": cmbSchoolID.Text = "NJNU"; break;                           //当选择“南京师范大学”时，学校编号为“NJNU”，下同
            }
            //cmbSchoolID.Enabled = false;
        }



        /// <summary>
        /// 当选择的教室地址发生变化时，教室编号也会发生相应的变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbClassAddress_SelectionChangeCommitted(object sender, EventArgs e)
        {
            String classlName = cmbClassAddress.SelectedItem.ToString();
            switch (classlName)
            {
                case "明理楼":                                                                //当选择“明理楼”时，教室地址全部以“（S2）”开头；以下同理
                    {
                        cmbClassID.Items.Clear();
                        cmbClassID.Items.Add("(S2)406");
                        cmbClassID.Items.Add("(S2)410");
                        cmbClassID.Items.Add("(S2)302");
                    }
                    break;

                case "学正楼":                                                               
                    {
                        cmbClassID.Items.Clear();
                         //一楼
                        cmbClassID.Items.Add("(J2)103");
                        cmbClassID.Items.Add("(J2)104");
                        cmbClassID.Items.Add("(J2)105");
                        cmbClassID.Items.Add("(J2)106");
                        cmbClassID.Items.Add("(J2)107");
                        cmbClassID.Items.Add("(J2)108");
                        cmbClassID.Items.Add("(J2)109");
                        cmbClassID.Items.Add("(J2)110");
                        cmbClassID.Items.Add("(J2)111");
                        cmbClassID.Items.Add("(J2)112");

                        //二楼
                        cmbClassID.Items.Add("(J2)201");
                        cmbClassID.Items.Add("(J2)202");
                        cmbClassID.Items.Add("(J2)203");
                        cmbClassID.Items.Add("(J2)204");
                        cmbClassID.Items.Add("(J2)205");
                        cmbClassID.Items.Add("(J2)206");
                        cmbClassID.Items.Add("(J2)207");
                        cmbClassID.Items.Add("(J2)208");
                        cmbClassID.Items.Add("(J2)209");
                        cmbClassID.Items.Add("(J2)210");
                        cmbClassID.Items.Add("(J2)211");
                        cmbClassID.Items.Add("(J2)212");
                        cmbClassID.Items.Add("(J2)213");
                        cmbClassID.Items.Add("(J2)214");

                        //三楼
                        cmbClassID.Items.Add("(J2)303");
                        cmbClassID.Items.Add("(J2)304");
                        cmbClassID.Items.Add("(J2)305");
                        cmbClassID.Items.Add("(J2)306");
                        cmbClassID.Items.Add("(J2)307");
                        cmbClassID.Items.Add("(J2)308");
                        cmbClassID.Items.Add("(J2)310");
                        cmbClassID.Items.Add("(J2)311");
                        cmbClassID.Items.Add("(J2)312");
                        cmbClassID.Items.Add("(J2)313");
                        cmbClassID.Items.Add("(J2)314");

                        //四楼
                        cmbClassID.Items.Add("(J2)401");
                        cmbClassID.Items.Add("(J2)402");
                        cmbClassID.Items.Add("(J2)403");
                        cmbClassID.Items.Add("(J2)404");
                        cmbClassID.Items.Add("(J2)405");
                        cmbClassID.Items.Add("(J2)406");
                        cmbClassID.Items.Add("(J2)407");
                        cmbClassID.Items.Add("(J2)408");
                        cmbClassID.Items.Add("(J2)409");
                        cmbClassID.Items.Add("(J2)410");
                        cmbClassID.Items.Add("(J2)411");
                        cmbClassID.Items.Add("(J2)412");
                        cmbClassID.Items.Add("(J2)413");
                        cmbClassID.Items.Add("(J2)414");

                        //五楼
                        cmbClassID.Items.Add("(J2)501");
                        cmbClassID.Items.Add("(J2)502");
                        cmbClassID.Items.Add("(J2)503");
                        cmbClassID.Items.Add("(J2)504");
                        cmbClassID.Items.Add("(J2)505");
                        cmbClassID.Items.Add("(J2)506");
                        cmbClassID.Items.Add("(J2)507");
                        cmbClassID.Items.Add("(J2)508");
                        cmbClassID.Items.Add("(J2)509");
                        cmbClassID.Items.Add("(J2)510");
                        cmbClassID.Items.Add("(J2)511");
                        cmbClassID.Items.Add("(J2)512");
                        cmbClassID.Items.Add("(J2)513");
                        cmbClassID.Items.Add("(J2)514");
                    }
                    break;

                case "学明楼":
                    {
                        cmbClassID.Items.Clear();
                        //一楼
                        cmbClassID.Items.Add("(J3)103");
                        cmbClassID.Items.Add("(J3)104");
                        cmbClassID.Items.Add("(J3)105");
                        cmbClassID.Items.Add("(J3)106");
                        cmbClassID.Items.Add("(J3)107");
                        cmbClassID.Items.Add("(J3)108");
                        cmbClassID.Items.Add("(J3)109");
                        cmbClassID.Items.Add("(J3)110");
                        cmbClassID.Items.Add("(J3)111");
                        cmbClassID.Items.Add("(J3)112");

                        //二楼
                        cmbClassID.Items.Add("(J3)201");
                        cmbClassID.Items.Add("(J3)202");
                        cmbClassID.Items.Add("(J3)203");
                        cmbClassID.Items.Add("(J3)204");
                        cmbClassID.Items.Add("(J3)205");
                        cmbClassID.Items.Add("(J3)206");
                        cmbClassID.Items.Add("(J3)207");
                        cmbClassID.Items.Add("(J3)208");
                        cmbClassID.Items.Add("(J3)209");
                        cmbClassID.Items.Add("(J3)210");
                        cmbClassID.Items.Add("(J3)211");
                        cmbClassID.Items.Add("(J3)212");
                        cmbClassID.Items.Add("(J3)213");
                        cmbClassID.Items.Add("(J3)214");

                        //三楼
                        cmbClassID.Items.Add("(J3)303");
                        cmbClassID.Items.Add("(J3)304");
                        cmbClassID.Items.Add("(J3)305");
                        cmbClassID.Items.Add("(J3)306");
                        cmbClassID.Items.Add("(J3)307");
                        cmbClassID.Items.Add("(J3)308");
                        cmbClassID.Items.Add("(J3)310");
                        cmbClassID.Items.Add("(J3)311");
                        cmbClassID.Items.Add("(J3)312");
                        cmbClassID.Items.Add("(J3)313");
                        cmbClassID.Items.Add("(J3)314");

                        //四楼
                        cmbClassID.Items.Add("(J3)401");
                        cmbClassID.Items.Add("(J3)402");
                        cmbClassID.Items.Add("(J3)403");
                        cmbClassID.Items.Add("(J3)404");
                        cmbClassID.Items.Add("(J3)405");
                        cmbClassID.Items.Add("(J3)406");
                        cmbClassID.Items.Add("(J3)407");
                        cmbClassID.Items.Add("(J3)408");
                        cmbClassID.Items.Add("(J3)409");
                        cmbClassID.Items.Add("(J3)410");
                        cmbClassID.Items.Add("(J3)411");
                        cmbClassID.Items.Add("(J3)412");
                        cmbClassID.Items.Add("(J3)413");
                        cmbClassID.Items.Add("(J3)414");

                        //五楼
                        cmbClassID.Items.Add("(J3)501");
                        cmbClassID.Items.Add("(J3)502");
                        cmbClassID.Items.Add("(J3)503");
                        cmbClassID.Items.Add("(J3)504");
                        cmbClassID.Items.Add("(J3)505");
                        cmbClassID.Items.Add("(J3)506");
                        cmbClassID.Items.Add("(J3)507");
                        cmbClassID.Items.Add("(J3)508");
                        cmbClassID.Items.Add("(J3)509");
                        cmbClassID.Items.Add("(J3)510");
                        cmbClassID.Items.Add("(J3)511");
                        cmbClassID.Items.Add("(J3)512");
                        cmbClassID.Items.Add("(J3)513");
                        cmbClassID.Items.Add("(J3)514");
                    }
                    break;

                case "学海楼":
                    {
                        cmbClassID.Items.Clear();
                        //一楼
                        cmbClassID.Items.Add("(J1)103");
                        cmbClassID.Items.Add("(J1)104");
                        cmbClassID.Items.Add("(J1)105");
                        cmbClassID.Items.Add("(J1)106");
                        cmbClassID.Items.Add("(J1)107");
                        cmbClassID.Items.Add("(J1)108");
                        cmbClassID.Items.Add("(J1)109");
                        cmbClassID.Items.Add("(J1)110");
                        cmbClassID.Items.Add("(J1)111");
                        cmbClassID.Items.Add("(J1)112");

                        //二楼
                        cmbClassID.Items.Add("(J1)201");
                        cmbClassID.Items.Add("(J1)202");
                        cmbClassID.Items.Add("(J1)203");
                        cmbClassID.Items.Add("(J1)204");
                        cmbClassID.Items.Add("(J1)205");
                        cmbClassID.Items.Add("(J1)206");
                        cmbClassID.Items.Add("(J1)207");
                        cmbClassID.Items.Add("(J1)208");
                        cmbClassID.Items.Add("(J1)209");
                        cmbClassID.Items.Add("(J1)210");
                        cmbClassID.Items.Add("(J1)211");
                        cmbClassID.Items.Add("(J1)212");
                        cmbClassID.Items.Add("(J1)213");
                        cmbClassID.Items.Add("(J1)214");

                        //三楼
                        cmbClassID.Items.Add("(J1)303");
                        cmbClassID.Items.Add("(J1)304");
                        cmbClassID.Items.Add("(J1)305");
                        cmbClassID.Items.Add("(J1)306");
                        cmbClassID.Items.Add("(J1)307");
                        cmbClassID.Items.Add("(J1)308");
                        cmbClassID.Items.Add("(J1)310");
                        cmbClassID.Items.Add("(J1)311");
                        cmbClassID.Items.Add("(J1)312");
                        cmbClassID.Items.Add("(J1)313");
                        cmbClassID.Items.Add("(J1)314");

                        //四楼
                        cmbClassID.Items.Add("(J1)401");
                        cmbClassID.Items.Add("(J1)402");
                        cmbClassID.Items.Add("(J1)403");
                        cmbClassID.Items.Add("(J1)404");
                        cmbClassID.Items.Add("(J1)405");
                        cmbClassID.Items.Add("(J1)406");
                        cmbClassID.Items.Add("(J1)407");
                        cmbClassID.Items.Add("(J1)408");
                        cmbClassID.Items.Add("(J1)409");
                        cmbClassID.Items.Add("(J1)410");
                        cmbClassID.Items.Add("(J1)411");
                        cmbClassID.Items.Add("(J1)412");
                        cmbClassID.Items.Add("(J1)413");
                        cmbClassID.Items.Add("(J1)414");
                    }
                    break;
            }
        }

      
    }
}
