var AlertModel = Model.extend({

    initialize: function (data) {
        if (data != undefined) {
            // Map Data
            this.AutoMap(data);
        }
    },

    /**
    * Load alerts from server.
    * @param       options     Object      Query options
    * @returns                 Array       List of alert models and original server response
    */
    GetAlerts: function (options, callback) {
        $.ajax({
            url: CFC.Url('/Alert/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            success: function (response) {
                CFC.Log(response);
                callback(response);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                CFC.Log(XMLHttpRequest);
                CFC.Log(textStatus);
                CFC.Log(errorThrown);
            }
        });
    },

    /**
    * Convert a list of raw JSON alerts into a list of AlertModel objects
    */
    MapAlerts: function (alerts) {

        var mapped = [];
        for (var i in alerts) {
            var alert = alerts[i];
            mapped.push(new AlertModel(alert));
        }
        return mapped;
    },

    GetAlertUpdates: function (options, callback) {
        var self = this;

        CFC.Log("op: " + options);
        $.ajax({
            url: CFC.Url('/Alert/Get'),
            dataType: 'json',
            data: options,
            traditional: true,
            cache: false,
            success: function (response) {
                
                response.Alerts = self.MapAlerts(response.Alerts);
                callback(response);
            },
            error: function (a, b, c) {
                CFC.Log(a);
                CFC.Log(b);
                CFC.Log(c);
            }
        });
    }



});