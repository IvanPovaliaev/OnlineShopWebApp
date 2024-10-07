﻿/*Регистрация аттрибута PasswordValidation для jQuery*/
$.validator.addMethod("passwordvalidation", function (value, element) {
    var pattern = $(element).data("val-passwordvalidation-pattern");
    var regex = new RegExp(pattern);
    return this.optional(element) || regex.test(value);
});

$.validator.unobtrusive.adapters.add("passwordvalidation", [], function (options) {
    options.rules["passwordvalidation"] = true;
    options.messages["passwordvalidation"] = options.message;
});