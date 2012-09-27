var LoginController = Controller.extend({

    /**
     * Reset user password
     */
    Reset : function () {

        var panel = '#resetPanel';
        var controller = this;

        // Submit login information
        $(panel).submit(function (e) {
            e.preventDefault();
            var params =  CFC.DialogInstance.Serialize();
            $.post('/Login/Reset', params, function (data) {
                var vm = new ResponseModel(data);
                if (vm.Success) {
                     CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors, CFC.MessageType.Success);
                    // Redirect to homepage in 3 seconds
                    setTimeout(function () {
                        window.location = window.RootUrl;
                    }, 3000);
                    return;
                }
                CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
            }, 'json');

        });
    }

});