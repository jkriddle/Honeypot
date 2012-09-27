var TripController = Controller.extend({

    // Dom Element this controll will bind to
    DomId: '#tripList',

    // Interval used for counting down timer to trip expiration
    CountdownInterval: null,

    // Map used for dashboard
    Map: new Map('#map', CFC.CurrentUser.Location),

    /**
    * Constructor
    */
    initialize: function initialize(id) {
        var controller = this;
        if (id != undefined) controller.DomId = id;
    },

    InitCountdown: function (removeAtZero) {
        var self = this;
        if (removeAtZero == undefined) removeAtZero = true;
        self.CountdownInterval = setInterval(function () {
            self.UpdateCountdown(removeAtZero);
        }, 1000);
    },

    /**
    * Update trip timer countdown. Remove trip once timer hits 0.
    */
    UpdateCountdown: function (removeAtZero) {
        var self = this;
        if (removeAtZero == undefined) removeAtZero = true;
        $('.remainingSeconds', self.DomId).each(function () {
            var el = $(this);
            var seconds = $(el).val();
            if (seconds < 0) {
                var expiredText = 'Trip Expired';
                var html = $(el).siblings('.remaining');
                if (html != expiredText) $(el).siblings('.remaining').html(expiredText);
                return;
            }

            var dt = new Date(1970, 0, 1);
            dt.setSeconds(seconds);
            var stamp = CFC.Pad(dt.getHours(), 2) + ':' + CFC.Pad(dt.getMinutes(), 2) + ':' + CFC.Pad(dt.getSeconds(), 2);
            $(el).siblings('.remaining').html(stamp);

            var secondsLeft = (dt.getHours() * 60 * 60) + (dt.getMinutes() * 60) + dt.getSeconds() - 1;

            // If under 2 minutes left, start flashing the timer.
            var alertThreshold = 120;

            if (secondsLeft <= alertThreshold) {
                var topLeft = el.parent().parent();
                var isAnim = topLeft.data('animating');
                if (!isAnim) {
                    var highlightBg = "#FF8787";
                    var animateMs = 1000;
                    var originalBg = topLeft.css("background-color");
                    setInterval(function () {
                        topLeft.stop().css("background-color", highlightBg).animate({ backgroundColor: originalBg }, animateMs);
                    }, animateMs * 2);
                    topLeft.data('animating', true);
                }
            }

            // Trip has expired
            if (removeAtZero && secondsLeft <= 0) {
                $(el).closest('.item').fadeOut();
            }

            $(el).val(secondsLeft);
        });
    },

    /**
    * List historical trips and autoload on scroll
    */
    List: function (options) {
        var controller = this;

        var action = {
            Incoming: options.Incoming || false,
            Panel: controller.DomId,
            SearchPanel: '.searchBar',
            SearchTerm: '.searchField',
            CurrentPage: 1,
            HasMorePages: true,
            IsLoading: false,
            IntervalLoaded: false,
            Timeout: 3000,
            NumPerPage: options.NumPerPage || 20,
            TripFilterType: options.TripFilterType || 'AllIncoming',
            ShowTripActions: (options.ShowTripActions == undefined ? false : options.ShowTripActions),
            ShowBidActions: (options.ShowBidActions == undefined ? true : options.ShowBidActions),
            ShowNoShowActions: (options.ShowNoShowActions == undefined ? false : options.ShowNoShowActions),
            GetParams: function () {
                return {
                    CurrentPage: this.CurrentPage,
                    TripFilterType: this.TripFilterType,
                    SearchQuery: $(this.SearchTerm).val(),
                    NumPerPage: this.NumPerPage,
                    Radius: $('input[name="Radius"]').val(),
                    ShowTripActions: action.ShowTripActions,
                    ShowBidActions: action.ShowBidActions,
                    ShowNoShowActions: action.ShowNoShowActions
                };
            }
        };

        // Place Bid button
        $('.bid').die('click');
        $('.bid').live('click', function (e) {
            e.preventDefault();
            // Get trip id from anchor hash
            var tripId = this.hash.slice(1);
            var bidModel = new BidModel();
            bidModel.PlaceBid(tripId);
        });

        // Always stay 1 seconds behind. Otherwise a bid could be placed between the time of the request/response
        // latency and not appear until the screen is refreshed. This is definitely not a good method for doing this. Maybe
        // the server should return the last date/time the data was retrieve (after querying the DB) and send it back to the client?
        var lastUpdated = CFC.GetUtc();

        action.LoadTrips = function (callback) {
            $('.more', controller.DomId).fadeIn();
            var model = new TripModel();

            model.GetTrips(action.GetParams(), function (viewModel) {

                viewModel.ShowBidActions = action.ShowBidActions;
                viewModel.ShowTripActions = action.ShowTripActions;
                viewModel.ShowNoShowActions = action.ShowNoShowActions;

                controller.Map.Update(viewModel.Trips);

                var tripListTemplate = $('#tripListTemplate').html();
                var html = Mustache.to_html(tripListTemplate, viewModel);

                $(controller.DomId).append(html);

                action.HasMorePages = viewModel.HasMorePages;

                $('.more', controller.DomId).fadeOut();


                // Check server for new trips
                setInterval(function () {
                    // Don't send a new request if still loading last one.
                    if (!action.IsLoading) {

                        action.IsLoading = true;

                        // Get any new trips
                        var newParams = action.GetParams();
                        newParams.LastUpdated = lastUpdated.toString("MM/dd/yyyy HH:mm:ss");

                        CFC.Log('Request update time: ' + newParams.LastUpdated);
                        var t = new TripModel();
                        t.GetTripUpdates(newParams, function (updateViewModel) {

                            CFC.Log(updateViewModel.Trips.length + ' trips found.');

                            var tripVm = jQuery.extend(true, {}, updateViewModel);
                            tripVm.Trips = updateViewModel.Trips;
                            tripVm.ShowBidActions = action.ShowBidActions;
                            tripVm.ShowTripActions = action.ShowTripActions;
                            tripVm.ShowNoShowActions = action.ShowNoShowActions;

                            if (updateViewModel.Trips.length > 0) {
                                controller.Map.Update(updateViewModel.Trips);
                            }

                            var newCount = 0;
                            for (var i = 0; i < updateViewModel.Trips.length; i++) {

                                var trip = updateViewModel.Trips[i];
                                var tripEl = $('.trip-' + trip.Id, controller.DomId);
                                var isNew = (tripEl.length == 0);

                                var tripVm = updateViewModel;
                                tripVm.Trips = [trip];
                                tripVm.ShowBidActions = action.ShowBidActions;
                                tripVm.ShowTripActions = action.ShowTripActions;
                                tripVm.ShowNoShowActions = action.ShowNoShowActions;
                                if (isNew && trip.TripStatus == 'Bidding') {
                                    newCount++;
                                    CFC.Log('New trip found.');
                                    CFC.Log(trip);
                                    // If a new trips has been found, add it to the page
                                    var h = Mustache.to_html(tripListTemplate, tripVm);

                                    // Fade in new elements
                                    var newEl = $('<div />').append(h);
                                    $(controller.DomId).prepend(newEl);
                                    CFC.Highlight(newEl.children('.item'));

                                } else {
                                    CFC.Log('Updated trip found.');
                                    // Remove trips no longer in the bidding stage
                                    if (trip.TripStatus != CFC.TripStatus.Bidding) {
                                        var num = $('.trip-' + trip.Id, controller.DomId).length;

                                        $('.trip-' + trip.Id, controller.DomId).fadeOut(400, function () { $(this).remove(); });

                                        controller.Map.RemoveMarker(trip);
                                        CFC.Log('Removing trip ' + trip.Id + ' from ' + controller.DomId + '...' + num + ' elements found');
                                    } else {
                                        // If a trip has been updated, render the updates to the DOM
                                        tripEl.remove();
                                        CFC.Log('Updating trip bid.');

                                        // If a new trips has been found, add it to the page
                                        var h = Mustache.to_html(tripListTemplate, tripVm);

                                        // Fade in new elements
                                        var newEl = $('<div />').append(h);
                                        $(controller.DomId).prepend(newEl);
                                        //CFC.Highlight(newEl.children('.item'));
                                    }

                                }
                            }

                            if (newCount > 0) {
                                action.UpdateListClasses();
                            }

                            // set the date of the last update to the one that the server received
                            // .NET date format in JSON response must be parsed
                            // @see http://stackoverflow.com/questions/726334/asp-net-mvc-jsonresult-date-format
                            lastUpdated = CFC.GetUtc(new Date(parseInt(updateViewModel.DateQueried.substr(6))));
                            CFC.Log('Server update time: ' + lastUpdated.toString("MM/dd/yyyy hh:mm:ss"));
                            action.IsLoading = false;

                        });
                    }
                }, action.Timeout);

                action.IntervalLoaded = true;
                if (callback != undefined) callback();

                action.UpdateListClasses();
            });
        };

        action.UpdateListClasses = function () {
            $(".grid .item:odd").addClass("odd");
            $(".grid .item:not(.odd)").addClass("even");

            $(".grid .item, " + controller.DomId + " .item").each(function () {

                if ($(this).find(".bidAmount").text() == "no bids") {
                    $(this).find(".bidAmount").addClass('noBid');
                }

                if ($(this).data('immediate') == true) {
                    $(this).find('.iconImage').addClass('immediate');
                    $(this).find('li.pickupDateTime').html('<span>Pickup Date/Time:</span><span class="now">NOW</span>');
                }
            });
        };

        // Initial load
        action.LoadTrips(function () {
            $('.more', controller.DomId).fadeOut();
            $(".grid .item:odd").addClass("odd");
            $(".grid .item:not(.odd)").addClass("even");

        });

        // Choose current trip filter
        $('#tripFilterType').val(action.TripFilterType);

        // Clear search term on focus
        $(action.SearchTerm).focus(function () {
            $(this).val('');
        });

        // Update service area radius when changed
        $('#radius input').change(function () {
            $(controller.DomId).children().remove();
            action.Radius = $(this).val();
            action.LoadTrips(function () {
                $('.more', controller.DomId).fadeOut();
            });
        });

        // Submit new search query
        $('.searchSubmit', action.SearchPanel).click(function (e) {
            e.preventDefault();
            action.CurrentPage = 1;
            $(controller.DomId).children(':not(.more)').remove();
            action.LoadTrips();
        });

        // Submit new search query
        $('#tripFilterType').change(function (e) {
            e.preventDefault();
            action.CurrentPage = 1;
            action.TripFilterType = $(this).val();
            $(controller.DomId).children(':not(.more)').remove();
            action.LoadTrips();
        });

        // Load next page on page scroll
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadTrips();
            }
        });

        // Cancel trip
        $('.cancel').die('click');
        $('.cancel').live("click", function (e) {
            e.preventDefault();
            var el = $(this);
            var tripId = this.hash.slice(1);
            if (confirm('Are you sure you want to cancel this trip?')) {
                var model = new TripModel();
                model.CancelTrip(tripId, function () {
                    $(el).closest('.item').fadeOut();
                });
            }
        });

        // Mark Trip as NoShow 
        $('.noshow').die('click');
        $('.noshow').live("click", function (e) {
            e.preventDefault();
            var el = $(this);
            var tripId = this.hash.slice(1);
            if (confirm('Are you sure you want to do this?')) {
                var model = new TripModel();
                model.NoShow(tripId, function () {
                    $(el).closest('.item').fadeOut();
                });
            }
        });


        // Hover over trip or bid - show window on map 
        $('.item').die('mouseenter');
        $('.item').die('mouseleave');
        $('.item').live({
            mouseenter: function () {
                var tripId = $(this).data('trip');
                controller.Map.OpenWindow(tripId);
            },
            mouseleave: function () {
                controller.Map.CloseCurrentWindow();
            }
        });

        // Update times
        controller.InitCountdown();

    }
});