using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace ReportsWeb.Reports.SO
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection("Dsn=wec;Driver={C:\\Program Files\\OpenLink\\olod3032.dll};fetchbuffersize=30;host=142.2.70.8;nologinbox=No;protocol=TCP/IP;readonly=No;servertype=Progress 83C");
            SqlDataAdapter myda = new SqlDataAdapter("select * from siv_mstr where siv_nbr = 'D300056'",myconn);
            DataSet myds = new DataSet();
            myconn.Open();
            myda.Fill(myds);
            myconn.Close();
            ReportDataSource rds = new ReportDataSource("siv_mstr", myds.Tables[0]);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(rds);
            ReportViewer1.LocalReport.Refresh();


           
        }
    }
}