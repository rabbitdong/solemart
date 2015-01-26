$(function () {
    var imgs = $('#slide-show-images img').hide(), index = 0;

    imgs.eq(index).show();

    function swapImages() {
        imgs.eq(index).fadeOut(2000, function () {

            index++;
            if (index == imgs.length) {
                index = 0;
            }
            imgs.eq(index).fadeIn(2000);
        });
    }

    window.setInterval(swapImages, 5000);
});