//
// DotVue - Vue plugin for update ViewModel in server
//
// register vue plugin to server call (vue.$server)
const DotVue = {
    install: function (Vue, options) {

        var _queue = [];
        var _running = false;

        // request new server call
        Vue.prototype.$server = function $server(name, ...params) {

            var vm = this;

            return new Promise(function (resolve, reject) {

                params = params || [];

                _queue.push({
                    vm: vm,
                    name: name,
                    params: params,
                    resolve: resolve,
                    reject: reject
                });

                if (!_running) nextQueue();

            }).then(function (r) {

                vm.$emit(name + ':after', r, ...params);

            });
        };

        // Execute queue
        function nextQueue() {

            if (_running === false) _running = true;

            // get first request from queue
            var request = _queue.shift();

            var before = new Promise(function (resolve, reject) {

                // manage cancel event
                var p = {
                    waiting: 0,
                    wait() {
                        this.waiting++;
                    },
                    resolve() {
                        if (--this.waiting <= 0) resolve();
                    },
                    reject
                };

                request.vm.$emit(request.name + ':before', p, ...request.params);

                // if not used (or no event capture), just resolve
                if (p.waiting <= 0)  resolve();
            });
                
            before.then(function () {

                ajax(request, function (result) {

                    // resolve request promise
                    request.resolve(result);

                    // if no more items in queue, stop running
                    if (_queue.length === 0) return _running = false;

                    nextQueue();
                });

            }).catch(function () {

                log('> ' + request.name + ':before rejected');

                // if no more items in queue, stop running
                if (_queue.length === 0) return _running = false;

                nextQueue();

            });

        }

        // Execute ajax POST request for update model
        function ajax(request, finish) {

            var xhr = new XMLHttpRequest();

            xhr.onload = function () {

                if (xhr.status < 200 || xhr.status >= 400) {
                    _queue = [];
                    _running = false;
                    request.vm.$el.innerHTML = xhr.responseText;
                    request.reject();
                    return;
                }

                var response = JSON.parse(xhr.responseText);
                var update = response['update'];
                var script = response['script'];
                var result = response['result'];

                Object.keys(update).forEach(function (key) {
                    var value = update[key];
                    log('>  $data["' + key + '"] = ', value);

                    // update viewmodel
                    request.vm.$data[key] = value;
                });

                if (result !== null) {
                    log('>  $result:', result);
                }

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

            console.log('request.vm.$data', JSON.stringify(request.vm.$data || {}));

            form.append('method', request.name);
            form.append('props', JSON.stringify(request.vm.$props || {}));
            form.append('data', JSON.stringify(request.vm.$data || {}));

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

        // Load async component from current page
        DotVue.async = function load(name) {

            return function (resolve, reject) {
                var xhr = new XMLHttpRequest();

                xhr.onload = function () {
                    if (xhr.status < 200 || xhr.status >= 400) {
                        document.body.innerHTML = xhr.responseText;
                        return reject();
                    }
                    try {
                        var fn = new Function('return ' + xhr.responseText);
                        resolve(fn());
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

// execute console log without showing file: http://stackoverflow.com/questions/34762774/how-to-hide-source-of-log-messages-in-console
function log() {
    setTimeout(console.log.bind(console, arguments[0], arguments[1] || ''));
}

window.DotVue = DotVue;
