// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Скрипт для переноса модального окна входа в конец body (для корректной работы окна)
$(document).on('click', '[data-bs-target="#loginModal"]', function () {
    $('#loginModal').appendTo('body');
});

//Скрипт для переноса модального окна выхода в конец body (для корректной работы окна)
$(document).on('click', '[data-bs-target="#logoutModal"]', function () {
    $('#logoutModal').appendTo('body');
});

//Скрипт для переноса модального окна регистрации в конец body (для корректной работы окна)
$(document).on('click', '[data-bs-target="#registrationModal"]', function () {
    $('#registrationModal').appendTo('body');
});

//Скрипт для переноса модального окна смены пароля в конец body (для корректной работы окна)
$(document).on('click', '[data-bs-target="#changePasswordModal"]', function () {
    $('#changePasswordModal').appendTo('body');
});

//Скрипт для работы с модальным окном модальным окном регистрации пользователя
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

//Скрипт для работы с модальным окном модальным окном входа пользователя
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

//Скрипт для работы с модальным окном добавления роли
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

//Скрипт для работы с модальным окном смены пароля
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

//Скрипт для обновления иконок в шапке при добавлении в корзину, избранное и в сравнение. При статус коде 401 выводим уведомление
$(document).ready(function () {
    $(document).on('click', '.addToCart, .addToFavorites, .addToComparisons', function (event) {
        event.preventDefault();

        $.ajax({
            type: 'POST',
            url: $(this).attr('href'),

            success: function (response) {
                $('#navUserIcons').html(response);
            },
            error: function (response) {
                if (response.status === 401) {
                    
                    var toastElement = document.getElementById('unauthorizedToast');
                    var toast = new bootstrap.Toast(toastElement);
                    toast.show();
                }
            }
        });
    });
});