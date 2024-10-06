// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', '[data-bs-target="#loginModal"]', function () {
    $('#loginModal').appendTo('body');
});

$(document).on('click', '[data-bs-target="#registrationModal"]', function () {
    $('#registrationModal').appendTo('body');
});

$(document).ready(function () {
    $('#registrationForm').on('submit', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                $('#registrationFormWrapper').html(response);
                $.validator.unobtrusive.parse($("#registrationForm"));
            }
        });
    });
});
