$(function () {

});

function createtable(oitems) {
    var oitems = eval(oitems);
    var th = "<table class='m'><thead><tr><td>商品名称</td><td>单价(￥)</td><td>数量</td><td>总价</td></tr></thead>"
    var tbody = "<tbody>";
    for (var i = 0; i < oitems.length; ++i) {
        tbody += "<tr><td><a href='/Product/Detail/" + oitems[i].Product.ProductID + "' title='" + oitems[i].Product.Name + "' target='_blank'>"
            + oitems[i].Product.Name + "</a></td><td>" + oitems[i].UnitPrice + "</td><td>" + oitems[i].Amount + "</td><td>" + oitems[i].UnitPrice * oitems[i].Amount + "</td></tr>";
    }
    tbody += "</tbody>";

    return th + tbody;
}

/*
查看订单的商品信息
*/
function expand_order(oid, btn) {
    $.post("/Account/OrderDetail/"+oid,{ },
        function (result) {
            if (result != "error") {
                var otb = createtable(result);
                $("#orderinfo_" + oid + " .orderinfo_conent").append(otb);
                $(btn).remove();
            }
        });
}

/*
用户确认订单，在用户收到商品后确认没有问题，就确认订单
*/
function confirm_order(oid, btn) {
    $.post("/Account/ConfirmOrder/"+oid, { },
        function (result) {
            if (result == "ok") {
                $("#order_state_" + oid).html("已收货");
                $("#pay_status_" + oid).html("已付款");
                $(btn).remove();
            }
            else
                alert(result);
        });
}

/*
用户取消订单，只有订单还没发货前可以取消
*/
function cancel_order(oid, btn) {
    if (!window.confirm("确定取消订单? \r\n取消后，该订单不再生效，我们不会派送您订购的商品")) {
        return false;
    }

    $.post("/Account/CancelOrder/" + oid, {},
        function (result) {
            if (result == "ok") {
                $("#order_state_" + oid).html("已取消");
                $("#orderinfo_" + oid).addClass("cancel");
                $("#orderinfo_" + oid).removeClass("unsend");
                $(btn).remove();
            }
            else
                alert(result);
        });
}