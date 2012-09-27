var DataList = Class.define({

    Panel: null,
    Template: null,

    /**
    * Constructor
    */
    initialize: function (panel, template) {
        var self = this;
        self.Panel = panel;
        self.Template = template;
    },

    /**
    * Add data to end of a list using the specified template
    * @param   panel       string      jQuery specifier of panel to add to
    * @param   template    string      Mustache template data
    * @param   data        object      Data to use for mustache template
    */
    Append: function (data) {
        var self = this;
        
        var newPage = $(document.createElement('div'));
        var filledTemplate = Mustache.to_html(self.Template, data);
        $(newPage).append(filledTemplate);
        newPage.hide();
        newPage.appendTo(self.Panel);
        newPage.fadeIn();
    },

    /**
    * Add data to beginning of a list using the specified template
    * @param   panel       string      jQuery specifier of panel to add to
    * @param   template    string      Mustache template data
    * @param   data        object      Data to use for mustache template
    */
    Prepend: function (data) {
        var self = this;
        var newPage = $(document.createElement('div'));
        var filledTemplate = Mustache.to_html(self.Template, data);
        $(newPage).append(filledTemplate);
        newPage.hide();
        newPage.prependTo(self.Panel);
        newPage.fadeIn();
    }
});