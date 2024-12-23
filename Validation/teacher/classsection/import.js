$(document).ready(function () {
    $('body').on('click', '[id^="btnimportsv-"]', function () {
        var btn = $(this);
        var idLhp = btn.attr('name');
        var title = btn.attr('tieudeForm');

        $('body').find('[id="fileImportDssv"]').attr('name', idLhp);
        $('body').find('[id="fileImportDssv"]').val('');
        $('body').find('[id="importsvTitle"]').text(title);

        $('body').find('[id="importsv"]').modal('toggle');

    });

    $('body').on('click', '[id="btnSubmitImport"]', function () {
        var btn = $(this);
        var idLhp = $('body').find('[id="fileImportDssv"]').attr('name');
        var fileImport = $('body').find('[id="fileImportDssv"]')[0].files;
        var fileName = fileImport[0].name;

        var check = true;
        if (fileImport.length < 1) {
            check = false;
            $('body').find('[id="valid-import"]').text("Vui lòng chọn file import.");
        }
        else if (fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1).toLowerCase() == "xlsx"
            || fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1).toLowerCase() == "xls") {

            check = false;
            $('body').find('[id="valid-import"]').text("Vui lòng chọn file excel có định dạng xlsx, xls.");
        }

        if (check == true) {
            btn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang import...');

            var formData = new FormData();
            formData.append('fileImport', fileImport[0]);
            formData.append('idLhp', idLhp);
            formData.append('confirm', '');

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "ClassManagement/ImportStudient",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false
            }).done(function (ketqua) {
                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    btn.html('Import');

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua.indexOf("Có vẻ như bạn đã sai") != -1) {
                    btn.html('Import');

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    });

                }
                else if (ketqua.indexOf("Đã có lỗi") != -1) {
                    btn.html('Import');

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua == "Exist") {
                    Swal.fire({
                        text: 'Danh sách sinh viên đã có dữ liệu trong hệ thống, bạn muốn cập nhật hay thay thế danh sách sinh viên?',
                        icon: "question",
                        showCancelButton: true,
                        cancelButtonColor: "#d33",
                        showDenyButton: true,
                        confirmButtonText: "Cập nhật",
                        denyButtonText: `Thay thế`,
                        denyButtonColor: '#198754',
                        cancelButtonText: "Hủy"
                    }).then((result) => {
                        if (result.isConfirmed) { //Cập nhật
                            formData = new FormData();
                            formData.append('fileImport', fileImport[0]);
                            formData.append('idLhp', idLhp);
                            formData.append('confirm', 'addnew');

                            $.ajax({
                                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                                url: $('#requestPath').val() + "ClassManagement/ImportStudient",
                                data: formData,
                                dataType: 'html',
                                type: 'POST',
                                processData: false,
                                contentType: false
                            }).done(function (ketqua) {
                                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: ketqua
                                    }).then(() => {
                                        window.location.reload();
                                    });
                                }
                                else if (ketqua.indexOf("Đã có lỗi") != -1) {
                                    btn.html('Import');
                                    Toast.fire({
                                        icon: "error",
                                        title: ketqua
                                    }).then(() => {
                                        window.location.reload();
                                    });

                                }
                                else if (ketqua == "more50mb") { //File quá lớn không thể upload
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: 'Kích thước file vượt quá 50MB vui lòng import file <= 50MB.'
                                    });
                                }
                                else if (ketqua.indexOf("Có vẻ như bạn đã sai") != -1) {
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: ketqua
                                    });
                                }
                                else if (ketqua == "INCORRECT") { // Mẫu import không hợp lệ
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: 'Mẫu file import không hợp lệ vui lòng kiểm tra lại.'
                                    });

                                }
                                else {
                                    btn.html('Import');

                                    $('body').find('[id="importsv"]').modal('toggle');

                                    Toast.fire({
                                        icon: "success",
                                        title: 'Đã cập nhật danh sách sinh viên!'
                                    }).then(() => {
                                        window.location.reload();
                                    });

                                }
                            });
                        }
                        else if (result.isDenied) { //Thay thế
                            formData = new FormData();
                            formData.append('fileImport', fileImport[0]);
                            formData.append('idLhp', idLhp);
                            formData.append('confirm', 'replace');

                            $.ajax({
                                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                                url: $('#requestPath').val() + "ClassManagement/ImportStudient",
                                data: formData,
                                dataType: 'html',
                                type: 'POST',
                                processData: false,
                                contentType: false
                            }).done(function (ketqua) {
                                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                                    btn.html('Import');

                                    Swal.fire({
                                        title: "Thất bại!",
                                        text: ketqua,
                                        icon: "error"
                                    }).then(() => {
                                        window.location.reload();
                                    });
                                }
                                else if (ketqua.indexOf("Đã có lỗi") != -1) {
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: ketqua
                                    }).then(() => {
                                        window.location.reload();
                                    });
                                }
                                else if (ketqua.indexOf("Có vẻ như bạn đã sai") != -1) {
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: ketqua
                                    });
                                }
                                else if (ketqua == "more50mb") { //File quá lớn không thể upload
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: 'Kích thước file vượt quá 50MB vui lòng import file <= 50MB.'
                                    });
                                }
                                else if (ketqua == "INCORRECT") { // Mẫu import không hợp lệ
                                    btn.html('Import');

                                    Toast.fire({
                                        icon: "error",
                                        title: 'Mẫu file import không hợp lệ vui lòng kiểm tra lại.'
                                    });
                                }
                                else {
                                    btn.html('Import');

                                    $('body').find('[id="importsv"]').modal('toggle');

                                    Toast.fire({
                                        icon: "success",
                                        title: 'Đã thay thế danh sách sinh viên!'
                                    }).then(() => {
                                        window.location.reload();
                                    });
                                }
                            });
                        }
                    });
                }
                else if (ketqua == "more50mb") { //File quá lớn không thể upload
                    btn.html('Import');

                    Toast.fire({
                        icon: "error",
                        title: 'Kích thước file vượt quá 50MB vui lòng import file <= 50MB.'
                    });

                }
                else if (ketqua == "INCORRECT") { // Mẫu import không hợp lệ
                    btn.html('Import');

                    Toast.fire({
                        icon: "error",
                        title: 'Mẫu file import không hợp lệ vui lòng kiểm tra lại.'
                    });
                }
                else {
                    btn.html('Import');

                    $('body').find('[id="importsv"]').modal('toggle');

                    Toast.fire({
                        icon: "success",
                        title: 'Đã import danh sách sinh viên'
                    }).then(() => {
                        window.location.reload();
                    });

                }
            });
        }
    });
});
