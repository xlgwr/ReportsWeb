using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

using Microsoft.Reporting.WebForms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Web.Services;
using System.Web.Security;


namespace ReportsWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        private MemoryStream m_rdl;
        //invoice
        DataSet ds_siv = new DataSet();
        DataSet ds_sivd = new DataSet();

        //360Data
        DataSet ds_hw = new DataSet();
        DataSet ds_softs = new DataSet();

        ReportParameter bk = new ReportParameter();
        ReportParameter cust_siv_amt_desc = new ReportParameter();
        ReportParameter seal = new ReportParameter();

        string cust_amt_desc = "";
        Char sp = '@';
        string strdoctype;
        string[] strsp;
        //roles
        string url;
        string[] rolesname;
        MembershipUser username = Membership.GetUser();
        string userplace;
        string rip;
        string rlogdesc;
        string serviceNum;

        //for server gettable dataset
        //wsas011:siv_mstr,wsas012: sivd_det
        string inSystem = "wec",uSystem = "WWTS",inSysWebValue, inTable = "siv_mstr", inWhere = "D300068",rvdispname;

        protected void Page_Load(object sender, EventArgs e)
        {

            url = this.Request.Path;
            rip = HttpContext.Current.Request.UserHostAddress;
            try
            {
                rolesname = Roles.GetRolesForUser();
                if (!Sqlhelper.authUrlInRolesname(url,rolesname))
                  {

                      var errmsg = "Error: [" + username.UserName + "] have no Right Roles to visit [" + url + "],please ask admin for help,thank you!";
                      this.Response.Redirect("~/Account/LogonDomain.aspx?ErrorMsg="+HttpUtility.UrlEncode(errmsg), false);
                 }
            }
            catch (Exception ex)
            {
                
                //Response.Write("<script language='javascript'>alert('" + "Error01: " + ex.Message + "')</script>");
                this.Response.Redirect("~/Account/LogonDomain.aspx?ErrorMsg=" + ex.Message,false);
            }
            if (username.Email.Split('@').Count() >= 2)
            {
               userplace = username.Email.Split('@')[1].ToLower();
            }
            else
            {
                userplace = "wwts";
            }
            
            laberror.Text = "";
            TextBox1.Focus();
            
        }
        private void reportviewerinit()
        {

            inSysWebValue = DropDown_List_rSystem.SelectedItem.Text.Replace("'", "`");
            inSystem = DropDown_List_rSystem.SelectedValue;
            inWhere = TextBox1.Text.Trim();
            strdoctype = DropDl_docType.SelectedItem.Text;
            strsp = strdoctype.Split(sp);
            label_doc_Type.Text = inSysWebValue + "-->" + strsp[0] + sp + strsp[1] + " NO: ";
            if (strsp[0].ToString() == "360Data" || strsp[0].ToString() == "MISPC")
            {
                errormsg(" Enter Computer's IP or Name. like: "+rip);
            }
             else
             {
                 errormsg(" Enter W308818 or or D300068 or D33....");
             }
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            authUsystemDocTypeInRoles();

            rlogdesc =  url + ":->["+label_doc_Type.Text + "]-->BackGround:->[" + RoBtnLst_Print_Type.SelectedValue+"]-->Seal Type:->"+this.DropDownList_SealType.SelectedValue;
                
                
        }

        private void authUsystemDocTypeInRoles()
        {
            if (!Sqlhelper.authUrlInRolesname(this.DropDown_List_rSystem.SelectedValue, rolesname))
            {
                errormsg("[" + username + "] You have no right roles to visit the " + this.label_doc_Type.Text + this.TextBox1.Text);               
                this.Button1.Visible = false;
                return;
            }
            this.Button1.Visible = true;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            //Add_DataSource_SetParam_Show();  

            if (string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                errormsg("Error: Please enter the " + this.label_doc_Type.Text);                
                return;
            }
            try
            {
                reportviewerinit();
                Sqlhelper.rlog(inWhere,rlogdesc, inSysWebValue, username.UserName, rip);
                if (!string.IsNullOrWhiteSpace(strsp[0]))
	            {
                    if (strsp[0].ToString() == "INVOICE")
                    {
                        getInvoideDataSource();
                        if (isNullOrNoRows(ds_siv) || isNullOrNoRows(ds_sivd))
                        {                            
                            return;
                        } 
                         this.laberror.Visible = false;
                         this.laberror.Text = "";
                         uSystem = ds_siv.Tables[0].Rows[0]["siv__chr02"].ToString();
                         //Response.Write(uSystem);
                         cust_amt_desc = ds_siv.Tables[0].Rows[0]["siv_amt_desc"].ToString();

                    }
                    else if (strsp[0].ToString() == "MISPC")
                    {
                        char spc = '-';
                        string[] stru = strsp[1].Split(spc);
                        uSystem = stru[0].ToUpper();
                        if (uSystem=="WWTS")
                        {                            
                            get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                        }
                        else if (uSystem == "WTSZ")
                        {
                            get360DataDataSource(Sqlhelper.connstrpostgresqlWTSZ);
                        }
                        else
                        {
                            get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                        }
                        if (isNullOrNoRows(ds_hw) || isNullOrNoRows(ds_softs))
                        {
                            return;
                        }
                        this.laberror.Visible = false;
                        this.laberror.Text = "";
                    }
                    else if (strsp[0].ToString() == "360Data")
                    {
                        get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                        if (isNullOrNoRows(ds_hw) || isNullOrNoRows(ds_softs))
                        {
                            return;
                        }
                        this.laberror.Visible = false;
                        this.laberror.Text = "";
                        uSystem = this.DropDown_List_rSystem.SelectedValue;
                    }

	            }
            
                txt_amt_desc.Text = cust_amt_desc;

                systeminit(uSystem,inSystem);
               
                add_ReportPara();
                ReportViewer1.LocalReport.DisplayName = rvdispname;
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.Visible = true;

               // Btn_Cust.Visible = true;



            }
            catch (Exception ex)
            {

               // Response.Write("<h2 class='error'>Btn-Error:" + ex.Message + "</h2>");
                
                return;
            }
            
        }

        private void getInvoideDataSource()
        {
            string intable11 = Sqlhelper.findIntableFromDataSourcename("invoice", "siv_mstr");
            string intable12 = Sqlhelper.findIntableFromDataSourcename("invoice", "sivd_det");
            serviceNum = Sqlhelper.getSystemServiceNum(inSystem);
            ds_siv = add_reportDataSource_ReportViewer(serviceNum,inSystem, intable11, inWhere, ReportViewer1, "siv_mstr");
            ds_sivd = add_reportDataSource_ReportViewer(serviceNum,inSystem, intable12, inWhere, ReportViewer1, "sivd_det");
        }
        private void get360DataDataSource(string connstrpostgresql)
        {
            char cip = '.';
            int isip = 0; 
            bool isdshw;
            bool isdssofts; 
            if (inWhere.IndexOf(cip,0) > 0)
            {
              string[] strip = inWhere.Split(cip);
              if (int.Parse(strip[0].ToString()) > 0)
              {
                  isip = strip.Count();  
              }     
            }
            //inWhere = inWhere.ToUpper().Trim();
            if (isip == 4)
            {
                ds_hw = Sqlhelper.get360HWDatasetByIp(inWhere, connstrpostgresql);
                ds_softs = Sqlhelper.get360SoftDatasetByIp(inWhere, connstrpostgresql);
            }
            else
            {
                ds_hw = Sqlhelper.get360HWDatasetByName(inWhere, connstrpostgresql);
                ds_softs = Sqlhelper.get360SoftDatasetByName(inWhere, connstrpostgresql);
            }
            isdshw=add_reportDataSource_ReportViewer(this.ReportViewer1, "user_hw", ds_hw);
            isdssofts=add_reportDataSource_ReportViewer(this.ReportViewer1,"user_softs",ds_softs);
        }

        private bool isNullOrNoRows(DataSet iNullds)
        {
            if (iNullds == null)
            {
                string msgs = "Error1: " + inSysWebValue + " have no " + strsp[0].ToString() + " ,Please Select the Right System " + strsp[0].ToString() + ". Thank you!";
                errormsg(msgs);
                //Response.Write("<h2 class='error'> Error: " + inSystem + " have no " + strsp[0].ToString() + " programs to send dataset!" + "</h2>");
                this.ReportViewer1.Visible = false;
                Btn_Cust.Visible = false;
                return true;
            }
            if (iNullds.Tables[0].Rows.Count == 0)
            {
                string msgs="Error2:";
                if (inSysWebValue=="MIS" || inSysWebValue=="360System")
                {
                    msgs = "Error2:" + inSysWebValue + " have no " + this.DropDl_docType.SelectedValue +  " of [" + inWhere + "] Record,Please Reinstall the Last of 360EntClient, If The PC (" + inWhere + ") of 360EntClientSvc Server have no active,Please set the 360EntClientSvc Server Active Now.Thank you!";

                }
                else
                {
                     msgs = "Error2: " + inSysWebValue + " have no " + strsp[0].ToString() + " of [" + inWhere + "] Record,Please enter the right " + strsp[0].ToString() + " No. Thank you!";
                }
                errormsg(msgs);
                //Response.Write("<h2 class='error'> Error: " + inSystem + " have no " + strsp[0].ToString() + " of " + inWhere + " Record,Please enter the right " + strsp[0].ToString() + " No. Thank you!" + "</h2>");
                this.ReportViewer1.Visible = false;
                Btn_Cust.Visible = false;
                return true;

            }
            return false;
        }

        private void errormsg(string msg)
        {
            this.laberror.Visible = true;
            this.laberror.Text = msg;

        }
        private bool add_reportDataSource_ReportViewer(ReportViewer rv,string reportDataSourceName,DataSet ds)
        {
            try
            {
             ReportDataSource rd = new ReportDataSource(reportDataSourceName, ds.Tables[0]);
             rv.LocalReport.DataSources.Add(rd);
             return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        private DataSet add_reportDataSource_ReportViewer(string serviceNum,string insystem,string intable,string inwhere,ReportViewer rv,string reportDataSourceName)
        {
            try
            {
            DataSet ds = null;
            if (serviceNum == "99")
            {
               Service99.ServiceSoapClient service = new Service99.ServiceSoapClient();
               ds = service.GetTable_n(insystem, intable, inwhere);
            }
            else if (serviceNum == "100")
            {
                Service100.ServiceSoapClient service = new Service100.ServiceSoapClient();
                ds = service.GetTable_n(insystem, intable, inwhere);
            }
            else
            {
                Service99.ServiceSoapClient service = new Service99.ServiceSoapClient();
                ds = service.GetTable_n(insystem, intable, inwhere);
            }
            //ds = service.GetTable_n("testwec","siv_mstr","siv_nbr = 'D300068'");
            //ds = service.GetTable_n(insystem,intable,inwhere);

            if (ds == null)
            {
                //Response.Write("<h2 class='error'>add_reportDataSource_ReportViewer:" + ds.DataSetName.ToString() + " is null </h2>");
                return null;
            }
            ReportDataSource rd = new ReportDataSource(reportDataSourceName,ds.Tables[0]);

            rv.LocalReport.DataSources.Add(rd);

            return ds;
            }
            catch (Exception ex)
            {

                //Response.Write("<h2 class='error'>add_reportDataSource_ReportViewer:" + ex.Message + "</h2>");
                return null;
            }
          
            //return service.GetTable_n(inSystem, inTable, inWhere);
        }

        private void systeminit(string uSystem,string inSystem)
        {
            string strconsys = Sqlhelper.connstr;
            string strsqlsys;
            if (!string.IsNullOrWhiteSpace(uSystem))
            {
               strsqlsys = "select top 1 * from rsystem where  sysname = '" + uSystem + "'";
            }
            else
            {
                strsqlsys = "select top 1 * from rsystem where  syswebvalue = '" + inSystem + "'";
            }
             
             string strsqltemple = "select top 1 * from dbo.rtemplate where tdoctype='" + strsp[0].ToString() + "' and [tdesc]='" + strsp[1].ToString() + "'";
            
          
            SqlConnection myconnsql = new SqlConnection(strconsys);
            try
            {
                SqlCommand comsql = new SqlCommand(strsqlsys, myconnsql);
                SqlCommand comsqltemplate = new SqlCommand(strsqltemple, myconnsql);

                
                SqlDataAdapter mydasql = new SqlDataAdapter();
                SqlDataAdapter mydastemplate = new SqlDataAdapter();

                mydasql.SelectCommand = comsql;
                mydastemplate.SelectCommand = comsqltemplate;

                DataSet mydssql = new DataSet();
                DataSet mydssqltemplate = new DataSet();

                myconnsql.Open();

                mydasql.Fill(mydssql);
                mydastemplate.Fill(mydssqltemplate);

                ReportDataSource rdssql = new ReportDataSource("rsystem", mydssql.Tables[0]);

                string tempxml = mydssqltemplate.Tables[0].Rows[0]["tempxml"].ToString();
                XmlDocument sourceDoc = new XmlDocument();
                //string path = AppDomain.CurrentDomain.BaseDirectory + @"Reports\SO\siv_mstr_p.rdlc";
                //sourceDoc.Load(path);
                sourceDoc.LoadXml(tempxml);
                XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
                m_rdl = new MemoryStream();
                
                serializer.Serialize(m_rdl, sourceDoc);

                
                if (m_rdl == null)
                {
                    //Response.Write("<h2 class='error'> " + strdoctype + " of " + inSystem + " tempxml is nothing</h2>");
                    return;
                    
                }
                m_rdl.Position = 0;
                ReportViewer1.LocalReport.LoadReportDefinition(m_rdl);
                ReportViewer1.LocalReport.DataSources.Add(rdssql);

            }
            catch (Exception ex)
            {
                //Response.Write("<h2 class='error'>systeminit:" + ex.Message + "</h2>");
                return;
            }
            finally 
            {
                myconnsql.Close();
            }

        }
        private void Add_DataSource_SetParam_Show()
        {     
            string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string strconsys = System.Configuration.ConfigurationManager.ConnectionStrings["WebReportConnectionString"].ConnectionString;
            strdoctype = DropDl_docType.SelectedItem.Text;
            strsp = strdoctype.Split(sp);

            inSystem = DropDown_List_rSystem.SelectedValue;
            inTable = "siv_mstr";
            inWhere = TextBox1.Text.Trim();

            OdbcConnection myconn = new OdbcConnection(strcon);
            SqlConnection myconnsql = new SqlConnection(strconsys);
            try
            {

                string strsql1 = "select * from siv_mstr where siv_nbr = '" + inWhere + "'";
                string strsql2 = "select * from sivd_det where sivd_nbr = '" + inWhere + "'";
                string strsqlsys = "select * from rsystem where  sysname = '" + inSystem + "'";
                string strsqltemple = "select * from dbo.rtemplate where tdoctype='" + strsp[0].ToString() + "' and [version]='" + DropDl_docType.SelectedItem.Value + "'";


                OdbcCommand com = new OdbcCommand(strsql1, myconn);
                OdbcCommand com2 = new OdbcCommand(strsql2, myconn);
                SqlCommand comsql = new SqlCommand(strsqlsys, myconnsql);
                SqlCommand comsqltemplate = new SqlCommand(strsqltemple, myconnsql);



                OdbcDataAdapter myda = new OdbcDataAdapter();
                OdbcDataAdapter myda2 = new OdbcDataAdapter();
                SqlDataAdapter mydasql = new SqlDataAdapter();
                SqlDataAdapter mydastemplate = new SqlDataAdapter();

                myda.SelectCommand = com;
                myda2.SelectCommand = com2;
                mydasql.SelectCommand = comsql;
                mydastemplate.SelectCommand = comsqltemplate;

                DataSet myds = new DataSet();
                DataSet myds2 = new DataSet();
                DataSet mydssql = new DataSet();
                DataSet mydssqltemplate = new DataSet();

                myconn.Open();

                myda.Fill(myds);
                myda2.Fill(myds2);
                mydasql.Fill(mydssql);
                mydastemplate.Fill(mydssqltemplate);

                myconn.Close();

                ReportDataSource rds = new ReportDataSource("siv_mstr", myds.Tables[0]);
                ReportDataSource rds2 = new ReportDataSource("sivd_det", myds2.Tables[0]);
                ReportDataSource rdssql = new ReportDataSource("rsystem", mydssql.Tables[0]);

                cust_amt_desc = myds.Tables[0].Rows[0]["siv_amt_desc"].ToString();

                string tempxml = mydssqltemplate.Tables[0].Rows[0]["tempxml"].ToString();
                XmlDocument sourceDoc = new XmlDocument();
                //string path = AppDomain.CurrentDomain.BaseDirectory + "siv_mstr.rdlc";
                sourceDoc.LoadXml(tempxml);
                XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
                m_rdl = new MemoryStream();
                
                serializer.Serialize(m_rdl, sourceDoc);

                
                if (m_rdl == null)
                {
                    return;
                }
                ReportViewer1.Reset();
                m_rdl.Position = 0;
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.LoadReportDefinition(m_rdl);
                
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.DataSources.Add(rds2);
                ReportViewer1.LocalReport.DataSources.Add(rdssql);

                bk = new ReportParameter("bk", RoBtnLst_Print_Type.SelectedValue);
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bk });

              
                ReportViewer1.LocalReport.Refresh();
                ReportViewer1.Visible = true;
            }
            catch (Exception ex)
            {
                //Response.Write("<h2 class='error'>" + ex.Message + "</h2>");
                return;
            }
            finally 
            {
                myconn.Close();
            }
            
        }

        protected void Btn_Cust_Click(object sender, EventArgs e)
        {
            
            if (Panel_Cust.Visible == false)
            {                
                Panel_Cust.Visible = true;
            }
            else
            {
                Panel_Cust.Visible = false;
            }
        }

        protected void Btn_Refresh_Click(object sender, EventArgs e)
        {

            label_doc_type();
            Sqlhelper.rlog(inWhere, rlogdesc+",CustomerDefine:-->"+ txt_amt_desc.Text, inSysWebValue, username.UserName, rip);
        }

  
       

        protected void DropDown_List_rSystem_DataBound(object sender, EventArgs e)
        {
            for (int i = 0; i < DropDown_List_rSystem.Items.Count; i++)
            {
                if (Sqlhelper.authUrlInRolesname(DropDown_List_rSystem.Items[i].Value,rolesname))
                {
                    DropDown_List_rSystem.Items[i].Selected = true;
                    break;
                }
            }
        }

        protected void DropDl_docType_DataBound(object sender, EventArgs e)
        {
            for (int i = 0; i < DropDl_docType.Items.Count; i++)
            {
                if (DropDl_docType.Items[i].Value.Contains(userplace))
                {
                    DropDl_docType.Items[i].Selected = true;
                    break;
                }
            }            
           
            label_doc_type();
        }

        protected void DropDl_docType_SelectedIndexChanged(object sender, EventArgs e)
        { 
            label_doc_type();
         
        }

        protected void DropDl_docType_TextChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }
       

        protected void RoBtnLst_Print_Type_DataBound(object sender, EventArgs e)
        {

            this.RoBtnLst_Print_Type.SelectedIndex = 0;
        }

     

        protected void DropDown_List_rSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

        protected void DropDown_List_rSystem_TextChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

        protected void DropDownList_SealType_DataBound(object sender, EventArgs e)
        {
           
            try
            {
            string strseal = Sqlhelper.getMaxLogSealtype(username.UserName);
            this.DropDownList_SealType.Items.FindByValue(strseal).Selected = true;
            }
            catch (Exception)
            {
                this.DropDownList_SealType.SelectedIndex = 0;
            }


        }

        protected void DropDownList_SealType_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

        protected void DropDownList_SealType_TextChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

        private void add_ReportPara()
        {
            bk = new ReportParameter("bk", RoBtnLst_Print_Type.SelectedValue);
            cust_siv_amt_desc = new ReportParameter("cust_siv_amt_desc", txt_amt_desc.Text);
            seal = new ReportParameter("seal", DropDownList_SealType.SelectedItem.Value);


            if (!string.IsNullOrWhiteSpace(RoBtnLst_Print_Type.SelectedValue))
            {
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bk });
            }
            if (!string.IsNullOrWhiteSpace(txt_amt_desc.Text))
            {
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { cust_siv_amt_desc });
            }
            if (!string.IsNullOrWhiteSpace(DropDownList_SealType.SelectedItem.Value))
            {
                ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { seal });
            }
            rvdispname = inSysWebValue + inWhere.ToString() + "Date" + DateTime.Today.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Today.Day.ToString() + "H" + DateTime.Now.Hour + "M" + DateTime.Now.Minute;
               
        }

        private void label_doc_type()
        {
            reportviewerinit();
            ReportViewer1.Visible = false;
            if (ReportViewer1.Visible)
            {

                if (strsp[0].ToString() == "INVOICE")
                {
                    getInvoideDataSource();
                    if (isNullOrNoRows(ds_siv) || isNullOrNoRows(ds_sivd))
                    {
                        return;
                    }
                    this.laberror.Visible = false;
                    this.laberror.Text = "";
                    uSystem = ds_siv.Tables[0].Rows[0]["siv__chr02"].ToString();
                    //Response.Write(uSystem);
                    cust_amt_desc = ds_siv.Tables[0].Rows[0]["siv_amt_desc"].ToString();

                }
                else if (strsp[0].ToString() == "MISPC")
                {
                    char spc = '-';
                    string[] stru = strsp[1].Split(spc);
                    uSystem = stru[0].ToUpper();
                    if (uSystem == "WWTS")
                    {
                        get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                    }
                    else if (uSystem == "WTSZ")
                    {
                        get360DataDataSource(Sqlhelper.connstrpostgresqlWTSZ);
                    }
                    else
                    {
                        get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                    }
                    if (isNullOrNoRows(ds_hw) || isNullOrNoRows(ds_softs))
                    {
                        return;
                    }
                    this.laberror.Visible = false;
                    this.laberror.Text = "";
                }
                else if (strsp[0].ToString() == "360Data")
                {
                    get360DataDataSource(Sqlhelper.connstrpostgresqlWWTS);
                    if (isNullOrNoRows(ds_hw) || isNullOrNoRows(ds_softs))
                    {
                        return;
                    }
                    this.laberror.Visible = false;
                    this.laberror.Text = "";
                }
                //uSystem = ds_siv.Tables[0].Rows[0]["siv__chr02"].ToString();
                //cust_amt_desc = ds_siv.Tables[0].Rows[0]["siv_amt_desc"].ToString();
                 systeminit(uSystem,inSystem);
                
                add_ReportPara();
                ReportViewer1.LocalReport.DisplayName = rvdispname;
                ReportViewer1.LocalReport.Refresh();
            }
        }

        protected void RoBtnLst_Print_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

        protected void RoBtnLst_Print_Type_TextChanged(object sender, EventArgs e)
        {
            label_doc_type();
        }

    
    }
}
