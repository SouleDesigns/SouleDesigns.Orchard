// Faux function to prevent IE iframe video error
function __flash__removeCallback(instance, name) {
    if (instance == null) return;
    instance[name] = null;
}

$(document).ready(function () {
    var gridContainer = $('#grid-container'),
        filtersContainer = $('#filters-container');

    // Radiobuttonize cubeportfolio filter
    $("#filters-container .btn").click(function () {
        $("#filters-container .btn").removeClass("active");
        $(this).addClass("active");
    });

    // init cubeportfolio
    gridContainer.cubeportfolio({

        defaultFilter: '*',

        animationType: 'flipBottom', /*flipOutDelay',*/

        gapHorizontal: 15,

        gapVertical: 15,

        gridAdjustment: 'responsive',

        caption: 'zoom',

        displayType: 'lazyLoading',

        displayTypeSpeed: 100,

        // lightbox
        lightboxDelegate: '.cbp-lightbox',
        lightboxGallery: true,
        lightboxTitleSrc: 'data-title',
        lightboxShowCounter: true,

        // singlePage popup
        singlePageDelegate: '.cbp-singlePage',
        singlePageDeeplinking: true,
        singlePageStickyNavigation: true,
        singlePageShowCounter: true,
        singlePageCallback: function (url, element) {        

            // to update singlePage content use the following method: this.updateSinglePage(yourContent)
            var t = this;

            $.ajax({
                url: url,
                type: 'GET',
                dataType: 'html',
                timeout: 60000
            })
            .done(function (result) {
                // Fix wonky IE iframe unload issues
                $("iframe[src*='//player.vimeo.com']").each(
                    function (index, item) {
                        try {
                            item.src = '';
                        } catch (ex) { }
                });
                
                // Allow iframes to clear
                setTimeout(function () { t.updateSinglePage(result); }, 500);                
                
            })
            .fail(function () {
                t.updateSinglePage("Error! Please refresh the page!");
            });

        },

        // single page inline
        singlePageInlineDelegate: '.cbp-singlePageInline',
        singlePageInlinePosition: 'above',
        singlePageInlineShowCounter: true,
        singlePageInlineInFocus: true,
        singlePageInlineCallback: function (url, element) {
            // to update singlePage Inline content use the following method: this.updateSinglePageInline(yourContent)                               
        }
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


    // add listener for load more click
    $('.cbp-l-loadMore-button-link').on('click', function (e) {

        e.preventDefault();

        var clicks, me = $(this), oMsg;

        if (me.hasClass('cbp-l-loadMore-button-stop')) return;

        // get the number of times the loadMore link has been clicked
        clicks = $.data(this, 'numberOfClicks');
        clicks = (clicks) ? ++clicks : 1;
        $.data(this, 'numberOfClicks', clicks);

        // set loading status
        oMsg = me.text();
        me.text('LOADING...');

        // perform ajax request
        $.ajax({
            url: me.attr('href'),
            type: 'GET',
            dataType: 'HTML'
        })
        .done(function (result) {
            var items, itemsNext;

            // find current container
            items = $(result).filter(function () {
                return $(this).is('div' + '.cbp-loadMore-block' + clicks);
            });

            gridContainer.cubeportfolio('appendItems', items.html(),
                 function () {
                     // put the original message back
                     me.text(oMsg);

                     // check if we have more works
                     itemsNext = $(result).filter(function () {
                         return $(this).is('div' + '.cbp-loadMore-block' + (clicks + 1));
                     });

                     if (itemsNext.length === 0) {
                         me.text('NO MORE WORKS');
                         me.addClass('cbp-l-loadMore-button-stop');
                     }

                 });

        })
        .fail(function () {
            // error
        });

    });

});
