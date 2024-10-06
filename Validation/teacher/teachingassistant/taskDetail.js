$(document).ready(function () {
    $('body').on('click', '[id^="btnTaskDetail-"]', function () {
        var id = $(this).attr('name');
        var tieude = $(this).attr("tieudeForm");
        $('body').find('[id="chitietmodalTitle"]').text(tieude);

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/TaskDetail",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="contentTaskDetail"]').replaceWith(ketqua);
                $('body').find('[id="editHanHoanThanh"]').flatpickr({ locale: "vn" });
                $('body').find('[id="chitietmodal"]').modal('toggle');
            }
        });
    });
});