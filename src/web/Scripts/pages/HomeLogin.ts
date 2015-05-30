

class HomeLogin {
    usernameInput: HTMLInputElement;
    passwordInput: HTMLInputElement;
    persistCheckBox: HTMLInputElement;
    createAccountAnchor: HTMLAnchorElement;    
    loginButton: HTMLButtonElement;

    returnUrl: string;
    postUrl: string;

    constructor() {
        this.usernameInput = <HTMLInputElement>document.getElementById("uname");
        this.passwordInput = <HTMLInputElement>document.getElementById("pwd");
        this.persistCheckBox = <HTMLInputElement>document.getElementById("persist");
        this.createAccountAnchor = <HTMLAnchorElement>document.getElementById("createAccount");
        this.loginButton = <HTMLButtonElement>document.getElementById("loginButton");

        if (window.location.href.indexOf("ReturnUrl=") != -1)
            this.returnUrl = window.location.href.substring(window.location.href.indexOf("ReturnUrl") + 10, window.location.href.length);
        else
            this.returnUrl = null;

        this.postUrl = "/Home/OnLogin";
        
        this.loginButton.onclick = this.onloginClick.bind(this);
    }

    public onloginClick() {
        $.post(this.postUrl,
            {
                username: this.usernameInput.value,
                password: this.passwordInput.value,
                isPersist: this.persistCheckBox.checked,
                returnUrl: this.returnUrl
            },
            function (result) {
                var webreturn = new WebReturn(result);
                if (webreturn.success)
                    window.location.href = decodeURIComponent(webreturn.content);
                else {
                    alert(webreturn.message);
                }
            });
    }
}

$(function () {
    var login = new HomeLogin();
    
    $(window.document).keypress(function (e) {
        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
            login.onloginClick();
            return false;
        } else {
            return true;
        }
    });
}); 