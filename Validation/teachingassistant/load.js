﻿$(document).ready(function () {
    $.ajax({
        error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
        url: $('#requestPath').val() + "TaskManagement/LoadContentTaskList",
        dataType: 'html',
        type: 'GET',
        processData: false,
        contentType: false
    }).done(function (ketqua) {
        $('#pageload').replaceWith(ketqua);
    });
});