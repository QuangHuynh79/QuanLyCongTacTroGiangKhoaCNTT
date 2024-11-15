$(document).ready(function () {
    $('body').on('click', '[id^="edit-btnXoa-"]', function () {
        var count = $('body').find('[id^="edit-tr-parent-"]').length;
        if (count != 1) {
            var numCurrent = $(this).attr('name');
            $('body').find('[id="edit-tr-parent-' + numCurrent + '"]').replaceWith('');

            $('body').find('[id^="edit-tr-parent-"]').each(function () {
                var numThis = $(this).attr('name');

                if (Number(numCurrent) < Number(numThis)) {
                    changNumThis = Number(numThis) - 1;

                    $('body').find('[id="edit-mota-' + numThis + '"]').attr('id', 'edit-mota-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="edit-khoiluong-' + numThis + '"]').attr('id', 'edit-khoiluong-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="edit-thoigian-' + numThis + '"]').attr('id', 'edit-thoigian-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="edit-noilamviec-' + numThis + '"]').attr('id', 'edit-noilamviec-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="edit-ketqua-' + numThis + '"]').attr('id', 'edit-ketqua-' + changNumThis).attr('name', changNumThis);

                    $('body').find('[id="valid-edit-mota-' + numThis + '"]').attr('id', 'valid-edit-mota-' + changNumThis);
                    $('body').find('[id="valid-edit-khoiluong-' + numThis + '"]').attr('id', 'valid-edit-khoiluong-' + changNumThis);
                    $('body').find('[id="valid-edit-thoigian-' + numThis + '"]').attr('id', 'valid-edit-thoigian-' + changNumThis);
                    $('body').find('[id="valid-edit-noilamviec-' + numThis + '"]').attr('id', 'valid-edit-noilamviec-' + changNumThis);
                    $('body').find('[id="valid-edit-ketqua-' + numThis + '"]').attr('id', 'valid-edit-ketqua-' + changNumThis);

                    $('body').find('[id="edit-btnXoa-' + numThis + '"]').attr('name', changNumThis);
                    $('body').find('[id="edit-btnXoa-' + numThis + '"]').attr('id', 'edit-btnXoa-' + changNumThis);

                    $('body').find('[id="edit-tr-parent-' + numThis + '"]').attr('name', changNumThis);
                    $('body').find('[id="edit-tr-parent-' + numThis + '"]').attr('id', 'edit-tr-parent-' + changNumThis);
                }
            });
        }
    });

    $('body').on('click', '[id="edit-addRow"]', function () {
        $(this).tooltip("hide");

        var count = $('body').find('[id^="edit-tr-parent-"]').length;
        count++;
        var html = `
            <tr id="edit-tr-parent-`+ count + `" name="` + count + `">
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="edit-mota-`+ count + `" name="` + count + `" class="form-control" placeholder="Mô tả tóm tắt công việc"></textarea>
                    <span class="text-danger" id="valid-edit-mota-`+ count + `"></span>
                </td>
                <td valign="middle" style="width: 80px;" class="pe-0 ps-3">
                    <input type="number" maxlength="3" id="edit-khoiluong-`+ count + `" name="` + count + `" class="form-control" placeholder="Số giờ" />
                    <span class="text-danger" id="valid-edit-khoiluong-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <input type="date" id="edit-thoigian-`+ count + `" name="` + count + `" class="form-control flatpickr" placeholder="Chọn hạn hoàn thành" />
                    <span class="text-danger" id="valid-edit-thoigian-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="edit-noilamviec-`+ count + `" name="` + count + `" class="form-control" placeholder="Giảng đường, tại nhà..."></textarea>
                    <span class="text-danger" id="valid-edit-noilamviec-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="edit-ketqua-`+ count + `" name="` + count + `" class="form-control" placeholder="Kết quả công việc"></textarea>
                    <span class="text-danger" id="valid-edit-ketqua-`+ count + `"></span>
                </td>
                <td valign="middle" class="ps-3 pe-3">
                    <a role="button" id="edit-btnXoa-`+ count + `" name="` + count + `" class="text-danger form-control">
                        <i class="bi bi-trash"></i>
                    </a>
                </td>
            </tr>
            <tr id="edit-row-append">
                <td colspan="7" class="text-center">
                    <a id="edit-addRow" data-bs-toggle="tooltip" data-bs-html="true" title="Thêm hàng mới" class="btn btn-sm btn-secondary">
                        <i style="font-size: 18px" class="bi bi-plus-square-dotted"></i>
                    </a>
                </td>
            </tr>
            `;

        $('body').find('[id="edit-row-append"]').replaceWith(html);
        $('body').find('[id="edit-thoigian-' + count + '"]').flatpickr({ locale: 'vn' });
    });

    $('body').on('click', '[id="btnviewtasklistmodalSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', true);

        var idLHP = $('body').find('[id="edit-idLHP"]').val();

        var mota = "";
        var khoiluong = "";
        var thoigian = "";
        var noilamviec = "";
        var ketqua = "";

        var check = true;
        $('body').find('[id^="valid-edit-mota-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-edit-khoiluong-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-edit-thoigian-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-edit-noilamviec-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-edit-ketqua-"]').each(function () { $(this).text(''); });

        $('body').find('[id^="edit-mota-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                $('body').find('[id^="valid-edit-mota-' + idInp + '"]').text('Vui lòng nhập mô tả.');
            }
            else {
                mota += val + "~";
            }
        });
        $('body').find('[id^="edit-khoiluong-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim().replace(",", ".");
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                $('body').find('[id^="valid-edit-khoiluong-' + idInp + '"]').text('Vui lòng nhập.');
            }
            else {
                khoiluong += val + "~";
            }
        });
        $('body').find('[id^="edit-thoigian-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                $('body').find('[id^="valid-edit-thoigian-' + idInp + '"]').text('Vui lòng nhập hạn hoàn thành.');
            }
            else {
                thoigian += val + "~";
            }
        });
        $('body').find('[id^="edit-noilamviec-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                $('body').find('[id^="valid-edit-noilamviec-' + idInp + '"]').text('Vui lòng nhập nơi làm việc.');
            }
            else {
                noilamviec += val + "~";
            }
        });
        $('body').find('[id^="edit-ketqua-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                $('body').find('[id^="valid-edit-ketqua-' + idInp + '"]').text('Vui lòng nhập kết quả mong đợi.');
            }
            else {
                ketqua += val + "~";
            }
        });

        if (check == true) {
            var formData = new FormData();

            formData.append('idLHP', idLHP);
            formData.append('mota', mota.substring(0, mota.length - 1));
            formData.append('khoiluong', khoiluong.substring(0, khoiluong.length - 1));
            formData.append('thoigian', thoigian.substring(0, thoigian.length - 1));
            formData.append('noilamviec', noilamviec.substring(0, noilamviec.length - 1));
            formData.append('ketqua', ketqua.substring(0, ketqua.length - 1));
            formData.append('camket', $('body').find('[id="editcamket"]').prop('checked'));
            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "ClassSection/EditTaskList",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="viewtasklistmodal"]').modal('toggle');

                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã cập nhật bảng mô tả công việc."
                    }).then(() => {
                        window.location.reload();
                    });
                    
                }
                else if (ketqua.indexOf("Sai định dạng") !== -1) {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Ô khối lượng công việc dòng số " + ketqua.split('-')[1] + " nhập sai định dạng."
                    });
                }
                else if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnviewtasklistmodalClose"]').prop('disabled', false);

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