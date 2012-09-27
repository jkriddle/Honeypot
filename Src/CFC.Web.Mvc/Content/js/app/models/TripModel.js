var TripModel = Model.extend({

    TripAddresses: [],

    LowestBidDisplay: 'no bids',

    /**
    * Constructor
    */
    initialize: function (data) {
        var self = this;

        // Auto mapping
        if (data != undefined) {
            this.AutoMap(data);

            // Map Trip Addresses
            for (var i in data.TripAddresses) {

                var address = new TripAddressModel(data.TripAddresses[i]);

                // Setup captions
                if (i == 0) {
                    address.Caption = "Pickup Location";
                }
                if (i == data.TripAddresses.length - 1) {
                    address.Caption = "Final Destination";
                }

                self.TripAddresses.push(address);
            }
        }

        // Define additional properties
        if (self.LowestBidAmount > 0) {
            self.LowestBidDisplay = "$" + self.LowestBidAmount;
        }

    },

    CancelTrip: function (tripId, callback) {
        $.ajax({
            url: CFC.Url('/Trip/Cancel'),
            dataType: 'json',
            type: 'POST',
            data: { TripId: tripId },
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
        callback();
    },

    NoShow: function (tripId, callback) {
        $.ajax({
            url: CFC.Url('/Trip/NoShow'),
            dataType: 'json',
            type: 'POST',
            data: { TripId: tripId },
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
        callback();
    },


    /**
    * Load trips from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of trip models
    */
    GetTrips: function (options, callback) {
        var self = this;

        $.ajax({
            url: CFC.Url('/Trip/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            cache: false,
            success: function (response) {

                // map trips to TripModels
                response.Trips = self.MapTrips(response.Trips);
                callback(response);

            }, error: function (a, b, c) {
                CFC.Log(a);
                CFC.Log(b);
                CFC.Log(c);
            }
        });
    },

    /**
    * Convert a list of raw JSON trips into a list of TripModel objects
    */
    MapTrips: function (trips) {
        var mapped = [];
        for (var i in trips) {
            var trip = trips[i];
            mapped.push(new TripModel(trip));
        }
        return mapped;
    },

    /**
    * Load updated trip information from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of trip models
    */
    GetTripUpdates: function (options, callback) {
        var self = this;
        $.ajax({
            url: CFC.Url('/Trip/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            cache: false,
            success: function (response) {

                response.Trips = self.MapTrips(response.Trips);
                callback(response);

            }
        });
    }

});

