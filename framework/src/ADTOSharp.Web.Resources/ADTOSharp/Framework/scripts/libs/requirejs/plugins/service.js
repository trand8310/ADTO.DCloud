define(function () {
    return {
        load: function (name, req, onload, config) {
            var url = adto.appPath + 'api/ADTOSharpServiceProxies/Get?name=' + name;
            req([url], function (value) {
                onload(value);
            });
        }
    };
});