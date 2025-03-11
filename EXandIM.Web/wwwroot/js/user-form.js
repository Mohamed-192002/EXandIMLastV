$(document).ready(function () {
    $('#CircleId').on('change', function () {
        var circleId = $(this).val();
        var TeamsList = $('#TeamId');

        TeamsList.empty();
        TeamsList.append('<option></option>');

        if (circleId !== '') {
            $.ajax({
                url: '/User/GetTeams?circleId=' + circleId,
                success: function (teams) {
                    $.each(teams, function (i, team) {
                        var item = $('<option></option>').attr("value", team.value).text(team.text);
                        TeamsList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });
});