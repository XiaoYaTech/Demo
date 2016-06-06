function submit() {
    var username = document.getElementById("username").value;
    var pwd = document.getElementById("pwd").value;
    var validcode = document.getElementById("authcode").value;
    if (username == "") {
        alert("请输入账号");
        document.getElementById("username").focus();
        return;
    }
    if (validcode == "") {
        alert("请输入验证码");
        document.getElementById("authcode").focus();
        return;
    }

    document.mainform.action = "/account/LogOnProcess";
    document.mainform.target = "_self";
    document.mainform.submit();
}

function regist() {
    var username = document.getElementById("username").value;
    var pwd = document.getElementById("pwd").value;
    var pwdcfm = document.getElementById("pwdcfm").value;

    if (username == "") {
        alert("请输入账号");
        document.getElementById("username").focus();
        return;
    }
    if (pwd == "") {
        alert("请输入密码");
        document.getElementById("pwd").focus();
        return;
    }

    if (pwdcfm == "") {
        alert("请确认密码");
        document.getElementById("pwdcfm").focus();
        return;
    }

    if (pwdcfm != pwd) {
        alert("两次密码不一样");
        document.getElementById("pwdcfm").focus();
        return;
    }

    document.mainform.action = "/account/RegisterProcess";
    document.mainform.target = "_self";
    document.mainform.submit();
}