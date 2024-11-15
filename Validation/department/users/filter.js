$(document).ready(function () {
    $('body').on('change', '[id="filterchucdanh"]', function () {
        FilterParentData();
    });

    function FilterParentData() {
        $('body').find('[id="filterParentLoad"]').html('<h3 class="text-center mt-4 mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var id = $('body').find('[id="filterchucdanh"] :selected').val();

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Users/Filter",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="filterParentLoad"]').replaceWith(ketqua);
            $('body').find('.table').DataTable();
        });
    }
});