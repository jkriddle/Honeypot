var TripAddressModel = Model.extend({

    Caption: "Waypoint",
    Duration: 0,

    /** 
    * Constructor
    */
    initialize: function (data) {
        if (data != undefined) {
            // Map Data
            this.AutoMap(data);
        }
    }

});