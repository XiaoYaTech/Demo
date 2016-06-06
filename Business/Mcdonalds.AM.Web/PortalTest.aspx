<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PortalTest.aspx.cs" Inherits="Mcdonalds.AM.Web.PortalTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <meta name="author" content="">
    <link href="favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <title>Simulation Portal</title>

    <style type="text/css">
    <!--
    body {
	    margin:0px;
    }
    -->
    </style>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td style="background-color: #EFF0F2; height: 45px">当前登录用户：<asp:Literal runat="server" ID="lbUser" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <%--<a href="LoginTest.aspx">注销</a>--%>
                    <asp:LinkButton runat="Server" id="lnkBtn_Logout">注销</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="height:100%">
                        <tr>
                            <td style="background-color: #1b1e25; width: 180px">
                                <div id="leftFrame" style="width: 100%; font-size: 12px; font-family: '微软雅黑'; color:#fff;">
                                    <asp:TreeView runat="server" ID="tvMenu" NodeIndent="5" NodeStyle-HorizontalPadding="4" ForeColor="#FFFFFF">
                                        <Nodes>
                                            <asp:TreeNode Text="Work Station" Value="Work Station" SelectAction="Expand">
                                                <asp:TreeNode Text="My Task" Value="My Task" Target="mainFrame" NavigateUrl="/home/Personal/#taskwork" />
                                                <asp:TreeNode Text="Reminding" Value="Reminding" Target="mainFrame" NavigateUrl="/home/Personal/#remindRegister" />
                                                <asp:TreeNode Text="Reminding List" Value="Reminding List" Target="mainFrame" NavigateUrl="/home/Personal/#remind" />
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Workflow" Value="" SelectAction="Expand">
                                                <asp:TreeNode Text="Create New Closure" Value="Closure" Target="mainFrame" NavigateUrl="/closure/Main#/Closure/Create"/>
                                                <asp:TreeNode Text="Create New MajorLease" Value="MajorLease" Target="mainFrame" NavigateUrl="/MajorLease/Main#/MajorLease/Create" />
                                                <asp:TreeNode Text="Create New TempClosure" Value="TempClosure" Target="mainFrame" NavigateUrl="/TempClosure/Main#/Create" />
                                            </asp:TreeNode>
                                            <asp:TreeNode Text="Store List" Value="Store List" Target="mainFrame" NavigateUrl="/StoreList/" />
                                            <asp:TreeNode Text="Project List" Value="Project List" Target="mainFrame" NavigateUrl="/home/Personal#projectList" />
                                            
                                            
                                            <asp:TreeNode Text="Setting" Value="Setting" SelectAction="Expand">
                                                <asp:TreeNode Text="Dictionary" Value="Dictionary" Target="mainFrame" NavigateUrl="/home/Personal/#dictionary"/>
                                                <asp:TreeNode Text="User Management" Value="User Management" Target="mainFrame" NavigateUrl="Fx.WebHostUri/#/user"/>
                                                <asp:TreeNode Text="Organization Management" Value="Organization Management" Target="mainFrame" NavigateUrl="Fx.WebHostUri/#/org"/>
                                                <asp:TreeNode Text="Position Management" Value="Position Management" Target="mainFrame" NavigateUrl="Fx.WebHostUri/#/position"/>
                                                <asp:TreeNode Text="FunctionDept Management" Value="FunctionDept Management" Target="mainFrame" NavigateUrl="Fx.WebHostUri/#/department"/>
                                                <asp:TreeNode Text="Role Management" Value="Role Management" Target="mainFrame" NavigateUrl="Fx.WebHostUri/#/role"/>
                                            </asp:TreeNode>

                                        </Nodes>
                                    </asp:TreeView>
                                </div>
                            </td>
                            <td>
                                <iframe src='/home/Personal/#taskwork?user-id=<%=Request["user-id"] %>' frameborder="0" scrolling="auto" name="mainFrame" width="100%"
                                    id="mainFrame" style=""></iframe>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>

    <script type="text/javascript">
        function PageResize() {
            var height = document.documentElement.clientHeight;
            var leftFrame = document.getElementById("leftFrame");
            var mainFrame = document.getElementById("mainFrame");

            if (height > 0) {
                leftFrame.style.height = height - 50 + "px";
                mainFrame.style.height = height - 50 + "px";
            }
        }
        PageResize();
        window.onresize = PageResize;

    </script>
</body>
</html>
