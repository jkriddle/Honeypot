/**
* UI Setup
*/
$(function () {
    CFC.Init();

    // Add CSS browser class to body element
    $(document).ready(function () {
        var a = navigator.userAgent.toLowerCase();
        $.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
        if ($.browser.msie) {
            $('body').addClass('browserIE');
            $('body').addClass('browserIE' + $.browser.version.substring(0, 1));
        }
        if ($.browser.chrome) {
            $('body').addClass('browserChrome');
            a = a.substring(a.indexOf('chrome/') + 7);
            a = a.substring(0, 1);
            $('body').addClass('browserChrome' + a);
            $.browser.safari = false
        }
        if ($.browser.safari) {
            $('body').addClass('browserSafari');
            a = a.substring(a.indexOf('version/') + 8);
            a = a.substring(0, 1);
            $('body').addClass('browserSafari' + a);
        }
        ;
        if ($.browser.mozilla) {
            if (navigator.userAgent.toLowerCase().indexOf('firefox') != -1) {
                $('body').addClass('browserFirefox');
                a = a.substring(a.indexOf('firefox/') + 8);
                a = a.substring(0, 1);
                $('body').addClass('browserFirefox' + a);
            } else {
                $('body').addClass('browserMozilla');
            }
        }
        if ($.browser.opera) {
            $('body').addClass('browserOpera');
        }
    });

    // Add IE support for placeholder HTML5 elements
    if (!Modernizr.input.placeholder) {
        $("input[type='text']").each(
        function () {
            if ($(this).val() == "" && $(this).attr("placeholder") != "") {
                $(this).val($(this).attr("placeholder"));
                $(this).focus(function () {
                    if ($(this).val() == $(this).attr("placeholder")) $(this).val("");
                });
                $(this).blur(function () {
                    if ($(this).val() == "") $(this).val($(this).attr("placeholder"));
                });
            }
        });
    }

    // Authentication/Signup/Forgot Password
    CFC.Load.Controller('auth', function () {
        var authController = new AuthController();
    });

});