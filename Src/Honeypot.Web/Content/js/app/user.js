﻿//----------------------------------------
// User
//----------------------------------------
var User = Observable.extend({


    MESSAGE_FORGOTPASSWORD: 'forgot-password',
    MESSAGE_RESETPASSWORD : 'reset-password',
    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_DELETED: 'deleted',
    MESSAGE_LIST: 'list',
    MESSAGE_SIGNIN: 'signin',
    MESSAGE_SIGNUP: 'signup',
    MESSAGE_UPDATEPROFILE: 'update-profile',
    
    init: function(){
        this._super();
    },

    // Public
    
    // Delete user
    delete : function(id) {
        var self = this;
        Honeypot.Api.post('/Api/User/Delete/' + id, null, function(resp) {
            self.notify(self.MESSAGE_DELETED, id);
        });
    },

    // Retrieve user
    getOne: function (id) {
        var self = this;
        Honeypot.Api.get("/Api/User/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve user list
    get: function (opt) {
        var self = this;
        Honeypot.Api.get("/Api/User/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    },

    // Send forgot password request
    forgotPassword: function (email) {
        var self = this;
        Honeypot.Api.post('/Api/User/ForgotPassword', {
            email: email
        }, function (resp) {
            self.notify(self.MESSAGE_FORGOTPASSWORD, resp);
        });
    },

    // Reset password
    resetPassword: function (opt) {
        var self = this;
        Honeypot.Api.post('/Api/User/ResetPassword', opt, function (resp) {
            self.notify(self.MESSAGE_RESETPASSWORD, resp);
        });
    },

    // Log user into system
    signIn: function (email, password, rememberMe) {
       var self = this;
       Honeypot.Api.post('/Api/User/SignIn', {
            email: email,
            password: password,
            rememberMe: rememberMe
        }, function (resp) {
            if (resp.Success) {
                Honeypot.Api.Auth.setToken(resp.Token, Date.parse(resp.Expires));
            }
            self.notify(self.MESSAGE_SIGNIN, resp);
        });
    },

    // Sign up new user
    signUp: function (data) {
        var self = this;
        Honeypot.Api.post('/Api/User/SignUp', data, function (resp) {
            self.notify(self.MESSAGE_SIGNUP, resp);
        });
    },

    // Update user data
    update: function (data) {
        var self = this;
        Honeypot.Api.post('/Api/User/Update', data, function (resp) {
            self.notify(self.MESSAGE_UPDATEPROFILE, resp);
        });
    }
});
    