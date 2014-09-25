using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Security;

using System.Data;
using Npgsql;

namespace ReportsWeb
{
    public class Sqlhelper
    {
        public static string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["WebReportConnectionString"].ConnectionString;
        public static string connstrpostgresqlWWTS = System.Configuration.ConfigurationManager.ConnectionStrings["Web360WWTSConnectionString"].ConnectionString;
        public static string connstrpostgresqlWTSZ = System.Configuration.ConfigurationManager.ConnectionStrings["Web360WTSZConnectionString"].ConnectionString;

        public static void rlog(string logname,string logdesc, string logsystem, string rusername,string rip)
        {    
            string strlogsql = "INSERT INTO [dbo].[rlog] ([rlogname],[rlogdesc],[rsystem],[rusername],[rip],[rdate]) VALUES('" + logname + "','" + logdesc + "','" + logsystem + "','" + rusername + "','" + rip + "','" + DateTime.Now + "')";
            SqlConnection conn = new SqlConnection(connstr);
            try
            {

                SqlCommand scom = new SqlCommand(strlogsql, conn);
                conn.Open();
                scom.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally {
                conn.Close();
            }


        }
        public static bool authUrlInRolesname(string url, string[] rolesname)
        {
            try
            {
            
            bool exiturlinrule = false;

            if (rolesname.Count() >= 1)
            {
                foreach (var item in rolesname)
                {
                    if (item == "admin")
                    {
                        exiturlinrule = true;
                        break;
                    }
                    if (Sqlhelper.findUrlInRoles(url, item))
                    {
                        exiturlinrule = true;
                        break;
                    }

                }
            }
            return exiturlinrule;
            }
            catch (Exception)
            {
                
                return false;
            }

        }
        public static bool findUrlInRoles(string Url, string rolesName) 
        {

            string strsqlurl = @"select * from dbo.rUurlInRoles where [urlenable] = '1' and [url]='" + Url + "' and [rolename]='"+ rolesName+"'";
            SqlConnection mysqlcon = new SqlConnection(connstr);
            try
            {
                SqlCommand com1 = new SqlCommand(strsqlurl, mysqlcon);
                mysqlcon.Open();
                var execread = com1.ExecuteReader();
                if (!execread.HasRows)
                {
                    //this.Response.Redirect("/");
                    return false;
                }
                else
                {
                    return true;
                }
                mysqlcon.Close();
            }
            catch (Exception ex)
            {
                mysqlcon.Close();
                //Response.Write("<h2 class='error'>systeminit:" + ex.Message + "</h2>");
                return false;
            }
            finally
            {
                mysqlcon.Close();
            }
        }
        public static string findIntableFromDataSourcename(string datatype,string dataSourceName) {
            string sqlgetmaxsela = "SELECT TOP (1) Id, uext, usystem FROM udefine WHERE (uname = 'db') AND (uvalue = '"+datatype+"') AND (udesc = '" + dataSourceName + "')";
            SqlConnection conn = new SqlConnection(connstr);
            try
            {
                SqlCommand com = new SqlCommand(sqlgetmaxsela, conn);
                conn.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    return dr["uext"].ToString();
                }
                else
                {
                    return "Nothing";
                }
            }
            catch (Exception ex)
            {
                return "Nothing";
            }
            finally
            {
                conn.Close();
            }
        }
        public static string getMaxLogSealtype(string username)
        {
            string sqlgetmaxsela = "SELECT top 1 [sealtypes],[maxSeal] FROM [dbo].[v_r_maxSeallog] where [rusername] = '" + username + "'";
            SqlConnection conn = new SqlConnection(connstr);
            try
            {
                SqlCommand com = new SqlCommand(sqlgetmaxsela, conn);
                conn.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    return dr["sealtypes"].ToString();
                }
                else
                {
                    return "Nothing";
                }
            }
            catch (Exception ex)
            {
                return "Nothing";
            }
            finally
            {
                conn.Close();
            }

        }
        public static string getSystemServiceNum(string systemvalue)
        {
            string sqlgetmaxsela = "SELECT TOP (1) syschar1 FROM rsystem WHERE (syswebvalue = '"+systemvalue+"') AND (sysenable = 1)";
            SqlConnection conn = new SqlConnection(connstr);
            try
            {
                SqlCommand com = new SqlCommand(sqlgetmaxsela, conn);
                conn.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    return dr["syschar1"].ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }

        }
        public static string getSecondPasswrdflag(string strdomain, string username, string secondpassword)
        {
            string flag = "";
            ServiceReference208.UserLoginCheckSoapClient service = new ServiceReference208.UserLoginCheckSoapClient();
            flag = service.WFUserSecondPwdChk(strdomain,username,secondpassword);
            return flag;
        }
        #region 360data Postgresql
        public static NpgsqlDataReader getTestDataReader() {
            NpgsqlConnection connpg = new NpgsqlConnection(connstrpostgresqlWWTS);
            connpg.Open();
            NpgsqlCommand comm = new NpgsqlCommand("select * from public.user", connpg);
            try
            {
                NpgsqlDataReader dr = comm.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                connpg.Close();
            }
 
        }
        public static DataSet get360HWDatasetByIp(string strIp, string connstrpostgresql)
        {
            NpgsqlConnection connpg = GetConnpg(connstrpostgresql);
            DataSet myds = new DataSet();
            connpg.Open();         
            try
            {
                string strsql = @"select * from  get_hws where ip = '" + strIp + "' order by s_type";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(strsql, connpg);
                da.Fill(myds);
                return myds;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connpg.Close();
            }
 
        }

        private static NpgsqlConnection GetConnpg(string connstrpostgresql)
        {
            NpgsqlConnection connpg = new NpgsqlConnection(connstrpostgresql);
            return connpg;
        }
        public static DataSet get360HWDatasetByName(string strName, string connstrpostgresql)
        {
            NpgsqlConnection connpg = GetConnpg(connstrpostgresql);
            DataSet myds = new DataSet();
            connpg.Open();
            try
            {
                string strsql = @"select * from  get_hws where name = '" + strName.ToUpper() + "' order by ip,s_type";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(strsql, connpg);
                da.Fill(myds);
                return myds;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connpg.Close();
            }

        }
        public static DataSet get360SoftDatasetByIp(string strIp, string connstrpostgresql)
        {
            NpgsqlConnection connpg = GetConnpg(connstrpostgresql);
            DataSet myds = new DataSet();
            connpg.Open();
            try
            {
                string strsql = @"select distinct mid,s_name,osver,s_time  from get_softs WHERE ip = '" + strIp + "' order by s_name ";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(strsql, connpg);
                da.Fill(myds);
                return myds;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connpg.Close();
            }

        }
        public static DataSet get360SoftDatasetByName(string strName, string connstrpostgresql)
        {
            NpgsqlConnection connpg = GetConnpg(connstrpostgresql);
            DataSet myds = new DataSet();
            connpg.Open();
            try
            {
                string strsql = @"select distinct mid,s_name,osver,s_time  from get_softs WHERE name = '" + strName.ToUpper() + "' order by s_name ";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(strsql, connpg);
                da.Fill(myds);
                return myds;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connpg.Close();
            }

        }

        #endregion
    }
}