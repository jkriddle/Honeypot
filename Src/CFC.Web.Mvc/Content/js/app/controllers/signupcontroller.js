var SignupController = Controller.extend({
    
    /**
     * Company signup panel
     */
    Company: function() {
        var controller = this;

        var panel = '#companyPanel';

        // If company DBA is not set, use legal name by default
        var companyName = $('input[name="CompanyLegalName"]');
        companyName.blur(function() {
            var dbaName = $('input[name="CompanyDba"]');
            if (dbaName.val() == '') {
                dbaName.val(companyName.val());
            }
        });

        // Numerical masks
        $(".numeric").blur(function() {
            var v = $(this).val();
            v = v.replace( /[^0-9]/g , '');
            $(this).val(v);
        });


        // Submit signup information
        $(panel).submit(function(e) {
            e.preventDefault();
            var params = $(panel).serialize();

            $.post('/Signup/Company', params, function(data) {
                var vm = new ResponseModel(data);
                if (vm.Success) {
                    window.location = CFC.Url("/page/view/signup-complete");
                    return;
                }
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');

        });

    }
});