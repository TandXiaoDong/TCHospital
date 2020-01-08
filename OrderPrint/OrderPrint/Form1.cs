using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using NEBasic;
using MySql.Data.MySqlClient;

namespace OrderPrint
{
    public partial class Form1 : Form
    {
        FormView1 view1 = new FormView1();
        FormView2 view2 = new FormView2();
        FormView3 view3 = new FormView3();
        FormView4 view4 = new FormView4();

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;

            NELog.logPath = @".\log";
            InitializeComponent();
            view1.MdiParent = this;
            view2.MdiParent = this;
            view3.MdiParent = this;
            view4.MdiParent = this;

            Config.G_READER_TIMER = int.Parse(INIOperationClass.INIGetStringValue(".\\config.ini", "params", "readtimer", "3000"));
            Config.G_DB_IP_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "params", "databaseip", "127.0.0.1");
            Config.G_DB_DATABASE_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "params", "databasename", "hkcdb2");
            Config.G_DB_USER_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "params", "databaseuser", "root");
            Config.G_DB_PWD_STR = INIOperationClass.INIGetStringValue(".\\config.ini", "params", "databasepwd", "qq1223");
            //MessageBox.Show();
            view1.Show();
        }

        private Thread thrRecv;
        private UdpClient udpcRecv;
        public static Queue mySyncdQ = Queue.Synchronized(new Queue());

        private void Form1_Load(object sender, EventArgs e)
        {
            IPEndPoint localIpep = new IPEndPoint(IPAddress.Any, 7016); // 本机IP和监听端口号
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
            
            timer1.Interval = 500;
            timer1.Enabled = true;

            // 启动灯控制线程
        //    new ModbusTCP("192.168.100.56");
         //   new LightControl().start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
            //    e.Cancel = false;
                System.Environment.Exit(0);
            } 
            else
            {
                e.Cancel = true;
            }
        }

        IPAddress printIPAddress;
        private void sendMessage(string code)
        {
            //     Console.WriteLine("----------------");
            //     Thread.Sleep(5000);
            byte[] sendbytes = Encoding.UTF8.GetBytes(code);

            IPEndPoint remoteIpep = new IPEndPoint(printIPAddress, 7015); // 发送到的IP地址和端口号

            int i = udpcRecv.Send(sendbytes, sendbytes.Length, remoteIpep);
            //     Console.WriteLine("----------------2");
        }

        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    printIPAddress = remoteIpep.Address;
                    string message = Encoding.UTF8.GetString(bytRecv, 0, bytRecv.Length);
                    if ("123456".Equals(message))
                    {
                        MessageBox.Show("收到指令：" + message);
                        return;
                    }
                    NELog.WriteLog("收到指令2：" + message+"-");
                    
                    if (message == null || message.Equals("") || message.Length < 5)
                    {
                        continue;
                    }

                    message = message.Substring(0, message.LastIndexOf("*") + 1);
                    message = "$" + DesCode.DESDecode(message.Substring(1, message.Length - 2), "YN200916") + "*";
                    
                    if (!message.Substring(0, 5).Equals("$Q703") && !message.Substring(0, 5).Equals("$Q701"))
                    {
                        continue;
                    }
                    mySyncdQ.Enqueue(message);
                }
                catch (Exception ex)
                {
                //    MessageBox.Show(ex.ToString());
                    NELog.WriteLog(ex.ToString());
                    continue;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Hide();
            }
            view1.Show();
         //   view1.Pause(false);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Hide();
            }
       //     view1.Pause(true);
            view2.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Hide();
            }
        //    view1.Pause(true);
            view3.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Hide();
            }
        //    view1.Pause(true);
            view4.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mySyncdQ.Count > 0)
                {
                    watitingDealCdoe = (string)mySyncdQ.Dequeue();
                    
                    if (watitingDealCdoe.Substring(0, 5).Equals("$Q701"))
                    {
                        string bs = watitingDealCdoe.Substring(6 + int.Parse(watitingDealCdoe.Substring(5, 1)), 2);
                    //    sendMessage(watitingDealCdoe.Substring(0, 6 + int.Parse(watitingDealCdoe.Substring(5, 1))).Replace("Q701", "A702") + LightControl.getFreeStock(bs) + "xxxx*");
                    } 
                    else
                    {
                     // Console.WriteLine("打印：" + watitingDealCdoe);
                        PrintPageer();
                    }
                    watitingDealCdoe = "";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        string watitingDealCdoe = "";
        private void PrintPageer()
        {
            PrintDocument pd = new PrintDocument();

            //System.Drawing.Printing.PageSettings df = new System.Drawing.Printing.PageSettings();
            //df.PaperSize = new PaperSize("new size", 80, 1000);
            //pd.DefaultPageSettings = df;

            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            pd.Print();
        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (watitingDealCdoe == null || watitingDealCdoe.Equals(""))
            {
                return;
            }
            // $Q701 00 00 Nn xxxxxx xxxx*  出药口/窗口/2位长/条码
            //MessageBox.Show(watitingDealCdoe.Substring(8, 2));
            string barCode = watitingDealCdoe.Substring(11, int.Parse(watitingDealCdoe.Substring(9, 2)));

            //中国人民解放军第一五九中心医院
            String hospital = INIOperationClass.INIGetStringValue(".\\config.ini", "cfg", "hospital", "");
            Font font = new Font("宋体", 12);
            Brush bru = Brushes.Black;
            e.Graphics.DrawString(hospital, font, bru, 10, 0);
            
            BarCode_EAN13.Paint_EAN13(barCode, e.Graphics, new Rectangle(10, 20, 150, 60));

            Font font2 = new Font("宋体", 30);
            Patient patient = getPrescription(barCode);

            e.Graphics.DrawString(patient.FetchWindow, font2, bru, 210, 30);
            try
            {
                Font fonts = new Font("宋体", 22);
                if ("X".Equals(watitingDealCdoe.Substring(6, 1)))
                {
                    e.Graphics.DrawString("下", fonts, bru, 245, 40);
                }
                else if ("S".Equals(watitingDealCdoe.Substring(6, 1)))
                {
                    e.Graphics.DrawString("上", fonts, bru, 245, 40);
                }
            }
            catch (System.Exception ex)
            {
            	//
            }
            e.Graphics.DrawString(patient.PName, font, bru, 0, 90);
            e.Graphics.DrawString(patient.Sex, font, bru, 120, 90);
            e.Graphics.DrawString(patient.Age, font, bru, 180, 90);
            e.Graphics.DrawString("=======================================", font, bru, 0, 105);

            Font font3 = new Font("宋体", 10);

            int yPosition = 120;
            int ocount = patient.PrescInfo.Count;
            foreach (PrescriptionInfo prescInfo in patient.PrescInfo)
            {
                e.Graphics.DrawString("※处方号:" + prescInfo.PrescriptionNO, font3, bru, 0, yPosition);

                int i = 1;
                foreach (PrescriptionDetl prescDetl in prescInfo.DetlList)
                {
                    e.Graphics.DrawString((i++) + "/" + prescInfo.DetlList.Count + " " + prescDetl.DrugName, font3, bru, 0, yPosition + 20);
                    e.Graphics.DrawString("    " + prescDetl.DrugSpec + "（" + prescDetl.Manufactory + "）", font3, bru, 0, yPosition + 35);
                    e.Graphics.DrawString("    " + prescDetl.StorageLoc, font, bru, 0, yPosition + 50);
                    e.Graphics.DrawString("    " + prescDetl.PrescriptionQty + prescDetl.PrescriptionUnit, font, bru, 160, yPosition + 50);
                    e.Graphics.DrawString("    " + prescDetl.Num_1, font, bru, 160, yPosition + 65);
                    e.Graphics.DrawString("    " + prescDetl.UseFrequency + "     " + prescDetl.UseDosage, font3, bru, 0, yPosition + 80);
                    e.Graphics.DrawString("    " + prescDetl.UseRoute + "     " + prescDetl.Notes, font3, bru, 0, yPosition + 95);
                    e.Graphics.DrawString("   " + "单价    "+prescDetl.Price, font, bru, 0, yPosition + 110);
                    yPosition += 110;
                }
                ocount--;
                if (ocount > 0)
                {
                    e.Graphics.DrawString("---------------------------------------", font, bru, 0, yPosition + 15);
                    yPosition += 30;
                }
            }
            //
            

            //e.Graphics.DrawString("---------------------------------------", font, bru, 0, 235);

            //e.Graphics.DrawString("※处方号:200000102103", font3, bru, 0, 250);
            //e.Graphics.DrawString("1/2 复方磺胺甲噁唑片", font3, bru, 0, 270);
            //e.Graphics.DrawString("    150mgx10粒/盒（天津金耀氨基酸有限公司）", font3, bru, 0, 285);
            //e.Graphics.DrawString("    22-1-1", font, bru, 0, 300);
            //e.Graphics.DrawString("    22盒", font, bru, 150, 300);

            //e.Graphics.DrawString("2/2 复方磺胺甲噁唑片", font3, bru, 0, 320);
            //e.Graphics.DrawString("    150mgx10粒/盒（天津金耀氨基酸有限公司）", font3, bru, 0, 335);
            //e.Graphics.DrawString("    22-1-1", font, bru, 0, 350);
            //e.Graphics.DrawString("    22盒", font, bru, 150, 350);

            e.Graphics.DrawString("=======================================", font, bru, 0, yPosition + 15);
            e.Graphics.DrawString(" 祝您早日康复", font3, bru, 0, yPosition + 30);

            e.Graphics.Dispose();
        }

        private Patient getPrescription(string procCode)
        {
            Patient patient = new Patient();
            List<PrescriptionInfo> pInfoList = new List<PrescriptionInfo>();
            if (true)
            {
                MySQLPool pool = MySQLPool.getInstance();
                MySqlConnection conn = null;
                try
                {
                    conn = pool.getConnection();
                    string sql = @"select PrescriptionNo,PatientName,Sex,Age,DeptName,DoctorName,Diagnosis,FetchWindow from prescriptionlist where procCode='"+procCode+"'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (patient.PName == null || "".Equals(patient.PName))
                        {
                            patient.PName = dr.IsDBNull(1) ? "" : dr.GetString(1);
                            patient.Sex = dr.IsDBNull(2) ? "" : dr.GetString(2);
                            patient.Age = dr.IsDBNull(3) ? "" : dr.GetString(3);
                            patient.FetchWindow = dr.IsDBNull(7) ? "" : dr.GetString(7);
                        }

                        PrescriptionInfo prescriptionInfo = new PrescriptionInfo();
                        prescriptionInfo.PrescriptionNO = dr.IsDBNull(0) ? "" : dr.GetString(0);

                        pInfoList.Add(prescriptionInfo);
                    }
                    dr.Close();

                    foreach (PrescriptionInfo prescInfo in pInfoList)
                    {
                        List<PrescriptionDetl> pDetlList = new List<PrescriptionDetl>();

                        string sql2 = "select b.Quantity,b.PrescriptionUnit,b.Price,b.UseFrequency,b.UseDosage,b.UseRoute,c.Drug_Name,c.Drug_Spec,c.Manufactory,c.StorageLoc,Notes,b.num_1 from prescriptiondetail b left join drug_list c on b.DrugCode=c.Drug_Code where b.PrescriptionNo='" + prescInfo.PrescriptionNO + "'";
                        MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                        MySqlDataReader dr2 = cmd2.ExecuteReader();
                        while (dr2.Read())
                        {
                            PrescriptionDetl prescriptionDetl = new PrescriptionDetl();
                            prescriptionDetl.DrugName = dr2.IsDBNull(6) ? "" : dr2.GetString(6);
                            prescriptionDetl.DrugSpec = dr2.IsDBNull(7) ? "" : dr2.GetString(7);
                            prescriptionDetl.Manufactory = dr2.IsDBNull(8) ? "" : dr2.GetString(8);
                            prescriptionDetl.PrescriptionQty = dr2.IsDBNull(0) ? "" : dr2.GetString(0);
                            prescriptionDetl.PrescriptionUnit = dr2.IsDBNull(1) ? "" : dr2.GetString(1);
                            prescriptionDetl.StorageLoc = dr2.IsDBNull(9) ? "" : dr2.GetString(9);
                            prescriptionDetl.UseFrequency = dr2.IsDBNull(3) ? "" : dr2.GetString(3);
                            prescriptionDetl.UseDosage = dr2.IsDBNull(4) ? "" : dr2.GetString(4);
                            prescriptionDetl.UseRoute = dr2.IsDBNull(5) ? "" : dr2.GetString(5);
                            prescriptionDetl.Notes = dr2.IsDBNull(10) ? "" : dr2.GetString(10);
                            prescriptionDetl.Num_1 = dr2.IsDBNull(11) ? "" : dr2.GetString(11);
                            prescriptionDetl.Price = dr2.IsDBNull(2) ? "" : dr2.GetString(2);
                            pDetlList.Add(prescriptionDetl);
                        }
                        dr2.Close();

                        prescInfo.DetlList = pDetlList;
                    }

                    patient.PrescInfo = pInfoList;

                    sql = "update prescriptionlist set ProcFlg=5 where ProcCode='" + procCode + "'";
                    MySqlCommand cmd3 = new MySqlCommand(sql, conn);
                    cmd3.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    NELog.WriteLog(ex.ToString());
                }
                finally
                {
                    pool.releaseConnection(conn);
                }
            } 
            else
            {
                patient.PName = "张珊珊";
                patient.Sex = "女";
                patient.Age = "2岁半";
                patient.FetchWindow = "④";

                {
                    PrescriptionInfo prescriptionInfo = new PrescriptionInfo();
                    prescriptionInfo.PrescriptionNO = "200000102103";

                    List<PrescriptionDetl> pDetlList = new List<PrescriptionDetl>();

                    {
                        PrescriptionDetl prescriptionDetl = new PrescriptionDetl();
                        prescriptionDetl.DrugName = "复方磺胺甲噁唑片";
                        prescriptionDetl.DrugSpec = "150mgx10粒/盒";
                        prescriptionDetl.Manufactory = "天津金耀氨基酸有限公司";
                        prescriptionDetl.PrescriptionQty = "2";
                        prescriptionDetl.PrescriptionUnit = "盒";
                        prescriptionDetl.StorageLoc = "22-2-2";
                        pDetlList.Add(prescriptionDetl);
                    }

                    {
                        PrescriptionDetl prescriptionDetl = new PrescriptionDetl();
                        prescriptionDetl.DrugName = "复方磺胺甲噁唑片";
                        prescriptionDetl.DrugSpec = "150mgx10粒/盒";
                        prescriptionDetl.Manufactory = "天津金耀氨基酸有限公司";
                        prescriptionDetl.PrescriptionQty = "2";
                        prescriptionDetl.PrescriptionUnit = "盒";
                        prescriptionDetl.StorageLoc = "22-2-2";
                        pDetlList.Add(prescriptionDetl);
                    }
                    prescriptionInfo.DetlList = pDetlList;

                    pInfoList.Add(prescriptionInfo);
                }
                {
                    PrescriptionInfo prescriptionInfo = new PrescriptionInfo();
                    prescriptionInfo.PrescriptionNO = "200000102102";

                    List<PrescriptionDetl> pDetlList = new List<PrescriptionDetl>();

                    {
                        PrescriptionDetl prescriptionDetl = new PrescriptionDetl();
                        prescriptionDetl.DrugName = "2复方磺胺甲噁唑片";
                        prescriptionDetl.DrugSpec = "150mgx10粒/盒";
                        prescriptionDetl.Manufactory = "天津金耀氨基酸有限公司";
                        prescriptionDetl.PrescriptionQty = "2";
                        prescriptionDetl.PrescriptionUnit = "盒";
                        prescriptionDetl.StorageLoc = "22-2-2";
                        pDetlList.Add(prescriptionDetl);
                    }

                    prescriptionInfo.DetlList = pDetlList;

                    pInfoList.Add(prescriptionInfo);
                }

                patient.PrescInfo = pInfoList;
            }

            return patient;
        }
    }
}