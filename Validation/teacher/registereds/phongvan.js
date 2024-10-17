$(document).ready(function () {

    //Open form phỏng vấn
    $('body').on('click', '[id^="ketquapv-"]', function () {
        var btn = $(this);

        var id = btn.attr('name');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/OpenPhongVanRegistereds",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="phongvanContent"]').replaceWith(ketqua);
                $('body').find('[id="phongvanTitle"]').text(btn.attr('titleForm'));

                if (btn.attr('editState') == 'false') {
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                }
                else {
                    $('body').find('[id="btnSubmit"]').prop('hidden', false);
                }
                $('body').find('[id="phongvan"]').modal('toggle');
            }
        });
    });

    $('body').on('change', '[id^="tieuchi-"]', function () {
        var tieuchi1 = 0;
        var tieuchi2 = 0;
        var tieuchi3 = 0;
        $('body').find('[id^="tieuchi-"]').each(function () {
            var tc = $(this);
            if (tc.prop('checked')) {
                if (tc.attr('ketquatieuchi') == "0") {
                    tieuchi1++;
                }
                else if (tc.attr('ketquatieuchi') == "1") {
                    tieuchi2++;
                }
                else if (tc.attr('ketquatieuchi') == "2") {
                    tieuchi3++;
                }
            }
        });
        $('body').find('[id="tongdiem-1"]').text(tieuchi1);
        $('body').find('[id="tongdiem-2"]').text(tieuchi2);
        $('body').find('[id="tongdiem-3"]').text(tieuchi3);

    });

    //Lưu thông tin
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var tieuchi1 = ""; $('body').find('[id^="tieuchi-1-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi1 = tc.attr('ketquatieuchi'); } });
        var tieuchi2 = ""; $('body').find('[id^="tieuchi-2-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi2 = tc.attr('ketquatieuchi'); } });
        var tieuchi3 = ""; $('body').find('[id^="tieuchi-3-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi3 = tc.attr('ketquatieuchi'); } });
        var tieuchi4 = ""; $('body').find('[id^="tieuchi-4-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi4 = tc.attr('ketquatieuchi'); } });
        var tieuchi5 = ""; $('body').find('[id^="tieuchi-5-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi5 = tc.attr('ketquatieuchi'); } });
        var tieuchi6 = ""; $('body').find('[id^="tieuchi-6-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi6 = tc.attr('ketquatieuchi'); } });
        var tieuchi7 = ""; $('body').find('[id^="tieuchi-7-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi7 = tc.attr('ketquatieuchi'); } });
        var tieuchi8 = ""; $('body').find('[id^="tieuchi-8-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi8 = tc.attr('ketquatieuchi'); } });
        var tieuchi9 = ""; $('body').find('[id^="tieuchi-9-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi9 = tc.attr('ketquatieuchi'); } });
        var tieuchi10 = ""; $('body').find('[id^="tieuchi-10-"]').each(function () { var tc = $(this); if (tc.prop('checked')) { tieuchi10 = tc.attr('ketquatieuchi'); } });

        var nhanxet = $('body').find('[id="nhanxet"]').val().trim();
        var ketqua = $('body').find('[id="ketqua"] :selected').val();
        var ngaypv = $('body').find('[id="ngaypv"]').val();
        var ngayduyetpv = $('body').find('[id="ngayduyetpv"]').val();

        var validnhanxet = $('body').find('[id="valid-nhanxet"]');
        var validketqua = $('body').find('[id="valid-ketqua"]');
        var validngaypv = $('body').find('[id="valid-ngaypv"]');
        var validngayduyetpv = $('body').find('[id="valid-ngayduyetpv"]');
        var validTieuChi = "";

        validnhanxet.text('');
        validketqua.text('');
        validngaypv.text('');
        validngayduyetpv.text('');

        var check = true;

        if (tieuchi1.length < 1) { validTieuChi += "1, "; }
        if (tieuchi2.length < 1) { validTieuChi += "2, "; }
        if (tieuchi3.length < 1) { validTieuChi += "3, "; }
        if (tieuchi4.length < 1) { validTieuChi += "4, "; }
        if (tieuchi5.length < 1) { validTieuChi += "5, "; }
        if (tieuchi6.length < 1) { validTieuChi += "6, "; }
        if (tieuchi7.length < 1) { validTieuChi += "7, "; }
        if (tieuchi8.length < 1) { validTieuChi += "8, "; }
        if (tieuchi9.length < 1) { validTieuChi += "9, "; }
        if (tieuchi10.length < 1) { validTieuChi += "10, "; }

        if (validTieuChi.length > 0) {
            check = false;

            validTieuChi = validTieuChi.trim();
            validTieuChi = validTieuChi.substring(0, validTieuChi.length - 1);

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            Toast.fire({
                icon: "error",
                title: "Vui lòng chọn đánh giá cho tiêu chí: " + validTieuChi + ".",
            });
        }


        if (nhanxet.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validnhanxet.text("Vui nhập nhận xét ứng viên.");
            $('body').find('[id="nhanxet"]').focus();
        }
        else if (nhanxet.length > 200) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validnhanxet.text("Vui nhập nhận xét không quá 200 kí tự.");
            $('body').find('[id="nhanxet"]').focus();
        }

        if (ketqua.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validketqua.text("Vui lòng chọn kết quả phỏng vấn.");
            $('body').find('[id="ketqua"]').focus();
        }

        if (ngaypv.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validngaypv.text("Vui lòng chọn ngày phỏng vấn.");
            $('body').find('[id="ngaypv"]').focus();
        }

        if (ngayduyetpv.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validngayduyetpv.text("Vui lòng chọn ngày duyệt phỏng vấn.");
            $('body').find('[id="ngayduyetpv"]').focus();
        }

        if (check == true) {
            var tieuchiTong = tieuchi1 + "#" + tieuchi2 + "#" + tieuchi3 + "#" + tieuchi4 + "#" + tieuchi5 + "#"
                + tieuchi6 + "#" + tieuchi7 + "#" + tieuchi8 + "#" + tieuchi9 + "#" + tieuchi10;
            var tongdiem1 = 0;
            var tongdiem2 = 0;
            var tongdiem3 = 0;

            var lstTc = tieuchiTong.split('#');
            for (var i = 0; i < lstTc.length; i++) {
                if (lstTc[i] == "0") {
                    tongdiem1++;
                } else if (lstTc[i] == "1") {
                    tongdiem2++;
                }
                else {
                    tongdiem3++;
                }
            }

            var formData = new FormData();
            formData.append('id', $('body').find('[id="idut"]').val());
            formData.append('tieuchi', tieuchiTong);
            formData.append('tongdiem', tongdiem1 + "#" + tongdiem2 + "#" + tongdiem3);
            formData.append('nhanxet', nhanxet);
            formData.append('ketqua', ketqua);
            formData.append('ngaypv', ngaypv);
            formData.append('ngayduyetpv', ngayduyetpv);
            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TeachingAssistant/SubmitPhongVanRegistereds",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="phongvan"]').modal('toggle');

                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã lưu thông tin phỏng vấn.",
                    }).then(() => {
                        window.location.reload();
                    });
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