var VehicleController = Controller.extend({

    /**
    * List vehicles in a panel and autoload on scroll
    */
    List: function (options) {
        var controller = this;

        controller.EditVehicleViewModel = options.EditVehicleViewModel;

        var action = {
            LoadUrl: '/Vehicle/Get',
            Panel: '#vehicleList',
            SearchPanel: '#vehicleSearch',
            SearchTerm: '#vehicleTerm',
            CurrentPage: 1,
            HasMorePages: true
        };

        action.LoadVehicles = function (callback) {
            $('.more', action.Panel).fadeIn();
            var searchQuery = $(action.SearchTerm).val();

            var model = new VehicleModel();
            model.GetVehicles({
                CurrentPage: action.CurrentPage,
                SearchQuery: searchQuery
            }, function (response) {

                var vehicleListTemplate = $('#vehicleListTemplate').html();
                var html = Mustache.to_html(vehicleListTemplate, response);

                // Fade in new elements
                $(action.Panel).append(html);
                action.HasMorePages = response.HasMorePages;

                $('.more', action.Panel).fadeOut();
                if (callback != undefined) callback();
                $(".grid .item:odd").addClass("odd");
                $(".grid .item:not(.odd)").addClass("even");

            });
        };

        // Initial template load and existing vehicles
        action.LoadVehicles(function () {
            $('.more', action.Panel).fadeOut();
        });

        // Clear search term
        $(action.SearchTerm).focus(function () {
            $(this).val('');
        });

        // Submit new search
        $('.searchSubmit', action.SearchPanel).click(function (e) {
            e.preventDefault();
            action.CurrentPage = 1;
            $(action.Panel).children(':not(.more)').remove();
            action.LoadVehicles();
        });

        // Load new on window scroll
        // Load next page on page scroll
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadVehicles();
            }
        });

        $('.delete').live('click', function (e) {
            e.preventDefault();

            var id = this.hash.slice(1);
            var el = $(this);

            var model = new VehicleModel();
            if (confirm('Are you sure you want to delete this vehicle?')) {
                model.DeleteVehicle(id, function (resp) {
                    if (resp.Success) {
                        $(el).closest('.item').slideUp();
                    } else {
                        // Failed
                    }
                });
            }
        });

        action.DisplayVehicleForm = function (isNew, viewModel) {

            var title = isNew ? 'Add a Vehicle' : 'Edit Vehicle';

            var editVehicleTemplate = $('#editVehicleTemplate').html();

            CFC.DialogInstance.Open({
                title: title,
                width: 660,
                content: editVehicleTemplate,
                viewData: viewModel
            }, function () {

                // Select dropdown values
                if (!isNew) {
                    $('select[name="VehicleCategory"]').val(viewModel.Vehicle.VehicleCategory);
                    $('select[name="Year"]').val(viewModel.Vehicle.Year);
                }

                // Bind uploader
                var uploader = new qq.FileUploader({
                    element: document.getElementById('photo-upload'),
                    action: CFC.Url('Image/Upload'),
                    allowedExtensions: ['jpeg', 'jpg'],
                    acceptFiles: ['jpeg', 'jpg'],
                    uploadButtonText: 'Upload a Photo',
                    sizeLimit: 2 * 1024 * 1024, // 2MB
                    debug: true,
                    onError: function (id, fileName, response) {
                        CFC.Log('An error occurred while uploading the photo.');
                        CFC.Log(id);
                        CFC.Log(fileName);
                        CFC.Log(response);
                    },
                    onComplete: function (id, fileName, response) {
                        $('#editVehiclePanel #vehiclePhoto').attr("src", CFC.Url("/Uploads/" + response.FileName));
                        $('#editVehiclePanel input[name="vehiclePhotoName"]').val(response.FileName);
                    }
                });
                
                // Add button style
                $('.qq-upload-button').addClass('btn');

                // Submit add vehicle form
                $('#editVehiclePanel').submit(function (e) {
                    e.preventDefault();

                    var params = CFC.DialogInstance.Serialize();
                    var postUrl = isNew ? 'Vehicle/Create' : 'Vehicle/Edit/' + viewModel.VehicleId;

                    $.post(CFC.Url(postUrl), params, function (data) {

                        var vm = new ResponseModel(data);
                        if (vm.Success) {
                            // For now just reload the page. Intention is to just reload the list itself but the images are failing to load for
                            // some reason.
                            window.location.reload();
                            /*CFC.ShowMessage('#vehicleSearch', vm.Message, vm.Errors, CFC.MessageType.Success);
                            $(action.Panel).empty();
                            action.LoadVehicles();
                            action.CurrentPage = 1;
                            CFC.DialogInstance.Close();*/
                            return;
                        }
                        CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                    }, 'json');
                });
            });
        };

        action.SetupVehicleForm = function (id, isNew) {
            var viewModel = controller.EditVehicleViewModel;
            viewModel.VehicleId = id;

            if (!isNew) {
                CFC.SendApiRequest(CFC.Url('Vehicle/GetOne/' + viewModel.VehicleId), null, 'GET', function (vehicle) {
                    viewModel.Vehicle = vehicle;
                    action.DisplayVehicleForm(isNew, viewModel);
                });
            } else {
                viewModel.Vehicle = null;
                action.DisplayVehicleForm(isNew, viewModel);
            }
        };

        $('.edit', action.Panel).live('click', function (e) {
            e.preventDefault();
            var isNew = false;
            var id = this.hash.slice(1);
            action.SetupVehicleForm(id, isNew);
        });


        // Click to add a vehicle
        $('#add').click(function (e) {
            e.preventDefault();
            var isNew = true;
            action.SetupVehicleForm(null, isNew);
        });
    }

});