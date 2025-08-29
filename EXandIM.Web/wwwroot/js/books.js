$(document).ready(function () {
    var tbody = $('#Books').find('tbody');
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(this.value).draw();
    });

    let columns = [
        { "data": "id", "name": "Id", "className": "d-none" },
        {
            "name": "Title",
            "className": "text-end",
            "render": function (data, type, row) {
                return `<div class="d-flex flex-column">
                             <a href="/${tbody.data('controller')}/Details/${row.id}" class="text-primary fw-bolder mb-1">${row.title}</a>
                         </div>`;
            }
        },
        {
            "name": "BookDate",
            "className": "text-center",
            "render": function (data, type, row) {
                return moment(row.bookDate).format('ll')
            }
        },

    ];
    columns.push({
        "name": "Passed",
        "className": "text-center",
        "render": function (data, type, row) {
            const badgeClass = row.passed ? "badge-light-success" : "badge-light-danger";
            return `<span class="badge ${badgeClass}">${row.passed ? "منجزة" : "في االانتظار"}</span>`;
        }
    });
    columns.push({
        "data": "entities", "name": "Entities", "orderable": false,
        render: function (data, type, row) {
            if (Array.isArray(data)) {
                var list = '<ul>';
                data.forEach(function (item) {
                    list += '<li>' + item + '</li>';
                });
                list += '</ul>';
                return list;
            }
            return data;
        }
    });
    columns.push({
        "data": "subEntities", "name": "SubEntities", "orderable": false,
        render: function (data, type, row) {
            if (Array.isArray(data)) {
                var list = '<ul>';
                data.forEach(function (item) {
                    list += '<li>' + item + '</li>';
                });
                list += '</ul>';
                return list;
            }
            return data;
        }
    });
    columns.push({
        "data": "secondSubEntities", "name": "SecondSubEntities", "orderable": false,
        render: function (data, type, row) {
            if (Array.isArray(data)) {
                var list = '<ul>';
                data.forEach(function (item) {
                    list += '<li>' + item + '</li>';
                });
                list += '</ul>';
                return list;
            }
            return data;
        }
    });
    if (tbody.data('controller') !== "Readings") {
        columns.push({ "data": "sideEntity", "name": "SideEntity" });

        columns.push({ "data": "bookNumber", "name": "BookNumber" });
    }
   
    if (tbody.data('controller') !== "Readings") {
        columns.push({
            "data": "referToBookNumber",
            "name": "ReferToBookNumber",
            "render": function (data, type, row) {
                if (data != null) {
                    let dateParam = moment(row.referDate).format("YYYY-MM-DD");
                    return `<a href="javascript:;" class="js-find-book" data-number="${row.referToBookNumber}" data-date="${dateParam}">${data}</a>`;
                } else {
                    return "";
                }
            }
        });

        columns.push({
            "className": 'text-center',
            "data": "referDate",
            "name": "ReferDate",
            "render": function (data, type, row) {
                if (data != null) {
                    return moment(row.referDate).format('ll')
                }
                else
                    return "";
            }
        });
    }
    if (tbody.data('controller') === "ImportBook") {
        columns.push({ "data": "importNumber", "name": "ImportNumber" });
        columns.push({
            "className": 'text-center',
            "data": "importDate",
            "name": "ImportDate",
            "render": function (data, type, row) {
                if (data != null) {
                    debugger
                    return moment(row.importDate).format('ll')
                }
                else
                    return "";
            }
        });
    }
    columns.push({
        "className": 'text-center',
        "data": "notes",
        "name": "Notes",
        "render": function (data, type, row) {
            if (data != null) {
                if (data.length > 20) {
                    return data.substring(0, 20) + '...'; // Truncate the text and add ellipsis
                }
            }
            return data; // If less than 20 characters, show the full text
        }
    });
    columns.push({ "className": 'text-center', "data": "userFullName", "name": "UserFullName" });
    var isSuperAdmin = $('#isSuperAdmin').val() === 'true';
    if (isSuperAdmin) {
        columns.push({
            "data": "isHidden", "name": "IsHidden", "orderable": false,
            render: function (data, type, row) {
                const lockIcon = row.isHidden ? "🔒" : "🔓";

                return `
            <a href="javascript:;" 
               class="menu-link flex-stack px-3 js-toggle-Hidden" 
               data-url="/${tbody.data('controller')}/ToggleHidden/${row.id}" 
               data-message="هل متأكد من الاخفاء؟"
               style="font-size: 24px;">
               ${lockIcon}
            </a>`;
            }
        });
    }
    //if (tbody.data('controller') === "ExportBook" || tbody.data('controller') === "ImportBook") {
    //    columns.push({
    //        data: null,
    //        className: "js-no-export text-center",
    //        orderable: false,
    //        render: function (data, type, row, meta) {
    //            return `<a href="javascript:;" type="button" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary js-render-modal"
    //                    data-title="اضافه" data-url="/Activity/AddBookToActivity?bookId=${row.id}" data-update="true">
    //                إضافة
    //            </a>`;
    //        }
    //    });
    //}
    //else {
    //    columns.push({
    //        data: null,
    //        className: "js-no-export text-center",
    //        orderable: false,
    //        render: function (data, type, row, meta) {
    //            return `<a href="javascript:;" type="button" class="btn btn-sm btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary js-render-modal"
    //                    data-title="اضافه" data-url="/Activity/AddReadingToActivity?readingId=${row.id}" data-update="true">
    //                إضافة
    //            </a>`;
    //        }
    //    });
    //}

    columns.push(
        {
            "className": 'text-start',
            "orderable": false,
            "render": function (data, type, row) {
                return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                            اعدادات أخرى
                            <!--begin::Svg Icon | path: icons/duotune/arrows/arr072.svg-->
                            <span class="svg-icon svg-icon-5 m-0">
                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M11.4343 12.7344L7.25 8.55005C6.83579 8.13583 6.16421 8.13584 5.75 8.55005C5.33579 8.96426 5.33579 9.63583 5.75 10.05L11.2929 15.5929C11.6834 15.9835 12.3166 15.9835 12.7071 15.5929L18.25 10.05C18.6642 9.63584 18.6642 8.96426 18.25 8.55005C17.8358 8.13584 17.1642 8.13584 16.75 8.55005L12.5657 12.7344C12.2533 13.0468 11.7467 13.0468 11.4343 12.7344Z" fill="currentColor"></path>
                                </svg>
                            </span>
                            <!--end::Svg Icon-->
                        </a>
                                <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-200px py-3" data-kt-menu="true" style="">
                            <!--begin::Menu item-->
                            <div class="menu-item px-3">
                                <a href="/${tbody.data('controller')}/Edit/${row.id}" class="menu-link px-3">
                                    تعديل
                                </a>
                            </div>
                            <!--end::Menu item-->
                             <!--begin::Menu item-->
                            <div class="menu-item px-3">
                                <a href="/${tbody.data('controller')}/Details/${row.id}" class="menu-link px-3">
                                    التفاصيل
                                </a>
                            </div>
                            <!--end::Menu item-->
                              <!--begin::Menu item-->
                            <div class="menu-item px-3">
                                        <a href="javascript:;" class="menu-link flex-stack px-3 js-confirm" data-url="/${tbody.data('controller')}/Delete/${row.id}" data-message="هل متأكد من حذف هذا العنصر؟">
                                    حذف
                                </a>
                            </div>
                            <!--end::Menu item-->
                        </div>`;
            }
        });
    columns.unshift({
        data: null,
        orderable: false,
        className: "text-center",
        render: function (data, type, row) {
            return `<input type="checkbox" class="book-checkbox" style = "width: 20px;height: 20px;transform: scale(1.3);cursor: pointer;"
                 value="${row.id}" />`;
        }
    });


    // Handle title filter change event
    var titleFilter = $('#titleSearchId');
    titleFilter.on('change', function () {
        var selectedTitle = $(this).val();
        datatable.search(selectedTitle).draw();
    });

    datatable = $('#Books').DataTable({
        serverSide: true,
        processing: true,
        stateSave: false,
        language: {
            processing: '<div class="d-flex justify-content-center text-primary align-items-center dt-spinner"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div><span class="text-muted ps-2">انتظار...</span></div>'
        },
        ajax: {
            url: tbody.data('url'),
            type: 'POST'
        },
        'drawCallback': function () {
            KTMenu.createInstances();
        },
        order: [[1, 'asc']],
        columnDefs: [{
            targets: [1],
            visible: false,
            searchable: false
        }],
        columns: columns

    });
});

$(document).on("click", ".js-find-book", function () {
    let number = $(this).data("number");
    let date = $(this).data("date");

    $.get(`/ImportBook/FindByRefer?number=${number}&date=${date}`, function (res) {
        debugger
        if (res.redirectUrl) {
            window.open(res.redirectUrl, "_blank");
        } else {
            showErrorMessage("📌 الكتاب غير موجود");
        }
    });
});
function showErrorMessage(message = 'حدث خطأ') {
    Swal.fire({
        icon: 'error',
        title: message,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary"
        }
    });
}