var AuthController = Controller.extend({

    loginDialog: null,
    signupDialog: null,
    forgotDialog: null,

    /**
    * Constructor
    */
    initialize: function () {
        var controller = this;

        // Click login link
        $('#login').click(function (e) {
            e.preventDefault();
            controller.Login();
        });

        // Click signup link
        $('#signup').click(function (e) {
            e.preventDefault();
            controller.Signup();
        });
    },

    /**
    * Display login panel
    */
    Login: function () {
        var controller = this;

        // Create/display dialog
        var loginTemplate = $('#loginTemplate').html();


        CFC.DialogInstance.Open({
            title : 'Login',
            width : 440,
            content : loginTemplate,
            submitText : 'Login'
        }, function() {
            $('#forgot').click(function (e) {
                e.preventDefault();
                controller.ForgotPassword();
            });

            // Submit login information
            $('form', '#dialog').submit(function (e) {
                e.preventDefault();

                // Collect form data
                var params = CFC.DialogInstance.Serialize();

                // Send to server
                $.post(CFC.Url('Login'), params, function (data) {
                    var vm = new ResponseModel(data);

                    if (vm.Success) {
                        // Reload current page to display authenticated information
                        window.location = CFC.Url('Dashboard');
                        return;
                    }

                    // Show errors
                    CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                }, 'json');
            });

        });

    },

    /**
    * Display forgotten password panel
    */
    ForgotPassword: function () {
        var controller = this;

        // Show dialog
        var forgotTemplate = $('#forgotTemplate').html();

        CFC.DialogInstance.Open({
            title : 'Forgot Password',
            width : 440,
            content : forgotTemplate,
            submitText : 'Send Reset Link'
        }, function() {
            // Back to login dialog
            $('#backToLogin').click(function (e) {
                e.preventDefault();
                controller.Login();
            });

            // Handle posted forgotten password info
            $('form', '#dialog').submit(function (e) {
                e.preventDefault();

                // Collect form data
                var params = CFC.DialogInstance.Serialize();

                // SEnd to server
                $.post(CFC.Url('Login/Forgot'), params, function (data) {
                    var vm = new ResponseModel(data);
                    if (vm.Success) {
                        // Success
                        CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors, CFC.MessageType.Success);
                        return;
                    }
                    // Show errors
                    CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                }, 'json');
            });

        });
    },

    /**
    * Show rider signup panel
    */
    Signup: function () {
        var controller = this;

        // Show dialog
        var signupTemplate = $('#signupTemplate').html();
        
        CFC.DialogInstance.Open({
            title : 'Sign Up',
            width : 660,
            content : signupTemplate,
            submitText : 'Sign Up'
        }, function() {

            // Handle posted user registration info
            $('form', '#dialog').submit(function (e) {

                e.preventDefault();

                // Collect form data
                var params = CFC.DialogInstance.Serialize();

                // Send to server
                $.post(CFC.Url('Signup'), params,
                function (data) {
                    var vm = new ResponseModel(data);
                    if (vm.Success) {
                        // Redirect user to homepage
                        window.location.hash = '';
                        window.location = CFC.Url('Dashboard');
                        return;
                    }

                    CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                }, 'json');
            });

        });
    }

});