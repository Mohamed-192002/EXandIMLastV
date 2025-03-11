$(document).ready(function () {

    $('#EntityId').on('change', function () {
        var entityId = $(this).val();
        var subEntityList = $('#SubEntityId');

        subEntityList.empty();
        subEntityList.append('<option></option>');

        if (entityId !== '') {
            $.ajax({
                url: '/ExportBook/GetSubEn?entityId=' + entityId,
                success: function (subEntities) {
                    $.each(subEntities, function (i, subEntity) {
                        var item = $('<option></option>').attr("value", subEntity.value).text(subEntity.text);
                        subEntityList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });


    $('#SideEntityId').on('change', function () {
        var selectedSideEntityId = $(this).val();
        var teamsList = $('#SelectedTeams');
        teamsList.empty();
        teamsList.append('<option value=""></option>');

        if (selectedSideEntityId !== '') {
            $.ajax({
                url: '/ExportBook/GetTeams',
                type: 'GET',
                data: { sideEntityId: selectedSideEntityId },
                success: function (teams) {
                    $.each(teams, function (i, team) {
                        var item = $('<option></option>').attr("value", team.value).text(team.text);
                        teamsList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });


    $('#SelectedEntities').on('change', function () {
        var selectedEntityIds = $(this).val();
        if (!Array.isArray(selectedEntityIds)) {
            selectedEntityIds = [selectedEntityIds]; // Convert to array if it's not
        }
        var subEntityList = $('#SelectedSubEntity');
        subEntityList.empty();
        subEntityList.append('<option></option>');

        if (selectedEntityIds.length > 0) { // Check if there are selected IDs
            $.ajax({
                url: '/ExportBook/GetSubEntities',
                method: 'POST', // Change to POST method
                data: { selectedEntityIds: selectedEntityIds }, // Send data as an object
                success: function (subEntities) {
                    $.each(subEntities, function (i, subEntity) {
                        var item = $('<option></option>').attr("value", subEntity.value).text(subEntity.text);
                        subEntityList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });


    $('#SelectedSubEntity').on('change', function () {
        var selectedSubEntityIds = $(this).val();
        // Check if selectedSubEntityIds is an array
        if (!Array.isArray(selectedSubEntityIds)) {
            selectedSubEntityIds = [selectedSubEntityIds]; // Convert to array if it's not
        }
        var SecondSubEntityList = $('#SelectedSecondSubEntity');
        SecondSubEntityList.empty();
        SecondSubEntityList.append('<option></option>');

        if (selectedSubEntityIds.length > 0) { // Check if there are selected IDs
            $.ajax({
                url: '/ExportBook/GetSecondSubEntities',
                method: 'POST', // Change to POST method
                data: { selectedSubEntityIds: selectedSubEntityIds }, // Send data as an object
                success: function (SecondSubEntities) {
                    $.each(SecondSubEntities, function (i, SecondSubEntity) {
                        var item = $('<option></option>').attr("value", SecondSubEntity.value).text(SecondSubEntity.text);
                        SecondSubEntityList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });


    $('#EntityId').on('change', function () {
        var entityId = $(this).val();
        var acceptArchiveField = $('.activeInside');

        if (entityId !== '') {
            $.ajax({
                url: '/Entity/GetEntity?entityId=' + entityId,
                success: function (entity) {
                    if (entity) {
                        acceptArchiveField.addClass("d-none")
                    } else {
                        acceptArchiveField.removeClass("d-none")
                    }
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });




});