// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', '[data-bs-target="#loginModal"]', function () {
    $('#loginModal').appendTo('body');
});

$(document).on('click', '[data-bs-target="#registrationModal"]', function () {
    $('#registrationModal').appendTo('body');
});

$(document).on('click', '[data-bs-target="#changePasswordModal"]', function () {
    $('#changePasswordModal').appendTo('body');
});

$(document).ready(function () {
    $(document).on('submit', '#registrationForm', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                if (response.redirectUrl) {

                    showRegisterSuccessToast();

                    setTimeout(function () {
                        window.location.href = response.redirectUrl;
                    }, 3000);  // Задержка в 3 секунды перед переходом
                }
                else {                    
                    $('#registrationFormWrapper').html(response);
                    $.validator.unobtrusive.parse($("#registrationForm"));
                }
            }
        });
    });

    function showRegisterSuccessToast() {
        var toastElement = new bootstrap.Toast(document.getElementById('registerSuccessToast'));
        toastElement.show();
    }
});

$(document).ready(function () {
    $(document).on('submit', '#loginForm', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                if (response.redirectUrl) {
                                        
                    showLoginSuccessToast();

                    setTimeout(function () {
                        window.location.href = response.redirectUrl;
                    }, 3000);  // Задержка в 3 секунды перед переходом
                }
                else {
                    $('#loginFormWrapper').html(response);
                    $.validator.unobtrusive.parse($("#loginForm"));
                }
            }
        });
    });

    function showLoginSuccessToast() {
        var toastElement = new bootstrap.Toast(document.getElementById('loginSuccessToast'));
        toastElement.show();
    }
});

$(document).ready(function () {
    $(document).on('submit', '#addRoleForm', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
                else {
                    $('#addRoleFormWrapper').html(response);
                    $.validator.unobtrusive.parse($("#addRoleForm"));
                }
            }
        });
    });
});

$(document).ready(function () {
    $(document).on('submit', '#changePasswordForm', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
                else {
                    $('#changePasswordFormWrapper').html(response);
                    $.validator.unobtrusive.parse($("#changePasswordForm"));
                }
            }
        });
    });
});

//Скрипт для работы с модальным окном удаления
$(document).ready(function () {
    $('#deleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);

        //Получение данных
        var message = button.data('message');
        var url = button.data('url');

        //Установка данных
        var modal = $(this);
        modal.find('.modal-body #message').html(message);
        modal.find('.modal-footer #confirmDeleteBtn').attr('href', url);
    });
});