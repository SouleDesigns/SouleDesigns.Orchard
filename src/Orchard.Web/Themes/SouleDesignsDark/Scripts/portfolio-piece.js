/*--------------------------------------------------------------------------
Soule Designs Portfolio Piece script
--------------------------------------------------------------------------*/
$(window).load(function () {

    // Find all Vimeo videos
    var $allVideos = $("iframe"),
        // The element that is fluid width
        $fluidEl = $(".portfolio-piece .container > ");

    // Figure out and save aspect ratio for each video
    $allVideos.each(function () {

        $(this)
          .data('aspectRatio', this.height / this.width)

          // and remove the hard coded width/height
          .removeAttr('height')
          .removeAttr('width');

    });

    // When the window is resized
    $(window).resize(function () {

        var newWidth = $fluidEl.innerWidth();

        // Resize all videos according to their own aspect ratio
        $allVideos.each(function () {
            var padding = 15;
            var $el = $(this);
            $el
              .width(newWidth - padding)
              .height((newWidth - padding) * $el.data('aspectRatio'));

        });

        // Kick off one resize to fix all videos on page load
    }).resize();

    // The slider being synced must be initialized first
    $('#carousel').flexslider({
        animation: "slide",
        controlNav: false,
        animationLoop: false,
        slideshow: false,
        itemWidth: 150,
        itemMargin: 5,
        asNavFor: '#slider'
    });

    $('#slider')
        .flexslider({
            animation: "slide",
            controlNav: false,
            animationLoop: false,
            slideshow: false,
            smoothHeight: true,
            video: true,
            sync: "#carousel"
        });

    // Fancy it up
    $('.fancy-box').fancybox({
        
        prevEffect: 'none',
        nextEffect: 'none',

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
