$(document).ready(function () {

    $('body').on('click', '[id^="viewKetQuaDiemDanh-"]', function () {
        var id = $(this).attr('name');
        var tieude = $(this).attr("tieudeForm");
        $('body').find('[id="ketquadiemdanhmodalTitle"]').text(tieude);

        var formData = new FormData();
        formData.append('idLhp', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "ClassManagement/ResultRollCall",
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
                $('body').find('[id="loadcontentketquadiemdanhmodal"]').replaceWith(ketqua);
                $('body').find('[id="tableKetQuaDiemDanh"]').DataTable({
                    pageLength: 100,
                    scrollX: true
                });
                $('body').find('[id="ketquadiemdanhmodal"]').modal('toggle');
            }
        });
    });
});