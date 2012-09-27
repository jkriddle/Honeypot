/**
 * Trip Map
 * 
 * Uses Google API to place markers and info windows on a map.
 */
function Map(MapId, CenterLocation) {
    var self = this;

    // Google map object
    self.MapObject = null;
    
    // DOM element of Map
    self.MapId = MapId;

    // Currently open InfoWindow
    self.CurrentInfoWindow = null;

    // Location to center in and mark as "ME" on the map
    self.CenterLocation = CenterLocation;

    // Markers with info windows on map
    self.Markers = [];
};

/**
 * Add a marker and window to map
 * @param   position        string      GPS coordinates separate by comma
 * @param   id              int/string  Unique ID of this marker
 * @param   description     string      Text to show in info window popup
 * @param   isMe            bool        If this marker is the current user
 */
Map.prototype.AddMarker = function(position, id, description, isMe) {
    var self = this;

    if (isMe == undefined) isMe = false;
    
    self.Markers.push(new Marker(self.MapObject, position, id, description, isMe));
};
var mm = null;
var nn = null;
/**
 * Map Marker object
 * @param   map             Map         Map to add marker to
 * @param   position        string      GPS coordinates separate by comma
 * @param   id              int/string  Unique ID of this marker
 * @param   description     string      Text to show in info window popup
 * @param   isMe            bool        If this marker is the current user
 */
function Marker(map, position, id, description, isMe) {
    var self = this;

    self.Map = map;
    self.MarkerId = id;
    
    var opts = {
        position: position,
        map: self.Map,
        title: description
    };

    if (!isMe) opts.icon = CFC.Url('Content/img/google_icon.png');
    
    self.MarkerObject = new google.maps.Marker(opts);
    self.InfoWindow = new google.maps.InfoWindow({
        content: '<div class="InfoWindow">' + description + '</div>'
    });

    google.maps.event.addListener(self.MarkerObject, 'click', (function (marker) {
        return function () {
            if (self.Map.CurrentInfoWindow != null) {
                self.Map.CurrentInfoWindow.close();
                self.CurrentInfoWindow = null;
            }
            self.InfoWindow.open(self.Map, marker);
            self.Map.CurrentInfoWindow = self.InfoWindow;
        };
    })(self.MarkerObject));

    mm = self.MarkerObject;
    nn = self.InfoWindow;
};

/**
* Show a specific info window
*/
Map.prototype.OpenWindow = function (tripId) {
    var self = this;
    var foundIndex = -1;
    for (var i in self.Markers) {
        var marker = self.Markers[i];
        if (marker.MarkerId == tripId) {
            foundIndex = i;
        }
    }

    if (foundIndex >= 0) {
        var m = self.Markers[foundIndex];
        if (self.CurrentInfoWindow != null) {
            self.CurrentInfoWindow.close();
            self.CurrentInfoWindow = null;
        }
        m.InfoWindow.open(self.MapObject, m.MarkerObject);
        self.CurrentInfoWindow = m.InfoWindow;
    }
};

/**
* Close all windows
*/
Map.prototype.CloseCurrentWindow = function (tripId) {
    var self = this;
    if (self.CurrentInfoWindow != null) self.CurrentInfoWindow.close();
    self.CurrentInfoWindow = null;
};

/**
* Remove a marker from the map
* @param   id              int/string  Unique ID of this marker
*/
Map.prototype.RemoveMarker = function(id) {
    var self = this;
    var foundIndex = -1;
    for (var i in self.Markers) {
        var marker = self.Markers[i];
        if (marker.MarkerId == id) {
            foundIndex = i;
        }
    }

    if (foundIndex >= 0) {
        self.Markers[foundIndex].MarkerObject.setMap(null);
        self.Markers.splice(foundIndex, 1);
    }
};

/**
 * Update Google Map markers
 * @param   trips              array       Array of trips to update on map
 */
Map.prototype.Update = function (trips) {
    var self = this;

    // Only update if map panel exists
    var mapEl = $(self.MapId);
    if (mapEl.length == 0) return;

    if (self.MapObject == null) {

        // Initial map load
        var userCoords = self.CenterLocation.split(',');
        var userPos = new google.maps.LatLng(userCoords[0], userCoords[1]);
        var mapOptions = {
            center: userPos,
            zoom: 8,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        self.MapObject = new google.maps.Map($(self.MapId).get(0), mapOptions);

        // Place marker for current user's company location
        self.AddMarker(userPos, "ME", "Me", true);

        var currWidth = $("#mainContent").width() - 150;
        var currHeight = $("body").height();

        
        mapEl.width(currWidth + 100).height(currHeight + 100);

        // Refresh map
        google.maps.event.trigger(self.MapObject, "resize");
    }

    // Add trip markers
    for (var i in trips) {

        var trip = new TripModel(trips[i]);
        var coords = trip.TripAddresses[0].Location.split(',');
        position = new google.maps.LatLng(coords[0], coords[1]);

        var from = trip.TripAddresses[0];
        var to = trip.TripAddresses[trip.TripAddresses.length - 1];
        var desc = trip.User.FullName + '<br /><br /><strong>Pickup:</strong><br />' + from.Line1 + ' ' + from.City + ' ' + from.State + '<br /><br />'
                    + '<strong>Destination:</strong><br /> ' + to.Line1 + ' ' + to.City + ' ' + to.State;
        self.AddMarker(position, trip.Id, desc);
    }
};