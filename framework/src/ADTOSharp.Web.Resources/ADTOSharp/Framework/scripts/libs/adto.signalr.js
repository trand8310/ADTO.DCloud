var adto = adto || {};
(function ($) {

    //Check if SignalR is defined
    if (!$ || !$.connection) {
        return;
    }

    //Create namespaces
    adto.signalr = adto.signalr || {};
    adto.signalr.hubs = adto.signalr.hubs || {};

    //Get the common hub
    adto.signalr.hubs.common = $.connection.adtoCommonHub;

    var commonHub = adto.signalr.hubs.common;
    if (!commonHub) {
        return;
    }

    //Register to get notifications
    commonHub.client.getNotification = function (notification) {
        adto.event.trigger('adto.notifications.received', notification);
    };

    //Connect to the server
    adto.signalr.connect = function() {
        $.connection.hub.start().done(function () {
            adto.log.debug('Connected to SignalR server!'); //TODO: Remove log
            adto.event.trigger('adto.signalr.connected');
            commonHub.server.register().done(function () {
                adto.log.debug('Registered to the SignalR server!'); //TODO: Remove log
            });
        });
    };

    if (adto.signalr.autoConnect === undefined) {
        adto.signalr.autoConnect = true;
    }

    if (adto.signalr.autoConnect) {
        adto.signalr.connect();
    }

    //reconnect if hub disconnects
    $.connection.hub.disconnected(function () {
        if (!adto.signalr.autoConnect) {
            return;
        }

        setTimeout(function () {
            if ($.connection.hub.state === $.signalR.connectionState.disconnected) {
                $.connection.hub.start();
            }
        }, 5000);
    });

})(jQuery);