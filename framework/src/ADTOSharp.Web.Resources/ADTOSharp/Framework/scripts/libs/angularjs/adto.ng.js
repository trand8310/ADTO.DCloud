(function (adto, angular) {

    if (!angular) {
        return;
    }

    adto.ng = adto.ng || {};

    adto.ng.http = {
        defaultError: {
            message: 'An error has occurred!',
            details: 'Error detail not sent by server.'
        },

        defaultError401: {
            message: 'You are not authenticated!',
            details: 'You should be authenticated (sign in) in order to perform this operation.'
        },

        defaultError403: {
            message: 'You are not authorized!',
            details: 'You are not allowed to perform this operation.'
        },

        defaultError404: {
            message: 'Resource not found!',
            details: 'The resource requested could not be found on the server.'
        },

        logError: function (error) {
            adto.log.error(error);
        },

        showError: function (error) {
            if (error.details) {
                return adto.message.error(error.details, error.message || adto.ng.http.defaultError.message);
            } else {
                return adto.message.error(error.message || adto.ng.http.defaultError.message);
            }
        },

        handleTargetUrl: function (targetUrl) {
            if (!targetUrl) {
                location.href = adto.appPath;
            } else {
                location.href = targetUrl;
            }
        },

        handleNonADTOSharpErrorResponse: function (response, defer) {
            if (response.config.adtoHandleError !== false) {
                switch (response.status) {
                    case 401:
                        adto.ng.http.handleUnAuthorizedRequest(
                            adto.ng.http.showError(adto.ng.http.defaultError401),
                            adto.appPath
                        );
                        break;
                    case 403:
                        adto.ng.http.showError(adto.ajax.defaultError403);
                        break;
                    case 404:
                        adto.ng.http.showError(adto.ajax.defaultError404);
                        break;
                    default:
                        adto.ng.http.showError(adto.ng.http.defaultError);
                        break;
                }
            }

            defer.reject(response);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
            if (messagePromise) {
                messagePromise.done(function () {
                    adto.ng.http.handleTargetUrl(targetUrl || adto.appPath);
                });
            } else {
                adto.ng.http.handleTargetUrl(targetUrl || adto.appPath);
            }
        },

        handleResponse: function (response, defer) {
            var originalData = response.data;

            if (originalData.success === true) {
                response.data = originalData.result;
                defer.resolve(response);

                if (originalData.targetUrl) {
                    adto.ng.http.handleTargetUrl(originalData.targetUrl);
                }
            } else if (originalData.success === false) {
                var messagePromise = null;

                if (originalData.error) {
                    if (response.config.adtoHandleError !== false) {
                        messagePromise = adto.ng.http.showError(originalData.error);
                    }
                } else {
                    originalData.error = defaultError;
                }

                adto.ng.http.logError(originalData.error);

                response.data = originalData.error;
                defer.reject(response);

                if (response.status == 401 && response.config.adtoHandleError !== false) {
                    adto.ng.http.handleUnAuthorizedRequest(messagePromise, originalData.targetUrl);
                }
            } else { //not wrapped result
                defer.resolve(response);
            }
        }
    }

    var adtoModule = angular.module('adto', []);

    adtoModule.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push(['$q', function ($q) {

                return {

                    'request': function (config) {
                        if (config.url.indexOf('.cshtml') !== -1) {
                            config.url = adto.appPath + 'ADTOSharpAppView/Load?viewUrl=' + config.url + '&_t=' + adto.pageLoadTime.getTime();
                        }

                        return config;
                    },

                    'response': function (response) {
                        if (!response.data || !response.data.__adto) {
                            //Non ADTO related return value
                            return response;
                        }

                        var defer = $q.defer();
                        adto.ng.http.handleResponse(response, defer);
                        return defer.promise;
                    },

                    'responseError': function (ngError) {
                        var defer = $q.defer();

                        if (!ngError.data || !ngError.data.__adto) {
                            adto.ng.http.handleNonADTOSharpErrorResponse(ngError, defer);
                        } else {
                            adto.ng.http.handleResponse(ngError, defer);
                        }

                        return defer.promise;
                    }

                };
            }]);
        }
    ]);

    adto.event.on('adto.dynamicScriptsInitialized', function () {
        adto.ng.http.defaultError.message = adto.localization.adtoWeb('DefaultError');
        adto.ng.http.defaultError.details = adto.localization.adtoWeb('DefaultErrorDetail');
        adto.ng.http.defaultError401.message = adto.localization.adtoWeb('DefaultError401');
        adto.ng.http.defaultError401.details = adto.localization.adtoWeb('DefaultErrorDetail401');
        adto.ng.http.defaultError403.message = adto.localization.adtoWeb('DefaultError403');
        adto.ng.http.defaultError403.details = adto.localization.adtoWeb('DefaultErrorDetail403');
        adto.ng.http.defaultError404.message = adto.localization.adtoWeb('DefaultError404');
        adto.ng.http.defaultError404.details = adto.localization.adtoWeb('DefaultErrorDetail404');
    });

})((adto || (adto = {})), (angular || undefined));