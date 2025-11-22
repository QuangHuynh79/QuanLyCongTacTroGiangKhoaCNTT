$(document).ready(function () {
    $('body').on('click', '[id="btn-search-thongbao"]', function () {
        var search = $('body').find('[id="inp-search-thongbao"]').val().trim();

        var formData = new FormData();
        formData.append('search', search);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Notifications/Search",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            $('body').find('[id="content-cpn-thongbao"]').replaceWith(ketqua);
        });

    });
});