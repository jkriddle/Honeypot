/**
 * Base class for common functionality across all controllers
 */
var Controller = Class.define({

    // Deprecated - current action's template being used
    Template: null,

    // Cached template info
    Templates: [],

    /**
    * Loads a specified mustache template, and runs
    * the callback method after loading is complete.
    * @param   string    template      Path of template to load  
    * @param   function  callback      Function to run after load completes
    */
    LoadTemplate: function (template, callback) {
        var controller = this;

        // Initial load
        if (controller.Templates[template] == null) {
            $.get(CFC.View(template), function (html) {
                controller.Templates[template] = html;
                if (callback != undefined) callback(html);
            });
        } else {
            if (callback != undefined) callback(controller.Templates[template]);
        }
    },

    /**
    * Hydrate a mustache view with model data and return back populated
    * template for further processing
    */
    View: function (template, model, callback) {
        var controller = this;

        // Load template
        controller.LoadTemplate(template, function (html) {
            var filledTemplate = Mustache.to_html(html, model);
            callback(filledTemplate);
        });
    }

});