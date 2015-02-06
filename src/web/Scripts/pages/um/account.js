function post_change(newval, type) {
    var result = true;

    $.post("/Account/Modify/"+type,
        {
            "val": newval
        },
        function (result) {
            if (result != "ok") {
                alert(result);
                result = false;
            }
            else {
                result = true;
            }
        });

    return result;
}

$(function () {
    tips = $(".validateTips");

    function updateTips(t) {
        tips.css("display", "block");
        tips.text(t).addClass("ui-state-highlight");
        setTimeout(function () {
            tips.removeClass("ui-state-highlight", 1500);
        }, 500);
    }

    $("#pwd_dialog").dialog({
        autoOpen: false,
        height: 300,
        width: 400,
        modal: true,

        create: function(event, ui) { 
            var widget = $(this).dialog("widget");
            $(".ui-dialog-titlebar-close span", widget)
                .removeClass("ui-icon-closethick")
                .addClass("myclosethick");
        },

        buttons: {
            '确定': function () {
                var isvalid = true;
                if ($.trim($("#newpwd").val()) == ""
                    || $.trim($("#newpwdcnf").val()) == "")
                    isvalid = false;
                if ($("#newpwd").val().length < 6)
                    isvalid = false;
                if ($("#newpwd").val() != $("#newpwdcnf").val())
                    isvalid = false;

                if (isvalid) {
                    $.post("/Account/Modify/password",
                        {
                            oldpwd: $('#oldpwd').val(),
                            newpwd: $("#newpwd").val()
                        },
                        function (result) {
                            if (result == "wrong pwd") {
                                updateTips();
                                return;
                            }

                            $("#pwd_dialog").dialog("close");
                            $("#pwd_dialog input[type=password]").val("");
                        });

                    return result;
                }
            },

            '取消': function () {
                $("#pwd_dialog").dialog("close");
            }
        }
    });

    $("#uname").click(function () {
        var inv = $(this).prev();
        if (!inv.children("input")[0]) {
            inv.html("<input type='text' value='" + inv.html() + "' id='uname_val' />");
            $("#uname_val").focus();
            $("#uname_val").focusin(function () {
                if ($('#err_for_uname').css('visibility') == 'visible')
                    $('#err_for_uname').css('visibility', 'hidden');
            });

            $("#uname_val").focusout(function () {
                var newval = inv.children("input").val();
                if ($.trim(newval) == "") {
                    $('#err_for_uname').html("用户名不能为空!");
                    $('#err_for_uname').css('visibility', 'visible');
                }
                else {
                    if (!post_change(newval, "uname")) {
                        $('#err_for_uname').html("用户名已经被使用，不能用该用户名!");
                        $('#err_for_uname').css('visibility', 'visible');
                    }
                    else {
                        inv.html(newval);
                    }
                }
            });
        }
        else {
            var val = inv.children("input").val();
            if (validateEmail(val)) {
                post_change(val, "uname");
                inv.html(val);
            }
            else {
                $('#err_for_uname').css('visibility', 'visible');
            }
        }
    });

    $("#pwd").click(function () {
        $("#pwd_dialog").dialog("open");
    });

    $("#email").click(function () {
        var inv = $(this).prev();
        if (!inv.children("input").val()) {
            inv.html("<input type='text' value='" + inv.html() + "' id='email_val' />");
            $("#email_val").focus();
            $("#email_val").focusin(function () {
                if ($('#err_for_email').css('visibility') == 'visible')
                    $('#err_for_email').css('visibility', 'hidden');
            });

            $("#email_val").focusout(function () {
                var newval = inv.children("input").val();
                if (!validateEmail(newval)) {
                    $('#err_for_email').css('visibility', 'visible');
                }
                else {
                    post_change(newval, "email");
                    inv.html(newval);
                }
            });
        }
        else {
            var val = inv.children("input").val();
            if (validateEmail(val)) {
                post_change(val, "email");
                inv.html(val);
            }
            else
                $('#err_for_email').css('visibility', 'visible');
        }
    });

    $("#sex").click(function () {        
        var inv = $(this).prev();
        var org_val = inv.html();
        if (!inv.children("input").val()) {
            var sex_html = "<input type='radio' name='sex' value='1' id='male'/><label for='male'>男</label><input type='radio' name='sex' value='2' id='female'/><label for='female'>女</label>";
            inv.html(sex_html);
            if (org_val == '男')
                $("#male").prop('checked', true);
            else if (org_val == '女')
                $("#female").prop("checked", true);
            inv.children("input[type=radio]").focusout(function () {
                var new_val = inv.children("input[type=radio]:checked").val();
                post_change(new_val, "sex");
                if (new_val == 1)
                    inv.html("男");
                else if (new_val == 2)
                    inv.html("女");
            });
        }
        else {
            var new_val = inv.children("input[type=radio]:checked").val();
            post_change(new_val, "sex");
            if (new_val == 1)
                inv.html("男");
            else if (new_val == 2)
                inv.html("女");
        }
    });

    $("#phone1").click(function () {
        var inv = $(this).prev();
        if (!inv.children("input")[0]) {
            inv.html("<input type='text' value='" + inv.html() + "' id='phone_val' />");
            $("#phone_val").focus();
            $("#phone_val").focusin(function () {
                if($('#err_for_phone1').css('visibility') == 'visible')
                    $('#err_for_phone1').css('visibility', 'hidden');
            });

            $("#phone_val").focusout(function () {
                var newval = inv.children("input").val();
                if (!validatePhone(newval)) {
                    $('#err_for_phone1').css('visibility', 'visible');
                }
                else {
                    post_change(newval, "phone1");
                    inv.html(newval);
                }
            });
        }
        else {
            var val = inv.children("input").val();
            if (validatePhone(val)) {
                post_change(val, "phone1");
                inv.html(val);
            }
            else
                $('#err_for_phone1').css('visibility', 'visible');
        }
    });

    $("#address").click(function () {
        var inv = $(this).prev();

        if (!inv.children("input").val()) {
            inv.html("<input type='text' value='" + inv.html() + "' />");
            inv.children("input").focus();
            inv.children("input").focusout(function () {
                var newval = inv.children("input").val();
                post_change(newval, "address");
                inv.html(newval);
            });
        }
        else {
            var newval = inv.children("input").val();
            post_change(newval, "address");
            inv.html(newval);
        }
    });

    $("#birthday").click(function () {
        var inv = $(this).prev();

        if (!inv.children("input").val()) {
            inv.html("<input type='text' value='" + inv.html() + "' id='datepicker' readonly='readonly' />");

            $("#datepicker").datepicker({
                dateFormat: 'yy年m月d日',
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                dayNamesMin: ['天', '一', '二', '三', '四', '五', '六']
            });
        }
        else {
            var newval = $("#datepicker").val();
            post_change(newval, "birthday");
            inv.html(newval);
        }
    });

    $("#pwd_dialog input[type=password]").focusin(function () {
        $("#err_for_" + this.id).css("visibility", "hidden");
        tips.css("display", "none");
    });

    $("#newpwd").focusout(function () {
        if ($.trim($(this).val()) == "" || $(this).val().length < 6)
            $("#err_for_newpwd").css("visibility", "visible");
    });

    $("#newpwdcnf").focusout(function () {        
        if($("#newpwd").val() != $("#newpwdcnf").val())
            $("#err_for_newpwdcnf").css("visibility", "visible");
    });
});