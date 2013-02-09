//----------------------------------------
// Log
//----------------------------------------
var Log = Observable.extend({

    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_LIST: 'list',
    
    init: function(){
        this._super();
    },

    // Public

    // Retrieve log
    getOne: function (id) {
        var self = this;
        Honeypot.Api.get("/Api/Log/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve log list
    get: function (opt) {
        var self = this;
        Honeypot.Api.get("/Api/Log/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    }

});
    