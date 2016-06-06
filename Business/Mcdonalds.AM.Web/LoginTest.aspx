<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginTest.aspx.cs" Inherits="Mcdonalds.AM.Web.LoginTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <title></title>
    <script src="Scripts/Libs/JQuery/jquery-1.10.2.min.js"></script>
    <script src="Scripts/Utils/Utils.js"></script>
   
   
</head>

<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox runat="server" ID="tb1" />
        <asp:Button runat="server" ID="btn" Text="登录" OnClick="btn_Click"  />
    </div>
    </form>
</body>
</html>
