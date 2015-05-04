var HomeLogin = (function () {
    function HomeLogin() {
        this.usernameInput = document.getElementById("uname");
        this.passwordInput = document.getElementById("pwd");
        this.persistCheckBox = document.getElementById("persist");
        this.createAccountAnchor = document.getElementById("createAccount");
        this.loginButton = document.getElementById("loginButton");
        if (window.location.href.indexOf("ReturnUrl=") != -1)
            this.returnUrl = window.location.href.substring(window.location.href.indexOf("ReturnUrl") + 10, window.location.href.length);
        else
            this.returnUrl = null;
        this.postUrl = "/Home/OnLogin";
        this.loginButton.onclick = this.onloginClick.bind(this);
    }
    HomeLogin.prototype.onloginClick = function () {
        $.post(this.postUrl, {
            username: this.usernameInput.value,
            password: this.passwordInput.value,
            isPersist: this.persistCheckBox.checked,
            returnUrl: this.returnUrl
        }, function (result) {
            var webreturn = new WebReturn(result);
            if (webreturn.success)
                window.location.href = webreturn.content;
            else {
                alert(webreturn.message);
            }
        });
    };
    return HomeLogin;
})();
$(function () {
    var login = new HomeLogin();
    $(window.document).keypress(function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            login.onloginClick();
            return false;
        }
        else {
            return true;
        }
    });
});
//# sourceMappingURL=HomeLogin.js.map