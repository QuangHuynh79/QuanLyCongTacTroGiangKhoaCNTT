$(document).ready(function () {
    $('body').on('change', '[id="nambatdau"]', function () {
        var year = Number($(this).val());
        year = year + 1;

        $('body').find('[id="namketthuc"]').html('<option selected value="' + year + '">' + year + '</option>');
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var tenhocky = $('body').find('[id="tenhocky"]').val().trim();
        var nambatdau = $('body').find('[id="nambatdau"] :selected').val().trim();
        var namketthuc = $('body').find('[id="namketthuc"] :selected').val().trim();
        var ngaybatdau = $('body').find('[id="ngaybatdau"]').val().trim();

        var validtenhocky = $('body').find('[id="valid-tenhocky"]');
        var validnambatdau = $('body').find('[id="valid-nambatdau"]');
        var validnamketthuc = $('body').find('[id="valid-namketthuc"]');
        var validngaybatdau = $('body').find('[id="valid-ngaybatdau"]');

        validtenhocky.text('');
        validnambatdau.text('');
        validnamketthuc.text('');
        validngaybatdau.text('');

        var check = true;
        
        if (ngaybatdau.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validngaybatdau.text("Vui lòng chọn ngày bắt đầu");
            $('body').find('[id="ngaybatdau"]').focus();
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
            formData.append('ngaybatdau', ngaybatdau);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/addSemester",
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

                    Toast.fire({
                        icon: "success",
                        title: "Đã lưu thông tin học kỳ mới."
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
                        title: "Học kỳ  " + tenhocky + "  đã tồn tại trên hệ thống."
                    });
                }
                else if (ketqua == "INVALIDYEAR") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    validngaybatdau.text("Ngày bắt đầu phải trong năm " + nambatdau + ".");
                    $('body').find('[id="ngaybatdau"]').focus();
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