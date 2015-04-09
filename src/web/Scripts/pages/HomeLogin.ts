

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

        this.returnUrl = window.location.href.substring(window.location.href.indexOf("ReturnUrl"), window.location.href.length);
        this.postUrl = "/Home/OnLogin";
        
        this.loginButton.onclick = this.onloginClick.bind(this);
    }

    public onloginClick() {
        $.post(this.postUrl,
            {
                username: this.usernameInput.value,
                password: this.passwordInput.value,
                isPersist: this.persistCheckBox.checked
            },
            function (result) {
                var webreturn = new WebReturn(result);
                if (webreturn.success)
                    window.location.href = webreturn.content;
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