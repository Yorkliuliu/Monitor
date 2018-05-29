using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using System.IO;
//using static Monitor.frmRegister;


namespace Monitor
{

    /// <summary>
    /// 查看之前已录入的信息
    /// </summary>
    public partial class frmSee : Form
    {
        public frmSee()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 加载时从全局变量中读取已写入的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmSee_Load(object sender, EventArgs e)
        {
            //城市信息
            lbl_CityID.Text = Monitor.frmRegister.global.CityID;                           //城市编号
            lbl_CityName.Text = Monitor.frmRegister.global.CityName;                       //城市名
            lbl_CountyName.Text = Monitor.frmRegister.global.CountyName;                   //区县名


            //学校信息
            lbl_SchoolID.Text = Monitor.frmRegister.global.SchoolID;                       //学校编号
            lbl_SchoolName.Text = Monitor.frmRegister.global.SchoolName;                   //学校名称
            lbl_SchoolKind.Text = Monitor.frmRegister.global.SchoolKind;                   //学校类型
            lbl_SchoolQuality.Text = Monitor.frmRegister.global.SchoolQuality;             //学校性质


            //教室信息
            lbl_ClassID.Text = Monitor.frmRegister.global.ClassID;                         //教室编号
            lbl_ClassAddress.Text = Monitor.frmRegister.global.ClassAddress;               //教室地址


            //电脑信息
            lbl_ComputerID.Text = Monitor.frmRegister.global.ComputerID;                   //电脑编号
            lbl_CPUID.Text = Monitor.frmRegister.global.CPU;                               //CPU序列号

        }



        /// <summary>
        /// 跳转到下一个窗体，从而查看已捕获到的进程信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSee_Click(object sender, EventArgs e)
        {

        }
    }
}
