$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var id = "";
        $('body').find('[id^="chonLamTA-"]').each(function () {
            var inp = $(this);

            if (inp.prop('checked')) {
                id = inp.attr('keyta') + "";
            }
        });

        if (id.length > 0) {
            var formData = new FormData();
            formData.append('idtk', id);
            formData.append('idlhp', $('body').find('[id="id-lhp-assign-update"]').val());
            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "Assignment/SubmitUpdateAssign",
                dataType: 'html',
                data: formData,
                type: 'POST',
                processData: false,
                contentType: false
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
                    $('body').find('[id="phancong"]').modal('toggle');

                    Toast.fire({
                        icon: "success",
                        title: "Đã cập nhật phân công TA"
                    }).then(() => {
                        window.location.reload();
                    });
                }
            });
        }
        else {
            Toast.fire({
                icon: "error",
                title: "Vui lòng chọn TA cần cập nhật!"
            });
        }
    });
});