/**
 * Basic model for handling server responses from
 * ajax queries
 */
var ResponseModel = Model.extend({    
    /**
     * Constructor
     */
    initialize: function(data) {
        var self = this;
        self.Success = data.Success;
        self.Message = data.Message;
        self.Errors = data.Errors;
    }

});