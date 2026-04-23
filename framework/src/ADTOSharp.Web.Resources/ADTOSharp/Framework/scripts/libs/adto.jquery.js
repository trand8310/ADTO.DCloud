(function (define) {
  define(['jquery', 'adto-web-resources'], function ($, adto) {
    return (function () {

      if (!$) {
        return;
      }

      if (!adto) {
        return;
      }

      /* JQUERY ENHANCEMENTS ***************************************************/

      // adto.ajax -> uses $.ajax ------------------------------------------------

      adto.ajax = function (userOptions) {
        userOptions = userOptions || {};

        var options = $.extend(true, {}, adto.ajax.defaultOpts, userOptions);
        var oldBeforeSendOption = options.beforeSend;
        options.beforeSend = function (xhr, settings) {
          if (oldBeforeSendOption) {
            oldBeforeSendOption(xhr, settings);
          }

          xhr.setRequestHeader("Pragma", "no-cache");
          xhr.setRequestHeader("Cache-Control", "no-cache");
          xhr.setRequestHeader("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
        };

        options.success = undefined;
        options.error = undefined;

        return $.Deferred(function ($dfd) {
          $.ajax(options)
            .done(function (data, textStatus, jqXHR) {
              if (data.__adto) {
                adto.ajax.handleResponse(data, userOptions, $dfd, jqXHR);
              } else {
                $dfd.resolve(data);
                userOptions.success && userOptions.success(data);
              }
            }).fail(function (jqXHR) {
              if (jqXHR.responseJSON && jqXHR.responseJSON.__adto) {
                adto.ajax.handleResponse(jqXHR.responseJSON, userOptions, $dfd, jqXHR);
              } else {
                adto.ajax.handleNonADTOSharpErrorResponse(jqXHR, userOptions, $dfd);
              }
            });
        });
      };

      $.extend(adto.ajax, {
        defaultOpts: {
          dataType: 'json',
          type: 'POST',
          contentType: 'application/json',
          headers: {
            'X-Requested-With': 'XMLHttpRequest'
          }
        },

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
          details: 'The resource requested could not found on the server.'
        },

        logError: function (error) {
          adto.log.error(error);
        },

        showError: function (error) {
          if (error.details) {
            return adto.message.error(error.details, error.message);
          } else {
            return adto.message.error(error.message || adto.ajax.defaultError.message);
          }
        },

        handleTargetUrl: function (targetUrl) {
          if (!targetUrl) {
            location.href = adto.appPath;
          } else {
            location.href = targetUrl;
          }
        },

        handleNonADTOSharpErrorResponse: function (jqXHR, userOptions, $dfd) {
          if (userOptions.adtoHandleError !== false) {
            switch (jqXHR.status) {
              case 401:
                adto.ajax.handleUnAuthorizedRequest(
                  adto.ajax.showError(adto.ajax.defaultError401),
                  adto.appPath
                );
                break;
              case 403:
                adto.ajax.showError(adto.ajax.defaultError403);
                break;
              case 404:
                adto.ajax.showError(adto.ajax.defaultError404);
                break;
              default:
                adto.ajax.showError(adto.ajax.defaultError);
                break;
            }
          }

          $dfd.reject.apply(this, arguments);
          userOptions.error && userOptions.error.apply(this, arguments);
        },

        handleUnAuthorizedRequest: function (messagePromise, targetUrl) {
          if (messagePromise) {
            messagePromise.done(function () {
              adto.ajax.handleTargetUrl(targetUrl);
            });
          } else {
            adto.ajax.handleTargetUrl(targetUrl);
          }
        },

        handleResponse: function (data, userOptions, $dfd, jqXHR) {
          if (data) {
            if (data.success === true) {
              $dfd && $dfd.resolve(data.result, data, jqXHR);
              userOptions.success && userOptions.success(data.result, data, jqXHR);

              if (data.targetUrl) {
                adto.ajax.handleTargetUrl(data.targetUrl);
              }
            } else if (data.success === false) {
              var messagePromise = null;

              if (data.error) {
                if (userOptions.adtoHandleError !== false) {
                  messagePromise = adto.ajax.showError(data.error);
                }
              } else {
                data.error = adto.ajax.defaultError;
              }

              adto.ajax.logError(data.error);

              $dfd && $dfd.reject(data.error, jqXHR);
              userOptions.error && userOptions.error(data.error, jqXHR);

              if (jqXHR.status === 401 && userOptions.adtoHandleError !== false) {
                adto.ajax.handleUnAuthorizedRequest(messagePromise, data.targetUrl);
              }
            } else { //not wrapped result
              $dfd && $dfd.resolve(data, null, jqXHR);
              userOptions.success && userOptions.success(data, null, jqXHR);
            }
          } else { //no data sent to back
            $dfd && $dfd.resolve(jqXHR);
            userOptions.success && userOptions.success(jqXHR);
          }
        },

        blockUI: function (options) {
          if (options.blockUI) {
            if (options.blockUI === true) { //block whole page
              adto.ui.setBusy();
            } else { //block an element
              adto.ui.setBusy(options.blockUI);
            }
          }
        },

        unblockUI: function (options) {
          if (options.blockUI) {
            if (options.blockUI === true) { //unblock whole page
              adto.ui.clearBusy();
            } else { //unblock an element
              adto.ui.clearBusy(options.blockUI);
            }
          }
        },

        ajaxSendHandler: function (event, request, settings) {
          var token = adto.security.antiForgery.getToken();
          if (!token) {
            return;
          }

          if (!adto.security.antiForgery.shouldSendToken(settings)) {
            return;
          }

          if (!settings.headers || settings.headers[adto.security.antiForgery.tokenHeaderName] === undefined) {
            request.setRequestHeader(adto.security.antiForgery.tokenHeaderName, token);
          }
        }
      });

      $(document).ajaxSend(function (event, request, settings) {
        return adto.ajax.ajaxSendHandler(event, request, settings);
      });

      /* JQUERY PLUGIN ENHANCEMENTS ********************************************/

      /* jQuery Form Plugin 
       * http://www.malsup.com/jquery/form/
       */

      // adtoAjaxForm -> uses ajaxForm ------------------------------------------

      if ($.fn.ajaxForm) {
        $.fn.adtoAjaxForm = function (userOptions) {
          userOptions = userOptions || {};

          var options = $.extend({}, $.fn.adtoAjaxForm.defaults, userOptions);

          options.beforeSubmit = function () {
            adto.ajax.blockUI(options);
            userOptions.beforeSubmit && userOptions.beforeSubmit.apply(this, arguments);
          };

          options.success = function (data) {
            adto.ajax.handleResponse(data, userOptions);
          };

          //TODO: Error?

          options.complete = function () {
            adto.ajax.unblockUI(options);
            userOptions.complete && userOptions.complete.apply(this, arguments);
          };

          return this.ajaxForm(options);
        };

        $.fn.adtoAjaxForm.defaults = {
          method: 'POST'
        };
      }

      adto.event.on('adto.dynamicScriptsInitialized', function () {
        adto.ajax.defaultError.message = adto.localization.adtoWeb('DefaultError');
        adto.ajax.defaultError.details = adto.localization.adtoWeb('DefaultErrorDetail');
        adto.ajax.defaultError401.message = adto.localization.adtoWeb('DefaultError401');
        adto.ajax.defaultError401.details = adto.localization.adtoWeb('DefaultErrorDetail401');
        adto.ajax.defaultError403.message = adto.localization.adtoWeb('DefaultError403');
        adto.ajax.defaultError403.details = adto.localization.adtoWeb('DefaultErrorDetail403');
        adto.ajax.defaultError404.message = adto.localization.adtoWeb('DefaultError404');
        adto.ajax.defaultError404.details = adto.localization.adtoWeb('DefaultErrorDetail404');
      });

      return adto;
    })();
  });
}(typeof define === 'function' && define.amd
  ? define
  : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) {
      module.exports = factory(require('jquery'), require('adto-web-resources'));
    } else {
      window.adto = factory(window.jQuery, window.adto);
    }
  }));