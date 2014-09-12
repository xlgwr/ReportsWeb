<%@ Page Title="Admin Management Center" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="ReportsWeb.Account.Admin" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="form-horizontal">
        <h3>
            Add/Remove User to Roles</h3>
        <div class="control-group">
            <label class="span2 control-label">
                Users Name:
            </label>
            <div class="controls">
                <asp:TextBox ID="Textbox_UserName" runat="server" CssClass="span2"></asp:TextBox>
                <span>OR</span>
                <asp:DropDownList ID="DropDownList_userName" runat="server" DataSourceID="SqlDataSourceUsers"
                    CssClass="span2" DataTextField="UserName" DataValueField="UserName" AutoPostBack="True"
                    OnSelectedIndexChanged="DropDownList_userName_SelectedIndexChanged" OnTextChanged="DropDownList_userName_TextChanged">
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceUsers" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                    SelectCommand="SELECT * FROM [vw_aspnet_Users] "></asp:SqlDataSource>
            </div>
        </div>
        <div class="control-group">
            <label class="span2 control-label">
                Roles Name:
            </label>
            <div class="controls">
                <asp:DropDownList ID="DropDownList_RoleName" runat="server" DataSourceID="SqlDataSourceRoles"
                    DataTextField="RoleName" DataValueField="RoleName" AutoPostBack="True" CssClass="span4"
                    OnSelectedIndexChanged="DropDownList_RoleName_SelectedIndexChanged" OnTextChanged="DropDownList_RoleName_TextChanged">
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceRoles" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                    SelectCommand="SELECT * FROM [vw_aspnet_Roles] order by RoleName"></asp:SqlDataSource>
            </div>
        </div>
        <div class="control-group">
            <div class="controls">
                <asp:Button ID="Btn_user_role_add" runat="server" Text="Add" CssClass="btn btn-large btn-primary active"
                    OnClick="Btn_user_role_add_Click" />
                <asp:Button ID="Button_user_role_add_now" runat="server" Text="Add Default" CssClass="btn btn-large"
                    OnClick="Button_user_role_add_now_Click" />
                <asp:Button ID="Btn_user_role_del" runat="server" Text="Remove" CssClass="btn btn-large btn-primary active"
                    OnClick="Btn_user_role_del_Click" />
            </div>
        </div>
        <div class="control-group">
            <asp:Label ID="Label_user_in_roles" runat="server" CssClass="span6 text-left error"></asp:Label>
            <asp:Label ID="Label_roles" runat="server" CssClass="span6 text-left error"></asp:Label>
        </div>
        <div class="control-group">
            <label class="span2 control-label">
                Roles Name:
            </label>
            <div class="controls">
                <asp:TextBox ID="txt_add_role" runat="server" CssClass="span2"></asp:TextBox>
            </div>
        </div> 
        <div class="control-group">
            <div class="controls">
                <asp:Button ID="btn_add_role" runat="server" Text="Add Role" 
                    CssClass="btn btn-large btn-primary active" onclick="btn_add_role_Click" />
            </div>
        </div>
    </div>
</asp:Content>
