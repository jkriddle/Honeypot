var UserModel = Model.extend({

    initialize: function (data) {
        if (data != undefined) {
            // Map Data
            this.AutoMap(data);
        }
    },
    
    /**
    * Load single user from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of user models and original server response
    */
    GetUser: function (id, callback) {
        $.ajax({
            url: CFC.Url('/User/GetOne/' + id),
            dataType: 'json',
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    },

    /**
    * Load users from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of user models and original server response
    */
    GetUsers: function (options, callback) {
        $.ajax({
            url: CFC.Url('/User/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    }


});