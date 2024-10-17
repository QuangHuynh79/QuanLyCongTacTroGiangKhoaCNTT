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

                Toast.fire({
                    icon: "success",
                    title: thongbao,
                });
            }
        });
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var id = btn.attr('name');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/AcceptedAdvances",
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
                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnClose"]').prop('disabled', false);

                Toast.fire({
                    icon: "success",
                    title: "Đã phê duyệt trợ giảng."
                }).then(() => {
                    window.location.reload();
                });
               
            }
        });
    });
});