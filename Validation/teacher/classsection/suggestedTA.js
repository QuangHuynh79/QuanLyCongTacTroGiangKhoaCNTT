$(document).ready(function () {
    $('body').on('click', '[id^="btnXoa-"]', function () {
        var count = $('body').find('[id^="tr-parent-"]').length;
        if (count != 1) {
            var numCurrent = $(this).attr('name');
            $('body').find('[id="tr-parent-' + numCurrent + '"]').replaceWith('');

            $('body').find('[id^="tr-parent-"]').each(function () {
                var numThis = $(this).attr('name');

                if (Number(numCurrent) < Number(numThis)) {
                    changNumThis = Number(numThis) - 1;

                    $('body').find('[id="mota-' + numThis + '"]').attr('id', 'mota-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="khoiluong-' + numThis + '"]').attr('id', 'khoiluong-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="thoigian-' + numThis + '"]').attr('id', 'thoigian-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="noilamviec-' + numThis + '"]').attr('id', 'noilamviec-' + changNumThis).attr('name', changNumThis);
                    $('body').find('[id="ketqua-' + numThis + '"]').attr('id', 'ketqua-' + changNumThis).attr('name', changNumThis);

                    $('body').find('[id="valid-mota-' + numThis + '"]').attr('id', 'valid-mota-' + changNumThis);
                    $('body').find('[id="valid-khoiluong-' + numThis + '"]').attr('id', 'valid-khoiluong-' + changNumThis);
                    $('body').find('[id="valid-thoigian-' + numThis + '"]').attr('id', 'valid-thoigian-' + changNumThis);
                    $('body').find('[id="valid-noilamviec-' + numThis + '"]').attr('id', 'valid-noilamviec-' + changNumThis);
                    $('body').find('[id="valid-ketqua-' + numThis + '"]').attr('id', 'valid-ketqua-' + changNumThis);

                    $('body').find('[id="btnXoa-' + numThis + '"]').attr('name', changNumThis);
                    $('body').find('[id="btnXoa-' + numThis + '"]').attr('id', 'btnXoa-' + changNumThis);

                    $('body').find('[id="tr-parent-' + numThis + '"]').attr('name', changNumThis);
                    $('body').find('[id="tr-parent-' + numThis + '"]').attr('id', 'tr-parent-' + changNumThis);
                }
            });
        }
    });

    $('body').on('click', '[id="addRow"]', function () {
        $(this).tooltip("hide");

        var count = $('body').find('[id^="tr-parent-"]').length;
        count++;
        var html = `
            <tr id="tr-parent-`+ count + `" name="` + count + `">
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="mota-`+ count + `" name="` + count + `" class="form-control" placeholder="Mô tả tóm tắt công việc"></textarea>
                    <span class="text-danger" id="valid-mota-`+ count + `"></span>
                </td>
                <td valign="middle" style="width: 80px;" class="pe-0 ps-3">
                    <input type="number" maxlength="3" id="khoiluong-`+ count + `" name="` + count + `" class="form-control" placeholder="Số giờ" />
                    <span class="text-danger" id="valid-khoiluong-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <input type="date" id="thoigian-`+ count + `" name="` + count + `" class="form-control flatpickr" placeholder="Chọn hạn hoàn thành" />
                    <span class="text-danger" id="valid-thoigian-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="noilamviec-`+ count + `" name="` + count + `" class="form-control" placeholder="Giảng đường, tại nhà..."></textarea>
                    <span class="text-danger" id="valid-noilamviec-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="ketqua-`+ count + `" name="` + count + `" class="form-control" placeholder="Kết quả công việc"></textarea>
                    <span class="text-danger" id="valid-ketqua-`+ count + `"></span>
                </td>
                <td valign="middle" class="ps-3 pe-3">
                    <a role="button" id="btnXoa-`+ count + `" name="` + count + `" class="text-danger form-control">
                        <i class="bi bi-trash"></i>
                    </a>
                </td>
            </tr>
            <tr id="row-append">
                <td colspan="7" class="text-center">
                    <a id="addRow" data-bs-toggle="tooltip" data-bs-html="true" title="Thêm hàng mới" class="btn btn-sm btn-secondary">
                        <i style="font-size: 18px" class="bi bi-plus-square-dotted"></i>
                    </a>
                </td>
            </tr>
            `;

        $('body').find('[id="row-append"]').replaceWith(html);
        $('body').find('[id="thoigian-' + count + '"]').flatpickr({ locale: 'vn' });
    });

    $('body').on('change', '[id="camket"]', function () {
        var inp = $(this);
        if (inp.prop('checked')) {
            $('body').find('[id="btnDeXuatSubmit"]').prop('disabled', false);
        }
        else {
            $('body').find('[id="btnDeXuatSubmit"]').prop('disabled', true);
        }
    });

    $('body').on('click', '[id^="dexuat-"]', function () {
        var id = $(this).attr('name');
        var tieude = $(this).attr("tieudeForm");
        $('body').find('[id="dexuattrogiangTitle"]').text(tieude);

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "ClassSection/OpenSuggest",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="loadcontentdexuat"]').replaceWith(ketqua);
                $('body').find('[id="idLHPDX"]').val(id);
                $('body').find('[id="dexuattrogiang"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="SyncTaskList"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);

        $('body').find('[id="valid-dexuatLHPID"]').text('');

        var id = $('body').find('[id="dexuatLHPID"] :selected').val();
        var check = true;
        if (id.length < 1) {
            check = false;

            var btn = $(this);
            btn.html('Sao chép');
            btn.prop('disabled', false);

            $('body').find('[id="valid-dexuatLHPID"]').text('Vui lòng chọn LHP cần sao chép');
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('id', id);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "ClassSection/SyncTaskList",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    Swal.fire({
                        title: "Đã xảy ra lỗi!",
                        text: ketqua,
                        icon: "error"
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    $('body').find('[id="contentAddTaskList"]').replaceWith(ketqua);
                    btn.html('Sao chép');
                    btn.prop('disabled', false);
                }
            });
        }
    });
});