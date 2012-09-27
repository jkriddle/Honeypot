var UserController = Controller.extend({

    /**
    * Edit a user's profile
    */
    EditProfile: function () {
        var controller = this;
        var panel = '#editPanel';

        $('#editpass', panel).click(function (e) {
            e.preventDefault();
            $('#editpass').parent().hide();
            $('#editPasswordPanel').slideDown();
        });

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
                $('#editPanel #userPhoto').attr("src", CFC.Url("/Uploads/" + response.FileName));
                $('#editPanel input[name="userPhotoName"]').val(response.FileName);
            }
        });

        // Add button style
        $('.qq-upload-button').addClass('btn');

        // Submit login information
        $(panel).submit(function (e) {
            e.preventDefault();
            var params = $(panel).serialize();
            $.post(CFC.Url('User/EditProfile'), params, function (data) {
                var vm = new EditProfileModel(data);
                if (vm.ChangeEmail) {
                    $('#userEmail', '#utility').text(vm.Email);
                }
                CFC.ShowMessage(panel, vm.Message, vm.Errors);
            }, 'json');
        });
    },

    /**
    * List users in a panel and autoload on scroll
    */
    List: function () {
        var controller = this;
        var action = {
            LoadUrl: '/User/Get',
            Panel: '#userList',
            SearchPanel: '#userSearch',
            SearchTerm: '#userTerm',
            CurrentPage: 1,
            HasMorePages: true
        };

        action.LoadUsers = function (callback) {
            $('.more', action.Panel).fadeIn();
            var searchQuery = $(action.SearchTerm).val();

            var model = new UserModel();
            model.GetUsers({
                CurrentPage: action.CurrentPage,
                SearchQuery: searchQuery
            }, function (response) {

                var userListTemplate = $('#userListTemplate').html();
                var html = Mustache.to_html(userListTemplate, response);

                // Fade in new elements
                $(action.Panel).append(html);
                action.HasMorePages = response.HasMorePages;

                $('.more', action.Panel).fadeOut();
                if (callback != undefined) callback();
                $(".grid .item:odd").addClass("odd");
                $(".grid .item:not(.odd)").addClass("even");

            });
        };

        // Initial template load and existing users
        action.LoadUsers(function () {
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
            action.LoadUsers();
        });

        // Load new on window scroll
        // Load next page on page scroll
        $(window).infinitescroll(function () {
            if (action.HasMorePages) {
                action.CurrentPage++;
                action.LoadUsers();
            }
        });

        // Click to add a user
        $('#add').click(function (e) {
            e.preventDefault();

            var editUserTemplate = $('#editUserTemplate').html();

            CFC.DialogInstance.Open({
                title: 'Add a User',
                width: 660,
                content: editUserTemplate
            }, function () {

                // Submit add user form
                $('#editUserPanel').submit(function (e) {
                    e.preventDefault();
                    var params = $(this).serialize();
                    $.post(CFC.Url('User/Create'), params, function (data) {
                        var vm = new ResponseModel(data);
                        if (vm.Success) {

                            // For now just reload the page
                            window.location.reload();
                            
                            /*CFC.ShowMessage('#userSearch', vm.Message, vm.Errors, CFC.MessageType.Success);
                            CFC.DialogInstance.Close();
                            $(action.Panel).empty();
                            controller.List();*/

                            return;
                        }
                        CFC.DialogInstance.ShowMessage(vm.Message, vm.Errors);
                    }, 'json');
                });

            });
        });
    }

});