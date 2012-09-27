function Trip() {
    var self = this;
    self.TripAddresses = [];
    self.map = "";
}

function TripAddress() {
    var self = this;
}

function ScheduleRide() {
    var self = this;
    self.Trip = new Trip();
    self.IsComplete = false;
    self.IsImmediate = true;
};


ScheduleRide.prototype.checkAddress = function (fieldBlock, callback) {
    var self = this;

    // Number of addresses
    var addNum = $(fieldBlock).children('.destination').children(".destinationInner").length;

    // Destination information
    var destArr = $(fieldBlock).children('.destination').children(".destinationInner");

    // Set address to being validated
    $(fieldBlock).data('addressCheck', true);
    
    for (var a = 0; a < addNum; a++) {
        var currentDestBlock = destArr[a];
        var address = $(destArr[a]).find('.address').val();
        self.geocodeAddress(currentDestBlock, address, (a + 1 == addNum), callback);
    }

};


/**
 * Retrieve Google geocode data
 */
ScheduleRide.prototype.geocodeAddress = function (currentDestBlock, address, isLastAddress, callback) {
    var self = this;

    // Do not attempt to validate empty address
    if (address == '') {
        if (isLastAddress && callback != undefined) callback();
        return;
    }

    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, function (results, status) {

        //get the current formstep
        var formStep = $("#rideForm").formwizard("state");
        var currentId = "#" + formStep.currentStep;
        if (status == google.maps.GeocoderStatus.OK) {
            if (results.length > 1) {
                // More than one address at this location
                $(currentDestBlock).parent().children('.blockTitle').css("background-color", "red");
                var parent = $(currentDestBlock).parent();

                self.showAddressOptions(parent, results);
                $(currentId).data('addressCheck', false);
            } else {
                // Only one address at this location
                $(currentDestBlock).parent().children('.blockTitle').css("background-color", "#A0D063");
                $(currentDestBlock).parent().data("address", results);
                $(currentId).data('addressCheck', true);
            }

        } else {
            var txt = 'Unable to retrieve location data.';
            if (status == 'ZERO_RESULTS') txt = 'Unable to find specified address.';
            alert(txt);
        }

        if (isLastAddress && callback != undefined) callback();

    });
};

/**
 * Display the possible addresses at this location
 */
ScheduleRide.prototype.showAddressOptions = function (fieldBlock, results) {

    var adDiv = $(fieldBlock).find('.addressSelection');
    var adDivInner = $(fieldBlock).find('.addressSelection').children('.formDialogInner');

    var addressOptions = new Object();

    for (i = 0; i < results.length; i++) {
        var selection = '<a href="#" class="addOpt-' + i + '">' + results[i].formatted_address + "</a>";
        addressOptions[i] = JSON.stringify(results[i]);
        adDivInner.append(selection);
    }

    $(adDiv).data('addressOptions', addressOptions);
    $(fieldBlock).find('.addressOptions').show();
    adDiv.show();
};

/**
* Displays trip on a Google map
*/
ScheduleRide.prototype.displayTripMap = function (allAddressInfo) {
    var locSplit = allAddressInfo.TripAddresses[0].Location.split(",");
    var lat = locSplit[0];
    var lng = locSplit[1];
    window.latlng = new google.maps.LatLng(lat, lng);

    latLng1 = allAddressInfo.TripAddresses[0].Location.split(",");
    lat1 = latLng1[0];
    lng1 = latLng1[1];
    latLng2 = allAddressInfo.TripAddresses[1].Location.split(",");
    lat2 = latLng2[0];
    lng2 = latLng2[1];


    var mapOptions = {
        center: new google.maps.LatLng(lat, lng),
        zoom: 9,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    this.map = new google.maps.Map(document.getElementById("mapCanvas"), mapOptions);

    var marker = new google.maps.Marker({
        position: this.map.getCenter(),
        map: this.map,
        title: 'Click to zoom'
    });

    this.map.panTo(window.latlng);
    //show directions
    var waypts = [];


    //create markers
    for (var m = 0; m < allAddressInfo.TripAddresses.length; m++) {
        currLoc = allAddressInfo.TripAddresses[m].Location.split(",");
        currLat = currLoc[0];
        currLng = currLoc[1];

        if (m > 0 & m < allAddressInfo.TripAddresses.length) {
            waypts.push({ location: new google.maps.LatLng(currLat, currLng), stopover: true });
        }

        window["marker_" + m] = new google.maps.Marker({
            position: new google.maps.LatLng(currLat, currLng),
            map: this.map,
            title: 'Click to zoom'
        });

        
        
    }
    

    var directionsService = new google.maps.DirectionsService();
    directionsDisplay = new google.maps.DirectionsRenderer();
    directionsDisplay.setMap(this.map);


    var start = new google.maps.LatLng(lat1, lng1);
    var end = new google.maps.LatLng(lat2, lng2);
    var request = {
        origin: start,
        destination: end,
        waypoints: waypts,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (result, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(result);
        }
    });


};

ScheduleRide.prototype.resetMap = function () {
    google.maps.event.trigger(this.map, "resize");
    this.map.setOptions({zoom: 10 } );
    this.map.panTo(window.latlng);
};

/**
 * Collect trip informatino and display in panel
 */
ScheduleRide.prototype.createConfirmation = function () {
    var ride = this;

    var currBlock = $("#confirmation .destinationInner");
    var allAddressInfo = ride.createJSON('false');
    
    // Display each address' information
    for (var a = 0; a < allAddressInfo.TripAddresses.length; a++) {
        var currAdd = allAddressInfo.TripAddresses[a];
        var addString = "<div class='addConfirmBlock'><span class='confirmNum'>" + a + "</span><span class='confirmAdRight'>" + currAdd.Line1 + " " + currAdd.Line2 + "<br/>" + currAdd.City + "," + currAdd.State + "</span></div>";
        $('#confirmationAddressContainer').append(addString);
    }



    // Create google map and display
    this.displayTripMap(allAddressInfo);
};

/**
 * Puts selected address into address field
 */
ScheduleRide.prototype.replaceAddress = function(fieldBlock, addressArr) {
    var addInfo = JSON.parse(addressArr);
    address = addInfo.formatted_address;
    $(fieldBlock).find('.address').val(address);
};

/**
 * Forms the address parts used for sending to the server based on return from google api
 */
ScheduleRide.prototype.getAddressParts = function (addressData) {
    var addObj = new Object();
    var add1 = "";
    var add2 = "";
    for (var a = 0; a < addressData.length; a++) {
        var addType = addressData[a].types;
        switch (addType[0]) {
            case 'street_number':
                add1 = addressData[a].long_name;
                break;
            case 'route':
                add2 = addressData[a].long_name;
                break;
            case 'locality':
                addObj.city = addressData[a].long_name;
                break;
            case 'administrative_area_level_1':
                addObj.state = addressData[a].short_name;
                break;
            case 'postal_code':
                addObj.state = addressData[a].long_name;
                break;
        }
    }

    addObj.address = String(add1 + " " + add2);
    return addObj;
};
/**
 * Format JSON for server calls
 */
ScheduleRide.prototype.createJSON = function (send) {
    var self = this;

    // record trip info
    var raw, parsed = null;
    var tripData = new Trip();
    tripData.Notes = "testing notes";
    tripData.NumPassengers = $(".numPassengers").val();

    var pickupDate = $('input[name="pickupDate"]').val();
    var pickupTime = $('input[name="pickupTime"]').val();
    if (pickupDate && pickupTime) {
        // Convert to UTC
        parsed = Date.parse(pickupDate + ' ' + pickupTime);
        tripData.PickupTime = new Date(parsed.getUTCFullYear(), parsed.getUTCMonth(), parsed.getUTCDate(), parsed.getUTCHours(), parsed.getUTCMinutes(), parsed.getUTCSeconds());
    }

    var preferredVehicleCategory = $('#vehicleType').val();
    if (preferredVehicleCategory) {
        tripData.PreferredVehicleCategory = preferredVehicleCategory;
    }

    var bidWindowDate = $('input[name="bidWindowDate"]').val();
    var bidWindowTime = $('input[name="bidWindowTime"]').val();
    if (bidWindowDate && bidWindowTime) {
        // Convert to UTC
        parsed = Date.parse(bidWindowDate + ' ' + bidWindowTime);
        tripData.ValidTill = new Date(parsed.getUTCFullYear(), parsed.getUTCMonth(), parsed.getUTCDate(), parsed.getUTCHours(), parsed.getUTCMinutes(), parsed.getUTCSeconds());
    }

    tripData.IsImmediate = self.IsImmediate;
    if (tripData.IsImmediate) {
        tripData.PickupTime = null;
    }

    tripData.TripType = "Mileage";
    tripData.TotalTripSeconds = 0;
    tripData.TotalTripMeters = 0;

    // record trip addresses
    var pickupData = $("#pickup .destination").data("address");

    if (pickupData && pickupData.length > 0) {
        var pickupObj = self.getAddressParts(pickupData[0].address_components);

        //get pickup location
        var latlng = [pickupData[0].geometry.location.lat(), pickupData[0].geometry.location.lng()];
        var location = String(latlng);

        var address = new TripAddress();
        address.Line1 = pickupObj.address;
        address.Line2 = "";
        address.City = pickupObj.city;
        address.State = pickupObj.state;
        address.WaitDuration = 0; // No duration for starting point
        address.Zip = pickupObj.zip;
        address.Location = location;
        address.AddressType = "Pickup";
        address.TotalMeters = 0;
        address.TotalSeconds = 0;
        address.Note = $('#pickup .notes').val();

        tripData.TripAddresses.push(address);

        //used to calculate estimate distance and duration
        var prevLatLng = location;

        //add destinations
        var dLength = $("#addresses .destination").length;

        CFC.Log("Addresses found: " + dLength);

        $("#addresses .destination .destinationInner").each(function () {

            var destData = $(this).parent().data("address");
            if (destData) {
                var destObj = self.getAddressParts(destData[0].address_components);
                latlng = [destData[0].geometry.location.lat(), destData[0].geometry.location.lng()];
                location = String(latlng);

                var duration = parseFloat($(this).find(".duration").val());
                if (isNaN(duration)) {
                    duration = 0;
                }

                address = new TripAddress();
                address.Line1 = destObj.address;
                address.Line2 = "";
                address.City = destObj.city;
                address.State = destObj.state;
                address.WaitDuration = duration;
                address.Zip = destObj.zip;
                address.Location = location;
                address.AddressType = (tripData.TripAddresses.length == dLength ? "DropOff" : "Waypoint");
                address.TotalMeters = 0;
                address.TotalSeconds = 0;
                address.Note = $(this).find(".notes").val();

                tripData.TripAddresses.push(address);

                // If this is the final destination
                if (tripData.TripAddresses.length == dLength + 1) {
                    self.Trip = tripData;
                    self.formTripInfo();
                }
                ;

                prevLatLng = location;
            }
        });
    }

    //send JSON to database
    $("#rideFormWrapper").data("tripInfo", tripData);

    if (send == "") {
        self.sendJSON();
    } else {
        return tripData;
    }

    return null;
};

/**
 * Get distance information for each trip destination/waypoint and sum up the totals
 */
ScheduleRide.prototype.formTripInfo = function () {
    var self = this;
    self.Trip.TotalTripMeters = 0;
    self.Trip.TotalTripSeconds = 0;
    CFC.Log(self.Trip);

    var tripLength = self.Trip.TripAddresses.length;
    var prevLocation = self.Trip.TripAddresses[0].Location;
    for (var a = 1; a < tripLength; a++) {
        var currLocation = self.Trip.TripAddresses[a].Location;
        self.getDistance(prevLocation, currLocation, a);
        prevLocation = currLocation;
    }
};

/**
 * Determine distance between two coordinates
 */
ScheduleRide.prototype.getDistance = function (latLng1, latLng2, addressIndex) {

    var self = this;
    var latLngArr1 = latLng1.split(',');
    var latLngArr2 = latLng2.split(',');
    var lat1 = latLngArr1[0];
    var lng1 = latLngArr1[1];
    var lat2 = latLngArr2[0];
    var lng2 = latLngArr2[1];
    var origin = new google.maps.LatLng(lat1, lng1);
    var destination = new google.maps.LatLng(lat2, lng2);

    var service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix(
        {
            origins: [origin],
            destinations: [destination],
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.IMPERIAL,
            avoidHighways: false,
            avoidTolls: false
        }, function (response, status) {
            var distance = response.rows[0].elements[0].distance.value;
            var duration = response.rows[0].elements[0].duration.value;
            self.Trip.TripAddresses[addressIndex].TotalMeters = distance;
            self.Trip.TripAddresses[addressIndex].TotalSeconds = duration;
            self.Trip.TotalTripMeters += distance;
            self.Trip.TotalTripSeconds += duration;

            // All addresses accounted for, ready to send to CFC API for storage
            if (addressIndex = self.Trip.TripAddresses.length) {
                self.IsComplete = true;
                $('#record').show();
                $('#next').hide();
            }
        });
};

/**
 * Send JSON to the server
 */
ScheduleRide.prototype.sendJSON = function () {
    var self = this;
    var tripInfo = $("#rideFormWrapper").data('tripInfo');

    CFC.SendApiRequest(CFC.Url('Trip/Create'), tripInfo, 'POST', function (response) {
        window.location = CFC.Url('/Trip/View/' + response.TripId);
    });

};