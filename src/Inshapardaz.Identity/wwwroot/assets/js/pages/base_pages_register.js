/*
 *  Document   : base_pages_register.js
 *  Author     : pixelcave
 *  Description: Custom JS code used in Register Page
 */

var BasePagesRegister = function() {
    // Init Register Form Validation, for more examples you can check out https://github.com/jzaefferer/jquery-validation
    var initValidationRegister = function () {
        $.validator.addMethod("pwcheck",
            function (value, element) {
                return /^(?=(.*[0-9]))(?=.*[a-z])(?=(.*[A-Z]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;"'<>,./?])(?=(.*)).{6,100}$/.test(value);
            },
            "Passwords must have at least one non alphanumeric character, one lowercase ('a'-'z') and one uppercase ('A' - 'Z').");

        jQuery('.js-validation-register').validate({
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
                'Email': {
                    required: true,
                    email: true
                },
                'Password': {
                    required: true,
                    pwcheck : true,
                    minlength: 6,
                    maxlength: 100
                },
                'ConfirmPassword': {
                    required: true,
                    equalTo: '#Password'
                }
            },
            messages: {
                'Email': 'Please enter a valid email address',
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
            initValidationRegister();
        }
    };
}();

jQuery(function(){ BasePagesRegister.init(); });