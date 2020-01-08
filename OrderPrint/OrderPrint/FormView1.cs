using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using NEBasic;

namespace OrderPrint
{
    public partial class FormView1 : Form
    {
        public FormView1()
        {
            InitializeComponent();
        }

        static readonly object syncRoot = new object();
        //定义Grid++Report报表主对象
      //  private GridppReport Report = new GridppReport();
        private Thread thrRecv;
        private UdpClient udpcRecv;

        private void FormView1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            this.listView1.GridLines = true; //显示表格线
            this.listView1.View = View.Details;//显示表格细节
            this.listView1.FullRowSelect = true;//是否可以选择行
            ImageList image = new ImageList();
            image.ImageSize = new Size(1, 25);
            this.listView1.SmallImageList = image;
            //添加表头
            this.listView1.Columns.Add("序号", 60, HorizontalAlignment.Center);
            this.listView1.Columns.Add("患者编号", 150, HorizontalAlignment.Center);
            this.listView1.Columns.Add("姓名", 150, HorizontalAlignment.Center);
            this.listView1.Columns.Add("处方号", 250, HorizontalAlignment.Center);
            this.listView1.Columns.Add("日期", 200, HorizontalAlignment.Center);
            this.listView1.Columns.Add("窗口号", 100, HorizontalAlignment.Center);
            this.listView1.Columns.Add("状态", 150, HorizontalAlignment.Center);
            this.listView1.Columns.Add("处理号", 200, HorizontalAlignment.Center);


            //// 加载报表
            //if (!INIOperationClass.INIGetStringValue(".\\config.ini", "params", "printername", "").Equals(""))
            //{
            //    Report.Printer.PrinterName = INIOperationClass.INIGetStringValue(".\\config.ini", "params", "printername", "");
            //}
            //Report.LoadFromFile(".\\op.grf");

            IPEndPoint localIpep = new IPEndPoint(IPAddress.Any, 7015); // 本机IP和监听端口号
            try
            {
                udpcRecv = new UdpClient(localIpep);
                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("端口被占用！", "提示");
                System.Environment.Exit(0);
            }

            
            // 待打印初始化
         //   WaitingPrintInit();

            timer1.Interval = Config.G_READER_TIMER;
            timer2.Interval = Config.G_STATUS_TIMER;

            timer1.Enabled = true;
            timer2.Enabled = true;

            pictureBox1.ImageLocation = @"pic\GreenBall.png";
            pictureBox2.ImageLocation = @"pic\GreenBall.png";
            pictureBox3.ImageLocation = @"pic\GreenBall.png";

            //if(INIOperationClass.INIGetStringValue(".\\config.ini", "params", "autoprint", "0").Equals("1"))
            //{
            //    timer2.Enabled = true;
            //}
            //else
            //{
            //    btnPause_Click(null, null);
            //}
        }

        private void FormView1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        string currentStateCode = "";
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 7215);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);
                //    MessageBox.Show("收到指令：" + code);
                    NELog.WriteLog("收到指令1：" + message);
                    message = message.Substring(0, message.LastIndexOf("*") + 1);
                    message = "$" + DesCode.DESDecode(message.Substring(1, message.Length - 2), "YN200916") + "*";
                    if (message == null || message.Equals("") || message.Length < 5 || !message.Substring(0, 5).Equals("$A712"))
                    {
                        continue;
                    }

                    currentStateCode = message;
                }
                catch (Exception ex)
                {
                //    MessageBox.Show(ex.ToString());
                    continue;;
                }
            }
        }

        private void sendMessage(string code)
        {
            try
            {
                code = "$" + DesCode.DESEncode(code.Substring(1, code.Length - 2), "YN200916") + "*";
                byte[] sendbytes = Encoding.UTF8.GetBytes(code);

                IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(NEIni.ReadValue(".\\config.ini", "cfg", "equipmentip", "")), 7015); // 发送到的IP地址和端口号
                NELog.WriteLog("Q-----------");
                int i = udpcRecv.Send(sendbytes, sendbytes.Length, remoteIpep);
            }
            catch (System.Exception ex)
            {
            	//
            }
        }

        private void btnPrintS_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                string procCode = listView1.SelectedItems[0].SubItems[7].Text;
                string hzbh = listView1.SelectedItems[0].SubItems[1].Text;
                string cfxh = listView1.SelectedItems[0].SubItems[3].Text;

                PrintOrder(procCode, hzbh, cfxh);

                listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
            }
            else
            {
                MessageBox.Show("请选择要打印的记录！");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            lock (syncRoot)
            {
                ReadOrderInfo();
            }
            timer1.Enabled = true;
        }

        /// <summary>
        /// 更新打印标识小于3的记录为0
        /// </summary>
        private void WaitingPrintInit()
        {
            MySQLPool pool = MySQLPool.getInstance();
            MySqlConnection conn = null;
            try
            {
                conn = pool.getConnection();
                string sql = "update order_info set print_flag=0 where print_flag<3";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                //
            }
            finally
            {
                pool.releaseConnection(conn);
            }
        }

        private void ReadOrderInfo()
        {
            while (listView1.Items.Count > 0)
            {
                listView1.Items.Clear();
            }
            try
            {
                MySQLPool pool = MySQLPool.getInstance();
                MySqlConnection conn = null;
                try
                {
                    conn = pool.getConnection();
                    string sql = "select PatientID,PatientName,PrescriptionNo,PrescriptionDate,FetchWindow,ProcFlg,ProcDate,ProcCode from prescriptionlist where ProcFlg<4 order by PatientID,PrescriptionDate";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string procInfo = "";
                        switch (dr.GetInt32(5))
                        {
                            case 0:
                                procInfo = "队列等待中"; break;
                            case 1:
                                procInfo = "预处理完毕"; break;
                            case 2:
                                procInfo = "发药等待中"; break;
                            case 3:
                                procInfo = "正在发药"; break;
                            case 4:
                                procInfo = "发药完毕"; break;
                        }
                        ListViewItem[] p = new ListViewItem[1];
                        p[0] = new ListViewItem(new string[] { "", dr.GetString(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4), procInfo, dr.IsDBNull(7) ? "" : dr.GetString(7) });
                        this.listView1.Items.AddRange(p);
                    }
                    dr.Close();
                }
                catch (System.Exception ex)
                {
                    //
                }
                finally
                {
                    pool.releaseConnection(conn);
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        private void PrintOrder(string procCode, string hzbh, string cfxh)
        {
            lock (syncRoot)
            {
                string sql = "";
                if ("".Equals(procCode))
                {
                    procCode = DateTime.Now.ToString("yyMMddHHmmss");
                    sql = "update prescriptionlist set ProcFlg=4,ProcDate=now(),ProcCode='" + procCode + "' where (ProcFlg=0 or ProcFlg=4) and PatientID='" + hzbh + "' and PrescriptionNo='" + cfxh + "'";
                } 
                else
                {
                    sql = "update prescriptionlist set ProcFlg=4,ProcDate=now() where (ProcFlg=0 or ProcFlg=4) and procCode='" + procCode + "'";
                }

                MySQLPool pool = MySQLPool.getInstance();
                MySqlConnection conn = null;
                try
                {
                    conn = pool.getConnection();

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // 查询是否存在待打印处方，准备处方信息
                    sql = "select PatientID from prescriptionlist where ProcCode='" + procCode + "'";
                    MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                    MySqlDataReader dr = cmd2.ExecuteReader();
                    while (dr.Read())
                    {
                        sql = "1";
                    }
                    dr.Close();

                    if (sql.Equals("1"))
                    {
                        sql = "update prescriptionlist set ProcFlg=5,ProcDate=now() where ProcCode='" + procCode + "'";
                        MySqlCommand cmd3 = new MySqlCommand(sql, conn);
                        cmd3.ExecuteNonQuery();

                        //$Q701 N xxx Nn xxxxxx xxxx*
                        string message = "$Q7030000" + procCode.Length + procCode + "xxxx*";
                   //     NELog.WriteLog(message);
                        Form1.mySyncdQ.Enqueue(message);
                    }
                }
                catch (System.Exception ex)
                {
                    //
                }
                finally
                {
                    pool.releaseConnection(conn);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // 查询状态
            try
            {
                sendMessage("$Q711NxxxNnxxxxxxxxxx*");

                // 更新状态
                // $A712 2 K1 00 00       00 xxxx*
                //            正常/异常   急停/门控/对射异常编号%求余
                string code = currentStateCode;
                if (code == null || code.Equals(""))
                {
                    return;
                }
                code = code.Substring(6 + int.Parse(code.Substring(5, 1)));
                if (code.Substring(0, 2).Equals("00"))
                {
                    pictureBox1.ImageLocation = @"pic\GreenBall.png";
                    labelInfo1.Text = "正常";
                }
                else
                {
                    pictureBox1.ImageLocation = @"pic\RedBall.png";
                    labelInfo1.Text = "异常";
                }
                if (code.Substring(2, 2).Equals("00"))
                {
                    pictureBox2.ImageLocation = @"pic\GreenBall.png";
                    labelInfo2.Text = "正常";
                }
                else
                {
                    pictureBox2.ImageLocation = @"pic\RedBall.png";
                    labelInfo2.Text = "异常";
                }
                if (code.Substring(4, 2).Equals("00"))
                {
                    pictureBox3.ImageLocation = @"pic\GreenBall.png";
                    labelInfo3.Text = "空闲";
                    button1.Visible = false;
                }
                else if (code.Substring(4, 2).Equals("01"))
                {
                    pictureBox3.ImageLocation = @"pic\OrangeBall.png";
                    labelInfo3.Text = "发药中";
                    button1.Visible = false;
                }
                else
                {
                    errCode = code.Substring(4, 2);
                    ////99 严重错误，02 急停，03 门控，11 未回零，21 运动超时，30 出药板异常，31/32/33 N区异常
                    pictureBox3.ImageLocation = @"pic\RedBall.png";
                    if (errCode.Equals("02"))
                    {
                        labelInfo3.Text = "急停被按下";
                    }
                    else if (errCode.Equals("03"))
                    {
                        labelInfo3.Text = "设备门打开或未关紧";
                    }
                    else if (errCode.Equals("06"))
                    {
                        labelInfo3.Text = "翻板关闭异常";
                    }
                    else if (errCode.Equals("11"))
                    {
                        labelInfo3.Text = "升降机未回零";
                    }
                    else if (errCode.Equals("21"))
                    {
                        labelInfo3.Text = "升降机运动超时";
                    }
                    else if (errCode.Equals("30"))
                    {
                        labelInfo3.Text = "检测出药对射状态异常";
                    }
                    else if (errCode.Equals("31") || errCode.Equals("32") || errCode.Equals("33")
                         || errCode.Equals("34") || errCode.Equals("35") || errCode.Equals("36"))
                    {
                        labelInfo3.Text = "对射异常";
                    }
                    button1.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
             //   MessageBox.Show(ex.ToString());
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            //if (!bPause)
            //{
            //    return;
            //}
            
            //while (listView2.Items.Count > 0)
            //{
            //    listView2.Items.RemoveAt(0);
            //}
            
            //if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            //{
            //    string hzbh = listView1.SelectedItems[0].SubItems[1].Text;
            //    string cfxh = listView1.SelectedItems[0].SubItems[3].Text;
            //    string sql = "select a.drug_id,drug_name,drug_spec,manufactory,concat(quantity,ifnull(order_unit,'')) as qty,back_stock_pos from order_detail a left join drug_info b on a.drug_id=b.drug_id where patient_id='" + hzbh + "' and order_no='" + cfxh + "'";

            //    lock (syncRoot)
            //    {
            //        MySqlCommand cmd = new MySqlCommand(sql, DatabaseConn.conn);
            //        MySqlDataReader dr = cmd.ExecuteReader();
            //        while (dr.Read())
            //        {
            //            ListViewItem[] p = new ListViewItem[1];
            //            p[0] = new ListViewItem(new string[] { "", dr.IsDBNull(0) ? "" : dr.GetString(0), dr.IsDBNull(1) ? "" : dr.GetString(1), dr.IsDBNull(2) ? "" : dr.GetString(2), dr.IsDBNull(3) ? "" : dr.GetString(3), dr.IsDBNull(4) ? "" : dr.GetString(4), dr.IsDBNull(5) ? "" : dr.GetString(5) });
            //            this.listView2.Items.AddRange(p);
            //        }
            //        dr.Close();
            //    }
            //}
        }

        private string errCode;
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            if (errCode == null || errCode.Equals("") || errCode.Length != 2)
            {
                return;
            }
            sendMessage("$Q7132K1" + errCode + "xxxx*");
        }

        //public void Pause(bool flg)
        //{
        //    if (flg)
        //    {
        //        timer1.Enabled = false;
        //        timer2.Enabled = false;
        //    } 
        //    else
        //    {
        //        timer1.Enabled = true;
        //        if (!bPause)
        //        {
        //            timer2.Enabled = true;
        //        }
        //    }
        //}
    }
}