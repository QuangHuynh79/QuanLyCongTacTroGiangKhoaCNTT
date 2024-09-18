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

                    $('body').find('[id="tt-' + numThis + '"]').text(changNumThis);
                    $('body').find('[id="tt-' + numThis + '"]').attr('id', 'tt-' + changNumThis);
                    $('body').find('[id="mota-' + numThis + '"]').attr('id', 'mota-' + changNumThis);
                    $('body').find('[id="khoiluong-' + numThis + '"]').attr('id', 'khoiluong-' + changNumThis);
                    $('body').find('[id="thoigian-' + numThis + '"]').attr('id', 'thoigian-' + changNumThis);
                    $('body').find('[id="noilamviec-' + numThis + '"]').attr('id', 'noilamviec-' + changNumThis);
                    $('body').find('[id="ketqua-' + numThis + '"]').attr('id', 'ketqua-' + changNumThis);

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
                    <div id="tt-`+ count + `" class="form-control">
                        `+ count + `
                    </div>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="mota-`+ count + `" class="form-control" placeholder="Mô tả tóm tắt công việc"></textarea>
                    <span class="text-danger" id="valid-mota-`+ count + `"></span>
                </td>
                <td valign="middle" style="width: 80px;" class="pe-0 ps-3">
                    <input data-type="numbers" type="number" maxlength="3" id="khoiluong-`+ count + `" class="form-control" placeholder="giờ" />
                    <span class="text-danger" id="valid-khoiluong-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <input type="text" id="thoigian-`+ count + `" class="form-control" placeholder="Ngày/buổi/tuần" />
                    <span class="text-danger" id="valid-thoigian-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="noilamviec-`+ count + `" class="form-control" placeholder="Giảng đường, tại nhà..."></textarea>
                    <span class="text-danger" id="valid-noilamviec-`+ count + `"></span>
                </td>
                <td valign="middle" class="pe-0 ps-3">
                    <textarea rows="1" type="text" id="ketqua-`+ count + `" class="form-control" placeholder="Kết quả công việc"></textarea>
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
    });

});