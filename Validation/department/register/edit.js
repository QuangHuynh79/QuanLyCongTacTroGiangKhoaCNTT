$(document).ready(function () {
    function confirmCloseEditForm() {
        var hocky = $('body').find('[id="edithocky"] :selected').val();
        var nganh = $('body').find('[id="editnganh"] :selected').val();
        var thoigianmo = $('body').find('[id="editthoigianmo"]').val();
        var thoigiandong = $('body').find('[id="editthoigiandong"]').val();

        if (hocky.length > 0 || nganh.length > 0 || thoigianmo.length > 0 || thoigiandong.length > 0) {
            Swal.fire({
                title: 'Đóng form cập nhật đăng ký?',
                text: 'Dữ liệu chưa được lưu. Xác nhận đóng form?',
                icon: "question",
                showCancelButton: true,
                cancelButtonColor: "#d33",
                confirmButtonText: "Đóng ngay!",
                cancelButtonText: "Không đóng"
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#capnhat').modal('toggle');
                }
            });
        }
        else {
            $('#capnhat').modal('toggle');
        }
    }
    $('body').on('click', '[id="btnEditClose"]', function () {
        confirmCloseEditForm();
    });
    $('body').on('click', '[id="btnXEditClose"]', function () {
        confirmCloseEditForm();
    });

    $('body').on('click', '[id^="openSua-"]', function () {
        var formData = new FormData();
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/OpenEditRegister",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1 || ketqua.indexOf("Form đăng ký không tồn tại") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="capnhat-partial"]').replaceWith(ketqua);
                $('body').find('[id="capnhatTitle"]').text("Cập nhật form đăng ký học kỳ " + $('body').find('[id="edithocky"] :selected').text());
                $('body').find('[id="capnhat"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var hocky = $('body').find('[id="edithocky"] :selected').val();
        var nganh = $('body').find('[id="editnganh"] :selected').val();
        var thoigianmo = $('body').find('[id="editthoigianmo"]').val();
        var thoigiandong = $('body').find('[id="editthoigiandong"]').val();

        var validhocky = $('body').find('[id="valid-edithocky"]');
        var validnganh = $('body').find('[id="valid-editnganh"]');
        var validthoigianmo = $('body').find('[id="valid-editthoigianmo"]');
        var validthoigiandong = $('body').find('[id="valid-editthoigiandong"]');

        validhocky.text('');
        validnganh.text('');
        validthoigianmo.text('');
        validthoigiandong.text('');

        var check = true;

        if (hocky.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="EditbtnClose"]').prop('disabled', false);

            validhocky.text("Vui lòng không bỏ trống Học Kỳ.");
            $('body').find('[id="edithocky"]').focus();
        }

        if (nganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="EditbtnClose"]').prop('disabled', false);

            validnganh.text("Vui lòng không bỏ trống Ngành học.");
            $('body').find('[id="editnganh"]').focus();
        }

        if (thoigianmo.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="EditbtnClose"]').prop('disabled', false);

            validthoigianmo.text("Vui lòng không bỏ trống Thời gian mở.");
            $('body').find('[id="editthoigianmo"]').focus();
        }

        if (thoigiandong.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="EditbtnClose"]').prop('disabled', false);

            validthoigiandong.text("Vui lòng không bỏ trống Thời gian đóng.");
            $('body').find('[id="editthoigiandong"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('id', $('body').find('[id="idFdk"]').val());
            formData.append('hocky', hocky);
            formData.append('nganh', nganh);
            formData.append('thoigianmo', thoigianmo);
            formData.append('thoigiandong', thoigiandong);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TeachingAssistant/EditRegister",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="EditbtnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Cập nhật đăng ký trợ giảng thành công."
                    }).then(() => {
                        window.location.reload();
                    });
                    
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="EditbtnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Thời gian đăng ký " + hocky + " bị trùng."
                    });
                   
                }
                else if (ketqua == "LonHonDangKy") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="EditbtnClose"]').prop('disabled', false);

                    validthoigiandong.text("Thời gian đóng phải sau Thời gian mở.");
                    $('body').find('[id="editthoigiandong"]').focus();
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="EditbtnClose"]').prop('disabled', false);

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