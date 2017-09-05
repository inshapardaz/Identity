/*
 *  Document   : base_pages_login.js
 *  Author     : pixelcave
 *  Description: Custom JS code used in Login Page
 */

var BasePagesLogin = function() {
    // Init Login Form Validation, for more examples you can check out https://github.com/jzaefferer/jquery-validation
    var initValidationLogin = function(){
        $.validator.addMethod("pwcheck",
            function (value, element) {
                return /^(?=(.*[0-9]))(?=.*[a-z])(?=(.*[A-Z]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;"'<>,./?])(?=(.*)).{6,100}$/.test(value);
            },
            "Passwords must have at least one non alphanumeric character, one lowercase ('a'-'z') and one uppercase ('A' - 'Z').");

        jQuery('.js-validation-login').validate({
            errorClass: 'help-block text-right animated fadeInDown',
            errorElement: 'div',
            errorPlacement: function(error, e) {
                jQuery(e).parents('.form-group > div').append(error);
            },
            highlight: function(e) {
                jQuery(e).closest('.form-group').removeClass('has-error').addClass('has-error');
                jQuery(e).closest('.help-block').remove();
            },
            success: function(e) {
                jQuery(e).closest('.form-group').removeClass('has-error');
                jQuery(e).closest('.help-block').remove();
            },
            rules: {
                'OldPassword': {
                    required: true
                },
                'NewPassword': {
                    required: true,
                    pwcheck: true,
                    minlength: 6,
                    maxlength: 100
                },
                'ConfirmPassword': {
                    required: true,
                    equalTo: '#NewPassword'
                }
            },
            messages: {
                'OldPassword': {
                    required: 'Old Password is required'
                },
                'Password': {
                    required: 'Please provide a password',
                    minlength: 'Your password must be at least 6 characters long'
                },
                'ConfirmPassword': {
                    required: 'Please provide a password',
                    minlength: 'Your password must be at least 5 characters long',
                    equalTo: 'Please enter the same password as above'
                }
            }
        });
    };

    return {
        init: function () {
            // Init Login Form Validation
            initValidationLogin();
        }
    };
}();

// Initialize when page loads
jQuery(function(){ BasePagesLogin.init(); });