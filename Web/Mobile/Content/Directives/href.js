Vue.directive('href', {
    inserted: function (el, binding) {
        el.dataset.href = binding.value;
        el.addEventListener('click', function () {
            var href = el.dataset.href;
            if (href && href != 'undefined') {
                location.href = href;
            }
        });
    },
    update: function (el, binding) {
        el.dataset.href = binding.value;
    }
});