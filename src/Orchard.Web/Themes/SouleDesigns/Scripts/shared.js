/*
Soule Designs shared javascript functionality
*/

//-------------------------------------------------------
// Page init
//-------------------------------------------------------
$(document).ready(function () {
    // Prevent funky fresh navbar render
    setTimeout(ShowNavbar, 200);

    // To top visibility
    $(window).scroll(function () {
        if ($(this).scrollTop() != 0) {
            $("#toTop").fadeIn()
        }
        else {
            $("#toTop").fadeOut()
        }
    });

    // Scroll to top
    $("#sd-top-link").click(function () {
        $("body,html").animate({ scrollTop: 0 }, 2e3);
    });

    // Toggle search popup
    $("#sd-nav-search > .btn").click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        var isVisible = $("#sd-nav-searchform").is(":visible");
        if (isVisible) {
            $("#sd-nav-search > .btn,#sd-nav-searchform").removeClass("popped");
        } else {
            $("#sd-nav-search > .btn,#sd-nav-searchform").addClass("popped");
            $("#sd-nav-searchform input[type=text]").focus();
        }
    });

    // Hide search popup			
    $(document).on('touchstart click', function (e) {
        var container = $("#sd-nav-searchform");

        // Assure click isn't in the search form
        // and the form is shown
        if (!container.is(e.target) &&
            container.has(e.target).length == 0 &&
            container.hasClass("popped")) {
            $("#sd-nav-search > .btn,#sd-nav-searchform").removeClass("popped");
        }
    });


    // Pagination
    var pagerCurrent = $('#pager-current');
    if(pagerCurrent.length > 0) {
        pagerCurrent.parent('li').addClass('active');
        pagerCurrent.closest('ul').removeClass('pager').addClass('pagination');
    }

});

//-------------------------------------------------------
// Show the navbar
//-------------------------------------------------------
function ShowNavbar() {
    $("#navbar-wrapper").show();
}

// This needs a flag to tell it not to slide when in mobile mode, investigate later

////-------------------------------------------------------
//// Toggle navbar visibility
////-------------------------------------------------------
//function ToggleNavbar(show) {

//    if (show) {
//        $("#sd-nav-searchform").show('slide', { direction: 'right' }, 400);
//        setTimeout(function () { SlideNavbar_Complete(show); }, 500);
//    } else {
//        $("#sd-nav-searchform").hide('slide', { direction: 'right' }, 400);
//        setTimeout(function () { SlideNavbar_Complete(show); }, 500);
//    }

//}

////-------------------------------------------------------
//// Show the navbar
////-------------------------------------------------------
//function SlideNavbar_Complete(show) {
//    if (show) {
//        $("#sd-nav-search > .btn,#sd-nav-searchform").addClass("popped");
//        $("#sd-nav-searchform input[type=text]").focus();
//    } else {
//        $("#sd-nav-search > .btn,#sd-nav-searchform").removeClass("popped");
//    }
//}