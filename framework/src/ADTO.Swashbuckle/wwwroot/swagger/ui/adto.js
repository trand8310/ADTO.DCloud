var adto = adto || {};
(function () {

    /* Application paths *****************************************/

    //Current application root path (including virtual directory if exists).
    var baseElement = document.querySelector('base');
    var baseHref = baseElement ? baseElement.getAttribute('href') : null;
    adto.appPath = baseHref || adto.appPath || '/';


    adto.auth = adto.auth || {};

    adto.auth.tokenCookieName = 'ADTOSharp.AuthToken';
    adto.auth.tokenHeaderName = 'Authorization';

    adto.auth.setToken = function (authToken, expireDate) {
        adto.utils.setCookieValue(adto.auth.tokenCookieName, authToken, expireDate, adto.appPath);
    };

    adto.auth.getToken = function () {
        return adto.utils.getCookieValue(adto.auth.tokenCookieName);
    }

    adto.auth.clearToken = function () {
        adto.auth.setToken();
    }




    /* UTILS ***************************************************/

    adto.utils = adto.utils || {};

    /**
     * Sets a cookie value for given key.
     * This is a simple implementation created to be used by ADTO.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} value
     * @param {Date} expireDate (optional). If not specified the cookie will expire at the end of session.
     * @param {string} path (optional)
     */
    adto.utils.setCookieValue = function (key, value, expireDate, path) {
        var cookieValue = encodeURIComponent(key) + '=';

        if (value) {
            cookieValue = cookieValue + encodeURIComponent(value);
        }

        if (expireDate) {
            cookieValue = cookieValue + "; expires=" + expireDate.toUTCString();
        }

        if (path) {
            cookieValue = cookieValue + "; path=" + path;
        }

        document.cookie = cookieValue;
    };

    /**
     * Gets a cookie with given key.
     * This is a simple implementation created to be used by ADTO.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @returns {string} Cookie value or null
     */
    adto.utils.getCookieValue = function (key) {
        var equalities = document.cookie.split('; ');
        for (var i = 0; i < equalities.length; i++) {
            if (!equalities[i]) {
                continue;
            }

            var splitted = equalities[i].split('=');
            if (splitted.length != 2) {
                continue;
            }

            if (decodeURIComponent(splitted[0]) === key) {
                return decodeURIComponent(splitted[1] || '');
            }
        }

        return null;
    };

    /**
     * Deletes cookie for given key.
     * This is a simple implementation created to be used by ADTO.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} path (optional)
     */
    adto.utils.deleteCookie = function (key, path) {
        var cookieValue = encodeURIComponent(key) + '=';

        cookieValue = cookieValue + "; expires=" + (new Date(new Date().getTime() - 86400000)).toUTCString();

        if (path) {
            cookieValue = cookieValue + "; path=" + path;
        }

        document.cookie = cookieValue;
    }

    /* SECURITY ***************************************/
    adto.security = adto.security || {};
    adto.security.antiForgery = adto.security.antiForgery || {};

    adto.security.antiForgery.tokenCookieName = 'XSRF-TOKEN';
    //adto.security.antiForgery.tokenHeaderName = 'RequestVerificationToken';
    adto.security.antiForgery.tokenHeaderName = 'X-XSRF-TOKEN';

    adto.security.antiForgery.getToken = function () {
        return adto.utils.getCookieValue(adto.security.antiForgery.tokenCookieName);
    };

})();
