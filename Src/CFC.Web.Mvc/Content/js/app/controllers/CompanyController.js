var CompanyController = Controller.extend({

    /**
    * Edit company information
    */
    Edit: function () {
        var controller = this;
        var panel = '#editPanel';

        // Submit login information
        $('#editPanel').submit(function (e) {
            e.preventDefault();
            var params = $(this).serialize();
            $.post(CFC.Url('Company/Edit'), params, function (data) {
                var vm = new ResponseModel(data);
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');
        });

    },

    /**
    * Edit company settings
    */
    Settings: function () {
        var controller = this;
        var panel = '#settingsPanel';

        // Submit login information
        $('#settingsPanel').submit(function (e) {
            e.preventDefault();
            var params = $(this).serialize();
            $.post(CFC.Url('Company/Settings'), params, function (data) {
                var vm = new ResponseModel(data);
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');
        });

    },

    /**
    * Display list of companies
    */
    List: function () {
        var controller = this;

        var action = {
            Panel: '#companyList',
            SearchPanel: '#companySearch',
            SearchTerm: '#companyTerm',
            CurrentPage: 1,
            HasMorePages: true
        };

        // Load company data
        action.LoadCompanies = function (callback) {

            $('.more', action.Panel).fadeIn();
            var searchQuery = $(action.SearchTerm).val();

            var model = new CompanyModel();
            model.GetCompanies({
                CurrentPage: action.CurrentPage,
                SearchQuery: searchQuery
            }, function (response) {


                var companyListTemplate = $('#companyListTemplate').html();
                var html = Mustache.to_html(companyListTemplate, response);

                // Fade in new elements
                $(action.Panel).append(html);

                action.HasMorePages = response.HasMorePages;

                $('.more', action.Panel).fadeOut();
                if (callback != undefined) callback();
                $(".grid .item:odd").addClass("odd");
                $(".grid .item:not(.odd)").addClass("even");

            });

        };

        // Clear search term
        $(action.SearchTerm).focus(function () {
            $(this).val('');
        });

        // Search with new terms
        $('.searchSubmit', '#companySearch').click(function (e) {
            e.preventDefault();
            action.CurrentPage = 1;
            $(action.Panel).children(':not(.more)').remove();
            action.LoadCompanies();
        });

        // Initial template load and existing companies
        action.LoadCompanies(function () {
            $('.more', action.Panel).fadeOut();
        });

        // Automatically add more when reaching bottom of page
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadCompanies();
            }
        });

    },

    /**
    * Company acceptance of site and financial terms
    */
    AcceptTerms: function () {
        var controller = this;
        var panel = '#acceptPanel';

        var rejected = false;

        // Reject company
        $('button[name="Reject"]').click(function () {
            // User clicked reject button
            rejected = true;
        });

        // Approve company
        $('button[name="Accept"]').click(function () {
            rejected = false;
        });

        // Submit login information
        $(panel).submit(function (e) {
            e.preventDefault();
            var params = $(panel).serialize();
            this.params += '&rejected=' + rejected;
            $.post(CFC.Url('Company/AcceptTerms'), params, function (data) {
                var vm = new ResponseModel(data);
                if (vm.Success) {
                    if (rejected) window.location = CFC.Url("/Company/TermsRejected");
                    else window.location = CFC.Url("/Company/TermsAccepted");
                    return;
                }
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');

        });

    },

    /**
    * Admin approval of company
    */
    Approve: function () {

        var controller = this;
        var panel = '#approvePanel';
        var rejected = false;

        // Reject company
        $('button[name="Reject"]').click(function () {
            // User clicked reject button
            rejected = true;
        });

        // Approve company
        $('button[name="Approve"]').click(function () {
            rejected = false;
        });

        // Update percent vs fixed fees
        $('#FeeType').change(function () {
            var val = $(this).val();
            if (val == 'Flat') $('#feeDecorator').text('USD');
            else $('#feeDecorator').text('%');
        });

        // Submit login information
        $(panel).submit(function (e) {
            e.preventDefault();
            var params = $(panel).serialize();
            params += '&rejected=' + rejected;

            // Submit to server
            $.post(CFC.Url('Company/Approve'), params, function (data) {
                var vm = new ResponseModel(data);
                if (vm.Success) {
                    CFC.ShowMessage(panel, vm.Message, vm.Errors, CFC.MessageType.Success);
                    $('.actions', panel).remove();
                    return;
                }
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');

        });

    }

});