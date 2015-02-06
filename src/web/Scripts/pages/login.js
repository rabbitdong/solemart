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
                uname: $("#uname").val(),
                pwd: $("#pwd").val(),
                ispersist: $("#persist").is(":checked")
            },
            function (result) {
                if (result != "error")
                    window.location.href = result;
                else {
                    $("#login_err_msg").css("visibility", "visible");
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