///
///  Sketchbook page init
///
$(document).ready(function () {

    // Fancy it up
    $('.fancy-box').fancybox({
        prevEffect: 'none',
        nextEffect: 'none',
        type : "image",

        closeBtn: true,
        arrows: false,
        nextClick: true,

        helpers: {
            thumbs: {
                width: 50,
                height: 50
            }
        }
    });

});
