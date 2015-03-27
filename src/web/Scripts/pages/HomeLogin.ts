class HomeLogin {
    usernameInput: HTMLInputElement;
    passwordInput: HTMLInputElement;
    persistCheckBox: HTMLInputElement;
    createAccountAnchor: HTMLAnchorElement;    
    loginButton: HTMLButtonElement;

    constructor() {
        this.usernameInput = <HTMLInputElement>document.getElementById("uname");
        this.passwordInput = <HTMLInputElement>document.getElementById("pwd");
        this.persistCheckBox = <HTMLInputElement>document.getElementById("persist");
        this.createAccountAnchor = <HTMLAnchorElement>document.getElementById("createAccount");
        this.loginButton = <HTMLButtonElement>document.getElementById("loginButton");
    }

    public onloginClick() {

    }

    $(function () {
        $("#login_pane input[type!=checkbox]").focusin(function () {
            var login_err = $("#login_err_msg");
            if (login_err.css("visibility") == "visible")
                login_err.css("visibility", "hidden");
        });

        $("#user_login").click(function () {
            var url = "/Home/OnLogin?" + window.location.href.substring(window.location.href.indexOf("ReturnUrl"), window.location.href.length);
            $.post(url,
                {
                    username: $("#uname").val(),
                    password: $("#pwd").val(),
                    isPersist: $("#persist").is(":checked")
                },
                function (result) {
                    var webreturn = new WebReturn(result);
                    if (webreturn.success)
                        window.location.href = webreturn.content;
                    else {
                        alert(webreturn.message);
                    }
                });
            return false;
        });

        $(window.document).keypress(function (e) {
            if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                $('div#login_pane button').click();
                return false;
            } else {
                return true;
            }
        });
    });
} 