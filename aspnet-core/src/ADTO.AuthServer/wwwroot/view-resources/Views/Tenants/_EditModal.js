(function ($) {
    var _tenantService = adto.services.app.tenant,
        l = adto.localization.getSource('AuthServer'),
        _$modal = $('#TenantEditModal'),
        _$form = _$modal.find('form');

    function save() {
        if (!_$form.valid()) {
            return;
        }

        var tenant = _$form.serializeFormToObject();

        adto.ui.setBusy(_$form);
        _tenantService.update(tenant).done(function () {
            _$modal.modal('hide');
            adto.notify.info(l('SavedSuccessfully'));
            adto.event.trigger('tenant.edited', tenant);
        }).always(function () {
            adto.ui.clearBusy(_$form);
        });
    }

    _$form.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        save();
    });

    _$form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            save();
        }
    });

    _$modal.on('shown.bs.modal', function () {
        _$form.find('input[type=text]:first').focus();
    });
})(jQuery);
