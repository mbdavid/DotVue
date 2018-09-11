//
// DotVue - Vue plugin for update ViewModel in server
//
(function () {

    // register vue plugin to server call (vue.$update)
    const DotVue = {
        install: function (Vue, options) {

            var _queue = [];
            var _running = false;

            // in watch, test before call if field are not been updated
            Vue.prototype.$updating = false;

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
            };

            // Execute queue
            function nextQueue() {

                if (_running === false) _running = true;

                // get first request from queue
                var request = _queue.shift();

                setTimeout(function () {

                    ajax(request, function (result) {

                        // resolve request promise
                        request.resolve(result);

                        // if no more items in queue, stop running
                        if (_queue.length === 0) return _running = false;

                        nextQueue();
                    });

                }, 1);
            }

            // Execute ajax POST request for update model
            function ajax(request, finish) {

                var xhr = new XMLHttpRequest();

                xhr.onload = function () {

                    if (xhr.status < 200 || xhr.status >= 400) {
                        _queue = [];
                        _running = false;
                        request.vm.$el.innerHTML = xhr.responseText;
                        return;
                    }

                    var response = JSON.parse(xhr.responseText);
                    var update = response['update'];
                    var script = response['script'];
                    var result = response['result'];

                    // server-side changes not call watch methods
                    request.vm.$updating = true;

                    Object.keys(update).forEach(function (key) {
                        var value = update[key];
                        log('>  $data["' + key + '"] = ', value);

                        // update viewmodel
                        request.vm.$data[key] = value;
                    });

                    if (result !== null) {
                        log('>  $result:', result);
                    }

                    request.vm.$nextTick(function () {
                        request.vm.$updating = false;
                    });

                    if (script) {
                        log('>  $eval = ', script);
                        setTimeout(function () {
                            new Function(script).call(request.vm);
                        });
                    }

                    finish(result);
                };

                // create form with all data
                var form = new FormData();

                form.append('method', request.name);
                form.append('props', JSON.stringify(request.vm.$props || {}));

                // add data (check if has local properties)
                if (request.vm.$options.local.length === 0) {
                    form.append('data', JSON.stringify(request.vm.$data || {}));
                }
                else {
                    // create a simple copy
                    var copy = Object.assign({}, request.vm.$data);

                    // and remove local propertis before send to server
                    request.vm.$options.local.forEach(function (key) {
                        delete copy[key];
                    });

                    form.append('data', JSON.stringify(copy));
                }

                // upload file
                request.params.forEach(function (value, index, arr) {

                    var isFile = value instanceof HTMLInputElement && value.type === 'file';

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

                form.append('params', JSON.stringify(request.params));

                log('$update ("' + request.name + '") = ', request.params);

                xhr.open('POST', request.vm.$options.name + '.vue', true);
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
            };

            // Load async component from current page
            Vue.$loadComponent = function $loadComponent(name) {

                // if name is a function, just execute and return component
                if (typeof name === 'function') return name();

                return function (resolve, reject) {
                    var xhr = new XMLHttpRequest();

                    xhr.onload = function () {
                        if (xhr.status < 200 || xhr.status >= 400) {
                            document.body.innerHTML = xhr.responseText;
                            return reject();
                        }
                        try {
                            var fn = new Function(xhr.responseText);

                            var obj = fn();

                            // run includes (js/css)
                            loadjs(obj.includes, function () {
                                resolve(obj.options());
                            });
                        }
                        catch (e) {
                            alert(e);
                        }
                    };

                    xhr.open('GET', name + '.vue', true);
                    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                    xhr.send();
                };
            };
        }
    };

    Vue.use(DotVue);

    var _loaded = {};

    // very simple queue script/css loader, based on loadjs (https://github.com/muicss/loadjs/blob/master/dist/loadjs.js)
    function loadjs(items, fn, index, numTries) {

        index = index || 0;

        // finish queue
        if (items.length === index) return fn();

        var path = resolveUrl(items[index]);

        // if already loaded
        if (_loaded[path]) return loadjs(items, fn, index + 1);

        var isCss = /(^css!|\.css$)/.test(path);
        var pathStripped = path.replace(/^(css|js)!/, '');
        var e = null;

        if (isCss) {
            e = document.createElement('link');
            e.rel = 'stylesheet';
            e.href = pathStripped;
        }
        else {
            e = document.createElement('script');
            e.src = path;
            e.async = true;
        }

        log('loading: ' + path);

        e.onload = e.onerror = e.onbeforeload = function (ev) {

            var result = ev.type[0];

            // note: The following code isolates IE using `hideFocus` and treats empty stylesheets as failures to get around lack of onerror support
            if (isCss && 'hideFocus' in e) {
                try {
                    if (!e.sheet.cssText.length) result = 'e';
                } catch (x) {
                    // sheets objects created from load errors don't allow access to `cssText`
                    result = 'e';
                }
            }

            // handle retries in case of load failure
            if (result === 'e' && (numTries || 0) + 1 < 3) {
                return loadjs(items, fn, index, numTries + 1);
            }

            // add into cache
            _loaded[path] = true;

            // execute next
            return loadjs(items, fn, index + 1);
        };

        // add into html document
        document.head.appendChild(e);
    }

    // get full path from url
    function resolveUrl(url) {
        var a = document.createElement('a');
        a.href = url;
        return a.href;
    }

    // execute console log without showing file: http://stackoverflow.com/questions/34762774/how-to-hide-source-of-log-messages-in-console
    function log() {
        setTimeout(console.log.bind(console, arguments[0], arguments[1] || ''));
    }

})();