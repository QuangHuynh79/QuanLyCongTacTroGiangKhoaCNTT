$(document).ready(function () {
    $('body').on('click', '[id^="delete-anhcu-"]', function () {
        var img = $(this);
        var index = img.attr('name');

        var urlImg = $('body').find('[id="anhcu-' + index + '"]').val();
        var lstDelete = $('body').find('[id="file-delete-list"]').val();

        if (lstDelete.length < 1) {
            $('body').find('[id="file-delete-list"]').val(urlImg);
        }
        else {
            $('body').find('[id="file-delete-list"]').val(lstDelete + "#" + urlImg);
        }

        $('body').find('[id="parent-anhcu-' + index + '"]').replaceWith(null);

        img.tooltip("hide");
    });

    $('body').on('input', '[id="dienthoai"]', function () {
        var dienthoai = $(this).val().trim();
        var validdienthoai = $('body').find('[id="valid-dienthoai"]');
        validdienthoai.text('');

        if (dienthoai.length < 1) {
            validdienthoai.text("Vui lòng không bỏ trống Số điện thoại.");
            $('body').find('[id="dienthoai"]').focus();
        }
        else if (dienthoai.length != 10 || dienthoai.indexOf("-") != -1 || dienthoai.indexOf(".") != -1) {
            validdienthoai.text("Số điện thoại chưa đúng định dạng.");
            $('body').find('[id="dienthoai"]').focus();
        }
    });

    $('body').on('input', '[id="ngaysinh"]', function () {
        var gioitinh = $('body').find('[id="ngaysinh"] :selected').val();
        var validngaysinh = $('body').find('[id="valid-ngaysinh"]');
        validngaysinh.text('');

        if (ngaysinh.length < 1) {
            validngaysinh.text("Vui lòng chọn Ngày sinh.");
            $('body').find('[id="ngaysinh"]').focus();
        }
    });

    $('body').on('change', '[id="gioitinh"]', function () {
        var gioitinh = $(this).val().trim();
        var validgioitinh = $('body').find('[id="valid-gioitinh"]');
        validgioitinh.text('');

        if (gioitinh.length < 1) {
            validgioitinh.text("Vui lòng chọn Giới tính.");
            $('body').find('[id="gioitinh"]').focus();
        }
    });

    $('body').on('input', '[id="tbctl"]', function () {
        var tbctl = $('body').find('[id="tbctl"]').val().trim().replace(",", ".");
        var validtbctl = $('body').find('[id="valid-tbctl"]');
        validtbctl.text('');

        if (tbctl.length < 1) {
            validtbctl.text("Vui lòng không bỏ trống Điểm TB tích lũy.");
            $('body').find('[id="tbctl"]').focus();
        }
        else if (tbctl.indexOf("-") != -1) {
            validtbctl.text("Điểm TB tích lũy chưa đúng định dạng");
            $('body').find('[id="tbctl"]').focus();
        }
        else if (tbctl < 7.0 || tbctl > 10.0) {
            validtbctl.text("Điểm TB tích lũy phải đạt từ 7.0 - 10.0 điểm");
            $('body').find('[id="tbctl"]').focus();
        }
    });

    $('body').on('input', '[id="drl"]', function () {
        var drl = $('body').find('[id="drl"]').val().trim().replace(",", ".");
        var validdrl = $('body').find('[id="valid-drl"]');
        validdrl.text('');

        if (drl.length < 1) {
            validdrl.text("Vui lòng không bỏ trống Điểm rèn luyện.");
            $('body').find('[id="drl"]').focus();
        }
        else if (drl.indexOf("-") != -1) {
            validdrl.text("Điểm rèn luyện chưa đúng định dạng");
            $('body').find('[id="drl"]').focus();
        }
        else if (drl < 65 || drl > 100) {
            validdrl.text("Điểm rèn luyện phải đạt từ 65 - 100 điểm");
            $('body').find('[id="drl"]').focus();
        }
    });

    $('body').on('input', '[id="dtk"]', function () {
        var dtk = $('body').find('[id="dtk"]').val().trim().replace(",", ".");
        var validdtk = $('body').find('[id="valid-dtk"]');
        validdtk.text('');

        if (dtk.length < 1) {
            validdtk.text("Vui lòng không bỏ trống Điểm tổng kết môn.");
            $('body').find('[id="dtk"]').focus();
        }
        else if (dtk.indexOf("-") != -1) {
            validdtk.text("Điểm tổng kết môn chưa đúng định dạng");
            $('body').find('[id="dtk"]').focus();
        }
        else if (dtk < 7.0 || dtk > 10.0) {
            validdtk.text("Điểm TK phải đạt từ 7.0 - 10.0 điểm");
            $('body').find('[id="dtk"]').focus();
        }
    });

    $('body').on('change', '[id="hinhanhmc"]', function () {
        var hamc = $('body').find('[id="hinhanhmc"]');
        var validhamc = $('body').find('[id="valid-hinhanhmc"]');
        validhamc.text('');

        var anhcuCount = 0;
        $('body').find('[id^="anhcu-"]').each(function () {
            anhcuCount++;
        });

        if (hamc[0].files.length + anhcuCount < 1) {
            validhamc.text("Vui lòng không bỏ trống Hình ảnh minh chứng.");
            $('body').find('[id="hamc"]').focus();
        }
        else if (hamc[0].files.length + anhcuCount > 3) {
            validhamc.text("Vui lòng gửi tối đa 3 hình ảnh.");
            $('body').find('[id="hamc"]').focus();
            hamc.val(null);
        }
        else {
            var sizes = 0;
            for (var i = 0; i < hamc[0].files.length; i++) {
                sizes += (hamc[0].files[i].size / 1024).toFixed(1);
            }
            if (sizes > (50 * 1024)) {
                validhamc.text("Vui lòng gửi tối đa 3 hình ảnh < 50MB.");
                $('body').find('[id="hamc"]').focus();
                hamc.val(null);
            }
        }
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnInfoClose"]').prop('disabled', true);

        var idFORM = $('body').find('[id="idFORMDKY"]').val();
        var idLHP = $('body').find('[id="idLHPApply"]').val();
        var idTK = $('body').find('[id="idtkApply"]').val();

        var dienthoai = $('body').find('[id="dienthoai"]').val().trim();
        var ngaysinh = $('body').find('[id="ngaysinh"]').val().trim();
        var gioitinh = $('body').find('[id="gioitinh"] :selected').val();

        var tbctl = $('body').find('[id="tbctl"]').val().trim();
        var drl = $('body').find('[id="drl"]').val().trim();
        var dtk = $('body').find('[id="dtk"]').val().trim();
        var hamc = $('body').find('[id="hinhanhmc"]');

        var validdienthoai = $('body').find('[id="valid-dienthoai"]');
        var validngaysinh = $('body').find('[id="valid-ngaysinh"]');
        var validgioitinh = $('body').find('[id="valid-gioitinh"]');
        var validtbctl = $('body').find('[id="valid-tbctl"]');
        var validdrl = $('body').find('[id="valid-drl"]');
        var validdtk = $('body').find('[id="valid-dtk"]');
        var validhamc = $('body').find('[id="valid-hinhanhmc"]');

        validdienthoai.text('');
        validngaysinh.text('');
        validgioitinh.text('');
        validtbctl.text('');
        validdrl.text('');
        validdtk.text('');
        validhamc.text('');

        var check = true;
        var anhcuCount = 0;
        $('body').find('[id^="anhcu-"]').each(function () {
            anhcuCount++;
        });

        if (hamc[0].files.length + anhcuCount < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validhamc.text("Vui lòng không bỏ trống Hình ảnh minh chứng.");
            $('body').find('[id="hamc"]').focus();
        }
        else if (hamc[0].files.length + anhcuCount > 3) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validhamc.text("Vui lòng gửi tối đa 3 hình ảnh < 50MB.");
            $('body').find('[id="hamc"]').focus();
        }
        else {
            var sizes = 0;
            for (var i = 0; i < hamc[0].files.length; i++) {
                sizes += (hamc[0].files[i].size / 1024).toFixed(1);
            }
            if (sizes > (50 * 1024)) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnClose"]').prop('disabled', false);

                validhamc.text("Vui lòng gửi tối đa 3 hình ảnh < 50MB.");
                $('body').find('[id="hamc"]').focus();
            }
        }

        if (dtk.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdtk.text("Vui lòng không bỏ trống Điểm tổng kết môn.");
            $('body').find('[id="dtk"]').focus();
        }
        else if (dtk.indexOf("-") != -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdtk.text("Điểm tổng kết môn chưa đúng định dạng");
            $('body').find('[id="dtk"]').focus();
        }
        else if (dtk < 7.0 || dtk > 10.0) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdtk.text("Điểm TK phải đạt từ 7.0 - 10.0 điểm");
            $('body').find('[id="dtk"]').focus();
        }

        if (drl.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdrl.text("Vui lòng không bỏ trống Điểm rèn luyện.");
            $('body').find('[id="drl"]').focus();
        }
        else if (drl.indexOf("-") != -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdrl.text("Điểm rèn luyện chưa đúng định dạng");
            $('body').find('[id="drl"]').focus();
        }
        else if (drl < 65 || drl > 100) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdrl.text("Điểm rèn luyện phải đạt từ 65 - 100 điểm");
            $('body').find('[id="drl"]').focus();
        }

        if (tbctl.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtbctl.text("Vui lòng không bỏ trống Điểm TB tích lũy.");
            $('body').find('[id="tbctl"]').focus();
        }
        else if (tbctl.indexOf("-") != -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtbctl.text("Điểm TB tích lũy chưa đúng định dạng");
            $('body').find('[id="tbctl"]').focus();
        }
        else if (tbctl < 7.0 || tbctl > 10.0) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtbctl.text("Điểm TB tích lũy phải đạt từ 7.0 - 10.0 điểm");
            $('body').find('[id="tbctl"]').focus();
        }

        if (gioitinh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validgioitinh.text("Vui lòng chọn Giới tính.");
            $('body').find('[id="gioitinh"]').focus();
        }

        if (ngaysinh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validngaysinh.text("Vui lòng chọn Ngày sinh.");
            $('body').find('[id="ngaysinh"]').focus();
        }

        if (dienthoai.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdienthoai.text("Vui lòng không bỏ trống Số điện thoại.");
            $('body').find('[id="dienthoai"]').focus();
        }
        else if (dienthoai.length != 10 || dienthoai.indexOf("-") != -1 || dienthoai.indexOf(".") != -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validdienthoai.text("Số điện thoại chưa đúng định dạng.");
            $('body').find('[id="dienthoai"]').focus();
        }

        if (check == true) {
            var urlImg = $('body').find('[id="file-delete-list"]').val().trim();

            if (urlImg.length < 1) {
                formData = new FormData();
                formData.append('idFORM', idFORM);
                formData.append('idLHP', idLHP);
                formData.append('idTK', idTK);

                formData.append('dienthoai', dienthoai);
                formData.append('ngaysinh', ngaysinh);
                formData.append('gioitinh', gioitinh);

                formData.append('tbctl', tbctl);
                formData.append('drl', drl);
                formData.append('dtk', dtk);

                for (var i = 0; i < hamc[0].files.length; i++) {
                    var file = hamc[0].files[i];
                    formData.append('hamc', file);
                }
                $.ajax({
                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                    url: $('#requestPath').val() + "TeachingAssistant/SubmitApply",
                    data: formData,
                    dataType: 'html',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                }).done(function (ketqua) {
                    $('body').find('[id="apply"]').modal('toggle');

                    if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
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
                    else if (ketqua == "SUCCESS") {
                        btn.html('Lưu thông tin');
                        btn.prop('disabled', false);
                        $('body').find('[id="btnClose"]').prop('disabled', false);

                        Toast.fire({
                            icon: "success",
                            title: "Ứng tuyển thành công."
                        }).then(() => {
                            window.location.reload();
                        });
                    }
                    else if (ketqua == "SUCCESS2") {
                        btn.html('Lưu thông tin');
                        btn.prop('disabled', false);
                        $('body').find('[id="btnClose"]').prop('disabled', false);

                        Toast.fire({
                            icon: "success",
                            title: "Cập nhật thông tin ứng tuyển thành công."
                        });
                    }
                });
            }
            else {
                var formData = new FormData();
                formData.append('idFORM', idFORM);
                formData.append('idLHP', idLHP);
                formData.append('idTK', idTK);
                formData.append('url', urlImg);

                $.ajax({
                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                    url: $('#requestPath').val() + "TeachingAssistant/DeleteImageApply",
                    data: formData,
                    dataType: 'html',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                }).done(function (ketqua) {
                    if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
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
                    else {
                        formData = new FormData();
                        formData.append('idFORM', idFORM);
                        formData.append('idLHP', idLHP);
                        formData.append('idTK', idTK);

                        formData.append('dienthoai', dienthoai);
                        formData.append('ngaysinh', ngaysinh);
                        formData.append('gioitinh', gioitinh);

                        formData.append('tbctl', tbctl);
                        formData.append('drl', drl);
                        formData.append('dtk', dtk);

                        for (var i = 0; i < hamc[0].files.length; i++) {
                            var file = hamc[0].files[i];
                            formData.append('hamc', file);
                        }
                        $.ajax({
                            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                            url: $('#requestPath').val() + "TeachingAssistant/SubmitApply",
                            data: formData,
                            dataType: 'html',
                            type: 'POST',
                            processData: false,
                            contentType: false,
                        }).done(function (ketqua) {
                            $('body').find('[id="apply"]').modal('toggle');

                            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
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
                            else if (ketqua == "SUCCESS") {
                                btn.html('Lưu thông tin');
                                btn.prop('disabled', false);
                                $('body').find('[id="btnClose"]').prop('disabled', false);

                                Toast.fire({
                                    icon: "success",
                                    title: "Ứng tuyển thành công."
                                }).then(() => {
                                    window.location.reload();
                                });
                            }
                            else if (ketqua == "SUCCESS2") {
                                btn.html('Lưu thông tin');
                                btn.prop('disabled', false);
                                $('body').find('[id="btnClose"]').prop('disabled', false);

                                Toast.fire({
                                    icon: "success",
                                    title: "Cập nhật thông tin ứng tuyển thành công."
                                });
                            }
                        });

                    }
                });
            }
        }
    });
});