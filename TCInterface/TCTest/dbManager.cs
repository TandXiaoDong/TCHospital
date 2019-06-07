using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows.Forms;


namespace TCTest
{
    class dbManager
    {
        
        private static void localCon(string sql){
            LogManager log = new LogManager();
         MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
        try {
            con.Open();
            MySqlCommand cmd = new MySqlCommand(sql,con);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }catch(Exception ex){
            log.info("localCon:"+ex.Message);
        }finally{
            con.Close();
            con.Dispose();
        }
    }
        private Boolean checkPrescList(string prescNo,string state) 
        {
            string presclistBack = "select PrescriptionNo,State from prescriptionlist_backup where PrescriptionNo='" + prescNo + "' and State='"+state+"'";
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCOn"].ToString());
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(presclistBack, con);
                cmd.CommandText = presclistBack;
                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    prescNo = rd[0].ToString();
                    state = rd[1].ToString();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                new LogManager().info("checkPrescListdbManager:"+ex.Message);
            }
            finally {
                rd.Close();
                rd.Dispose();
                con.Close();
                con.Dispose();
            }
            return true;
        }
        private Boolean checkPrescDetail(string prescNo, string drugcode)
        {
            string presclistBack = "select PrescriptionNo,DrugCode from prescriptiondetail_backup where PrescriptionNo='" + prescNo + "' and drugcode='" + drugcode + "'";
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCOn"].ToString());
            MySqlDataReader rd = null;
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(presclistBack, con);
                cmd.CommandText = presclistBack;
                rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    prescNo = rd[0].ToString();
                    drugcode = rd[1].ToString();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                new LogManager().info("checkPrescDetaildbManager:"+ex.Message);
            }
            finally
            {
                rd.Close();
                rd.Dispose();
                con.Close();
                con.Dispose();
            }
            return true;
        }
        public void dbManagers_insert()
        {//为防止影响正常发药，每天备份前一天的处方表与处方明细
            presclistbackup();
            prescddetailbackup();
        }
        private void presclistbackup() {
            //查询处方表中的处方是否有已在备份表中，有则不执行，没有则执行
            string preslist = "select PrescriptionNo,State from prescriptionlist";
            MySqlConnection con_0 = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
            string presc_no = null;
            string state = null;
            MySqlDataReader rd = null;
            try
            {
                con_0.Open();
                MySqlCommand cmd_0 = new MySqlCommand(preslist, con_0);
                cmd_0.CommandText = preslist;
                rd = cmd_0.ExecuteReader();
                while (rd.Read())
                {
                    presc_no = rd[0].ToString();
                    state = rd[1].ToString();

                    //备份处方表
                    MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
                    string presclistBackup = "insert into prescriptionlist_backup select * from prescriptionlist where TO_DAYS(PrescriptionDate)<TO_DAYS(SYSDATE()) and PrescriptionNo='" + presc_no + "' and state='" + state + "'";
                    try
                    {
                        if (!checkPrescList(presc_no, state))
                        {
                            con.Open();
                            MySqlCommand cmd = new MySqlCommand(presclistBackup, con);
                            cmd.CommandText = presclistBackup;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //如果存在，便执行更新操作
                            MySqlConnection cond = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
                            string updlist = "update prescriptionlist_backup set PrescriptionNo='" + presc_no + "' ,State='" + state + "' where PrescriptionNo='" + presc_no + "' and State='" + state + "'";
                            try
                            {
                                cond.Open();
                                MySqlCommand cmd1 = new MySqlCommand(updlist, cond);
                                cmd1.CommandText = updlist;
                                cmd1.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                new LogManager().info("update prescriptionlist_backup:"+ex.Message);
                            }
                            finally
                            {
                                cond.Close();
                                cond.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        new LogManager().info("insert into prescriptionlist_backup:"+ex.Message);
                    }
                    finally
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                new LogManager().info("presclistbackup:"+ex.Message);
            }
            finally
            {
                con_0.Close();
                con_0.Dispose();
            }
        }
        private void prescddetailbackup() {
            //查询处方明细中是否已备份到表中
            string presdetail = "select PrescriptionNo,DrugCode from prescriptiondetail";
            MySqlConnection con_1 = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
            string presc_no_1 = null;
            string drug_code = null;
            MySqlDataReader rd1 = null;
            try
            {
                con_1.Open();
                MySqlCommand cmd_1 = new MySqlCommand(presdetail, con_1);
                cmd_1.CommandText = presdetail;
                rd1 = cmd_1.ExecuteReader();
                while (rd1.Read())
                {
                    presc_no_1 = rd1[0].ToString();
                    drug_code = rd1[1].ToString();

                    //备份处方明细表
                    MySqlConnection con2 = new MySqlConnection(ConfigurationManager.ConnectionStrings["strCon"].ToString());
                    string prescdetailBackup = "insert into prescriptiondetail_backup select m.* from prescriptiondetail m ,prescriptionlist d where m.PrescriptionNo=d.PrescriptionNo and TO_DAYS(d.PrescriptionDate)<TO_DAYS(SYSDATE()) and m.PrescriptionNo='" + presc_no_1 + "' and m.DrugCode='" + drug_code + "'";
                    try
                    {

                        if (!checkPrescDetail(presc_no_1, drug_code))
                        {
                            con2.Open();
                            MySqlCommand cmd2 = new MySqlCommand(prescdetailBackup, con2);
                            cmd2.CommandText = prescdetailBackup;
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        new LogManager().info("insert into prescriptiondetail_backup:"+ex.Message);
                    }
                    finally
                    {
                        con2.Close();
                        con2.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                new LogManager().info("prescddetailbackup:"+ex.Message);
            }
            finally
            {
                con_1.Close();
                con_1.Dispose();
            }
        }
        public void dbManagers_delete() {
            string presclistDelete = "delete from prescriptionlist where TO_DAYS(PrescriptionDate)<TO_DAYS(SYSDATE())";
            string prescdetailDelete = "delete m from prescriptiondetail m,prescriptionlist d where m.PrescriptionNo=d.PrescriptionNo and TO_DAYS(d.PrescriptionDate)<TO_DAYS(SYSDATE())";
            
            localCon(prescdetailDelete);
            localCon(presclistDelete);
        }
    }
}
