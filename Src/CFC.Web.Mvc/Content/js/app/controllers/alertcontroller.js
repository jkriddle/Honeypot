var AlertController = Controller.extend({

    AlertList: new DataList(),

    /**
    * Constructor
    */
    initialize: function initialize(id) {
        var controller = this;
        if (id != undefined) controller.DomId = id;
    },

    /**
    * Display incoming and historical Alerts in list panel
    */
    Incoming: function (options) {
        var controller = this;
        options.Incoming = true;
        this.List(options);
    },

    /**
    * Display list of Alerts
    */
    List: function (options) {
        var controller = this;

        var action = {
            Panel: '#alertList',
            SearchPanel: '#alertSearch',
            SearchTerm: '#alertTerm',
            CurrentPage: 1,
            HasMorePages: true,
            IsLoading: false,
            IntervalLoaded: false,
            Timeout: 3000,
            NumPerPage: 20,
            GetParams: function () {
                return {
                    CurrentPage: this.CurrentPage,
                    SearchQuery: $(this.SearchTerm).val()
                };
            }
        };

        if (options != undefined) {
            if (options.NumPerPage != undefined) action.NumPerPage = options.NumPerPage;
            if (options.Incoming != undefined) action.Incoming = options.Incoming;
        }
        var listTemplate = CFC.CurrentUser.Role == 'Rider' ? 'alerts/rideralertlist.mustache' : 'alerts/alertlist.mustache';
        var lastUpdated = new Date();


        // Retrieve alerts from server
        action.LoadAlerts = function (callback) {

            var searchQuery = $(action.SearchTerm).val();

            var model = new AlertModel();
            model.GetAlerts(action.GetParams(), function (viewModel) {

                controller.View(listTemplate, viewModel, function (html) {
                    // Fade in alerts
                    $(action.Panel).append(html);
                    action.HasMorePages = viewModel.HasMorePages;
                    $('.more', action.Panel).fadeOut();

                    //if (!action.IntervalLoaded && action.Incoming) {

                    // Check server for new bids
                    setInterval(function () {
                        // Don't send a new request if still loading last one.
                        if (!action.IsLoading) {
                            action.IsLoading = true;

                            // Get any new alerts
                            var newParams = action.GetParams();
                            newParams.LastUpdated = CFC.FormatDate(lastUpdated);
                            var a = new AlertModel();

                            a.GetAlertUpdates(newParams, function (updateViewModel) {

                                for (var i = 0; i < updateViewModel.Alerts.length; i++) {
                                    var alert = updateViewModel.Alerts[i];
                                    var alertEl = $('#alert-' + alert.Id, controller.DomId);
                                    var isNew = (alertEl.length == 0);
                                    var alertVm = jQuery.extend(true, {}, updateViewModel);
                                    alertVm.Alerts = [alert];

                                    if (isNew) {
                                        // New Alert
                                        controller.View(listTemplate, alertVm, function (h) {
                                            // Fade in new elements
                                            
                                            var newEl = $('</div>').append(h);
                                            $(action.Panel).prepend(h);
                                        });
                                    } else {
                                        alertEl.remove();
                                        controller.View(listTemplate, alertVm, function (h) {
                                            // Fade in new elements
                                            $(controller.DomId).prepend(h);
                                        });
                                    }
                                }

                                lastUpdated = new Date();
                                action.IsLoading = false;
                            });
                        }
                    }, action.Timeout);


                    action.IntervalLoaded = true;
                    if (callback != undefined) callback();



                });
            });

        };




        // Load existing alerts
        action.LoadAlerts(function () {
            $('.more', action.Panel).fadeOut();
        });

        // Clear out search term when entering text box
        $(action.SearchTerm).focus(function () {
            $(this).val('');
        });

        // Submit new search terms
        $('.searchSubmit', action.SearchPanel).click(function (e) {
            e.preventDefault();
            action.CurrentPage = 1;
            $(action.Panel).children(':not(.more)').remove();
            action.LoadAlerts();
        });

        // Autoload more bids when at bottom of page
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadAlerts();
            }
        });


        $("a.alertPanelTab").live('click', function () {
            $(this).parent().toggleClass("active");

            if ($(this).parent().hasClass('active')) {
                $(this).parent().animate({
                    left: "+=200px"
                });
            } else {
                $(this).parent().animate({
                    left: "-=200px"
                });
            }

        });
    }

});