using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Data;

namespace TCInterface
{
    public class OracleHelper
    {
        //select * from yh where id=:id” oracle的
        //连接字符串Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=这里填写服务器名称)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=数据库用户名;Password=登录密码
        
        /// <summary>
        /// 执行存储过程获取带有Out的参数
        /// </summary>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="outParameters">输出的参数名</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static string ExecToStoredProcedureGetString(string cmdText, string outParameters, OracleParameter[] oracleParameters, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                //OracleParameter[] SqlParameters = {

                //    new OracleParameter("ZYH", OracleDbType.Varchar2,"",ParameterDirection.Input),
                //    new OracleParameter("YJJE", OracleDbType.Varchar2,100,"", ParameterDirection.Output)
                //};
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(oracleParameters);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return cmd.Parameters[outParameters].Value.ToString();
            }
        }

        /// <summary>
        /// 执行存储过程获取带有Out的游标数据集
        /// </summary>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="outParameters">输出的游标名</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static DataTable ExecToStoredProcedureGetTable(string storedProcedName, OracleParameter[] oracleParameters, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(storedProcedName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(oracleParameters);
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                conn.Open();
                DataSet ds = new DataSet();
                oda.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 执行存储过程没有返回值
        /// </summary>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="outParameters">参数</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static void ExecToStoredProcedure(string cmdText, OracleParameter[] oracleParameters, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(oracleParameters);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 执行sql获取数据集
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static DataTable ExecToSqlGetTable(string cmdText, OracleParameter[] oracleParameters, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(oracleParameters);
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                conn.Open();
                DataSet ds = new DataSet();
                oda.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 执行sql获取数据集
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static DataTable ExecToSqlGetTable(string cmdText, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                conn.Open();
                DataSet ds = new DataSet();
                oda.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
        }

        /// <summary>
        /// 执行sql执行增删改
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static int ExecToSqlNonQuery(string cmdText, OracleParameter[] oracleParameters, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(oracleParameters);
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }
        
        /// <summary>
        /// 执行sql执行增删改
        /// </summary>
        /// <param name="cmdText">sql语句</param>
        /// <param name="oracleParameters">所传参数（必须按照存储过程参数顺序）</param>
        /// <param name="strConn">链接字符串</param>
        /// <returns></returns>
        public static int ExecToSqlNonQuery(string cmdText, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {
                OracleCommand cmd = new OracleCommand(cmdText, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }

        /// <summary>
        /// 执行多条Sql获取返回值
        /// </summary>
        /// <param name="listSelectSql"></param>
        /// <returns></returns>
        public DataSet Query(List<string> listSelectSql, string strConn)
        {
            using (OracleConnection conn = new OracleConnection(strConn))
            {


                OracleCommand cmd = new OracleCommand();
                StringBuilder strSql = new StringBuilder();
                strSql.Append("begin ");
                for (int i = 0; i < listSelectSql.Count; i++)
                {
                    string paraName = "p_cursor_" + i;
                    strSql.AppendFormat("open :{0} for {1}; ", paraName, listSelectSql[i]);
                    cmd.Parameters.Add(paraName, OracleDbType.RefCursor, DBNull.Value, ParameterDirection.Output);
                }
                strSql.Append("end;");
                cmd.Connection = conn;
                cmd.CommandText = strSql.ToString();
                OracleDataAdapter oda = new OracleDataAdapter();
                oda.SelectCommand = cmd;
                conn.Open();
                DataSet ds = new DataSet();
                oda.Fill(ds);
                conn.Close();
                return ds;
            }
        }
    }
}
