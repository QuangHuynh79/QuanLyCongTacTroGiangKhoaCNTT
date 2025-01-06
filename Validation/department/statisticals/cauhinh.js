$(document).ready(function () {
    $('body').on('click', '[id="btncauhinhthulao"]', function () {
        var thulao = $('body').find('[id="giathulao"]').attr('defaultData');
        $('body').find('[id="giathulao"]').val(thulao);
        $('body').find('[id="valid-giathulao"]').text('');

        var giotoida = $('body').find('[id="giotoida"]').attr('defaultData');
        $('body').find('[id="giotoida"]').val(giotoida);
        $('body').find('[id="valid-giotoida"]').text('');

        $('body').find('[id="cauhinh"]').modal('toggle');
    });

    $('body').on('click', '[id="btnSubmitThuLao"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnThuLaoClose"]').prop('disabled', true);

        var thulao = $('body').find('[id="giathulao"]').val();
        var validthulao = $('body').find('[id="valid-giathulao"]');
        validthulao.text('');

        var giotoida = $('body').find('[id="giotoida"]').val();
        var validgiotoida = $('body').find('[id="valid-giotoida"]');
        validgiotoida.text('');

        var check = true;

        if (thulao.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);

            validthulao.text("Vui lòng không bỏ trống giá thù lao.");
            $('body').find('[id="giathulao"]').focus();
        }
        else if (Number(thulao) <= 0) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);

            validthulao.text("Giá thù lao phải lớn hơn 0 VNĐ");
            $('body').find('[id="giathulao"]').focus();
        }

        if (giotoida.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);

            validgiotoida.text("Vui lòng không bỏ trống số giờ tối đa.");
            $('body').find('[id="giotoida"]').focus();
        }
        else if (Number(giotoida) <= 0) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);

            validgiotoida.text("Số giờ tối đa phải lớn hơn 0");
            $('body').find('[id="giotoida"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('thulao', thulao.replace(/,/g, ''));
            formData.append('giotoida', giotoida);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "Statisticals/UpdateRemuneratiion",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="cauhinh"]').modal('toggle');

                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);
                    $('body').find('[id="giathulao"]').attr('defaultData', thulao);

                    Toast.fire({
                        icon: "success",
                        title: "Đã cập nhật giá thù lao."
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnThuLaoClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                    
                }
            });
        }
    });
});