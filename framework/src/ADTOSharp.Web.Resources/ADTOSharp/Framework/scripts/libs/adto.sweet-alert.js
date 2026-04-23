var adto = adto || {};
(function ($) {
    if (!sweetAlert || !$) {
        return;
    }

    /* DEFAULTS *************************************************/

    adto.libs = adto.libs || {};
    adto.libs.sweetAlert = {
        config: {
            'default': {

            },
            info: {
                icon: 'info'
            },
            success: {
                icon: 'success'
            },
            warn: {
                icon: 'warning'
            },
            error: {
                icon: 'error'
            },
            confirm: {
                icon: 'warning',
                title: 'Are you sure?',
                buttons: ['Cancel', 'Yes']
            }
        }
    };

    /* MESSAGE **************************************************/

    var showMessage = function (type, message, title, callback, options) {
        options = options || {};
        var messageContent = {};
        if(title){
            messageContent.title = title;
        }

        if (options.isHtml) {
            delete options.isHtml;
            var el = document.createElement('div');
            //https://github.com/t4t5/sweetalert/issues/842
            el.style = 'position: relative;';
            el.innerHTML = message;

            messageContent.content = el;
        } else {
            messageContent.text = message;
        }

        var opts = $.extend(
            {},
            adto.libs.sweetAlert.config['default'],
            adto.libs.sweetAlert.config[type],
            messageContent,
            options
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts).then(function (isConfirmed) {
                callback && callback(isConfirmed);
                $dfd.resolve(isConfirmed);
            });
        });
    };

    adto.message.info = function (message, title, options) {
        return showMessage('info', message, title, null, options);
    };

    adto.message.success = function (message, title, options) {
        return showMessage('success', message, title, null, options);
    };

    adto.message.warn = function (message, title, options) {
        return showMessage('warn', message, title, null, options);
    };

    adto.message.error = function (message, title, options) {
        return showMessage('error', message, title, null, options);
    };

    adto.message.confirm = function (message, title, callback, options) {
        return showMessage('confirm', message, title, callback, options);
    };

    adto.event.on('adto.dynamicScriptsInitialized', function () {
        adto.libs.sweetAlert.config.confirm.title = adto.localization.adtoWeb('AreYouSure');
        adto.libs.sweetAlert.config.confirm.buttons = [adto.localization.adtoWeb('Cancel'), adto.localization.adtoWeb('Yes')];
    });

})(jQuery);