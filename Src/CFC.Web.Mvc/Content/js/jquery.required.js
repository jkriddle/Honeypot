//= require <jquery>
/*!
* Mark labels of associated fields as required.
*/
(function ($) {
    $.fn.extend({
        RequiredMarkers: function (options) {

            // Add required element to labels explicitly marked as required.
            $("label.required").append("<span class='required'>*</span>");

            //Iterate over the current set of matched elements
            return $(this).each(function () {

                // Add required element to labels. Do not mark inline labels as required
                var sib = $(this).siblings("label:not(.inline)").append("<span class='required'>*</span>");
            });
        }
    });
})(jQuery);