var BidModel = Model.extend({

    initialize: function (data) {
        if (data != undefined) {
            // Map Data
            this.AutoMap(data);
        }
    },

    /**
    * Load bids from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of bid models and original server response
    */
    GetBids: function (options, callback) {
        $.ajax({
            url: CFC.Url('/Bid/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    },

    /**
    * Load updated bid information from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of bid models
    */
    GetBidUpdates: function (options, callback) {
        $.ajax({
            url: CFC.Url('/Bid/GetUpdatedBids'),
            dataType: 'json',
            data: options,
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    },

    /**
    * Display dialog for placing a bid
    * @param       tripId     int       ID of the trip this bid is for
    */
    PlaceBid: function (tripId, callback) {
        var panel = $('#bidPanel');

        CFC.DialogInstance.ShowLoader();

        CFC.SendApiRequest('/Trip/SetupBid/' + tripId, null, 'GET', function (trip) {

            // Show dialog
            var template = $('#bidTemplate').html();
            
            CFC.DialogInstance.Open({
                title : 'Place a Bid',
                width : 660,
                content : template,
                viewData : trip
            }, function() {

                // Submit to server
                $('#bidPanel', CFC.DialogInstance.domElement).submit(function (e) {
                    e.preventDefault();
                   
                    var params = CFC.DialogInstance.Serialize();
                    CFC.Log(params);
                    $.post(CFC.Url('Bid/Create'), params, function (data) {

                        var vm = new ResponseModel(data);
                        if (vm.Success) {
                            // Close dialog and show success message
                            CFC.ShowMessage('.contentWrapper', vm.Message, vm.Errors, CFC.MessageType.Success);
                            CFC.DialogInstance.Close();

                            if (callback != null) callback();
                            return;
                        }

                        // Show errors
                        CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                    }, 'json');
                });

            });
        });
    }

});