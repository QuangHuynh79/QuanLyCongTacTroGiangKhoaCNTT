$(document).ready(function () {
    $('body').on('click', '[id^="btnOpenAssign-"]', function () {
        var id = $(this).attr('name');

        var formData = new FormData();
        formData.append('lhp', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Assignment/LoadListTA",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="modal-content-assign"]').replaceWith(ketqua);
            $('body').find('[id="phancong"]').modal('toggle');

            if ($('body').find('[id="quantity-ta"]').val() == 0) {
                $('body').find('[id="btnSubmit"]').prop('hidden', true);
            }
            else {
                $('body').find('[id="btnSubmit"]').prop('hidden', false);
            }
        });
    });
});