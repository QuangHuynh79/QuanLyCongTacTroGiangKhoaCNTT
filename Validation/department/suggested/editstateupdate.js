$(document).ready(function () {
    $('body').on('change', '[id^="mocapnhat-"]', function () {
        var btn = $(this);

        var id = btn.attr('name');
        var trangthai = btn.prop('checked');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/EditStateAdvances",
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
                });
                if (trangthai == true) {
                    btn.prop('checked', false);
                }
            }
            else {
                var thongbao = "Đã mở cập nhật mô tả công việc.";
                if (trangthai == false) {
                    thongbao = "Đã đóng cập nhật mô tả công việc.";
                }
                else {
                    $('body').find('[id="trangthaiduyet-' + id + '"]').replaceWith('<span id="trangthaiduyet-' + id + '" class="badge bg-danger"><i class="bi bi-x-square me-2"></i>Chưa duyệt</span>')
                }

                Toast.fire({
                    icon: "success",
                    title: thongbao,
                });
            }
        });
    });
});