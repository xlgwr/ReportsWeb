using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ReportsWeb.Account
{
    public partial class UserManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = this.Request.Path;
            try
            {
                if (Roles.GetRolesForUser().Contains("admin"))
                {
                    this.Response.Redirect("~/Account/Admin.aspx");
                }
            }
            catch (Exception ex)
            {
                this.Response.Redirect("~/Account/Admin.aspx");
            }
        }
    }
}