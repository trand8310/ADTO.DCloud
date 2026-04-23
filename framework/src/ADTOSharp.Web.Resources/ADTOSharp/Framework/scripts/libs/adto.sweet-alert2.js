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

        options.reverseButtons = true;

        if (options.isHtml) {
            options.html = message;
        } else {
            options.text = message;
        }

        var opts = $.extend(
            {},
            adto.libs.sweetAlert.config['default'],
            adto.libs.sweetAlert.config[type],
            messageContent,
            options
        );

        return $.Deferred(($dfd) => {
            Swal.fire(opts).then((result) => {
                callback && callback(result);
                $dfd.resolve(result)
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
        options = options || {};
        options.showCancelButton = true;

        const confirmFunc = (result) => {
            let isCancelled = result.dismiss === Swal.DismissReason.cancel;

            return callback && callback(result.isConfirmed, isCancelled);
        }

        return showMessage('confirm', message, title, confirmFunc, options);
    };

    adto.event.on('adto.dynamicScriptsInitialized', function () {
        adto.libs.sweetAlert.config.confirm.title = adto.localization.adtoWeb('AreYouSure');
        adto.libs.sweetAlert.config.confirm.confirmButtonText = adto.localization.adtoWeb('Yes');
        adto.libs.sweetAlert.config.confirm.cancelButtonText = adto.localization.adtoWeb('Cancel');
        adto.libs.sweetAlert.config.confirm.denyButtonText = adto.localization.adtoWeb('No');
    });

})(jQuery);