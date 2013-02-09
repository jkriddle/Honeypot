// Console log for unsupported browsers
var debugging = true; // or true
if (typeof console == "undefined") var console = { log: function () { } };
else if (!debugging || typeof console.log == "undefined") console.log = function () { };

// Handlebars helper to determine if two values are equal.
Handlebars.registerHelper('equals', function (p1, p2, options) {
    return p1 == p2 ? options.fn(this) : null;
});

// Handlebars helper to determine if two values are not equal.
Handlebars.registerHelper('notEquals', function (p1, p2, options) {
    return p1 != p2 ? options.fn(this) : null;
});

var Honeypot = {};

/* Configurations
 *************************************************************/
Honeypot.Config = {};

Honeypot.Config.dateRanges = {
    'Today': ['today', 'today'],
    'Yesterday': ['yesterday', 'yesterday'],
    'Last 7 Days': [Date.today().add({ days: -6 }), 'today'],
    'Last 30 Days': [Date.today().add({ days: -29 }), 'today'],
    'This Month': [Date.today().moveToFirstDayOfMonth(), Date.today().moveToLastDayOfMonth()],
    'Last Month': [Date.today().moveToFirstDayOfMonth().add({ months: -1 }), Date.today().moveToFirstDayOfMonth().add({ days: -1 })]
};

/* Querystring
 *************************************************************/
// Create a "QueryString" object containing all URL parameters.
// Usage: Honeypot.QueryString.myParam (?myParam=foo)
Honeypot.QueryString = function () {
    // This function is anonymous, is executed immediately and 
    // the return value is assigned to QueryString!
    var queryString = {};
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        // If first entry with this name
        if (typeof queryString[pair[0]] === "undefined") {
            queryString[pair[0]] = pair[1];
            // If second entry with this name
        } else if (typeof queryString[pair[0]] === "string") {
            var arr = [queryString[pair[0]], pair[1]];
            queryString[pair[0]] = arr;
            // If third or later entry with this name
        } else {
            queryString[pair[0]].push(pair[1]);
        }
    }
    return queryString;
}();

/* Templating
 *************************************************************/
// Helper to generate Handlebars template from jQuery ID
Honeypot.Template = function (el, ob) {
    var html = $(el).html();
    var temp = Handlebars.compile(html);
    return temp(ob);
};

// Helper to generate handlebars template from raw HTML
Honeypot.Template.raw = function (html, ob) {
    var temp = Handlebars.compile(html);
    return temp(ob);
};

/* API
 *************************************************************/
Honeypot.Api = {

    Auth: {
        setToken: function (token, expires) {
            $.cookie('authToken', token, { expires: expires, path: '/' });
        },
        getToken: function (token) {
            return $.cookie('authToken');
        },
        clearToken: function () {
            $.removeCookie('authToken', { path: '/' });
        }
    },

    // Send a GET request
    get: function (url, data, success, error) {
        Honeypot.Api._sendRequest(url, data, 'GET', success, error);
    },

    // Send a POST request
    post: function (url, data, success, error) {
        Honeypot.Api._sendRequest(url, data, 'POST', success, error);
    },

    // Private function to handle all API traffic
    _sendRequest: function (url, data, method, success, error) {
        if (error == undefined) {
            error = function (xhr, status, p3, p4) {
                console.log(xhr);
                var err = "";
                if (xhr.responseText && xhr.responseText[0] == "{") {
                    var obj = JSON.parse(xhr.responseText);
                    err = obj.ExceptionMessage + "<br /><pre>" + obj.StackTrace + "</pre>";
                }
                else {
                    err = "An error has occurred: " + status + " " + p3;
                }

                // User login has expired.
                if (obj.ExceptionMessage == "Unable to validate your device credentials.") {
                    window.location.href = '/User/SignIn?Message=You+have+been+logged+out.';
                    return;
                }

                if (bootbox != undefined) bootbox.alert(err);
                else alert(err);
            };
        }

        $.ajax({
            url: url,
            type: method,
            data: data,
            success: success,
            headers: { "AuthToken": Honeypot.Api.Auth.getToken() },
            error: error,
            cache: false
        });
    },
};