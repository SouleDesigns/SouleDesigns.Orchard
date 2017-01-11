/*--------------------------------------------------
Soule Designs Portfolio Page Script
--------------------------------------------------*/

$(document).ready(function () {
    var gridContainer = $('#grid-container'),
        filtersContainer = $('#filters-container');

    // "Radiobuttonize" cubeportfolio filter
    $("#filters-container .btn").click(function () {
        $("#filters-container .btn").removeClass("active");
        $(this).addClass("active");
    });

    // init cubeportfolio
    gridContainer.cubeportfolio({
        defaultFilter: '*',
        animationType: 'flipBottom', 
        gapHorizontal: 15,
        gapVertical: 15,
        gridAdjustment: 'responsive',
        caption: 'zoom',
        displayType: 'lazyLoading',
        displayTypeSpeed: 100,

        // lightbox not used
        lightboxDelegate: '.cbp-lightbox',
        lightboxGallery: false,
        lightboxTitleSrc: 'data-title',
        lightboxShowCounter: false,

        // singlePage popup not used
        singlePageDelegate: '.cbp-singlePage',
        singlePageDeeplinking: false,
        singlePageStickyNavigation: false,
        singlePageShowCounter: false,
        singlePageCallback: null,

        // single page inline not used
        singlePageInlineDelegate: '.cbp-singlePageInline',
        singlePageInlinePosition: 'above',
        singlePageInlineShowCounter: false,
        singlePageInlineInFocus: false,
        singlePageInlineCallback: null
    });

    // add listener for filters click
    filtersContainer.on('click', '.cbp-filter-item', function (e) {
        var me = $(this), wrap;

        // get cubeportfolio data and check if is still animating (reposition) the items.
        if (!$.data(gridContainer[0], 'cubeportfolio').isAnimating) {

            if (filtersContainer.hasClass('cbp-l-filters-dropdown')) {
                wrap = $('.cbp-l-filters-dropdownWrap');

                wrap.find('.cbp-filter-item').removeClass('cbp-filter-item-active');

                wrap.find('.cbp-l-filters-dropdownHeader').text(me.text());

                me.addClass('cbp-filter-item-active');
            } else {
                me.addClass('cbp-filter-item-active').siblings().removeClass('cbp-filter-item-active');
            }

        }

        // filter the items
        gridContainer.cubeportfolio('filter', me.data('filter'), function () { });
    });

    // activate counters
    gridContainer.cubeportfolio('showCounter', filtersContainer.find('.cbp-filter-item'));


    $(".cbp-caption-activeWrap").on("click", function () {
        var anchor = $(this).find("a").first();
        if(anchor.length > 0)
        {
            window.location = anchor.first().attr("href");
        }
    });
});
