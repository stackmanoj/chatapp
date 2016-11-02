function validateForm(form) {
    $(form).bootstrapValidator({
        container: function ($field, validator) {
            //return $field.parent().next('.messageContainer');
            return $('body').find('label[data-required-message-for="' + $field.attr('name') + '"]');
        },
        feedbackIcons: {
            valid: 'fa fa-check',
            invalid: 'fa fa-close',
            validating: 'fa fa-refresh'
        },
    });
}