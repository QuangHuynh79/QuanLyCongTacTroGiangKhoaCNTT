$(document).ready(function () {
    function confirmCloseForm() {
        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var thoigianmo = $('body').find('[id="thoigianmo"]').val();
        var thoigiandong = $('body').find('[id="thoigiandong"]').val();

        if (nganh.length > 0 || thoigiandong.length > 0) {
            var defaultHocky = $('body').find('[id="hocky"] :selected').attr('data-default');
            var defaultThoigianmo = $('body').find('[id="thoigianmo"]').attr('data-default');

            if (hocky != defaultHocky || thoigianmo != defaultThoigianmo) {
                Swal.fire({
                    title: 'Đóng form đăng ký?',
                    text: 'Dữ liệu chưa được lưu. Xác nhận đóng form?',
                    icon: "question",
                    showCancelButton: true,
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Đóng ngay!",
                    cancelButtonText: "Không đóng"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#themmoi').modal('toggle');

                        $('body').find('[id="hocky"]').val(defaultHocky).change;

                        $('body').find('[id="nganh"]').val("").change;

                        $('body').find('[id="thoigianmo"]').val(defaultThoigianmo);

                        $('body').find('[id="thoigiandong"]').val("");
                    }
                });
            }
        }
        else {
            $('#themmoi').modal('toggle');
        }
    }
    $('body').on('click', '[id="btnClose"]', function () {
        confirmCloseForm();
    });
    $('body').on('click', '[id="btnXClose"]', function () {
        confirmCloseForm();
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var thoigianmo = $('body').find('[id="thoigianmo"]').val();
        var thoigiandong = $('body').find('[id="thoigiandong"]').val();

        var validhocky = $('body').find('[id="valid-hocky"]');
        var validnganh = $('body').find('[id="valid-nganh"]');
        var validthoigianmo = $('body').find('[id="valid-thoigianmo"]');
        var validthoigiandong = $('body').find('[id="valid-thoigiandong"]');

        validhocky.text('');
        validnganh.text('');
        validthoigianmo.text('');
        validthoigiandong.text('');

        var check = true;

        if (hocky.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validhocky.text("Vui lòng không bỏ trống Học Kỳ.");
            $('body').find('[id="hocky"]').focus();
        }

        if (nganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validnganh.text("Vui lòng không bỏ trống Ngành học.");
            $('body').find('[id="nganh"]').focus();
        }

        if (thoigianmo.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validthoigianmo.text("Vui lòng không bỏ trống Thời gian mở.");
            $('body').find('[id="thoigianmo"]').focus();
        }
        else if (thoigianmo.length > 10) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validthoigianmo.text("Thời gian mở chưa đúng định dạng.");
            $('body').find('[id="thoigianmo"]').focus();
        }

        if (thoigiandong.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validthoigiandong.text("Vui lòng không bỏ trống Thời gian đóng.");
            $('body').find('[id="thoigiandong"]').focus();
        }
        else if (thoigiandong.length > 10) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validthoigiandong.text("Thời gian đóng chưa đúng định dạng.");
            $('body').find('[id="thoigiandong"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('hocky', hocky);
            formData.append('nganh', nganh);
            formData.append('thoigianmo', thoigianmo);
            formData.append('thoigiandong', thoigiandong);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TeachingAssistant/AddRegister",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    $('body').find('[id="themmoi"]').modal('toggle');

                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Mở đăng ký thành công."
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Thời gian đăng ký bị trùng."
                    });

                }
                else if (ketqua == "NhoHonHienTai") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    validthoigianmo.text("Ngày mở đăng ký phải sau ngày hiện tại.");
                    $('body').find('[id="thoigianmo"]').focus();
                }
                else if (ketqua == "LonHonDangKy") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    validthoigiandong.text("Thời gian đóng phải sau Thời gian mở.");
                    $('body').find('[id="thoigiandong"]').focus();
                }
                else {
                    $('body').find('[id="themmoi"]').modal('toggle');

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