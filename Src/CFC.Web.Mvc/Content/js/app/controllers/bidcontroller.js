var BidController = Controller.extend({

    BidList: new DataList(),

    /**
    * Constructor
    */
    initialize: function initialize() {
        var controller = this;

        $('.acceptBid').live('click', function (e) {

            e.preventDefault();

            var bidId = this.hash.slice(1);

            CFC.SendApiRequest('/Bid/GetOne/' + bidId, null, 'GET', function (bid) {

                // Show dialog
                var template = $('#acceptBidTemplate').html();

                CFC.DialogInstance.Open({
                    title: 'Accept Bid',
                    width: 660,
                    content: template,
                    viewData: bid
                }, function () {

                    $("#acceptBidPanel").submit(function (e) {
                        e.preventDefault();
                        var params = CFC.DialogInstance.Serialize();
                        CFC.DialogInstance.ShowLoader();
                        //accept bid 
                        $.post(CFC.Url('Bid/Accept'), params, function (data) {
                            var vm = new ResponseModel(data);
                            if (vm.Success) {
                                // Close dialog and show success message in main area
                                CFC.ShowMessage('.contentWrapper', vm.Message, vm.Errors, CFC.MessageType.Success);

                                // Hide accept buttons
                                $('.acceptBid').fadeOut();

                                // Update the style of this bid's price to reflect a winning style
                                $('.bidAmount').removeClass('outbid winning');
                                $('.bidAmount', '#bid-' + bidId).addClass('winning');

                                CFC.DialogInstance.Close();
                                return;
                            }

                            // Show errors
                            CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                        }, 'json');
                    });

                });
            });
        });

        $('.driver').die();
        $('.driver').live('click', function (e) {
            e.preventDefault();

            CFC.DialogInstance.ShowLoader();

            var id = this.hash.slice(1);

            var driverTemplate = $('#userProfileTemplate').html();
            var userModel = new UserModel();
            userModel.GetUser(id, function (driver) {
                CFC.DialogInstance.Open({
                    title: 'Driver Profile',
                    width: 440,
                    content: driverTemplate,
                    showSubmit: false,
                    viewData: driver
                });
            });
        });


        $('.company').die();
        $('.company').live('click', function (e) {
            e.preventDefault();

            CFC.DialogInstance.ShowLoader();

            var id = this.hash.slice(1);

            var companyTemplate = $('#companyProfileTemplate').html();
            var companyModel = new CompanyModel();
            companyModel.GetCompany(id, function (company) {
                CFC.DialogInstance.Open({
                    title: 'Company Profile',
                    width: 440,
                    content: companyTemplate,
                    showSubmit: false,
                    viewData: company
                });
            });
        });
    },

    /**
    * Display incoming and historical bids in list panel
    */
    Incoming: function (options) {
        var controller = this;

        options.Incoming = true;

        this.List(options);
    },

    /**
    * Display list of trips
    */
    List: function (options) {
        var controller = this;

        var action = {
            Panel: '#bidList',
            SearchPanel: '#bidSearch',
            SearchTerm: '#bidTerm',
            CurrentPage: 1,
            HasMorePages: true,
            BidDialog: null,
            IsLoading: false,
            IntervalLoaded: false,
            TripId: null,
            Timeout: 3000,
            NumPerPage: 20,
            Incoming: true,
            ShowBidActions: true,
            ShowRiderActions: (CFC.CurrentUser.Role == 'Rider'),
            GetParams: function () {
                return {
                    CurrentPage: this.CurrentPage,
                    SearchQuery: $(this.SearchTerm).val(),
                    TripId: action.TripId,
                    Incoming: action.Incoming,
                    ShowBidActions: action.ShowBidActions,
                    ShowRiderActions: action.ShowRiderActions
                };
            }
        };

        if (options != undefined) {
            if (options.TripId != undefined) action.TripId = options.TripId;
            if (options.NumPerPage != undefined) action.NumPerPage = options.NumPerPage;
            if (options.Incoming != undefined) action.Incoming = options.Incoming;
            if (options.ShowBidActions != undefined) action.ShowBidActions = options.ShowBidActions;
        }

        var lastUpdated = CFC.GetUtc();

        // Retrieve bids from server
        action.LoadBids = function (callback) {
            $('.more', action.Panel).fadeIn();
            var searchQuery = $(action.SearchTerm).val();

            var model = new BidModel();
            model.GetBids(action.GetParams(), function (viewModel) {

                viewModel.ShowBidActions = action.ShowBidActions;
                viewModel.ShowRiderActions = action.ShowRiderActions;

                var bidListTemplate = $('#bidListTemplate').html();
                var html = Mustache.to_html(bidListTemplate, viewModel);

                // Fade in bids
                $(action.Panel).append(html);
                action.HasMorePages = viewModel.HasMorePages;

                $('.more', action.Panel).fadeOut();

                //if (!action.IntervalLoaded && action.Incoming) {

                // Check server for new bids
                setInterval(function () {
                    // Don't send a new request if still loading last one.
                    if (!action.IsLoading) {
                        action.IsLoading = true;

                        // Get any new bid info
                        var newParams = action.GetParams();
                        newParams.LastUpdated = lastUpdated.toString("MM/dd/yyyy HH:mm:ss");

                        model.GetBidUpdates(newParams, function (data) {
                            for (var i = 0; i < data.Bids.length; i++) {
                                var bid = data.Bids[i];

                                var tripEl = $('#bid-' + bid.Id);
                                var isNew = (tripEl.length == 0);

                                if (isNew) {

                                    // New bid
                                    console.log('New bid found: ' + bid.Id);
                                    var h = Mustache.to_html(bidListTemplate, { Bids: bid });

                                    // Fade in new elements
                                    var newEl = $('<div />').append(h);
                                    console.log(newEl);
                                    $(action.Panel).prepend(newEl);
                                    console.log('Appended to ' + action.Panel);
                                    CFC.Highlight(newEl.children('.item'));

                                } else {

                                    var amountEl = $('.bidAmount .center', tripEl);
                                    amountEl.html('$' + bid.BidAmount);

                                    // If bid was updated by someone else, bid has been underbid
                                    amountEl.removeClass('winning outbid');
                                    if (CFC.CurrentUser.CompanyId != bid.BiddingCompanyId) {
                                        amountEl.addClass('outbid');
                                    } else {
                                        amountEl.addClass('winning');
                                    }

                                    tripEl.find('.bid').remove();
                                }


                            }

                            // set the date of the last update to the one that the server received
                            // .NET date format in JSON response must be parsed
                            // @see http://stackoverflow.com/questions/726334/asp-net-mvc-jsonresult-date-format
                            lastUpdated = CFC.GetUtc(new Date(parseInt(data.DateQueried.substr(6))));
                            action.IsLoading = false;
                        });
                    }
                }, action.Timeout);

                action.IntervalLoaded = true;
                // }


                if (callback != undefined) callback();

            });

        };

        // Load existing bids
        action.LoadBids(function () {
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
            action.LoadBids();
        });

        // Autoload more bids when at bottom of page
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadBids();
            }
        });
    }

});