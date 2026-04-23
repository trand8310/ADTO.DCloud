var adto = adto || {};
(function () {

    /* Swagger */

    adto.swagger = adto.swagger || {};

    adto.swagger.addAuthToken = function () {
        var authToken = adto.auth.getToken();
        if (!authToken) {
            return false;
        }

        var cookieAuth = new SwaggerClient.ApiKeyAuthorization(adto.auth.tokenHeaderName, 'Bearer ' + authToken, 'header');
        swaggerUi.api.clientAuthorizations.add('bearerAuth', cookieAuth);
        return true;
    }

    adto.swagger.addCsrfToken = function () {
        var csrfToken = adto.security.antiForgery.getToken();
        if (!csrfToken) {
            return false;
        }
        var csrfCookieAuth = new SwaggerClient.ApiKeyAuthorization(adto.security.antiForgery.tokenHeaderName, csrfToken, 'header');
        swaggerUi.api.clientAuthorizations.add(adto.security.antiForgery.tokenHeaderName, csrfCookieAuth);
        return true;
    }

    function addAntiForgeryTokenToXhr(xhr) {
        var antiForgeryToken = adto.security.antiForgery.getToken();
        if (antiForgeryToken) {
            xhr.setRequestHeader(adto.security.antiForgery.tokenHeaderName, antiForgeryToken);
        }
    }

    function loginUserInternal(tenantId, callback) {
        var userName = document.getElementById('userName').value;
        if (!userName) {
            alert('用户名 不能为空 !');
            return false;
        }

        var password = document.getElementById('password').value;
        if (!password) {
            alert('密码不能为空 !');
            return false;
        }

        var xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var responseJSON = JSON.parse(xhr.responseText);
                    var result = responseJSON.result;
                    var expireDate = new Date(Date.now() + (result.expireInSeconds * 1000));
                    adto.auth.setToken(result.accessToken, expireDate);
                    callback();   
                } else {
                    alert('登录失败 !');
                }
            }
        };

        xhr.open('POST', '/api/TokenAuth/Authenticate', true);
        xhr.setRequestHeader('ADTOSharp-TenantId', tenantId);
        xhr.setRequestHeader('Content-type', 'application/json');
        addAntiForgeryTokenToXhr(xhr);
        xhr.send(
            JSON.stringify(
                { userName: userName, password: password }
            )
        );
    };

    adto.swagger.login = function (callback) {
        //Get TenantId first
        var tenancyName = document.getElementById('tenancyName').value;

        if (tenancyName) {
            var xhrTenancyName = new XMLHttpRequest();
            xhrTenancyName.onreadystatechange = function () {
                if (xhrTenancyName.readyState === XMLHttpRequest.DONE && xhrTenancyName.status === 200) {
                    var responseJSON = JSON.parse(xhrTenancyName.responseText);
                    var result = responseJSON.result;
                    if (result.state === 1) { // Tenant exists and active.
                        loginUserInternal(result.tenantId, callback); // Login for tenant    
                    } else {
                        alert('租户无效 !');
                    }
                }
            };

            xhrTenancyName.open('POST', '/api/services/app/Account/IsTenantAvailable', true);
            xhrTenancyName.setRequestHeader('Content-type', 'application/json');
            addAntiForgeryTokenToXhr(xhrTenancyName);
            xhrTenancyName.send(
                JSON.stringify({ tenancyName: tenancyName })
            );
        } else {
            loginUserInternal(null, callback); // Login for host
        }
    };

    adto.swagger.logout = function () {
        adto.auth.clearToken();
    }

    adto.swagger.closeAuthDialog = function () {
        if (document.getElementById('adto-auth-dialog')) {
            document.getElementsByClassName("swagger-ui")[1].removeChild(document.getElementById('adto-auth-dialog'));
        }
    }

    adto.swagger.openAuthDialog = function (loginCallback) {
        adto.swagger.closeAuthDialog();

        var adtoAuthDialog = document.createElement('div');
        adtoAuthDialog.className = 'dialog-ux';
        adtoAuthDialog.id = 'adto-auth-dialog';

        document.getElementsByClassName("swagger-ui")[1].appendChild(adtoAuthDialog);

        // -- backdrop-ux
        var backdropUx = document.createElement('div');
        backdropUx.className = 'backdrop-ux';
        adtoAuthDialog.appendChild(backdropUx);

        // -- modal-ux
        var modalUx = document.createElement('div');
        modalUx.className = 'modal-ux';
        adtoAuthDialog.appendChild(modalUx);

        // -- -- modal-dialog-ux
        var modalDialogUx = document.createElement('div');
        modalDialogUx.className = 'modal-dialog-ux';
        modalUx.appendChild(modalDialogUx);

        // -- -- -- modal-ux-inner
        var modalUxInner = document.createElement('div');
        modalUxInner.className = 'modal-ux-inner';
        modalDialogUx.appendChild(modalUxInner);

        // -- -- -- -- modal-ux-header
        var modalUxHeader = document.createElement('div');
        modalUxHeader.className = 'modal-ux-header';
        modalUxInner.appendChild(modalUxHeader);

        var modalHeader = document.createElement('h3');
        modalHeader.innerText = '登录';
        modalUxHeader.appendChild(modalHeader);

        // -- -- -- -- modal-ux-content
        var modalUxContent = document.createElement('div');
        modalUxContent.className = 'modal-ux-content';
        modalUxInner.appendChild(modalUxContent);

        modalUxContent.onkeydown = function (e) {
            if (e.keyCode === 13) {
                //try to login when user presses enter on authorize modal
                adto.swagger.login(loginCallback);
            }
        };

        //Inputs
        createInput(modalUxContent, 'tenancyName', '租户');
        createInput(modalUxContent, 'userName', '用户名');
        createInput(modalUxContent, 'password', '密码', 'password');

        //Buttons
        var authBtnWrapper = document.createElement('div');
        authBtnWrapper.className = 'auth-btn-wrapper';
        modalUxContent.appendChild(authBtnWrapper);

        //Close button
        var closeButton = document.createElement('button');
        closeButton.className = 'btn modal-btn auth btn-done button';
        closeButton.innerText = '关闭';
        closeButton.style.marginRight = '5px';
        closeButton.onclick = adto.swagger.closeAuthDialog;
        authBtnWrapper.appendChild(closeButton);

        //Authorize button
        var authorizeButton = document.createElement('button');
        authorizeButton.className = 'btn modal-btn auth authorize button';
        authorizeButton.innerText = '登录';
        authorizeButton.onclick = function() {
            adto.swagger.login(loginCallback);
        };
        authBtnWrapper.appendChild(authorizeButton);
    }

    function createInput(container, id, title, type) {
        var wrapper = document.createElement('div');
        wrapper.className = 'wrapper';
        container.appendChild(wrapper);

        var label = document.createElement('label');
        label.innerText = title;
        wrapper.appendChild(label);

        var section = document.createElement('section');
        section.className = 'block-tablet col-10-tablet block-desktop col-10-desktop';
        wrapper.appendChild(section);

        var input = document.createElement('input');
        input.id = id;
        input.type = type ? type : 'text';
        input.style.width = '100%';

        section.appendChild(input);
    }

})();