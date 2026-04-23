var adto = adto || {};
(function () {

    // Check if SignalR is defined
    if (!signalR) {
        return;
    }

    // Create namespaces
    adto.signalr = adto.signalr || {};
    adto.signalr.hubs = adto.signalr.hubs || {};
    adto.signalr.reconnectTime = adto.signalr.reconnectTime || 5000;
    adto.signalr.maxTries = adto.signalr.maxTries || 8;
    adto.signalr.increaseReconnectTime = adto.signalr.increaseReconnectTime || function (time) {
        return time * 2;
    };
    adto.signalr.withUrlOptions = adto.signalr.withUrlOptions || {};

    // Configure the connection for adto.signalr.hubs.common
    function configureConnection(connection) {
        // Set the common hub
        adto.signalr.hubs.common = connection;

        let reconnectTime = adto.signalr.reconnectTime;

        // Register to get notifications
        connection.on('getNotification', function (notification) {
            adto.event.trigger('adto.notifications.received', notification);
        });
    }

    // Connect to the server for adto.signalr.hubs.common
    function connect() {
        var url = adto.signalr.url || (adto.appPath + 'signalr');

        // Start the connection
        startConnection(url, configureConnection)
            .then(function (connection) {
                adto.log.debug('Connected to SignalR server!'); //TODO: Remove log
                adto.event.trigger('adto.signalr.connected');
                // Call the Register method on the hub
                connection.invoke('register').then(function () {
                    adto.log.debug('Registered to the SignalR server!'); //TODO: Remove log
                });
            })
            .catch(function (error) {
                adto.log.debug(error.message);
            });
    }

    // Starts a connection with transport fallback - if the connection cannot be started using
    // the webSockets transport the function will fallback to the serverSentEvents transport and
    // if this does not work it will try longPolling. If the connection cannot be started using
    // any of the available transports the function will return a rejected Promise.
    function startConnection(url, configureConnection) {
        if (adto.signalr.remoteServiceBaseUrl) {
            url = adto.signalr.remoteServiceBaseUrl + url;
        }

        // Add query string: https://github.com/aspnet/SignalR/issues/680
        if (adto.signalr.qs) {
            url += (url.indexOf('?') == -1 ? '?' : '&') + adto.signalr.qs;
        }

        return function start(transport) {
            adto.log.debug('Starting connection using ' + signalR.HttpTransportType[transport] + ' transport');
            adto.signalr.withUrlOptions.transport = transport;
            var connection = new signalR.HubConnectionBuilder()
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: retryContext => {
                        adto.log.debug('Retry to connect to SignalR');
                        if (retryContext.previousRetryCount > maxTries) {
                            adto.log.debug('Max retries reached');
                            return null;
                        }
                        reconnectTime *= 2;
                        adto.log.debug('Waiting ' + reconnectTime + 'ms before retrying');
                        return reconnectTime;
                    }
                })
                .withUrl(url, adto.signalr.withUrlOptions)
                .build();

            if (configureConnection && typeof configureConnection === 'function') {
                configureConnection(connection);
            }

            return connection.start()
                .then(function () {
                    return connection;
                })
                .catch(function (error) {
                    adto.log.debug('Cannot start the connection using ' + signalR.HttpTransportType[transport] + ' transport. ' + error.message);
                    if (transport !== signalR.HttpTransportType.LongPolling) {
                        return start(transport + 1);
                    }

                    return Promise.reject(error);
                });
        }(signalR.HttpTransportType.WebSockets);
    }

    adto.signalr.autoConnect = adto.signalr.autoConnect === undefined ? true : adto.signalr.autoConnect;
    adto.signalr.autoReconnect = adto.signalr.autoReconnect === undefined ? true : adto.signalr.autoReconnect;
    adto.signalr.connect = adto.signalr.connect || connect;
    adto.signalr.startConnection = adto.signalr.startConnection || startConnection;

    if (adto.signalr.autoConnect && !adto.signalr.hubs.common) {
        adto.signalr.connect();
    }
})();
