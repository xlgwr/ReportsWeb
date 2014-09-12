using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Security;
using System.Runtime.InteropServices;


namespace ReportsWeb.Account
{
    public partial class LogonDomain : System.Web.UI.Page
    {
        string rip;
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            rip = HttpContext.Current.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(HttpUtility.UrlDecode(Request.QueryString["ErrorMsg"])))
            {
                this.labLogonError.Text = HttpUtility.UrlDecode(Request.QueryString["ErrorMsg"]);
            }
            this.TextBoxName.Focus();
        }     
        protected void ButLogon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxName.Text) || string.IsNullOrWhiteSpace(TextBoxpassword.Text) || string.IsNullOrWhiteSpace(TextBoxPassPhrase.Text) || string.IsNullOrWhiteSpace(TextBoxPassPhrase.Text))
            {
                this.labLogonError.Text = "Please enter Your Name or Password,Thank you!";
                return;
            }
            try
            {
            string[] userdomains= this.DropDownListUserDomain.SelectedValue.Split('@');
            string passwd = this.TextBoxpassword.Text;
            string returntxt = syslogon.ADValidate(userdomains[0], TextBoxName.Text.Trim(), this.TextBoxpassword.Text);
            //string returntxt = IdentityScope.IdentityScope_u(TextBoxName.Text.Trim(), userdomains[2], this.TextBoxpassword.Text);
            string secondpasswdflag = Sqlhelper.getSecondPasswrdflag(userdomains[1],TextBoxName.Text,TextBoxPassPhrase.Text);
            if ( returntxt == "OK")
            {
                var users = Membership.FindUsersByName(TextBoxName.Text.Trim());
                var exist = users.Count;
                if (exist <= 0)
	            {
		            Membership.CreateUser(this.TextBoxName.Text.Trim(),this.TextBoxpassword.Text);
                    if (Roles.RoleExists("aaNewUser"))
                    {
                        Roles.AddUserToRole(this.TextBoxName.Text.Trim(), "aaNewUser");
                    }
                    else
                    {
                        Roles.CreateRole("aaNewUser");
                        Roles.AddUserToRole(this.TextBoxName.Text.Trim(), "aaNewUser");
                    }
                }
                else
                {
                    users[TextBoxName.Text.Trim()].Email = TextBoxName.Text.Trim() + "@" + userdomains[1].ToString();
                    users[TextBoxName.Text.Trim()].Comment = HttpContext.Current.Request.UserHostAddress;
                    users[TextBoxName.Text.Trim()].LastLoginDate = DateTime.Now;
                    Membership.UpdateUser(users[TextBoxName.Text.Trim()]);
                }
                
                if (secondpasswdflag == "0")
                {
                    

                    FormsAuthentication.RedirectFromLoginPage(this.TextBoxName.Text.Trim(),
                    PersistCookie.Checked);
                }
                else if (secondpasswdflag == "1")
                {
                    labLogonError.Text = "Error1:Your have no second passwrd, Please go http://142.2.70.208/workflow initialize the Pass Phrase. Thank you!";
                    string urls = @"http://142.2.70.208/workflow";
                    Response.Write("<script language='javascript'>window.open('" + urls +"'" + ","+"'_blank');</script>");
                }
                else if (secondpasswdflag == "2")
                {
                    this.TextBoxPassPhrase.Focus();
                    labLogonError.Text = "Error2: Invalid Pass Phrase,Please try again.Thank you!";
                }
                else
                {
                    labLogonError.Text = "Error"+secondpasswdflag+": System error please ask admin for help.";
                }


                
            }
            else
            {
                this.TextBoxpassword.Focus();
                labLogonError.Text = returntxt;
            }
            }
            catch (Exception ex)
            {
                labLogonError.Text = ex.Message;
                return;
            }

            
        }

        protected void DropDownListUserDomain_DataBound(object sender, EventArgs e)
        {
            if (rip.IndexOf('.')>0)
            {
            for (int i = 0; i < DropDownListUserDomain.Items.Count; i++)
            {
                if (DropDownListUserDomain.Items[i].Value.Contains(rip.Substring(0,rip.Length-rip.IndexOf('.')+2)))
                {
                    DropDownListUserDomain.Items[i].Selected = true;
                    break;
                }
            }
            }

        }

    }
}