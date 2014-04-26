///
///  Blog post page init
///
$(document).ready(function () {

    // Prep fancy
    $('.blogpost a').has('img')
        .addClass('fancy-box')
        .attr("data-fancybox-group", "gallery");

    // Fancy it up
    $('.fancy-box').fancybox({
        prevEffect : 'none',
		nextEffect : 'none',

		closeBtn  : true,
		arrows    : false,
		nextClick : true,

		helpers : {
			thumbs : {
				width  : 50,
				height : 50
			}
		}
        });
    
});
