$(function () {
    var txtarea = $("textarea#adviser");

    txtarea.focusin(function () {
        $("p.err_tip").css("visibility", "hidden");
    });

    txtarea.focusout(function () {
        if ($.trim(txtarea.val()).length < 8) {
            $("p.err_tip").css("visibility", "visible");
        }
    });
});

function commit_advise() {
    var txtarea = $("textarea#adviser");

    if ($.trim(txtarea.val()).length <= 8) {
        $("p.err_tip").css("visibility", "visible");
        return false;
    }

    $.post("/Help/NewAdvise",
        {
            content: $("textarea#adviser").val()
        },
        function (result) {
            alert(result);
        });
}