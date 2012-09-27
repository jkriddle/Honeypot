/**
 * Base class for common functionality across all models
 */
var Model = Class.define({

    /**
    * Maps data from a specified object into the current model (similar to AutoMapper for .NET)
    */
    AutoMap: function (data) {
        var self = this;
        for (var p in data) {
            self[p] = data[p];
        }
    }

});