/**
 * Very basic implementation of an infinite scroll for a page
 */
(function ($) {

    $.fn.infinitescroll = function (callback) {
        $(window).scroll(function () {
            // If user is at bottom of page, run the callback
            if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                callback();
            }
        });
    };

})(jQuery);

