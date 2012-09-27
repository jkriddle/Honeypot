var CFC = {
    /**
    * Remove error message for element
    * @param   el                          string          jQuery path message parent div
    */
    ClearError: function (el) {
        $(el).fadeOut();
    },

    /**
    * Currently logged in user
    */
    CurrentUser: {
        UserId: null,
        CompanyId: null,
        Location: null
    },

    /**
    * Perform a function after a specified amount of time
    */
    Delay: (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    }),

    /**
    * Modal dialog instance
    */
    DialogInstance: null,

    /**
    * Used to show loading panel after period of time
    */
    DialogTimeout: null,

    /**
    * Run a controller action. Allows methods of an object to be called as case-insenstive strings.
    * @param   controller          object          Controller object
    * @param   action              string          Action name
    */
    DoAction: function (controller, action) {

        var cLower = controller.toLowerCase();
        var aLower = action.toLowerCase();

        for (var c in window) {
            if (c.toLowerCase() == cLower) {
                var cObj = new window[c];
                for (var a in cObj) {
                    if (a.toLowerCase() == aLower) {
                        cObj[a]();
                    }
                }
            }
        }
        return null;
    },

    /**
    * Converts a dataeobject into a .NET MVC compatible format (2012-08-01 14:30:00)
    * If first parameter is a Date object, it will use that and return the string
    * If dt and time parameters are strings, it is expecting you are sending human readable strings in this format:
    *     FormatDate('08/01/2012', '15:00') - i.e. 8/1/2012 at 3:00pm
    *     MUST include leading zeroes for all dates/times (uses regex)
    * and will return that in the properly formatted date
    */
    FormatDate: function (dt, time) {

        var d = dt;

        // First parameter is a date object.
        if (d.getMonth == undefined && time != undefined) {

            // Take a date and time stamp and return the properly formatted date string
            var fullPickupDate = dt + ' ' + time;

            var pattern = new RegExp("(\\d{2})/(\\d{2})/(\\d{4}) (\\d{2}):(\\d{2})");
            var parts = fullPickupDate.match(pattern);
            CFC.Log(parts);
            d = new Date(parseInt(parts[3]),
                parseInt(parts[1], 10) - 1,
                parseInt(parts[2], 10),
                parseInt(parts[4], 10),
                parseInt(parts[5], 10), 0);
        }

        var day = d.getUTCDate();
        var month = d.getUTCMonth() + 1; //Months are zero based
        var year = d.getUTCFullYear();
        var hours = d.getUTCHours();
        var minutes = d.getUTCMinutes();
        var seconds = d.getUTCSeconds();
        return year + '-' + CFC.Pad(month, 2) + '-' + CFC.Pad(day, 2) + ' ' + CFC.Pad(hours, 2) + ':' + CFC.Pad(minutes, 2) + ':' + CFC.Pad(seconds, 2);

        return null;
    },
    
    GetParam : function(name) {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars[name];
    },
    
    /**
     * Returns current date as UTC based on client side settings
     */
    GetUtc : function(dt) {
        if (dt == undefined) dt = new Date();
        return new Date(dt.getUTCFullYear(), dt.getUTCMonth(), dt.getUTCDate(),
                      dt.getUTCHours(), dt.getUTCMinutes(), dt.getUTCSeconds());
    },

    /**
    * Pad a number with leading zeroes
    */
    Pad: function (number, length) {
        var str = '' + number;
        while (str.length < length) {
            str = '0' + str;
        }
        return str;
    },

    /**
    * Hide loader and re-display original element
    * @param    el      string          jQuery path to element to re-display (button/link/etc)     
    */
    HideLoader: function (el) {
        $('.loadingOverlay').fadeOut();
        $('input,button,select', el).removeAttr("disabled");
        clearInterval(CFC.DialogTimeout);
    },

    /**
    * Flash the background color of an element
    * @param   el              string      jQuery path to element
    * @param   highlightColor  string      Color to highlight element
    * @param   duration        int         Time of animation effect (ms)
    */
    Highlight: function (el, highlightColor, duration) {
        var highlightBg = highlightColor || "#FFFF9C";
        var animateMs = duration || 1500;
        var originalBg = $(el).css("backgroundColor");
        $(el).stop().css("background-color", highlightBg).animate({ backgroundColor: originalBg }, animateMs);
    },

    /**
    * Setup application
    */
    Init: function () {
        CFC.InitForm('form');

        // Setup buttons
        $("input:submit, a.button, button").addClass('btn');

        // Phone masking
        $(".phone").mask("(999) 999-9999", { placeholder: " " });

        CFC.DialogInstance = new CFC.ModalDialog('#dialog');
    },

    /** 
    * Setup form display
    */
    InitForm: function (el) {
        $("input.required, select.required", el).RequiredMarkers();
        $("label.inline", el).InlineLabels();

        // Setup buttons
        $("input:submit, a.button, button").addClass('btn');

        // Phone masking
        $(".phone", el).mask("(999) 999-9999", { placeholder: " " });
    },

    Load: {
        Controller: function (name, callback) {
            jQuery.getScript(CFC.Path('controllers/' + name.toLowerCase() + 'controller.js'), callback);
        }
    },

    /**
    * Log a message to the browser's console if it exists
    */
    Log: function (message) {
        if (window.console != undefined)
            if (window.console.log != undefined)
                window.console.log(message);
    },

    /**
    * Types of messages to display
    */
    MessageType: {
        Error: 'ui-state-error',
        Warning: 'ui-state-highlight',
        Success: 'ui-state-highlight'
    },

    /**
    * Types of messages to display
    */
    TripStatus: {
        Bidding: 'Bidding',
        InProcess: 'InProcess',
        Canceled: 'Canceled'
    },

    /**
    * Return absolute path to specified item
    * @param       originalPath    string      Path to asset
    * @returns                     string      Absolute path
    */
    Path: function (originalPath) {
        return window.AppRoot + CFC.removeLeadingSlash(originalPath);
    },

    /**
    * Removes leading slash from string if exists
    */
    removeLeadingSlash: function (originalPath) {
        var path = originalPath;
        if (path[0] == '/') path = path.substring(1);
        return path;
    },

    /**
    * Send a web request to the site (works remotely and locally)
    */
    SendApiRequest: function (url, data, method, callback) {
        method = method || 'GET';
        var serialized = $.toDictionary(data);
        /*CFC.Log('Sending ' + method + ' request to ' + url);
        CFC.Log(serialized);*/
        $.ajax({
            type: method,
            url: url,
            dataType: 'json',
            data: serialized,
            cache: false,
            //crossDomain: true,
            traditional: true
        }).done(
            function (resp) {
                callback(resp);
            });
    },

    HideMessage: function (el) {
        $('.message', el).fadeOut(300);
    },

    /**
    * Display error message for element
    * @param   el                          string           jQuery path message parent div
    * @param   message                     string           Message to display
    * @param   listOfMessages              array/string     List of messages
    * @param   type                        string           Type of message (error, notice, success)
    * @param   scrollWindow                bool             If window should scroll to top (when showing message in page, not in modal dialog)
    */
    ShowMessage: function (el, message, listOfMessages, type, scrollWindow) {
        scrollWindow = (scrollWindow == undefined ? true : scrollWindow);
        if (listOfMessages == undefined) listOfMessages = [];

        var messageList = listOfMessages;
        if (type == undefined) type = CFC.MessageType.Warning;
        if (message != null && message) {
            messageList.push({ PropertyName: "", Message: message });
        }
        var messageEl = $('.message', el);
        $(messageEl).fadeOut(200, function () {
            $(messageEl).children().remove();
            $(messageEl).removeClass('success notice error');
            $(messageEl).addClass(type);
            $("#errorTemplate").tmpl(messageList).appendTo(messageEl);
            // If only one element is shown, do not display it as a list
            if (messageList.length == 1) {
                $(messageEl).addClass('inline-list');
            }
            $(messageEl).fadeIn();

            if (scrollWindow) $('html, body').animate({ scrollTop: 0 }, 800);
            else $('.modal-body').animate({ scrollTop: 0 }, 800);
        });
    },

    /**
    * Returns path with translated Root URL based on server configuratino
    */
    Url: function (path) {
        return window.RootUrl + CFC.removeLeadingSlash(path);
    },

    /**
    * Returns path to a view file
    */
    View: function (path) {
        return CFC.Path('views/' + CFC.removeLeadingSlash(path));
    },

    /**
    * Sets Active navigation link
    */
    ActiveNav: function (id) {

        $("#dashboardNavigation li").each(function () {
            $(this).removeClass('active');
        });

        var newClass = "." + id;
        $(newClass).addClass('active');
    }
};

$(".searchField").live('click', function (e) {
    $(this).parent().children('label').text('');

});


CFC.ModalDialog = function (domId) {
    var self = this;
    self.domElement = domId;

    // Submit modal form on click

    $('.btn-primary', self.domElement).click(function (e) {
        $('form', self.domElement).submit();
        self.Disable();
    });

    $('body').keyup(function (e) {
        // Close dialog on escape key
        if (e.which == 27) self.Close();
    });

};

CFC.ModalDialog.prototype.Enable = function () {
    var self = this;

    $('.btn-primary', self.domElement).removeAttr("disabled");
};

CFC.ModalDialog.prototype.Disable = function () {
    var self = this;

    $('.btn-primary', self.domElement).attr("disabled", "disabled");
};

CFC.ModalDialog.prototype.Serialize = function () {
    var self = this;
    return $('form', self.domElement).serialize();
};

CFC.ModalDialog.prototype.ShowMessage = function (message, errors, type) {
    var self = this;
    self.Enable();
    CFC.ShowMessage(self.domElement, message, errors, type, false);
};

CFC.ModalDialog.prototype.ShowLoader = function (title) {
    var self = this;
    if (title == undefined) title = 'Loading...';
    self.Open({
       title : title,
       width: 660,
       showSubmit: false,
       content:'<div class="more"><div class="circle"></div><div class="circle1"></div></div><p><br /></p>'
    });
};

CFC.ModalDialog.prototype.Close = function () {
    var self = this;
    $(self.domElement).modal('hide');
};


// Display modal dialog window
CFC.ModalDialog.prototype.Open = function (options, openCallback) {
    var self = this;

    self.Enable();

    // Available options
    var title = options.title;
    var width = options.width || 440;
    var embeddedContent = options.content;
    var viewData = options.viewData;
    var submitText = options.submitText || 'Save';
    var showSubmit = options.showSubmit == undefined ? true : options.showSubmit;

    // Twitter Bootstrap
    $(self.domElement).modal('show');

    // Reset view state
    if (showSubmit) $('.btn-primary', self.domElement).show();
    else $('.btn-primary', self.domElement).hide();
    $('.btn-primary', self.domElement).html(submitText);
    $('.message', self.domElement).hide();
    $('.loader', self.domElement).show();

    // Setup template
    var templateContent = Mustache.to_html(embeddedContent, viewData);
    $(self.domElement).css('width', width + 'px');
    $(self.domElement).css('margin-left', '-' + (width / 2) + 'px');

    // Parameterized form elements
    $('.modal-header h3', self.domElement).text(title);

    // Inject content and display. Must remove inner element so that elements
    // are not longer in the DOM (even after removed they remain in memory unless
    // parent is removed?
    var newForm = $('<div class="modal-body" />');
    newForm.append(templateContent);
    $('.modal-body', self.domElement).replaceWith(newForm);

    // Show rendered content
    $('.loader', self.domElement).hide();

    CFC.InitForm(self.domElement);

    // Focus on the first form element
    $('input:first', '.modal-body').focus();

    if (openCallback) openCallback();

};
