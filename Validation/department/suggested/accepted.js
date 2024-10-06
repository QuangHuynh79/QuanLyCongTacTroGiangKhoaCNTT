$(document).ready(function () {
    $('body').on('click', '[id^="openChiTietDeXuat-"]', function () {
        var btn = $(this);

        var id = btn.attr('name');
        var trangthai = btn.attr('trangthaidexuat');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/OpenSuggest",
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
                $('body').find('[id="contentChiTietDeXuat"]').replaceWith(ketqua);
                $('body').find('[id="btnSubmit"]').attr('name', id);
                if (trangthai == "true") {
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                }
                else {
                    $('body').find('[id="btnSubmit"]').prop('hidden', false);
                }
                $('body').find('[id="chiTietDeXuat"]').modal('toggle');
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
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnClose"]').prop('disabled', false);

                Swal.fire({
                    title: "Thành công!",
                    text: "Đã phê duyệt trợ giảng.",
                    icon: "success"
                }).then(() => {
                    window.location.reload();
                });
            }
        });
    });
});