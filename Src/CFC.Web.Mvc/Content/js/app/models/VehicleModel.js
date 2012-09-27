var VehicleModel = Model.extend({

    initialize: function () {
    },
    
    /**
    * Load vehicles from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of vehicle models
    */
    DeleteVehicle: function (id, callback) {
        CFC.SendApiRequest(CFC.Url('Vehicle/Delete/' + id), null, 'POST', function (response) {
            callback(response);
        });
    },

    /**
    * Load vehicles from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of vehicle models
    */
    GetVehicles: function (options, callback) {
        CFC.SendApiRequest(CFC.Url('Vehicle/Get'), options, 'GET', function (response) {
            callback(response);
        });
    }

});