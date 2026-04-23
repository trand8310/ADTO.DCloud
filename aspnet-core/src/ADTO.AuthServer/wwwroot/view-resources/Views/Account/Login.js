(function () {
    $('#ReturnUrlHash').val(location.hash);

    var _$form = $('#LoginForm');

    _$form.submit(function (e) {
        e.preventDefault();

        if (!_$form.valid()) {
            return;
        }

        adto.ui.setBusy(
            $('body'),

            adto.ajax({
                contentType: 'application/x-www-form-urlencoded',
                url: _$form.attr('action'),
                data: _$form.serialize()
            })
        );
    });
})();
