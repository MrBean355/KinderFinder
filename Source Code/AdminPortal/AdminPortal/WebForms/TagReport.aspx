<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TagReport.aspx.cs" Inherits="AdminPortal.WebForms.TagReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" Height="440px" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="752px">
            <LocalReport ReportPath="rdlcReports\TagReport.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" DeleteMethod="Delete" InsertMethod="Insert" OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" TypeName="AdminPortal.App_Code.DataSetTableAdapters.TagTableAdapter" UpdateMethod="Update">
            <DeleteParameters>
                <asp:Parameter Name="Original_ID" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="Label" Type="String" />
                <asp:Parameter Name="Restaurant" Type="Int32" />
                <asp:Parameter Name="CurrentUser" Type="Int32" />
                <asp:Parameter Name="OutOfOrder" Type="Boolean" />
                <asp:Parameter Name="LastAccessed" Type="DateTime" />
                <asp:Parameter Name="UUID" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="Label" Type="String" />
                <asp:Parameter Name="Restaurant" Type="Int32" />
                <asp:Parameter Name="CurrentUser" Type="Int32" />
                <asp:Parameter Name="OutOfOrder" Type="Boolean" />
                <asp:Parameter Name="LastAccessed" Type="DateTime" />
                <asp:Parameter Name="UUID" Type="String" />
                <asp:Parameter Name="Original_ID" Type="Int32" />
            </UpdateParameters>
        </asp:ObjectDataSource>
        </div>
    </form>
</body>
</html>
