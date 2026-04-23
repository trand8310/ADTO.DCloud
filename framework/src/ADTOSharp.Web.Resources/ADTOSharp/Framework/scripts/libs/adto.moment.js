var adto = adto || {};
(function () {
    if (!moment || !moment.tz) {
        return;
    }

    /* DEFAULTS *************************************************/

    adto.timing = adto.timing || {};

    /* FUNCTIONS **************************************************/

    adto.timing.convertToUserTimezone = function (date) {
        var momentDate = moment(date);
        var targetDate = momentDate.clone().tz(adto.timing.timeZoneInfo.iana.timeZoneId);
        return targetDate;
    };

})();