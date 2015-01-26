$(function () {
    $("#sendinfo1_input input[type=text]").focusin(function () {
        $(this).next().css("visibility", "hidden");
        $("#err_for_noinfo").css("visibility", "hidden");
    });

    $("#receiver").focusout(function () {
        if ($.trim($("#receiver").val()) == "")
            $("#err_for_receiver").css('visibility', 'visible');
    });

    $("#address").focusout(function () {
        if ($("#address").val().indexOf("罗源县") == -1)
            $("#err_for_address").css("visibility", "visible");
    });

    $("#phone").focusout(function () {
        if (!validatePhone($("#phone").val()))
            $("#err_for_phone").css("visibility", "visible");
    });
});

function submitorder() {
    if ($("#sendinfo1_input").css("display") != "none") {
        $("#err_for_noinfo").css("visibility", "visible");
        return false;
    }

    $.post("/Cart/CheckoutOrder",
        {
            remark: $("#order_remark").val(),
            paytype:$("#paytype").val()
        },
        function (result) {
            if (result.substring(0, 2) == "ok") {
                if (result.indexOf("ok-alipay-") != -1) {
                    document.write(result.substring(10, result.length));
                }
                else {
                    window.location.href = "/Cart/CheckOutCompleted/" + result.substring(3, result.length);
                }
            }
            else {
                alert("error");
            }
        }
    );
}

function save_info() {
    if ($("#save_send_info").html() == "填写送货信息") {
        var is_validate = true;
        if ($.trim($("#receiver").val()) == "") {
            $("#err_for_receiver").css('visibility', 'visible');
            is_validate = false;
        }
        if ($("#address").val().indexOf("罗源县") == -1) {
            $("#err_for_address").css("visibility", "visible");
            is_validate = false;
        }
        if (!validatePhone($("#phone").val())) {
            $("#err_for_phone").css("visibility", "visible");
            is_validate = false;
        }

        if (!is_validate) {
            return false;
        }

        $.post("/Cart/SaveAddrInfo",
            {
                receiver: $("#receiver").val(),
                address: $("#address").val(),
                post: $("#post").val(),
                phone: $("#phone").val(),
                paytype: $("#sendinfo1_paytype").val()
            },
            function (result) {
                if (result == "ok")
                    window.location.reload();
                else if (result != "error")
                    window.location.href = result;
            });
    }
    else {  /* 是修改的情况 */
        $("#sendinfo1_input").css("display", "block");
        $("#sendinfo1_paytype").css("display", "inline");
        $("#sendinfo1_content_paytype").css("display", "none");
        $("#save_send_info").html("填写送货信息");
    }
}