<%@ Page Title="Welcome to Wong's Document Printing System" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ReportsWeb._Default" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <%--<h3>
        Documents Printing System</h3>--%>
    <br />
    <div class="row">
        <label class="span4">
        </label>
        <label class="span2 control-label text-right">
            Select System:</label>
        <label class="span2 control-label text-center">
            Document Type:</label>
        <label class="span3">
        </label>
    </div>
    <div class="row control-group">
        <div class="span4 text-right">
        </div>
        <div class="span2 text-right">
            <asp:DropDownList ID="DropDown_List_rSystem" runat="server" DataSourceID="rsystem"
                DataTextField="sysname" DataValueField="syswebvalue" CssClass="span2 form-control"
                OnDataBound="DropDown_List_rSystem_DataBound" AutoPostBack="True" 
                OnSelectedIndexChanged="DropDown_List_rSystem_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:SqlDataSource ID="rsystem" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                SelectCommand="SELECT [Id], [sysname],[syswebvalue] FROM [rsystem] where [sysenable] = 1 order by [Id]">
            </asp:SqlDataSource>
        </div>
        <div class="span3">
            <asp:DropDownList ID="DropDl_docType" runat="server" DataSourceID="sqlds_DocType"
                DataTextField="tdoctype" DataValueField="tdoctype" CssClass="span3 form-control"
                AutoPostBack="True" OnDataBound="DropDl_docType_DataBound" 
                OnSelectedIndexChanged="DropDl_docType_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:SqlDataSource ID="sqlds_DocType" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                
                SelectCommand="SELECT [url] + '@' + [comment] as 'tdoctype' FROM  [rUurlInRoles] WHERE ([rolename] = @rolename) ORDER BY [Id] desc, [comment]">
                <SelectParameters>
                    <asp:ControlParameter ControlID="DropDown_List_rSystem" DefaultValue="WEC" 
                        Name="rolename" PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
        <div class="span3">
        </div>
    </div>
    <div class="row control-group">
        <strong class="span4 text-right">
            <asp:Label ID="label_doc_Type" runat="server" Text="Invoice NO: "></asp:Label></strong>
        <div class="span5 controls">
            <asp:TextBox ID="TextBox1" runat="server" CssClass="span5"></asp:TextBox>
        </div>
        <div class="span1 controls">
            <asp:Button ID="Button1" runat="server" Text="Search" OnClick="Button1_Click" CssClass="btn btn-primary active" />
        </div>
        <div class="span2 controls">
            <asp:Button ID="Btn_Cust" runat="server" Text="Customer Define" CssClass="btn" OnClick="Btn_Cust_Click"
                Visible="False" />
        </div>
    </div>
    <div class="row">
        <div class="span4">
        </div>
        <div class="span10 error topleft text-left">
            <asp:Label ID="laberror" runat="server" Text="Label" CssClass="error" Visible="False"></asp:Label>
        </div>
    </div>
    <asp:Panel ID="Panel_Cust" runat="server" Visible="False">
        <h3>
            Customer Define:</h3>
        <div class="row control-group">
            <label class="span2 text-right control-label input-lg">
                Amt Desc:</label>
            <div class="span9 text-left">
                <asp:TextBox ID="txt_amt_desc" runat="server" CssClass="span9 form-control"></asp:TextBox>
            </div>
            <div class="span1">
                <asp:Button ID="Btn_Refresh" runat="server" Text="Refresh" CssClass="btn" OnClick="Btn_Refresh_Click" />
            </div>
        </div>
    </asp:Panel>
    <div class="row">
        <label class="span4">
        </label>
        <label class="span2 text-center">
            Print Type:</label>
        <label class="span2">
            Select Seal Type:</label>
        <label class="span4">
        </label>
    </div>
    <div class="row control-group">
        <div class="span4">
        </div>
        <div class="span2 controls">
            <asp:RadioButtonList ID="RoBtnLst_Print_Type" runat="server" RepeatDirection="Horizontal"
                CssClass="span2" DataSourceID="SqlDataSourcePrinttype" DataTextField="uvalue"
                DataValueField="uvalue" OnDataBound="RoBtnLst_Print_Type_DataBound"
                OnSelectedIndexChanged="RoBtnLst_Print_Type_SelectedIndexChanged" 
                OnTextChanged="RoBtnLst_Print_Type_TextChanged">
            </asp:RadioButtonList>
            <asp:SqlDataSource ID="SqlDataSourcePrinttype" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                SelectCommand="SELECT * FROM [udefine] WHERE ( [uname] = 'printtype' and [uenable] = 1)">
            </asp:SqlDataSource>
        </div>
        <div class="span2 controls">
            <asp:DropDownList ID="DropDownList_SealType" runat="server" DataSourceID="SqlDataSourceSealType"
                CssClass="span2 form-control" DataTextField="uvalue" DataValueField="uvalue"
                OnDataBound="DropDownList_SealType_DataBound" 
                OnSelectedIndexChanged="DropDownList_SealType_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSourceSealType" runat="server" ConnectionString="<%$ ConnectionStrings:WebReportConnectionString %>"
                
                SelectCommand="SELECT * FROM [udefine] WHERE ((uenable = 1) and ([uext] = SUBSTRING(@uext,0,charindex('@',@uext))) AND (CHARINDEX(@usystem,usystem) >= 1)) order by uvalue">
                <SelectParameters>
                    <asp:ControlParameter ControlID="DropDl_docType" Name="uext" DefaultValue="INVOICE"
                        PropertyName="SelectedValue" Type="String" />
                    <asp:ControlParameter ControlID="DropDown_List_rSystem" Name="usystem" DefaultValue="WEC"
                        PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
        </div>
        <div class="span4">
        </div>
    </div>
    <div class='reportviewer'>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
            InteractiveDeviceInfos="(集合)" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
            Visible="False" Height="100%" Width="100%" ExportContentDisposition="AlwaysAttachment" SizeToReportContent="False">
            <LocalReport ReportPath="">
            </LocalReport>
        </rsweb:ReportViewer>
    </div>
</asp:Content>
