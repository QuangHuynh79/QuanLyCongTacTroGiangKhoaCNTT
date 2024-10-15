$(document).ready(function () {
    $('body').on('change', '[id="hocky"]', function () {
        FilterData();
    });
    $('body').on('change', '[id="nganh"]', function () {
        FilterData();
    });
    $('body').on('change', '[id="trangthaiut"]', function () {
        FilterData();
    });

    function FilterData() {
        $('body').find('[id="load-fill-page"]').prop('hidden', false);

        $('body').find('[id="filterLoad"]').html('<h3 class="text-center mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var trangthai = $('body').find('[id="trangthaiut"] :selected').val();

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);
        formData.append('trangthai', trangthai);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/FilterRegistered",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="filterLoad"]').replaceWith(ketqua);
            $('body').find('[id="load-fill-page"]').prop('hidden', true);
        });
    }
});