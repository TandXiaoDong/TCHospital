using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
//using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Threading;
using TCInterface;
using SharpContent.ApplicationBlocks.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Threading.Tasks;

namespace TCTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Form.CheckForIllegalCrossThreadCalls = false;  //如果捕获了对错误线程的调用，则为 true；否则为 false。
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
      
        string prescNo;
        string name;
        LogManager log = new LogManager();
        private void Form1_Load(object sender, EventArgs e)
        {
            //ConnectDBTest();
            Thread t = new Thread(new ThreadStart(DoPresc));
            t.IsBackground = true; 
            t.Start();
            //设置药品库存更新
            Thread thread_drug = new Thread(new ThreadStart(DoDrugView));
            thread_drug.IsBackground = true;
            thread_drug.Start();

            //////设置数据库管理
            //Thread th_d = new Thread(new ThreadStart(DodbManager));
            //th_d.IsBackground = true;
            //th_d.Start();

        }
        private void DoPresc()
        {
            while (true)
            {
                prescList(); 
                Thread.Sleep(500);                
            }
        }
        private void DoDrugView()
        {//3h更新一次10800000
            while (true)
            {
                drugView();
                //drugExpDate();
                Thread.Sleep(10800000);
            }
        }
        private void DodbManager()
        {
            while (true)
            {   //8h更新一次28800000
                dbManager db = new dbManager();
                db.dbManagers_insert();
                db.dbManagers_delete();
                Thread.Sleep(28800000);
            }
        }

        #region 未使用
        private void hisCon(string sql){
            string strCon = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
            OracleConnection con = new OracleConnection(strCon);
            OracleCommand cmd = null;
            try{
            cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText=sql;
            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            }catch(Exception ex){
                log.info("hisCon:"+ex.Message);
            }finally{
                if (cmd.Connection.State == ConnectionState.Open) 
                {
                    cmd.Connection.Close();
                    con.Dispose();
                }
            }
        }
        private Boolean checkPrescDetail(string presc_no,string drug_code,string prescDate) {
            string sql = "select PrescriptionNo,DrugCode,prescdate from prescriptiondetail where PrescriptionNo= '" + presc_no + "' and DrugCode='" + drug_code + "' and prescDate='"+prescDate+"'";              
            string str = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            MySqlConnection con = new MySqlConnection(str);
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql, con);
                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    presc_no = rd[0].ToString();
                    drug_code = rd[1].ToString();
                    prescDate = rd[2].ToString();
                }
                else
                {
                    return false;
                }
            }catch(Exception ex){
                log.info("checkPrescDetail:"+ex.Message);
            }
            finally {
                rd.Close();
                con.Close();
            }
            return true;
        }

        #endregion
        private void prescDetail(string prescNo) { 
        //更新处方明细
            string conString = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
            OracleConnection conn = new OracleConnection(conString);
            OracleDataReader reader = null;
            OracleCommand cmd = null;
            try
            {
                //string sql = "select n.处方号,n.药品代码,n.数量,n.单价,n.用法,n.规格,n.剂量,m.门诊号,m.姓名,convert(varchar(19),m.处方日期,20) as 处方日期  from prescription_detail_view_m n,prescription_state_m m where m.处方号='" + prescNo + "' and m.处方号=n.处方号 and convert(varchar(12),m.处方日期,103) = convert(varchar(12),getdate(),103)";
                //,n.生产厂家,convert(varchar(12),m.处方日期,103) as 处方日期
                //string sql = "select n.处方号,n.药品代码,n.数量,n.单价,n.用法,n.规格,n.剂量 from prescription_detail_view_m n,prescription_state_m m where m.处方号=n.处方号 and convert(varchar(12),m.处方日期,103) = convert(varchar(12),getdate(),103)";
                var sql = "select " +
                    "PRESCRIPTIONNO,DRUGID," +
                    "QUANTITY,PRICE,USEFREQUENCY," +
                    "DRUGSPEC,USEDOSAGE," +
                    "PATIENTNAME,PRESCRIPTIONDATE " +
                    "from PRESCRIPTION_DETAIL_VIEW " +
                    "where to_char(PRESCRIPTIONDATE, 'yyyymmdd') = to_char(sysdate, 'yyyymmdd') " +
                    $"and PRESCRIPTIONNO ='{prescNo}'";
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                { cmd.Connection.Open(); }
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string presc_No = reader[0].ToString().Trim();
                    string drugCode = reader[1].ToString().Trim();
                    string number = reader[2].ToString().Trim();
                    string price = reader[3].ToString().Trim();
                    string usage = reader[4].ToString().Trim();
                    string drugSpec = reader[5].ToString().Trim();
                    string dosage = reader[6].ToString().Trim();
                    string name = reader[7].ToString().Trim();
                    string prescDate = reader[8].ToString().Trim();
                    prescDate = Convert.ToDateTime(prescDate).ToString("yyyy-MM-dd HH:mm:ss");
                    string num = reader[2].ToString().Trim();
                    string presctionUnit= "";

                    if (checkPrescDetail(prescNo, drugCode))
                    {
                        continue;
                    }
                    else
                    {
                        log.info($"插入明细 prescNo={prescNo} drugCode={drugCode}");
                    }
                    //log.info($"读取处方明细 姓名{name}");
                    //插入前将药品单位转换：;处方中有的药可能在本地药品库中找不到，需要添加判断，处方中的这种药品是否在本地中有这个药，没有便不执行
                    if (checkDrugView(drugCode))
                    {
                        string sql_read_unit = "select Package_Unit,Package_Per_Box,Package_Per_Unit from drug_list " +
                            "where Drug_Code='" + drugCode + "' ";
                        MySqlConnection cons = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
                        string Package_unit = null;
                        string Package_Per_Box = null;//每盒系数
                        string Package_Per_Unit = null;
                        try
                        {
                            cons.Open();
                            MySqlCommand cmds = new MySqlCommand(sql_read_unit, cons);
                            cmds.CommandText = sql_read_unit;
                            MySqlDataReader read = cmds.ExecuteReader();
                            if (read.Read())
                            {
                                Package_unit = read[0].ToString();
                                Package_Per_Box = read[1].ToString();
                                Package_Per_Unit = read[2].ToString();
                            }
                            if (Package_Per_Box != null)
                            {
                                int m = Convert.ToInt32(number);
                                int n = Convert.ToInt32(Package_Per_Box);
                                int res_package_box = m % n;
                                if (res_package_box == 0)
                                {//均能整盒发药
                                    number = Convert.ToString(Convert.ToInt32(number) / Convert.ToInt32(Package_Per_Box));
                                    if (!Package_unit.Equals(""))
                                    {
                                         presctionUnit = Package_unit;
                                    }
                                    else
                                    {//整盒发药，单位为大单位
                                        //盒：盒-粒，盒-支，合-支，盒-片，盒-包，盒-袋，盒-贴，盒-瓶，盒-丸，
                                        //瓶：瓶-粒，瓶-片，瓶-ML，
                                        //袋：袋-支，袋-片，袋-克

                                        if ((Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("丸") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("瓶") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("贴") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("袋") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("包") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("片") >= 0) || (Package_Per_Unit.IndexOf("合") >= 0 && Package_Per_Unit.IndexOf("支") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("粒") >= 0) || (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("支") >= 0))
                                        {
                                            presctionUnit = "盒";
                                        }
                                        else if ((Package_Per_Unit.IndexOf("甁") >= 0 && Package_Per_Unit.IndexOf("片") >= 0)||(Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("粒") >= 0) || (Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("片") >= 0) || (Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("ML") >= 0))
                                        {
                                            presctionUnit = "瓶";
                                        }
                                        else if ((Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("支") >= 0) || (Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("片") >= 0) || (Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("克") >= 0))
                                        {
                                            presctionUnit = "袋";
                                        }
                                        else
                                        {//非整盒 
                                                presctionUnit = Package_Per_Unit;
                                        }
                                    }
                                }
                                else
                                {
                                    if ((Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("粒") >= 0) || (Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("粒") >= 0))
                                    {
                                        presctionUnit = "粒";
                                    }
                                    else if ((Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("支") >= 0) || (Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("支") >= 0) || (Package_Per_Unit.IndexOf("合") >= 0 && Package_Per_Unit.IndexOf("支") >= 0))
                                    {
                                        presctionUnit = "支";
                                    }
                                    else if ((Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("片") >= 0) || (Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("片") >= 0) || (Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("片") >= 0))
                                    {
                                        presctionUnit = "片";
                                    }
                                    else if (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("包") >= 0)
                                    {
                                        presctionUnit = "包";
                                    }
                                    else if (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("袋") >= 0)
                                    {
                                        presctionUnit = "袋";
                                    }
                                    else if (Package_Per_Unit.IndexOf("瓶") >= 0 && Package_Per_Unit.IndexOf("ML") >= 0)
                                    {
                                        presctionUnit = "毫升";
                                    }
                                    else if (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("贴") >= 0)
                                    {
                                        presctionUnit = "贴";
                                    }
                                    else if (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("瓶") >= 0)
                                    {
                                        presctionUnit = "瓶";
                                    }
                                    else if (Package_Per_Unit.IndexOf("盒") >= 0 && Package_Per_Unit.IndexOf("丸") >= 0)
                                    {
                                        presctionUnit = "丸";
                                    }
                                    else if (Package_Per_Unit.IndexOf("袋") >= 0 && Package_Per_Unit.IndexOf("克") >= 0)
                                    {
                                        presctionUnit = "克";
                                    }
                                    else
                                    {
                                        presctionUnit = Package_Per_Unit;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.info("readDruglist:"+ex.Message);
                        }
                        finally
                        {
                            cons.Close();
                        }
                    }

                    else 
                    {
                        presctionUnit = "无药"; 
                    }

                    string strMysql = ConfigurationManager.ConnectionStrings["strCon"].ToString();
                    MySqlConnection con = new MySqlConnection(strMysql);
                    string mysql = "insert into prescriptiondetail(PrescriptionNo,DrugCode,Quantity," +
                        "Price,UseDosage,Drug_spec,UseRoute,PrescriptionUnit,Num,patientid,patientname,PrescDate ) " +
                        "values('" + presc_No + "','" + drugCode + "','" + number + "','" + price + "'," +
                        "'" + usage + "','" + drugSpec + "','" + dosage + "','" + presctionUnit + "'," +
                        "'" + num + "','','"+name+"','"+prescDate+"')";
                  try
                    {

                        //判断本地库中没有相同的处方  
                        //if (!checkPrescDetail(presc_No, drugCode,prescDate))
                        //{//不存在相同处方便插入
                            //drugUnit();
                            con.Open();
                            MySqlCommand cmdM = new MySqlCommand(mysql, con);
                            cmdM.CommandText = mysql;
                            int rd = cmdM.ExecuteNonQuery();
                        //log.info($"插入处方明细成功 {rd}");
                        //}
                        //else { 
                        //    执行处方明细更新
                        //    MySqlConnection cond = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
                        //    string updateDetail = "update prescriptiondetail set PrescriptionNo='" + presc_No + "',DrugCode='" + drugCode + "',prescDate='"+prescDate+"' where PrescriptionNo='" + presc_No + "' and DrugCode='" + drugCode + "' and prescdate='"+prescDate+"'";                         
                        //    try {
                        //        cond.Open();
                        //        MySqlCommand cmdd = new MySqlCommand(updateDetail,cond);
                        //        cmdd.CommandText = updateDetail;
                        //        cmdd.ExecuteNonQuery();
                        //    }catch(Exception ex){
                        //        MessageBox.Show("处方明细更新异常："+ex.Message);
                        //    }
                        //    finally {
                        //        cond.Close();
                        //    }
                        //}
                        //MessageBox.Show("更新处方完成！");
                    }
                    catch (Exception ex)
                    {
                        log.info("insertPresdetail:"+ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.info("readpresdetailFail:"+ex.Message);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Connection.Close();
                    conn.Dispose();
                }
            }
        }
        private bool checkPrescList(string presc_no) {
            //判断本地数据库中处方表没有重复           
            string localPrescNo = $"select PrescriptionNo,state from prescriptionlist where PrescriptionNo='{presc_no}'";
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
            MySqlDataReader rd=null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(localPrescNo, conn);
                cmd.CommandText = localPrescNo;
                //presc_no = rd[0].ToString();
                rd = cmd.ExecuteReader();
                if (rd.HasRows)
                    return true;
                return false;
            }catch(Exception ex){
                log.info("checkrscList:"+ex.Message);
            }finally{
                rd.Close();
                conn.Close();
            }
            //listView1.Items.Add(prescNo);   
            return false;
        }

        private bool checkPrescDetail(string presc_no,string drugID)
        {
            //判断本地数据库中处方表没有重复     prescriptiondetail(PrescriptionNo      
            string localPrescNo = $"select * from prescriptiondetail where PrescriptionNo='{presc_no}' and DrugCode='{drugID}'";
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
            MySqlDataReader rd = null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(localPrescNo, conn);
                cmd.CommandText = localPrescNo;
                //presc_no = rd[0].ToString();
                rd = cmd.ExecuteReader();
                if (rd.HasRows)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                log.info("checkrscList:" + ex.Message);
            }
            finally
            {
                rd.Close();
                conn.Close();
            }
            //listView1.Items.Add(prescNo);   
            return false;
        }
        private void prescList()
        {//更新处方信息
            string conString = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
            OracleConnection conn = new OracleConnection(conString);
            OracleDataReader reader = null;
            OracleCommand cmd = null;
            try
            {
                //string sql = "select 处方号,仓库代码,门诊号,convert(varchar(19),处方日期,20) as 处方日期,姓名,性别,年龄,病情,view_section.deptname,大夫代码,窗口号,cfbz from prescription_state_m,view_section where convert(varchar(12),处方日期,103) = convert(varchar(12),getdate(),103) and view_section.deptcode=prescription_state_m.记账科室 and cfbz='01'  ";
                //string sql = "select 处方号,仓库代码,门诊号,convert(varchar(12),处方日期,103) as 处方日期,姓名,性别,年龄,病情,记账科室,大夫代码,convert(varchar(12),处方日期,103) as 发药日期,窗口号 from prescription_state_m where convert(varchar(12),处方日期,103) = convert(varchar(12),getdate(),103)";

                //string sql = "select 处方号,仓库代码,门诊号 ,处方日期,姓名,性别,年龄,病情,记账科室,大夫代码,发药日期,窗口号 from prescription_state_m where convert(varchar(12),处方日期,103) = convert(varchar(12),getdate(),103)";
                string sql = "select " +
                    "PRESCRIPTIONNO,PRESCRIPTIONDATE," +
                    "PATIENTNAME,SEX,AGE " +//,DEPTNAME,WINDOWSNO 
                    "from PRESCRIPTION_DETAIL_VIEW " +
                    "where to_char(PRESCRIPTIONDATE, 'yyyymmdd') = to_char(sysdate, 'yyyymmdd')";
                //Boolean boo = false;
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                { 
                    cmd.Connection.Open(); 
                }
                reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    return;
                while (reader.Read())
                {
                    prescNo = reader[0].ToString();                   
                    string prescDate = (reader[1]).ToString();
                    if (prescDate != "")
                        prescDate = Convert.ToDateTime(prescDate).ToString("yyyy-MM-dd HH:mm:ss");
                    else
                        prescDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    name = reader[2].ToString();
                    string sex_his = reader[3].ToString();
                    string sex;
                    if (sex_his.Equals("1"))
                    {
                        sex = "男";
                    }
                    else
                    {
                        sex = "女";
                    }
                    string age_h = reader[4].ToString();
                    string chargeDept = "";//reader[5].ToString();
                    string ckhm = "";//reader[6].ToString();
                    if (string.IsNullOrEmpty(ckhm))
                    {
                        ckhm = "1";
                        //log.info($"ckhm is null ,set default value {ckhm}");
                    }
                    string age = age_h + " " + chargeDept;

                    //将处方插入本地数据库
                    string strMysql = ConfigurationManager.ConnectionStrings["strCon"].ToString();
                    MySqlConnection con = new MySqlConnection(strMysql);
                    
                    string mysql = "insert into prescriptionlist(PrescriptionNo,Pharmacy,PatientID,PrescriptionDate,PatientName,Sex,Age,Diagnosis,DeptCode,DoctorCode,FetchWindow,State ) " +
                        "values('" + prescNo + "','','','" + prescDate + "','" + name + "','" + sex + "','" + age + "','','" + chargeDept + "','','" + ckhm + "','')";
                    string myUpdate = "update prescriptionlist set PrescriptionNo='" + prescNo + "'where PrescriptionNo='"+prescNo+"'";
                    //,Pharmacy='" + storageNo + "',ClinicNo='" + clinicNo + "',PrescriptionDate='" + prescDate + "',PatientName='" + name + "',Sex='" + sex + "',Age='" + age + "',Diagnosis='" + illness + "',DeptCode='" + chargeDept + "',DoctorCode='" + doctorNo + "',DispensingDate='" + dispensingDate + "',FetchWindow='" + ckhm + "'                                                                                                                                                                                                                    
                    try
                    {                          
                        //判断本地库中没有相同的处方 
                        if (!checkPrescList(prescNo))
                        {//本地数据库没有该处方，执行插入操作
                            con.Open();
                            MySqlCommand cmdM = new MySqlCommand(mysql, con);
                            cmdM.CommandText = mysql;
                            cmdM.ExecuteNonQuery();
                            //更新最新一条数据到界面
                            prescDetail(prescNo);
                            textBox1.Text = "处方号：" + prescNo + "   姓名：" + name;
                            //log.info($"插入到本地成功...姓名={name} 处方号={prescNo} ");
                            Application.DoEvents();
                        }
                        else
                        {//如果存在，便执行更新操作
                            MySqlConnection cond = new MySqlConnection(strMysql);
                            try
                            {
                                cond.Open();
                                MySqlCommand cmd1 = new MySqlCommand(myUpdate, cond);
                                cmd1.CommandText = myUpdate;
                                cmd1.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                log.info("updatePresclistFail:"+ex.Message);
                            }
                            finally
                            {
                                cond.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.info("insertpresclistfail:"+ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.info("readPresclistFail:"+ex.Message+ex.StackTrace);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Connection.Close();
                    conn.Dispose();
                }
            }
        }
        private Boolean checkDrugView(string drugCode) {
            string drugSql = "select drug_code  from drug_list where Drug_Code = '" + drugCode + "'";
            string strCon = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            MySqlConnection con = new MySqlConnection(strCon);
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmdL = new MySqlCommand(drugSql, con);
                cmdL.CommandText = drugSql;
                rd = cmdL.ExecuteReader();
                if(rd.Read()){
                    drugCode = rd[0].ToString(); 
                }
                else {
                    return false;
                }
            }catch(Exception ex){
                log.info("checkDrugView:"+ex.Message);
            }
            finally {
                rd.Close();
                con.Close();
            }
            return true;
        }
        private Boolean checkEquDrug(string drugCode)
        {
            string drugSql = "select drug_code from drug_list where drug_code in (select drug_code from v_f_stock2 where drug_code='"+drugCode+"' )";
            string strCon = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            MySqlConnection con = new MySqlConnection(strCon);
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmdL = new MySqlCommand(drugSql, con);
                cmdL.CommandText = drugSql;
                rd = cmdL.ExecuteReader();
                if (rd.Read())
                {
                    drugCode = rd[0].ToString();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.info("checkEquDrug:"+ex.Message);
            }
            finally
            {
                rd.Close();
                con.Close();
            }
            return true;
        }
        private void drugView()
        {
            string conString = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
            OracleConnection conn = null;
            OracleDataReader reader = null;
            OracleCommand cmd = null;
            try
            {
                //string sql = "select 药品代码,药品名称,拼音代码,计量单位,规格,计量,剂型 from drug_view";
                var sql = "select drugcode,drugname,shortcode,unit,drugspec,dose_per_unit,drugtype from drug_view";
                conn = new OracleConnection(conString);
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                { cmd.Connection.Open(); }
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string drugcode = reader[0].ToString().Trim();
                    string drugname = reader[1].ToString().Trim();
                    string drugShort = reader[2].ToString().Trim();
                    string drugUnit = reader[3].ToString().Trim();
                    string drugSpec = reader[4].ToString().Trim();
                    string drugJl = reader[5].ToString().Trim();
                    string drugType = reader[6].ToString().Trim();
                    string num = "";
                    if (drugSpec.Contains("*"))
                         num = drugSpec.Substring(drugSpec.IndexOf('*') + 1);
                    //string drugJUnit = reader[7].ToString();药品计量单位
                    //插入本地数据库

                    string strMysql = ConfigurationManager.ConnectionStrings["strCon"].ToString();
                    MySqlConnection con = new MySqlConnection(strMysql);
                    string mysql = "insert into drug_list(Drug_Code,Drug_Name,Short_Code,Package_Per_Unit,Drug_Spec,Dosage,Type,Package_Per_Box ) values('" + drugcode + "','" + drugname + "','" + drugShort + "','" + drugUnit + "','" + drugSpec + "','" + drugJl + "','" + drugType + "','"+num+"')";
                    try
                    {
                        con.Open();
                        MySqlCommand cmdM = new MySqlCommand(mysql, con);
                        cmdM.CommandText = mysql;

                        //判断本地库插入没有药品的药  
                        if (!checkDrugView(drugcode))
                        {
                            int rd = cmdM.ExecuteNonQuery();
                        }
                        else
                        {
                            MySqlConnection cond = new MySqlConnection(strMysql);
                            string updateDrug = "update drug_list set Drug_Code = '" + drugcode + "' where Drug_Code = '" + drugcode + "'";
                            try
                            {
                                cond.Open();
                                MySqlCommand cmdd = new MySqlCommand(updateDrug, cond);
                                cmdd.CommandText = updateDrug;
                                cmdd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                log.info("updatedrugview:"+ex.Message+ex.StackTrace);
                            }
                            finally
                            {
                                cond.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.info("insertDrugview"+ex.Message +ex.StackTrace);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.info("readdrugviewfail:"+ex.Message+ex.StackTrace);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Connection.Close();
                    conn.Dispose();
                }
            }
        }
        private Boolean checkDrugExDate(string drugCode,string bno)
        {
            string drugSql = "select drug_code,BNOTP from drug_bnotp_list where Drug_Code = '" + drugCode + "' and bnotp='"+bno+"'";
            string strCon = ConfigurationManager.ConnectionStrings["strCon"].ToString();
            MySqlConnection con = new MySqlConnection(strCon);
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmdL = new MySqlCommand(drugSql, con);
                cmdL.CommandText = drugSql;
                rd = cmdL.ExecuteReader();
                if (rd.Read())
                {
                    drugCode = rd[0].ToString();
                    bno = rd[1].ToString();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.info("checkDrugExdate:"+ex.Message);
            }
            finally
            {
                rd.Close();
                con.Close();
            }
            return true;
        }
        private void drugExpDate()
        {
            string conString = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
            OracleConnection conn = null;
            OracleDataReader reader = null;
            OracleCommand cmd = null;
            try
            {
                //string sql = "select 药品代码,生产批号,convert(varchar(19),有效期,20) as 有效期 ,生产厂家代码 from drug_batch_view";
                var sql = "select drugid,manubatch,convert(varchar(19),manudate,20) as startDate," +
                    "convert(varchar(19),drugspec,20) as endDate from drug_batch_view";
                conn = new OracleConnection(conString);
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                { cmd.Connection.Open(); }
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string drug_code = reader[0].ToString().Trim();
                    string bnotp = reader[1].ToString().Trim();
                    string exp_date = reader[2].ToString().Trim();
                    string manufactoryId = reader[3].ToString().Trim();
                    //插入本地数据库

                    string strMysql = ConfigurationManager.ConnectionStrings["strCon"].ToString();
                    MySqlConnection con = new MySqlConnection(strMysql);

                    string mysql = "insert into drug_bnotp_list(Drug_Code,bnotp,exp_date,ManufactoryId) values('" + drug_code + "','" + bnotp + "','" + exp_date + "','"+manufactoryId+"')";
                    try
                    {
                        con.Open();
                        MySqlCommand cmdM = new MySqlCommand(mysql, con);
                        cmdM.CommandText = mysql;

                        //判断本地库插入没有药品的药  
                        if (!checkDrugExDate(drug_code,bnotp))
                        {
                            int rd = cmdM.ExecuteNonQuery();
                        }
                        else
                        {
                            MySqlConnection cond = new MySqlConnection(strMysql);
                            string updateDrug = "update drug_bnotp_list set Drug_Code = '" + drug_code + "' where Drug_Code = '" + drug_code + "'";
                            try
                            {
                                cond.Open();
                                MySqlCommand cmdd = new MySqlCommand(updateDrug, cond);
                                cmdd.CommandText = updateDrug;
                                cmdd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                log.info("updateExpdate"+ex.Message + ex.StackTrace);
                            }
                            finally
                            {
                                cond.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.info("insertExpdate:"+ex.Message+ex.StackTrace);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.info("readExpdate:"+ex.Message);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    reader.Close();
                    reader.Dispose();
                    cmd.Connection.Close();
                    conn.Dispose();
                }
            }
        }

        //private void prescShow()
        //{
        //    //将药品数据更新到本地数据后显示到用户界面                           
        //    if (!checkPrescList(prescNo))
        //    {//当向本地插入时，同时同步到用户界面
        //        //listView1.View = View.Details;
        //        //listView1.LabelEdit = true;
        //        //listView1.AllowColumnReorder = true;
        //        //listView1.CheckBoxes = false;
        //        //listView1.FullRowSelect = true;
        //        //listView1.GridLines = true;
        //        //listView1.Sorting = SortOrder.Ascending;
        //        //columnHeader1.Text = "编号";
               
        //        //listView1.Columns[0].Width = 100;
        //        //ListViewItem item = new ListViewItem();
        //        //item.SubItems.Add(prescNo);
        //        //item.SubItems.Add(name);
        //        //listView1.Items.AddRange(new ListViewItem[] { item });

        //        DataTable tb = new DataTable();
        //        tb.Columns.Add(prescNo);
        //        tb.Columns.Add(name);
        //    }
        //}
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
        }

        private void ConnectDBTest()
        {
            #region OracleConnection
            try
            {
                log.info("开始连接...");
                //var sqlString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=tcdb)));User Id=zdby;Password=zdby";
                //var conString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=tcdb)));User Id=zdby;Password=zdby";
                string conString = ConfigurationManager.ConnectionStrings["oracleConString"].ToString();
                OracleConnection oralceConnection = new OracleConnection(conString);
                oralceConnection.Open();
                OracleCommand oracleCommand = new OracleCommand();
                var sql = "select * from PRESCRIPTION_DETAIL_VIEW";
                oracleCommand.CommandText = sql; //"select * from tctb1 t";//"
                oracleCommand.CommandType = CommandType.Text;
                oracleCommand.Connection = oralceConnection;
                if (oracleCommand.Connection.State != ConnectionState.Open)
                {
                    log.info("连接失败");
                }
                else if (oracleCommand.Connection.State == ConnectionState.Open)
                {
                    log.info("连接成功");
                }
                log.info(sql);
                var oracleReader = oracleCommand.ExecuteReader();
                var b = oracleReader.HasRows;
                log.info("读取数据状态："+b);
                int i = 0;
                while (oracleReader.Read())
                {
                    var v1 = oracleReader[0];
                    var v2 = oracleReader[1];
                    log.info(v1 + "  " + v2);
                    this.textBox1.Text += v1 + "  " + v2;
                    i++;
                    if (i == 10)
                        break;
                }
                oralceConnection.Close();
            }
            catch (Exception ex)
            {
                log.info("test db status:" + ex.Message + ex.StackTrace);
            }
            #endregion
        }

        public void SelectDBTest()
        {
            //连接字符串
            var sqlString =   "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=123.123.123.41)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=csk)));User Id=tcyybyj;Password=tcyybyj";
            var connString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.175)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=tcdb)));User ID=zdby;Password=zdby";
            try
            {
                //测试:通过DataReader简单查询
                using (OracleConnection con = new OracleConnection(connString))
                {
                    con.Open();
                    using (OracleCommand com = con.CreateCommand())
                    {
                        com.CommandText = "select * from TCTB1 t";
                        using (OracleDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //var v1 = reader["stuid"].ToString();
                                var v2 = reader[0].ToString();
                            }
                        }
                    }
                    Console.WriteLine("查询完毕！"); ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnDrugUpdate_Click(object sender, EventArgs e)
        {
            Task.Run(()=>
            {
                drugView();
            });
        }

        private void btnDrugEx_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                drugExpDate();
            });
        }
    }
}
