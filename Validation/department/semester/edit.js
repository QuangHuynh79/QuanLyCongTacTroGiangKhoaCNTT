$(document).ready(function () {
    function confirmCloseEditForm() {
        var ngaybatdau = $('body').find('[id="editngaybatdau"]').val().trim();
        var def = $('body').find('[id="editngaybatdau"]').attr('data-default');
        var validngaybatdau = $('body').find('[id="valid-editngaybatdau"]');
        validngaybatdau.text('');

        if (ngaybatdau.length < 1 || (ngaybatdau !== def)) {
            Swal.fire({
                text: 'Dữ liệu chưa được lưu. Xác nhận đóng form?',
                icon: "question",
                showCancelButton: true,
                cancelButtonColor: "#d33",
                confirmButtonText: "Đóng ngay!",
                cancelButtonText: "Không đóng"
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#capnhat').modal('toggle');
                    $('body').find('[id="editngaybatdau"]').val(def);
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
    $('body').on('click', '[id="btnEditXClose"]', function () {
        confirmCloseEditForm();
    });


    $('body').on('change', '[id="editnambatdau"]', function () {
        var year = Number($(this).val());
        year = year + 1;

        $('body').find('[id="editnamketthuc"]').html('<option selected value="' + year + '">' + year + '</option>');
    });

    $('body').on('change', '[id^="trangthai-"]', function () {
        var fullName = $(this).attr('fullname');
        var formData = new FormData();
        formData.append('trangthai', $(this).prop('checked'));
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TermAndMajor/editStateSemester",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua == "SUCCESS") {
                Toast.fire({
                    icon: "success",
                    title: "Đã cập nhật trạng thái học kỳ: " + fullName
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
        });
    });

    $('body').on('click', '[id^="openSua-"]', function () {
        var formData = new FormData();
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TermAndMajor/OpenEditSemester",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1 || ketqua.indexOf("Học kỳ không tồn tại") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="capnhathocky-partial"]').replaceWith(ketqua);
                $('body').find('[id="editngaybatdau"]').flatpickr();
                $('body').find('[id="capnhatTitle"]').text("Cập nhật học kỳ " + $('body').find('[id="edittenhocky"]').val());
                $('body').find('[id="capnhat"]').modal('toggle');
            }
        });
    });

    $('body').on('input', '[id="editngaybatdau"]', function () {
        var ngaybatdau = $(this).val();
        var validngaybatdau = $('body').find('[id="valid-editngaybatdau"]');
        validngaybatdau.text('');

        if (ngaybatdau.length < 1) {
            validngaybatdau.text("Vui lòng chọn ngày bắt đầu");
        }
    });

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var nambatdau = $('body').find('[id="editnambatdau"] :selected').val().trim();
        var namketthuc = $('body').find('[id="editnamketthuc"] :selected').val().trim();
        var ngaybatdau = $('body').find('[id="editngaybatdau"]').val().trim();

        var validnambatdau = $('body').find('[id="valid-editnambatdau"]');
        var validnamketthuc = $('body').find('[id="valid-editnamketthuc"]');
        var validngaybatdau = $('body').find('[id="valid-editngaybatdau"]');

        validnambatdau.text('');
        validnamketthuc.text('');
        validngaybatdau.text('');

        var check = true;

        if (ngaybatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validngaybatdau.text("Vui lòng chọn ngày bắt đầu");
            $('body').find('[id="editngaybatdau"]').focus();
        }
        
        if (check == true) {
            var formData = new FormData();

            formData.append('id', $('body').find('[id="idhk"]').val());
            formData.append('nambatdau', nambatdau);
            formData.append('namketthuc', namketthuc);
            formData.append('ngaybatdau', ngaybatdau);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/editSemester",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="capnhat"]').modal('toggle');

                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã lưu thông tin học kỳ"
                    }).then(() => {
                        window.location.reload();
                    });
                    
                }
                else if (ketqua == "INVALIDYEAR") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    validngaybatdau.text("Ngày bắt đầu phải trong năm " + nambatdau + ".");
                    $('body').find('[id="editngaybatdau"]').focus();
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

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