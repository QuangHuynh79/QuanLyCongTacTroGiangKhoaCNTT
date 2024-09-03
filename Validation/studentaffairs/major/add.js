$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var manganh = $('body').find('[id="manganh"]').val().trim();
        var tennganh = $('body').find('[id="tennganh"]').val().trim();
        var tenviettat = $('body').find('[id="tenviettat"]').val().trim();
        var ctdt = $('body').find('[id="ctdt"] :selected').val().trim();

        var validmanganh = $('body').find('[id="valid-manganh"]');
        var validtennganh = $('body').find('[id="valid-tennganh"]');
        var validtenviettat = $('body').find('[id="valid-tenviettat"]');
        var validctdt = $('body').find('[id="valid-ctdt"]');

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

        if (tenviettat.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtenviettat.text("Vui lòng nhập tên viết tắt.");
            $('body').find('[id="tenviettat"]').focus();
        }

        if (ctdt.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validctdt.text("Vui lòng chọn chương trình đào tạo.");
            $('body').find('[id="ctdt"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('manganh', manganh);
            formData.append('tennganh', tennganh);
            formData.append('tenviettat', tenviettat);
            formData.append('ctdt', ctdt);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "admin/dangnhap/logout"; } },
                url: $('#requestPath').val() + "studentaffairs/studentaffairssemesterandmajor/addMajor",
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
                        text: "Đã lưu thông tin ngành mới.",
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
                        text: "Ngành " + manganh + " đã tồn tại trên hệ thống.",
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