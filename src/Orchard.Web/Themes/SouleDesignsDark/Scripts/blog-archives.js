///
///  Archive page init
///
$(document).ready(function () {
    $(".blog-archives .expando").click(function () {
        Blog_Archives_Expando($(this));
    });

    $(document).ready(function () {
        $(".blog-archives .lazy").click(function () {
            var arrow = $(this);
            if (arrow.hasClass("expanded")) {
                Blog_Archives_Expando(arrow);
            } else {
                Blog_Archives_Lazy(arrow);
            }
        });

    });
});

///
/// Helper to expand/collapse archives
///
function Blog_Archives_Expando(arrow) {
    var ul = arrow.closest("li").find("ul:first-of-type").first();
    if (ul.is(":visible")) {        
        ul.slideUp(function () {
            arrow.html("<i class='fa fa-plus-square-o'></i>&nbsp;");
        });
    } else {        
        arrow.addClass("expanded");
        arrow.html("<i class='fa fa-minus-square-o'></i>&nbsp;");
        ul.slideDown();
    }
}


///
/// Helper to get article list for a month
///
function Blog_Archives_Lazy(arrow) {
    // Loading
    arrow.html("<i class='fa fa-spinner fa-spin'></i>&nbsp;");
    var arrowAnchor = arrow.closest("div").find("a").first();
    var url = arrowAnchor.attr("href");

    // Get it?
    $.get(
        url,
        function (data) {
            // Strip out script/etc
            var results = $.parseHTML(data);
            var monthUl = "<ul style='display: none'>";
            var resultsFound = false;

            // Build list
            $.each($(results).find(".post-title a"), function (index, anchor) {
                var postTitle = $(anchor).text();
                var postUrl = $(anchor).attr("href");
                monthUl += "<li><a href='" + postUrl + "'>" + postTitle + "</a></li>";
                resultsFound = true;
            });

            // Finish up
            if (!resultsFound) {
                monthUl += "<li>Oops!  Fail :-(</li>";
            }
            monthUl += "</ul>";

            // Done!
            arrow.closest("li").append(monthUl);
            Blog_Archives_Expando(arrow);
        });
}