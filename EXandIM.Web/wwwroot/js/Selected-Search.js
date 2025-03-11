$(document).ready(function () {
    $('#titleFilter').on('change', function () {
        var titleFilter = $(this).val();
        var titleSearchList = $('#titleSearchId');

        titleSearchList.empty();
        titleSearchList.append('<option></option>');

        if (titleFilter !== '') {
            $.ajax({
                url: '/Entity/GetSearchTitle?titleFilterId=' + titleFilter,
                success: function (titleSearchs) {
                    $.each(titleSearchs, function (i, titleSearch) {
                        var item = $('<option></option>').attr("value", titleSearch.text).text(titleSearch.text);
                        titleSearchList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });
});
