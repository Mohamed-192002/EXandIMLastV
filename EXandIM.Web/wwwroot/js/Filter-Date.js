$('#searchButton').on('click', function () {
    var fromDate = $('#fromDate').val();
    var toDate = $('#toDate').val();

    // Update DataTable AJAX URL with date filters
    datatable.ajax.url('/Readings/GetBooksAfterFilterDate?fromDate=' + encodeURIComponent(fromDate) + '&toDate=' + encodeURIComponent(toDate)).load();
});
