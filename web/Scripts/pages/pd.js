$(function () {
    $("#write_comment").hide();

    $("#write_comment .comment_star a").mouseenter(function () {
        $(this).nextAll().css("background-position", "left bottom");
        $(this).prevAll().css("background-position", "left top");
        $(this).css("background-position", "left top");
    });

    $("#write_comment .comment_star a").click(function () {
        star_level = this.className.substring(5, 6);
    });

    $("div.review_item .comment_star").children(function () {
        var grade = $(this).attr("title").substr(0, 1);
        var star = $(this).children(".star_" + grade);
        star.nextAll().css("background-position", "left bottom");
        star.prevAll().css("background-position", "left top");
        star.css("background-position", "left top");
    });
});


function show_image(url, src) {
    $("#main_img").attr("src", '/images/product/normal/' + url);
    $("div.thumb_area a.active").removeClass("active");
    $(src).addClass("active");
}

function write_comment_fun(btn) {
    var status = $(btn).children("a").html();
    if (status == "写评论...") {
        $("#write_comment").show();
        $(btn).children("a").html("提交");
    }
    else {
        var txtinput = $("#write_comment textarea");
        var content = txtinput.val();
        if ($.trim(content) == "") {
            alert("无信息提交无效!");
            return;
        }

        $.post("/Product/PostComment/"+cur_pid,
            {
                level: star_level,
                cnt: content
            },
            function (result) {
                var comment = eval(result);
                if (comment.IsSuccess) {
                    var html = "<div class='review_item newline'><h3>";
                    html += "<em>" + comment.Name + "</em><span>先生:</span>";
                    html += "<span class='comment_star' title='" + comment.Level + " 星'>";
                    html += "<a href='javascript:;' class='star_" + comment.Level + "'></a>";
                    html += "<a href='javascript:;' class='no_star'></a></span></h3>";
                    html += "<p class='r'>@" + comment.Time + "</p>";
                    html += "<p class='content'>" + comment.Content + "</p></div>";

                    $(html).insertBefore($("div#comments-list").children().first());
                }
                else
                    alert(comment.Message);
            });

        $(btn).children("a").html("写评论...");
        txtinput.val("");
        $("#write_comment").hide();
    }
}

function puttocart(pid) {
    $.post("/Cart/Add/" + pid, {},
        function (result) {
            if (result == "ok")
                window.location.href = "/Cart";
            else
                alert("error");
        });
}

function addtofavorite(pid) {
    var is_authenticate = false;
    $.post("/Home/IsAuthenticate", {},
        function (result) {
            is_authenticate = (result == "ok");

            if (!is_authenticate) {
                alert("你需要注册后，才能进行收藏！");
                window.location.href = "/Home/Register";
                return;
            }

            $.post("/Account/AddToFovarite/" + pid, {},
                function (result) {
                    if (result == "ok")
                        alert("收藏成功，您可以在收藏夹中查看");
                    else
                        alert("收藏失败!系统问题，我们会进行处理");
                });
        });
}