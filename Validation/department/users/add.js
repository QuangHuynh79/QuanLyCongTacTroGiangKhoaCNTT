$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var ma = $('body').find('[id="ma"]').val().trim();
        var hoten = $('body').find('[id="hoten"]').val().trim();
        var email = $('body').find('[id="email"]').val().trim();
        var chucdanh = $('body').find('[id="chucdanh"] :selected').val();
        var dienthoai = $('body').find('[id="dienthoai"]').val().trim();
        var khoa = $('body').find('[id="khoa"]').val().trim();
        var nganh = $('body').find('[id="nganh"] :selected').val();

        var validma = $('body').find('[id="valid-ma"]');
        var validhoten = $('body').find('[id="valid-hoten"]');
        var validemail = $('body').find('[id="valid-email"]');
        var validchucdanh = $('body').find('[id="valid-chucdanh"]');
        var validdienthoai = $('body').find('[id="valid-dienthoai"]');

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
                $('body').find('[id="btnClose"]').prop('disabled', false);

                validdienthoai.text("Điện thoại phải đủ 10 số (Không dùng mã quốc gia).");
                $('body').find('[id="dienthoai"]').focus();
            }
        }

        if (chucdanh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validchucdanh.text("Vui lòng chọn chức danh người dùng.");
            $('body').find('[id="chucdanh"]').focus();
        }

        if (email.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validemail.text("Vui lòng nhập Email người dùng.");
            $('body').find('[id="email"]').focus();
        }
        else if (testMail.test(email) == false) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validemail.text("Địa chỉ Email không hợp lệ.");
            $('body').find('[id="email"]').focus();
        }
        else if (email.toLowerCase().indexOf("@vanlanguni.vn") == -1
            && email.toLowerCase().indexOf("@vanlanguni.edu.vn") == -1
            && email.toLowerCase().indexOf("@vlu.edu.vn") == -1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validemail.text("Địa chỉ Email không thuộc về Văn Lang.");
            $('body').find('[id="email"]').focus();
        }

        if (hoten.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validhoten.text("Vui lòng nhập họ & tên người dùng.");
            $('body').find('[id="hoten"]').focus();
        }

        if (ma.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validma.text("Vui lòng nhập mã người dùng.");
            $('body').find('[id="ma"]').focus();
        }
        else if (ma.indexOf(" ") !== - 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validma.text(" Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="ma"]').focus();
        }
        else if (ma.match(specialChars)) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validma.text("Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="ma"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('ma', ma);
            formData.append('hoten', hoten);
            formData.append('email', email);
            formData.append('chucdanh', chucdanh);
            formData.append('dienthoai', dienthoai);
            formData.append('khoa', khoa);
            formData.append('nganh', nganh);
            formData.append('gioitinh', $('body').find('[id="gioitinh"] :selected').val());
            formData.append('quoctich', $('body').find('[id="quoctich"]').prop('checked'));
            formData.append('ngaysinh', $('body').find('[id="ngaysinh"]').val());
            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "Users/AddNew",
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
                        text: "Đã thêm một người dùng mới.",
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
                        text: "Người dùng đã tồn tại hoặc Email/Mã người dùng đã được sử dụng.",
                        icon: "error"
                    }).then(() => {
                        window.location.reload();
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