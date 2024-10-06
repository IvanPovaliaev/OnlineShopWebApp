/*Регистрация аттрибута PasswordValidation для jQuery*/
$.validator.addMethod("passwordvalidation", function (value, element) {
    var pattern = $(element).data("val-passwordvalidation-pattern");
    var regex = new RegExp(pattern);
    return this.optional(element) || regex.test(value);
});

$.validator.unobtrusive.adapters.add("passwordvalidation", [], function (options) {
    options.rules["passwordvalidation"] = true;
    options.messages["passwordvalidation"] = options.message;
});

/*Регистрация аттрибута NotCompare для jQuery*/
$.validator.addMethod("notcompare", function (value, element) {
    var otherValue = $(element).closest('form').find('input[name="' + $(element).data('val-notcompare-other') + '"]').val();
    return this.optional(element) || value !== otherValue;
});

$.validator.unobtrusive.adapters.add("notcompare", ["other"], function (options) {
    options.rules["notcompare"] = true;
    options.messages["notcompare"] = options.message;
});