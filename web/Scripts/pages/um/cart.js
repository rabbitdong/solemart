$(function () {
    $("input.prodamount").focusout(function () {
        var amnt = parseInt($(this).val());
        var prod_id = $(this).next().val();
        if (amnt < 0) {
            alert("数量不能小于0，等于0是删除该商品");
            return;
        }

        $.post("/Cart/Modify/"+prod_id,
            {
                amount: amnt
            },
            function (result) {
                if (result == "ok")
                    window.location.reload();
                else
                    alert("error");
            });
    });
});

function checkout(link) {
    if ($("table.cart tbody td").size() == 0) {
        alert("购物车中没有物品");
        return;
    }
    window.location.href = "/Cart/CheckOut";
}