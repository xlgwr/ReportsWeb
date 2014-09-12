using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace ReportsWeb.Account
{
    public partial class Admin : System.Web.UI.Page
    {
        static bool dropchanged = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = this.Request.Path;
            try
            {
                if (!Roles.GetRolesForUser().Contains("admin"))
                {
                    Response.Write("<script language='javascript'>alert('" + "Error0: " + Membership.GetUser().UserName + " have no Roles to visit [" + url + "]. Thank you!" + "')</script>");
                    this.Response.Redirect("~/Account/LogonDomain.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script language='javascript'>alert('" + "Error01: " + ex.Message + "')</script>");
                this.Response.Redirect("~/Account/LogonDomain.aspx");
            }
        }
        protected void Btn_user_role_add_Click(object sender, EventArgs e)
        {
            user_in_role_edit("Add");
       }

        private void user_in_role_edit(string tag)
        {
            string username1 = this.Textbox_UserName.Text.Trim();
            string username2 = this.DropDownList_userName.SelectedValue;
            string userrole = this.DropDownList_RoleName.SelectedValue;

            if (tag == "Add")
            {
                if (!string.IsNullOrWhiteSpace(username1) && Membership.FindUsersByName(username1).Count < 0)
                {
                    Membership.CreateUser(username1, username1);
                    Roles.AddUserToRole(username1, userrole);
                }
                else if (!string.IsNullOrWhiteSpace(username1) && Membership.FindUsersByName(username1).Count > 0 && Roles.FindUsersInRole(userrole, username1).Count() <= 0)
                {
                    Roles.AddUserToRole(username1, userrole);
                    this.Label_user_in_roles.Text = "Success: UserName: " + username1 + ", Role: " + userrole + " " + tag + " OK.";
                }
                else if (dropchanged && Roles.FindUsersInRole(userrole, username2).Count() <= 0)
                {
                    Roles.AddUserToRole(username2, userrole);
                    this.Label_roles.Text = "Success: UserName: " + username2 + ", Role: " + userrole + " " + tag + " OK.";
                }


            }
            else if (tag == "AddDefault")
            {
                 if (Roles.FindUsersInRole(userrole, username2).Count() <= 0)
                     { 
                        Roles.AddUserToRole(username2, userrole);
                        this.Label_roles.Text = "Success: UserName: " + username2 + ", Role: " + userrole + " " + tag + " OK.";
                     }
            }
            else if (tag == "Delete")
            {
                if (!string.IsNullOrWhiteSpace(username1) && Membership.FindUsersByName(username1).Count > 0 && Roles.FindUsersInRole(userrole, username1).Count() > 0)
                {
                    Roles.RemoveUserFromRole(username1, userrole);
                    this.Label_user_in_roles.Text = "Success: UserName: " + username1 + ", Role: " + userrole + " " + tag + " OK.";
                }
                if (dropchanged && Roles.FindUsersInRole(userrole, username2).Count() > 0)
                {
                    Roles.RemoveUserFromRole(username2, userrole);
                    this.Label_roles.Text = "Success: UserName: " + username2 + ", Role: " + userrole + " " + tag + " OK.";
                }
                
            }
            else {
                this.Label_user_in_roles.Text = "Error: nothing.";
                this.Label_roles.Text = "";
            }
            //this.Label_roles.Text = "";
           
        }

        protected void DropDownList_userName_SelectedIndexChanged(object sender, EventArgs e)
        {
            dGetRolesForUser();
        }

        protected void DropDownList_userName_TextChanged(object sender, EventArgs e)
        {
            dGetRolesForUser();
        }

        protected void DropDownList_RoleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            dGetUserForRoles();
        }

        protected void DropDownList_RoleName_TextChanged(object sender, EventArgs e)
        {
            dGetUserForRoles();
        }
        private void dGetRolesForUser()
        {
            dropchanged = true;
            string username = this.DropDownList_userName.SelectedValue;
            var usernames = Roles.GetRolesForUser(username);
            this.Label_user_in_roles.Text = username + " have ["+usernames.Count()+"] Roles:";
            foreach (var item in usernames)
            {
                this.Label_user_in_roles.Text = this.Label_user_in_roles.Text + "|" + item.ToString();
            }
            
            
        }

        private void dGetUserForRoles()
        {
            dropchanged = true;
            string roles = this.DropDownList_RoleName.SelectedValue;
            var rolesusers = Roles.GetUsersInRole(roles);
            this.Label_roles.Text = roles + " have ["+ rolesusers.Count()+"] UserNames:";
            foreach (var item in rolesusers )
            {
                this.Label_roles.Text = this.Label_roles.Text + "|" + item.ToString();
            }
        }

        protected void Btn_user_role_del_Click(object sender, EventArgs e)
        {
            user_in_role_edit("Delete");
        }

        protected void Button_user_role_add_now_Click(object sender, EventArgs e)
        {
            user_in_role_edit("AddDefault");
        }

        protected void btn_add_role_Click(object sender, EventArgs e)
        {
            if (!Roles.RoleExists(this.txt_add_role.Text.Trim()))
            {
                Roles.CreateRole(this.txt_add_role.Text.Trim());
                this.Label_roles.Text = this.txt_add_role.Text + " Add Success.";
            }
            else
            {
                this.Label_roles.Text = this.txt_add_role.Text + " has Exists.";
            }
        }
        
            
    }
}