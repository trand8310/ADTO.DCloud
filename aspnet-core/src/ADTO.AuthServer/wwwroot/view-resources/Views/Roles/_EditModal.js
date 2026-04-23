(function ($) {
    var _roleService = adto.services.app.role,
        l = adto.localization.getSource('AuthServer'),
        _$modal = $('#RoleEditModal'),
        _$form = _$modal.find('form');

    function save() {
        if (!_$form.valid()) {
            return;
        }

        var role = _$form.serializeFormToObject();
        role.grantedPermissions = [];
        var _$permissionCheckboxes = _$form[0].querySelectorAll("input[name='permission']:checked");
        if (_$permissionCheckboxes) {
            for (var permissionIndex = 0; permissionIndex < _$permissionCheckboxes.length; permissionIndex++) {
                var _$permissionCheckbox = $(_$permissionCheckboxes[permissionIndex]);
                role.grantedPermissions.push(_$permissionCheckbox.val());
            }
        }

        adto.ui.setBusy(_$form);
        _roleService.update(role).done(function () {
            _$modal.modal('hide');
            adto.notify.info(l('SavedSuccessfully'));
            adto.event.trigger('role.edited', role);
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
