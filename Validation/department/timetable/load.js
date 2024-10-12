$(document).ready(function () {
    $('body').find('[id="load-fill-page"]').prop('hidden', false);
    $.ajax({
        error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
        url: $('#requestPath').val() + "TimeTable/LoadContent",
        dataType: 'html',
        type: 'GET',
        processData: false,
        contentType: false
    }).done(function (ketqua) {
        $('#pageload').replaceWith(ketqua);
        $('body').find('[id="load-fill-page"]').prop('hidden', true);
    });
});