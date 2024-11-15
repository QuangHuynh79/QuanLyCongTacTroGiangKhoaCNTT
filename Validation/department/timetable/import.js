$(document).ready(function () {

    var isAdvancedUpload = function () {
        var div = document.createElement('div');
        return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
    }();

    let draggableFileArea = document.querySelector(".drag-file-area");
    let browseFileText = document.querySelector(".browse-files");
    let uploadIcon = document.querySelector(".upload-icon");
    let dragDropText = document.querySelector(".dynamic-message");
    let fileInput = document.querySelector(".default-file-input");
    let cannotUploadMessage = document.querySelector(".cannot-upload-message");
    let cancelAlertButton = document.querySelector(".cancel-alert-button");
    let uploadedFile = document.querySelector(".file-block");
    let fileName = document.querySelector(".file-name");
    let fileSize = document.querySelector(".file-size");
    let progressBar = document.querySelector(".progress-bar");
    let removeFileButton = document.querySelector(".remove-file-icon");
    let uploadButton = document.querySelector(".upload-button");
    let fileFlag = 0;

    $("body").on('change', ".default-file-input", function () {
        var fileName = $(this)[0].files[0].name;
        if (fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1) == ".xls" || fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1) == ".xlsx") {
            $('body').find(".upload-icon").html('check_circle');
            $('body').find(".dynamic-message").html('Đã chọn file Import');
            $('body').find(".label").html(`Kéo & thả file import <br> <span>hoặc</span><br/><span class="browse-files">  <span class="browse-files-text" style="top: 0;"> chọn file </span></span> từ thiết bị`);
            $('body').find(".upload-button").html('Import');
            $('body').find(".file-name").html(fileName);
            $('body').find(".file-size").html(($(this)[0].files[0].size / 1024).toFixed(1) + " KB");
            $('body').find(".file-block").css('display', 'flex');
            $('body').find(".progress-bar").css("width", 0 + "px");
            $('body').find(".cannot-upload-message").css('display', 'none');
            fileFlag = 0;
        }
        else {
            $('body').find(".upload-icon").html('file_upload');
            $('body').find(".cannot-upload-message").html(`<span class="material-icons-outlined">error</span> Chỉ có thể upload file Excel <span class="material-icons-outlined cancel-alert-button">cancel</span>`);
            $('body').find(".cannot-upload-message").css("display", "flex");
            $('body').find(".cannot-upload-message").css("animation", "fadeIn linear 1.5s");
        }

    });

    $('body').on('click', '.browse-files', function () {
        $('body').find('.default-file-input').click();
    });

    $('body').on('click', ".upload-button", function () {
        var btn = $(this);
        var hocky = $('body').find('[id="importhocky"] :selected').val();
        var nganh = $('body').find('[id="importnganh"] :selected').val();
        var checks = true;
        if (hocky.length < 1) {
            checks = false;

            Toast.fire({
                icon: "warning",
                title: "Chưa chọn học kỳ!"
            });

            return false;
        }

        if (nganh.length < 1) {
            checks = false;

            Toast.fire({
                icon: "warning",
                title: "Chưa chọn ngành!"
            });

            return false;
        }

        if (checks == true) {
            var InpFile = $('body').find('.default-file-input');
            var isFileUploaded = InpFile[0].files;
            if (isFileUploaded.length > 0) {
                $('body').find(".remove-file-icon").prop('hidden', true);
                btn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang import...');

                var formData = new FormData();
                formData.append('fileImport', isFileUploaded[0]);
                formData.append('hocky', hocky);
                formData.append('nganh', nganh);
                formData.append('confirm', '');

                $.ajax({
                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                    url: $('#requestPath').val() + "TimeTable/SubmitImport",
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
                    else if (ketqua == "NOTEXISTNGANH") {
                        btn.html('Import');

                        Toast.fire({
                            icon: "error",
                            title: "Ngành đã bị xóa bỏ khỏi hệ thống!"
                        }).then(() => {
                            window.location.reload();
                        });
                    }
                    else if (ketqua == "NOTEXISTHOCKY") {
                        btn.html('Import');

                        Toast.fire({
                            icon: "error",
                            title: "Học kỳ đã bị xóa bỏ khỏi hệ thống!"
                        }).then(() => {
                            window.location.reload();
                        });
                    }
                    else if (ketqua == "Close") {
                        btn.html('Import');
                        
                        Toast.fire({
                            icon: "error",
                            title: "Học kỳ đã đóng, không thể cập nhật thời khóa biểu."
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
                            text: 'Học kỳ và ngành này đã có dữ liệu trong hệ thống, bạn muốn cập nhật hay thay thế thời khoá biểu?',
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
                                formData.append('fileImport', isFileUploaded[0]);
                                formData.append('hocky', hocky);
                                formData.append('nganh', nganh);
                                formData.append('confirm', 'addnew');

                                $.ajax({
                                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                                    url: $('#requestPath').val() + "TimeTable/SubmitImport",
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
                                    else if (ketqua == "NOTEXISTNGANH") {
                                        btn.html('Import');

                                        Toast.fire({
                                            icon: "error",
                                            title: "Ngành đã bị xóa bỏ khỏi hệ thống!"
                                        }).then(() => {
                                            window.location.reload();
                                        });
                                    }
                                    else if (ketqua == "NOTEXISTHOCKY") {
                                        btn.html('Import');

                                        Toast.fire({
                                            icon: "error",
                                            title: "Học kỳ đã bị xóa bỏ khỏi hệ thống!"
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
                                        btn.html('<span class="material-icons-outlined upload-button-icon"> check_circle </span> Đã import');

                                        Toast.fire({
                                            icon: "success",
                                            title: 'Đã cập nhật thông tin thời khóa biểu!'
                                        }).then(() => {
                                            window.location.reload();
                                        });
                                       
                                    }
                                });
                            }
                            else if (result.isDenied) { //Thay thế
                                formData = new FormData();
                                formData.append('fileImport', isFileUploaded[0]);
                                formData.append('hocky', hocky);
                                formData.append('nganh', nganh);
                                formData.append('confirm', 'replace');

                                $.ajax({
                                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                                    url: $('#requestPath').val() + "TimeTable/SubmitImport",
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
                                    else if (ketqua == "NOTEXISTNGANH") {
                                        btn.html('Import');

                                        Toast.fire({
                                            icon: "error",
                                            title: "Ngành đã bị xóa bỏ khỏi hệ thống!"
                                        }).then(() => {
                                            window.location.reload();
                                        });
                                    }
                                    else if (ketqua == "NOTEXISTHOCKY") {
                                        btn.html('Import');

                                        Toast.fire({
                                            icon: "error",
                                            title: "Học kỳ đã bị xóa bỏ khỏi hệ thống!"
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
                                        btn.html('<span class="material-icons-outlined upload-button-icon"> check_circle </span> Đã import');

                                        Toast.fire({
                                            icon: "success",
                                            title: 'Đã thay thế thông tin thời khóa biểu!'
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
                        btn.html('<span class="material-icons-outlined upload-button-icon"> check_circle </span> Đã import');

                        Toast.fire({
                            icon: "success",
                            title: 'Đã import thời khóa biểu mới!'
                        }).then(() => {
                            window.location.reload();
                        });
                        
                    }
                });
            } else {
                $('body').find(".upload-icon").html('file_upload');
                $('body').find(".cannot-upload-message").css("display", "flex");
                $('body').find(".cannot-upload-message").css("animation", "fadeIn linear 1.5s");
            }
        }

    });

    $('body').on('click', ".cancel-alert-button", function () {
        $('body').find(".cannot-upload-message").css('display', 'none');
        $('body').find(".cannot-upload-message").html(`<span class="material-icons-outlined">error</span> Vui lòng chọn file import trước <span class="material-icons-outlined cancel-alert-button">cancel</span>`);
    });

    if (isAdvancedUpload) {
        ["drag", "dragstart", "dragend", "dragover", "dragenter", "dragleave", "drop"].forEach(evt =>
            draggableFileArea.addEventListener(evt, e => {
                e.preventDefault();
                e.stopPropagation();
            })
        );

        ["dragover", "dragenter"].forEach(evt => {
            draggableFileArea.addEventListener(evt, e => {
                e.preventDefault();
                e.stopPropagation();
                $('body').find(".upload-icon").html('file_download');
                $('body').find(".dynamic-message").html('Kéo & thả file import');
            });
        });

        draggableFileArea.addEventListener("drop", e => {
            let files = e.dataTransfer.files;
            fileName = files[0].name;
            if (fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1) == ".xls" || fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1) == ".xlsx") {
                $('body').find(".upload-icon").html('check_circle');
                $('body').find(".dynamic-message").html('Đã chọn file Import!');
                $('body').find(".label").html('Kéo & thả file import <br> hoặc <br><span class="browse-files"> <span class="browse-files-text" style="top: 0;"> chọn file </span></span> từ thiết bị');
                $('body').find(".upload-button").html('Import');

                fileInput.files = files;
                $('body').find(".file-name").html(files[0].name);
                $('body').find(".file-size").html((files[0].size / 1024).toFixed(1) + " KB");
                $('body').find(".file-block").css('display', 'flex');
                $('body').find(".progress-bar").css('width', '0');
                fileFlag = 0;
            }
            else {
                $('body').find(".upload-icon").html('file_upload');
                $('body').find(".cannot-upload-message").html(`<span class="material-icons-outlined">error</span> Chỉ có thể upload file Excel <span class="material-icons-outlined cancel-alert-button">cancel</span>`);
                $('body').find(".cannot-upload-message").css("display", "flex");
                $('body').find(".cannot-upload-message").css("animation", "fadeIn linear 1.5s");
            }
        });
    }

    $('body').on('click', ".remove-file-icon", function () {
        $('body').find(".file-block").css('display', 'none');
        $('body').find(".default-file-input").val('');
        $('body').find(".upload-icon").html('file_upload');
        $('body').find(".dynamic-message").html('Kéo & thả file import');
        $('body').find(".label").html(`<span>Hoặc</span><br/><br/><span class="browse-files"> <span class="browse-files-text">chọn file</span> <span>từ thiết bị</span> </span>`);
        $('body').find(".upload-button").html(`Import`);
    });
});
