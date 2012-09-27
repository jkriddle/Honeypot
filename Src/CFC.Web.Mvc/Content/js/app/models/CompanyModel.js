var CompanyModel = Model.extend({

    initialize: function (data) {
        if (data != undefined) {
            // Map Data
            this.AutoMap(data);
        }
    },

    /**
    * Load single company from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of company models and original server response
    */
    GetCompany: function (id, callback) {
        $.ajax({
            url: CFC.Url('/Company/GetOne/' + id),
            dataType: 'json',
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    },

    /**
    * Load companies from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of company models and original server response
    */
    GetCompanies: function (options, callback) {
        $.ajax({
            url: CFC.Url('/Company/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            success: function (response) {
                callback(response);
            }
        });
    }


});