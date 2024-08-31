$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var tenhocky = $('body').find('[id="tenhocky"]').val().trim();
        var nambatdau = $('body').find('[id="nambatdau"] :selected').val().trim();
        var namketthuc = $('body').find('[id="namketthuc"] :selected').val().trim();
        var tuanbatdau = $('body').find('[id="tuanbatdau"]').val().trim();
        var ngaybatdau = $('body').find('[id="ngaybatdau"]').val().trim();
        var tiettoida = $('body').find('[id="tiettoida"]').val().trim();
        var loptoida = $('body').find('[id="loptoida"]').val().trim();

        var validtenhocky = $('body').find('[id="valid-tenhocky"]');
        var validnambatdau = $('body').find('[id="valid-nambatdau"]');
        var validnamketthuc = $('body').find('[id="valid-namketthuc"]');
        var validtuanbatdau = $('body').find('[id="valid-tuanbatdau"]');
        var validngaybatdau = $('body').find('[id="valid-ngaybatdau"]');
        var validtiettoida = $('body').find('[id="valid-tiettoida"]');
        var validloptoida = $('body').find('[id="valid-loptoida"]');

        var check = true;

        if (loptoida.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validloptoida.text("Vui lòng nhập số lớp tối đa");
            $('body').find('[id="loptoida"]').focus();
        }

        if (tiettoida.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtiettoida.text("Vui lòng nhập số tiết tối đa");
            $('body').find('[id="tiettoida"]').focus();
        }

        if (ngaybatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validngaybatdau.text("Vui lòng chọn ngày bắt đầu");
            $('body').find('[id="ngaybatdau"]').focus();
        }

        if (tuanbatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtuanbatdau.text("Vui lòng nhập tuần bắt đầu");
            $('body').find('[id="tuanbatdau"]').focus();
        }

        if (tenhocky.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtenhocky.text("Vui lòng nhập học kỳ");
            $('body').find('[id="tenhocky"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('tenhocky', tenhocky);
            formData.append('nambatdau', nambatdau);
            formData.append('namketthuc', namketthuc);
            formData.append('tuanbatdau', tuanbatdau);
            formData.append('ngaybatdau', ngaybatdau);
            formData.append('tiettoida', tiettoida);
            formData.append('loptoida', loptoida);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "admin/dangnhap/logout"; } },
                url: $('#requestPath').val() + "studentaffairs/studentaffairssemesterandmajor/addSemester",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thành công!",
                        text: "Đã lưu thông tin học kỳ mới.",
                        icon: "success"
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thất bại!",
                        text: "Học kỳ  " + tenhocky + "  đã tồn tại trên hệ thống.",
                        icon: "warning"
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Đã xảy ra lỗi!",
                        text: ketqua,
                        icon: "error"
                    }).then(() => {
                        window.location.reload();
                    });
                }
            });

        }
    });
});