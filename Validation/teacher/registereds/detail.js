$(document).ready(function () {
    $('body').on('click', '[id^="chitietungtuyen-"]', function () {
        var btn = $(this);

        var id = btn.attr('name');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/DetailRegistereds",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="content-partialDetail"]').replaceWith(ketqua);
                $('body').find('[id="applyTitle"]').text(btn.attr('titleForm'));
                $('body').find('[id="apply"]').modal('toggle');
            }
        });
    });
});