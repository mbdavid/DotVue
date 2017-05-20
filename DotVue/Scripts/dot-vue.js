//
// DotVue - Vue plugin for update ViewModel in server
//
(function () {

    var _styles = {};
    var _json = new JsonEx();

    // register vue plugin to server call (vue.$update)
    const DotVue = {
        install: function (Vue, options) {

            var _queue = [];
            var _running = false;

            // in watch, test before call if field are not been updated
            Vue.prototype.$updating = false;
            Vue.prototype.$parseJsonEx = _json.parse;

            // request new server call
            Vue.prototype.$update = function $update(vm, name, params) {

                return new Promise(function (resolve) {

                    _queue.push({
                        vm: vm,
                        name: name,
                        params: params || [],
                        resolve: resolve
                    });

                    if (!_running) nextQueue();
                });
            }

            // Execute queue
            function nextQueue() {

                if (_running === false) _running = true;

                // get first request from queue
                var request = _queue.shift();

                setTimeout(function () {

                    ajax(request, function () {

                        // resolve request promise
                        request.resolve(request.vm);

                        // if no more items in queue, stop running
                        if (_queue.length === 0) return _running = false;

                        nextQueue();
                    });

                }, 1);
            }

            // Execute ajax POST request for update model
            function ajax(request, finish) {

                var xhr = new XMLHttpRequest();
                var files = [];

                xhr.onload = function () {
                    if (xhr.status < 200 || xhr.status >= 400) {
                        _queue = [];
                        _running = false;
                        request.vm.$el.innerHTML = xhr.responseText;
                        return;
                    }

                    var response = _json.parse(xhr.responseText);
                    var update = response['update'];
                    var js = response['js'];

                    // empty file inputs (if exists)
                    files.forEach(function (f) { f.value = ''; });

                    // server-side changes not call watch methods
                    request.vm.$updating = true;

                    Object.keys(update).forEach(function (key) {
                        var value = update[key];
                        log('>  $data["' + key + '"] = ', value);

                        // update viewmodel
                        request.vm.$data[key] = value;
                    });

                    request.vm.$nextTick(function () {
                        request.vm.$updating = false;
                    });

                    if (js) {
                        log('>  $eval = ', response['js']);
                        setTimeout(function () {
                            new Function(js).call(request.vm);
                        })
                    }

                    finish();
                };

                // create form with all data
                var form = new FormData();

                form.append('method', request.name);
                form.append('data', _json.stringify(request.vm.$data || {}));
                form.append('props', _json.stringify(request.vm.$props || {}));

                // upload file
                request.params.forEach(function (value, index, arr) {

                    var isFile = value instanceof HTMLInputElement && value.type == 'file';

                    if (isFile) {
                        var name = "files_" + (Math.floor(Math.random() * 899998) + 100000);
                        for (var i = 0; i < value.files.length; i++) {
                            form.append(name, value.files[i]);
                            log('$upload ("' + value.files[i].name + '")... ', value.files[i].size);
                        }
                        value.value = null;
                        arr[index] = name;
                    }
                });

                form.append('params', _json.stringify(request.params));

                log('$update ("' + request.name + '") = ', request.params);

                xhr.open('POST', request.vm.$options.vpath, true);
                xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                xhr.send(form);
            }

            // Add css style function into vue instance
            Vue.$addStyle = function (css) {

                var head = document.head || document.getElementsByTagName('head')[0];
                var style = document.createElement('style');

                style.type = 'text/css';
                if (style.styleSheet) {
                    style.styleSheet.cssText = css;
                } else {
                    style.appendChild(document.createTextNode(css));
                }

                head.appendChild(style);
            }

            // Load async component from current page
            Vue.$loadComponent = function $loadComponent(vpath) {

                // if vpath is a function, just return
                if (typeof vpath === 'function') return vpath();

                return function (resolve, reject) {
                    var xhr = new XMLHttpRequest();

                    xhr.onload = function () {
                        if (xhr.status < 200 || xhr.status >= 400) {
                            alert('Error on load component: ' + vpath);
                            return;
                        }
                        try {
                            var fn = new Function(xhr.responseText);
                            var options = fn();
                            resolve(options);
                        }
                        catch (e) {
                            alert(e);
                        }
                    };

                    xhr.open('GET', vpath, true);
                    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    xhr.send();
                }
            };

        }
    }

    Vue.use(DotVue);

    // Extended JSON class to parse/stringify with support DateTime from JSON.NET
    function JsonEx() {

        // JSON RegExp
        var rvalidchars = /^[\],:{}\s]*$/;
        var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
        var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
        var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
        var dateISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(Z|[-+]\d{2}:\d{2})?$/;

        function f(n) {
            return n < 10 ? '0' + n : n;
        }

        function convertStringify(key, value) {

            if (typeof value == "string") {
                var a = dateISO.exec(value);
                if (a) {
                    var val = new Date(value);
                    return val.getFullYear() + '-' +
                        f(val.getMonth() + 1) + '-' +
                        f(val.getDate()) + 'T' +
                        f(val.getHours()) + ':' +
                        f(val.getMinutes()) + ':' +
                        f(val.getSeconds()) + '.' +
                        f(val.getMilliseconds()) + 'Z';
                }
            }
            return value;
        }

        function convertParse(key, value) {
            if (typeof (value) === "string") {
                var a = dateISO.exec(value);
                if (a) {
                    return new Date(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
                }
            }
            return value;
        }

        this.parse = function (data) {

            if (typeof data !== "string" || !data) return null;

            data = data.replace(/^\s\s*/, "").replace(/\s\s*$/, "");

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            if (rvalidchars.test(data
                .replace(rvalidescape, "@")
                .replace(rvalidtokens, "]")
                .replace(rvalidbraces, ""))) {
                try {
                    return window.JSON.parse(data, convertParse);
                }
                catch (e) {
                    throw "Error in parseJSON(\"" + data + "\") : " + e;
                }
            }
            return null;
        }

        this.stringify = function (obj, space) {
            return window.JSON.stringify(obj, convertStringify, space);
        }
    }

    // execute console log without showing file: http://stackoverflow.com/questions/34762774/how-to-hide-source-of-log-messages-in-console
    function log() {
        setTimeout(console.log.bind(console, arguments[0], arguments[1] || ''));
    }

})();