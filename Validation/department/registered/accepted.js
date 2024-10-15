$(document).ready(function () {
    $('body').on('click', '[id^="chitietungtuyen-"]', function () {
        var btn = $(this);

        var id = btn.attr('name');
        var trangthai = btn.attr('trangthaiUT');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/DetailRegistered",
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

                $('body').find('[id="btnSubmit"]').attr('name', id);
                $('body').find('[id="btnCancel"]').attr('name', id);

                if (trangthai == "true") {
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                    $('body').find('[id="btnCancel"]').prop('hidden', false);

                }
                else {
                    $('body').find('[id="btnSubmit"]').prop('hidden', false);
                    $('body').find('[id="btnCancel"]').prop('hidden', true);
                }
                $('body').find('[id="apply"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var trangthai = "true";
        SubmitAcceptApply(trangthai);
    });

    $('body').on('click', '[id="btnCancel"]', function () {
        var trangthai = "false";
        SubmitAcceptApply(trangthai);
    });

    function SubmitAcceptApply(trangthai) {
        var btn = $('body').find('[id="btnSubmit"]');
        var htmlBtn = "Lưu thông tin";
        var titleNotify = "Đã phê duyệt trợ giảng.";

        if (trangthai == "false") {
            btn = $('body').find('[id="btnCancel"]');
            htmlBtn = "Bỏ duyệt ứng tuyển";
            titleNotify = "Đã bỏ phê duyệt trợ giảng.";
        }

        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var id = btn.attr('name');

        var formData = new FormData();
        formData.append('id', id);
        formData.append('trangthai', trangthai);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/AcceptedRegistered",
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
                btn.html(htmlBtn);
                btn.prop('disabled', false);
                $('body').find('[id="btnClose"]').prop('disabled', false);

                Toast.fire({
                    icon: "success",
                    title: titleNotify
                }).then(() => {
                    window.location.reload();
                });

            }
        });
    }
});