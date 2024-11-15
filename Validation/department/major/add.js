$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var manganh = $('body').find('[id="manganh"]').val().trim();
        var tennganh = $('body').find('[id="tennganh"]').val().trim();
        var khoa = $('body').find('[id="khoa"] :selected').val();

        var validmanganh = $('body').find('[id="valid-manganh"]');
        var validtennganh = $('body').find('[id="valid-tennganh"]');
        var validkhoa = $('body').find('[id="valid-khoa"]');

        validmanganh.text('');
        validtennganh.text('');
        validkhoa.text('');

        var check = true;

        if (manganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validmanganh.text("Vui lòng nhập mã ngành.");
            $('body').find('[id="manganh"]').focus();
        }

        if (tennganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtennganh.text("Vui lòng nhập tên ngành.");
            $('body').find('[id="tennganh"]').focus();
        }

        if (validkhoa.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validkhoa.text("Vui lòng chọn khoa.");
            $('body').find('[id="khoa"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('manganh', manganh);
            formData.append('tennganh', tennganh);
            formData.append('khoa', khoa);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "Majors/addMajor",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="themmoi"]').modal('toggle');
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã lưu thông tin ngành mới.",
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "warning",
                        title: "Ngành " + manganh + " đã tồn tại trên hệ thống.",
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

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