$(document).ready(function () {
    $('body').on('click', '[id="btnHuydangky"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnInfoClose"]').prop('disabled', true);

        var idFORM = $('body').find('[id="idFORMDKY"]').val();
        var idTK = $('body').find('[id="idtkApply"]').val();

        var formData = new FormData();
        formData.append('idFORM', idFORM);
        formData.append('idTK', idTK);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/CancelApply",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            $('body').find('[id="apply"]').modal('toggle');

            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                Toast.fire({
                    icon: "success",
                    title: "Đã hủy đăng ký trợ giảng."
                }).then(() => {
                    window.location.reload();
                });
            }
        });

    });
});