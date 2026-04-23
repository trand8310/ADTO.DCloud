(function ($) {
    var _userService = adto.services.app.user,
        l = adto.localization.getSource('AuthServer'),
        _$modal = $('#UserEditModal'),
        _$form = _$modal.find('form');

    function save() {
        if (!_$form.valid()) {
            return;
        }

        var user = _$form.serializeFormToObject();
        user.roleNames = [];
        var _$roleCheckboxes = _$form[0].querySelectorAll("input[name='role']:checked");
        if (_$roleCheckboxes) {
            for (var roleIndex = 0; roleIndex < _$roleCheckboxes.length; roleIndex++) {
                var _$roleCheckbox = $(_$roleCheckboxes[roleIndex]);
                user.roleNames.push(_$roleCheckbox.val());
            }
        }

        adto.ui.setBusy(_$form);
        _userService.update(user).done(function () {
            _$modal.modal('hide');
            adto.notify.info(l('SavedSuccessfully'));
            adto.event.trigger('user.edited', user);
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
