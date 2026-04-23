(function () {
    $('.tenant-change-component a')
        .click(function (e) {
            e.preventDefault();
            adto.ajax({
                url: adto.appPath + 'Account/TenantChangeModal',
                type: 'POST',
                dataType: 'html',
                success: function (content) {
                    $('#TenantChangeModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });
})();
