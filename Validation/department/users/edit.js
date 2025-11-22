$(document).ready(function () {
    $('body').on('change', '[id^="trangthai-"]', function () {
        var fullName = $(this).attr('fullname');
        var formData = new FormData();
        formData.append('trangthai', $(this).prop('checked'));
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Users/EditState",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua == "SUCCESS") {
                Toast.fire({
                    icon: "success",
                    title: "Đã cập nhật trạng thái người dùng: " + fullName
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
        var id = $(this).attr('name');
        var name = $(this).attr('fullname')
        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Users/openedit",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {

            if (ketqua.indexOf("Chi tiết lỗi") !== -1 || ketqua.indexOf("Người dùng không tồn tại") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                }); 
            }
            else {
                $('body').find('[id="btnEditSubmit"]').attr('name', id);
                $('body').find('[id="capnhat-partial"]').replaceWith(ketqua);
                $('body').find('[id="capnhatTitle"]').text("Cập nhật người dùng: " + name);
                $('body').find('[id="capnhat"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        var id = $(this).attr('name');
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var ma = $('body').find('[id="editma"]').val().trim();
        var hoten = $('body').find('[id="edithoten"]').val().trim();
        var email = $('body').find('[id="editemail"]').val().trim();
        var chucdanh = $('body').find('[id="editchucdanh"] :selected').val();
        var dienthoai = $('body').find('[id="editdienthoai"]').val().trim();
        var nganh = $('body').find('[id="editnganh"] :selected').val();

        var validma = $('body').find('[id="valid-editma"]');
        var validhoten = $('body').find('[id="valid-edithoten"]');
        var validemail = $('body').find('[id="valid-editemail"]');
        var validchucdanh = $('body').find('[id="valid-editchucdanh"]');
        var validdienthoai = $('body').find('[id="valid-editdienthoai"]');

        validma.text('');
        validhoten.text('');
        validemail.text('');
        validchucdanh.text('');
        validdienthoai.text('');

        var testMail = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        var specialChars = /[^a-zA-Z0-9 ]/g;

        var check = true;

        if (dienthoai.length > 0) {
            if (dienthoai.length != 10) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnEditClose"]').prop('disabled', false);

                validdienthoai.text("Điện thoại phải đủ 10 số (Không dùng mã quốc gia).");
                $('body').find('[id="editdienthoai"]').focus();
            }
        }

        if (chucdanh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validchucdanh.text("Vui lòng chọn Vai trò người dùng.");
            $('body').find('[id="editchucdanh"]').focus();
        }

        if (email.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validemail.text("Vui lòng nhập Email người dùng.");
            $('body').find('[id="editemail"]').focus();
        }
        else if (testMail.test(email) == false) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validemail.text("Địa chỉ Email không hợp lệ.");
            $('body').find('[id="editemail"]').focus();
        }
        else if (email.toLowerCase().indexOf("@vanlanguni.vn") == -1
            && email.toLowerCase().indexOf("@vanlanguni.edu.vn") == -1
            && email.toLowerCase().indexOf("@vlu.edu.vn") == -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validemail.text("Địa chỉ Email không thuộc về Văn Lang.");
            $('body').find('[id="editemail"]').focus();
        }

        if (hoten.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validhoten.text("Vui lòng nhập họ & tên người dùng.");
            $('body').find('[id="edithoten"]').focus();
        }

        if (ma.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validma.text("Vui lòng nhập mã người dùng.");
            $('body').find('[id="editma"]').focus();
        }
        else if (ma.indexOf(" ") !== - 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validma.text(" Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="editma"]').focus();
        }
        else if (ma.match(specialChars)) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validma.text("Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="editma"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('id', id);
            formData.append('ma', ma);
            formData.append('hoten', hoten);
            formData.append('email', email);
            formData.append('chucdanh', chucdanh);
            formData.append('dienthoai', dienthoai);
            formData.append('nganh', nganh);
            formData.append('gioitinh', $('body').find('[id="editgioitinh"] :selected').val());
            formData.append('ngaysinh', $('body').find('[id="editngaysinh"]').val());

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "Users/SubmitEdit",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {

                if (ketqua == "SUCCESS") {
                    $('body').find('[id="capnhat"]').modal('toggle');

                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã cập nhật thông tin người dùng."
                    }).then(() => {
                        window.location.reload();
                    }); 
                    
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Người dùng đã tồn tại hoặc Email/Mã người dùng đã được sử dụng."
                    }); 
                }
                else {
                    $('body').find('[id="capnhat"]').modal('toggle');

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