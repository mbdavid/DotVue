(function () {

    // cache do usuario
    var _user = null;

    Vue.config.silent = true;

    // inicia a instancia principal do Vue
    new Vue({
        el: '#app',
        data: {
            current: '',
            query: {},
            user: {}
        },
        created: function () {
            var vm = this;
            router(/.*/, auth, function (ctx, next) {
                vm.current = _user == null ? 'Login' : (ctx.segments[0] || 'Home');
                vm.query = ctx.query;
                vm.user = ctx.user;
            });

            router();
        }
    });

    // requisita dados do usuario. Caso não tenha autenticação, retorna erro
    function auth(ctx, next) {

        if (_user) next();

        var request = new XMLHttpRequest();
        request.open('GET', 'Content/Scripts/Auth.ashx', true);
        request.onload = function () {
            if (request.status >= 200 && request.status < 400) {
                _user = JSON.parse(request.responseText);
            } else {
                _user = null;
            }
            next();
        };
        request.send();
    }

})();
