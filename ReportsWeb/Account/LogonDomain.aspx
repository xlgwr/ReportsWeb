<%@ Page Title="Logon On" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="LogonDomain.aspx.cs" Inherits="ReportsWeb.Account.LogonDomain" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false"></asp:HyperLink>
    </p>
    <div class="div-container form-signin control-group">
        <h2 class="form-signin-heading text-center">
            <i class="icon icon-lock"></i>Logon On</h2>
        <div class="control-group">
            <label class="control-label">
                Domain Name:</label>
            <div class="controls">
                <asp:DropDownList ID="DropDownListUserDomain" runat="server" CssClass="input-block-level"
                    placeholder="Domain Name(wongs-sj.com)" DataSourceID="SqlDataSourceUserDomain"
                    DataTextField="uvalue" DataValueField="udesc" 
                    ondatabound="DropDownListUserDomain_DataBound">
                </asp:DropDownList>
                <asp:SqlDataSource ID="SqlDataSourceUserDomain" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                    SelectCommand="SELECT * FROM [udefine] WHERE (uenable = '1') AND (uname = 'UserDomain')">
                </asp:SqlDataSource>
                <%--
                <asp:TextBox ID="TxtBoxUserDomain" runat="server" CssClass="input-block-level" placeholder="Domain Name(wongs-sj.com)">wongs-sj.com</asp:TextBox>--%>
            </div>
            <label class="control-label">
                User Name:</label>
            <div class="controls">
                <asp:TextBox ID="TextBoxName" runat="server" CssClass="input-block-level" placeholder="Domain User Name"></asp:TextBox>
            </div>
            <label class="control-label">
                Password:</label>
            <div class="controls">
                <asp:TextBox ID="TextBoxpassword" runat="server" CssClass="input-block-level" placeholder=" Domain Password"
                    TextMode="Password"></asp:TextBox>
            </div>
             <label class="control-label">
                Pass Phrase(like http://142.2.70.208/workflow):</label>
            <div class="controls">
                <asp:TextBox ID="TextBoxPassPhrase" runat="server" CssClass="input-block-level" placeholder="PassPhrase(like http://142.2.70.208/workflow)"
                    TextMode="Password"></asp:TextBox>
            </div>
            <label class="checkbox">
                <asp:CheckBox ID="PersistCookie" runat="server" Text="Remember-me" />
            </label>
        </div>
        <div class="text-center">
            <asp:Button ID="ButLogon" runat="server" Text="Submit" CssClass="btn btn-primary btn-large active"
                OnClick="ButLogon_Click" />
        </div>
        <br />
        <div class="error">
            <asp:Label ID="labLogonError" runat="server" CssClass="topleft text-left error"></asp:Label>
        </div>
        
    </div>
</asp:Content>
