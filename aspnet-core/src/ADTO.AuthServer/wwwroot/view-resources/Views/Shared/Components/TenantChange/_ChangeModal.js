(function ($) {
    var _accountService = adto.services.app.account;
    var _$form = $('form[name=TenantChangeForm]');

    function switchToSelectedTenant () {
        var tenancyName = _$form.find('input[name=TenancyName]').val();

        if (!tenancyName) {
            adto.multiTenancy.setTenantIdCookie(null);
            location.reload();
            return;
        }

        _accountService.isTenantAvailable({
            tenancyName: tenancyName
        }).done(function (result) {
            switch (result.state) {
                case 1: //Available
                    adto.multiTenancy.setTenantIdCookie(result.tenantId);
                    //_modalManager.close();
                    location.reload();
                    return;
                case 2: //InActive
                    adto.message.warn(adto.utils.formatString(adto.localization
                        .localize("TenantIsNotActive", "AuthServer"),
                        tenancyName));
                    break;
                case 3: //NotFound
                    adto.message.warn(adto.utils.formatString(adto.localization
                        .localize("ThereIsNoTenantDefinedWithName{0}", "AuthServer"),
                        tenancyName));
                    break;
            }
        });
    }

    //Handle save button click
    _$form.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        switchToSelectedTenant();
    });

    //Handle enter key
    _$form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            switchToSelectedTenant();
        }
    });

    $('#TenantChangeModal').on('shown.bs.modal', function () {
        _$form.find('input[type=text]:first').focus();
    });
})(jQuery);
