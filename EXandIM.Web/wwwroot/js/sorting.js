$(document).ready(function () {

    var sortable = new Sortable(document.getElementById('sortable-timeline'), {
        animation: 150,
        handle: '.timeline-item',
        onEnd: function (evt) {
            var order = [];
            $('#sortable-timeline .timeline-item').each(function () {
                order.push($(this).data('id'));
            });
            window.newOrder = order;
        }
    });
    $('#saveOrderBtn').on('click', function () {
        var activityBookId = $('#activityBookId').val();

        if (window.newOrder && window.newOrder.length > 0) {
            $.ajax({
                url: '/Activity/SaveOrder',
                type: 'POST',
                data: { itemOrder: window.newOrder, activityBookId: activityBookId },
                success: function (response) {
                    alert('تم حفظ الترتيب بنجاح');
                    location.reload(); // Reloads the current page

                },
                error: function (xhr, status, error) {
                    alert('حدث خطأ أثناء حفظ الترتيب: ' + xhr.responseText);
                    location.reload(); // Reloads the current page
                }
            });
        } else {
            alert('الرجاء إعادة ترتيب العناصر أولاً');
        }
    });
});