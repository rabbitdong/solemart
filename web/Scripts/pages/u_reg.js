$(function () {
    $("#name").focusin(function () {
        $('#err_for_name').css('visibility', 'hidden');
    });

    $("#name").focusout(function () {
        var name = $("#name").val();
        if (name.length == 0 || name.match(/\s+/)) {
            $("#err_for_name").css('visibility', 'visible');
            $("#err_for_name").html('用户名不能为空');
        }
        else
            $("#err_for_name").css('visibility', 'hidden');
    });

    $("#name").keypress(function () {
        $("#err_for_name").css('visibility', 'hidden');
    });

    $("#email").focusin(function () {
        $("#err_for_email").css('visibility', 'hidden');
    });

    $("#email").focusout(function(){
        if (!validateEmail($("#email").val())) {
            $("#err_for_email").css('visibility', 'visible');
            $("#err_for_email").html('邮箱地址不合法，邮箱地址用于密码忘记找回');
        }
        else
            $("#err_for_email").css('visibility', 'hidden');
    });

    $("#email").keypress(function () {
        $("#err_for_email").css('visibility', 'visible');
    });

    $("#password").focusout(function () {
        if ($("#password").val().length < 6)
            $("#err_for_pwd").css("visibility", "visible");
        else
            $("#err_for_pwd").css("visibility", "hidden");
    });

    $("#password").focusin(function () {
        $("#err_for_pwd").css("visibility", "hidden");
    });

    $("#passwordConfirm").focusin(function () {
        $("#err_for_pwdcnfm").css("visibility", "hidden");        
    });

    $("#passwordConfirm").focusout(function () {
        if ($("#passwordConfirm").val() != $("#password").val())
            $("#err_for_pwdcnfm").css("visibility", "visible");
        else
            $("#err_for_pwdcnfm").css("visibility", "hidden");
    });

    $("#agreement").click(function () {
        if ($("#agreement").is(":checked")) {
            $("#submit").attr("disabled", false);
            $("#submit").removeClass("disablebtn");
        }
        else {
            $("#submit").attr("disabled", true);
            $("#submit").addClass("disablebtn");
        }
    });

    $("#name").focus();

});

function submitaccount() {
    var name = $("#name").val();
    var email = $("#email").val();
    var pwd = $("#password").val();
    var pwd_c = $("#passwordConfirm").val();

    var pass = true;
    if (name.length == 0 || name.match(/\s+/)) {
        pass = false;
    }

    if (!validateEmail(email))
        pass = false;

    if (pwd.length < 6) {
        pass = false;
    }

    if (pwd != pwd_c)
        pass = false;

    if (!pass)
        return false;

    $.post("/Home/RegisterNew",
        {
            'name': name,
            'email': email,
            'pwd': pwd,
            'subscript': $("#subscriptions_news").is(":checked")
        },
        function (result) {
            if (result.substring(0, 5) != "error") {
                alert("谢谢您！已经成功注册。现在转到网站首页");
                window.location.href = "/";
            }
            else {
                var err_type = (result.indexOf("name duplicate") != -1);
                if (err_type) {
                    $("#name").focus();
                    $('#err_for_name').css('visibility', 'visible');
                    $('#err_for_name').html('该用户名已经被注册');
                }
                else {
                    $("#email").focus();
                    $('#err_for_email').css('visibility', 'visible');
                    $('#err_for_email').html('该Email地址已经被使用');
                }
            }
        });
}