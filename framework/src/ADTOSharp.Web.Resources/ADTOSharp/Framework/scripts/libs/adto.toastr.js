(function (define) {
  define(['toastr', 'adto-web-resources'], function (toastr, adto) {
    return (function () {

      if (!toastr) {
        return;
      }

      if (!adto) {
        return;
      }

      /* DEFAULTS *************************************************/

      toastr.options.positionClass = 'toast-bottom-right';

      /* NOTIFICATION *********************************************/

      var showNotification = function (type, message, title, options) {
        toastr[type](message, title, options);
      };

      adto.notify.success = function (message, title, options) {
        showNotification('success', message, title, options);
      };

      adto.notify.info = function (message, title, options) {
        showNotification('info', message, title, options);
      };

      adto.notify.warn = function (message, title, options) {
        showNotification('warning', message, title, options);
      };

      adto.notify.error = function (message, title, options) {
        showNotification('error', message, title, options);
      };

      return adto;
    })();
  });
}(typeof define === 'function' && define.amd
  ? define
  : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) {
      module.exports = factory(require('toastr'), require('adto-web-resources'));
    } else {
      window.adto = factory(window.toastr, window.adto);
    }
  }));
