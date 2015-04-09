var HomeLogin = (function () {
    function HomeLogin() {
        this.usernameInput = document.getElementById("uname");
        this.passwordInput = document.getElementById("pwd");
        this.persistCheckBox = document.getElementById("persist");
        this.createAccountAnchor = document.getElementById("createAccount");
        this.loginButton = document.getElementById("loginButton");
        this.returnUrl = window.location.href.substring(window.location.href.indexOf("ReturnUrl"), window.location.href.length);
        this.postUrl = "/Home/OnLogin";
        this.loginButton.onclick = this.onloginClick.bind(this);
    }
    HomeLogin.prototype.onloginClick = function () {
        $.post(this.postUrl, {
            username: this.usernameInput.value,
            password: this.passwordInput.value,
            isPersist: this.persistCheckBox.checked
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