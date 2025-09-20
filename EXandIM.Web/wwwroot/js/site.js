var table;
var datatable;
var updatedRow;
var exportedCols = [];




function showSuccessMessage(message = 'اكتمل الحفظ بنجاح') {
    Swal.fire({
        icon: 'success',
        title: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}

function showErrorMessage(message = 'حدث خطأ') {
    Swal.fire({
        icon: 'error',
        title: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}
function showCustomErrorMessage(xhr) {
    // Try to get error message from response
    let errorMessage = xhr.responseText;
    try {
        const response = JSON.parse(xhr.responseText);
        if (response.errorMessage) {
            errorMessage = response.errorMessage;
        }
    } catch (e) {
        // If not JSON, use the response text as is
    }

    Swal.fire({
        icon: 'error',
        title: errorMessage,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}
function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalSuccess(item) {
    debugger
    showSuccessMessage();
    $('#Modal').modal('hide');

    if (updatedRow === undefined) {
        $('tbody').append(item);
    } else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined;
    }
    location.reload();
    KTMenu.init();
    KTMenu.initHandlers();
}
function onModalRefrenceNumSuccess(res) {
    debugger
    showSuccessMessage();
    $('#Modal').modal('hide');

    if ($('tbody').length > 0) {
        if (updatedRow === undefined) {
            $('tbody').append(res);
        } else {
            $(updatedRow).replaceWith(res);
            updatedRow = undefined;
        }
        location.reload();
    } else {
        if (res.id !== undefined && res.name !== undefined) {
            var option = new Option(res.name, res.id, true, true);
            $("#ReferenceNumberId").append(option).trigger('change');
        }
    }

    // إعادة تهيئة المينيو
    KTMenu.init();
    KTMenu.initHandlers();
}

function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

//DataTables
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables', {
                scrollX: true,
            });
            if (!table) {
                return;
            }

            initDatatable();
            handleSearchDatatable();
        }
    };
}();

//Select2
function applySelect2() {
    $('.js-select2').select2();
    $('.js-select2').on('select2:select', function (e) {
        $('form').not('#SignOut').validate().element('#' + $(this).attr('id'));
    });
}

$(document).ready(function () {
    //SweetAlert
    var message = $('#Message').text();
    if (message !== '') {
        showSuccessMessage(message);
    }
    // Count of UnAccepted
    $.ajax({
        url: '/ImportBook/GetImportUnAcceptedCount',
        type: 'GET',
        success: function (data) {
            $('.importUnAcceptedCount').text(data.count);
        },
        error: function (error) {
            console.log('Error:', error);
        }
    });

    //Select2
    applySelect2();

    //Datepicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        autoApply: true,
        drops: 'auto',
        maxDate: false,
    });
    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });
    $('body').delegate('.js-render-modal', 'click', function () {
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                modal.find('.modal-body').html(form);
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                showErrorMessage();
            }
        });

        // استخدام Bootstrap 5 modal
        var bootstrapModal = new bootstrap.Modal(modal[0]);
        bootstrapModal.show();
    });

    //Hanlde signout
    $('.js-signout').on('click', function () {
        $('#SignOut').submit();
    });
    //Handle Toggle Status
    $('body').delegate('.js-delete-user', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من حذف المستخدم؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            showSuccessMessage();
                            location.reload();
                        },
                        error: function () {
                            showErrorMessage("لا يمكن حذف هذا المستخدم");
                        }
                    });
                }
            }
        });
    });

    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من تغير حاله المستخدم؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'محظور' ? 'متاح' : 'محظور';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash');

                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
    $('body').delegate('.js-toggle-Hidden', 'click', function () {
        var btn = $(this);

        bootbox.confirm({
            message: "هل متأكد من التغير؟",
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            const isHidden = btn.text().trim() === "🔒";
                            btn.html(isHidden ? "🔓" : "🔒");
                            showSuccessMessage();
                        },
                        error: function () {
                            showErrorMessage();
                        }
                    });
                }
            }
        });
    });
    //Handle Confirm
    $('body').delegate('.js-confirm', 'click', function () {
        var btn = $(this);
        var errormassege = btn.data('errormessage');
        bootbox.confirm({
            message: btn.data('message'),
            buttons: {
                confirm: {
                    label: 'نعم',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'لا',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function () {
                            location.reload();
                        },
                        error: function () {
                            showErrorMessage(errormassege);
                        }
                    });
                }
            }
        });

    });

    //Handle open folder
    $('body').delegate('.js-open-folder', 'click', function () {
        var btn = $(this);
        var errormassege = btn.data('errormessage');
        $.ajax({
            url: btn.data('url'),
            type: 'GET',
            error: function () {
                showErrorMessage(errormassege);
            }
        });

    });

});

