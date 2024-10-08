﻿$(document).ready(function () {
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
                const Toast = Swal.mixin({
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.onmouseenter = Swal.stopTimer;
                        toast.onmouseleave = Swal.resumeTimer;
                    }
                });
                Toast.fire({
                    icon: "success",
                    title: "Đã cập nhật trạng thái học kỳ: " + fullName
                });
            }
            else {
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
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
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
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

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var nambatdau = $('body').find('[id="editnambatdau"] :selected').val().trim();
        var namketthuc = $('body').find('[id="editnamketthuc"] :selected').val().trim();
        var tuanbatdau = $('body').find('[id="edittuanbatdau"]').val().trim();
        var ngaybatdau = $('body').find('[id="editngaybatdau"]').val().trim();
        var tiettoida = $('body').find('[id="edittiettoida"]').val().trim();
        var loptoida = $('body').find('[id="editloptoida"]').val().trim();

        var validnambatdau = $('body').find('[id="valid-editnambatdau"]');
        var validnamketthuc = $('body').find('[id="valid-editnamketthuc"]');
        var validtuanbatdau = $('body').find('[id="valid-edittuanbatdau"]');
        var validngaybatdau = $('body').find('[id="valid-editngaybatdau"]');
        var validtiettoida = $('body').find('[id="valid-edittiettoida"]');
        var validloptoida = $('body').find('[id="valid-editloptoida"]');

        validtenhocky.text('');
        validnambatdau.text('');
        validnamketthuc.text('');
        validtuanbatdau.text('');
        validngaybatdau.text('');
        validtiettoida.text('');
        validloptoida.text('');

        var check = true;

        if (loptoida.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validloptoida.text("Vui lòng nhập số lớp tối đa");
            $('body').find('[id="editloptoida"]').focus();
        }

        if (tiettoida.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validtiettoida.text("Vui lòng nhập số tiết tối đa");
            $('body').find('[id="edittiettoida"]').focus();
        }

        if (ngaybatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validngaybatdau.text("Vui lòng chọn ngày bắt đầu");
            $('body').find('[id="editngaybatdau"]').focus();
        }

        if (tuanbatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validtuanbatdau.text("Vui lòng nhập tuần bắt đầu");
            $('body').find('[id="edittuanbatdau"]').focus();
        }

        if (check == true) {
            var formData = new FormData();

            formData.append('id', $('body').find('[id="idhk"]').val());
            formData.append('nambatdau', nambatdau);
            formData.append('namketthuc', namketthuc);
            formData.append('tuanbatdau', tuanbatdau);
            formData.append('ngaybatdau', ngaybatdau);
            formData.append('tiettoida', tiettoida);
            formData.append('loptoida', loptoida);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/editSemester",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thành công!",
                        text: "Đã lưu thông tin học kỳ " + tenhocky,
                        icon: "success"
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