@page / 

<div>
    <counter :number="counterNumber"></counter>
    <h1>My Id Counter is {{ id }}</h1>
    <button type="button" @click="serverExecute()">Server Execute Client Function</button>
    <button type="button" @click="clickMe(5)">ClickMe-Server</button>
    <button type="button" @click="id++">ClickMe-Client</button>
    <button type="button" @click="serverNavigate()">Go To Route Page-Server</button>
    <button type="button" @click="gotToRoute()">Go To Route Page-Client</button>
    <hr/>
    <input v-model="text" /> <i>(server-side watch-ed)</i>
    <hr/>
    {{text | upper }} | Code: {{ code }}
    <hr/>
    {{localOnly}}
    <hr/>
    <button @click="submitWithConfirm()">Submit (with confirm)</button>
    <hr/>
</div>

<style scoped>

    & {
        background-color: #f5f5f5;
        padding: 15px;
    }

    button {
        color:red;
    }

</style>

<script>

    this.$on('mounted', function () {
        console.log('Mounted');
	})

    this.$on('submitWithConfirm:before', function (r) {
        r.wait();
        if (confirm('Submit?')) {
            r.resolve();
        }
        else {
            r.reject();
        }
    })

	this.serverExecuted = function (arg1, arg2) {
		alert(`Server executed client function, returned args: ${arg1}, ${arg2}.`);
	};

</script>

<script global>

    // global script block will be added as script before load components

    Vue.filter('upper', function (value) {
        return value.toString().toUpperCase();
    })

</script>

<script mixin>

    return {
        props: {
           
        },
        data: function () {
            return {
                counterNumber: 100
            };
        },
        mounted: function () {
            console.log('Mounted (mixin)');
		},
        methods: {
            gotToRoute: function () {
                var myId = 123;
                console.log('Navigating to Route page');
                this.$router.push({ path: `/route/${myId}`, query: { xx: 'yy' } }); // -> /route/123?xx=yy
                //this.$router.push({ path: '/route/123' }); // -> /route/123
            }
        },
        watch: {
            id: function (newVal, oldVal) {
				console.log('newVal', newVal, 'oldVal', oldVal);
            }
        },
		beforeRouteEnter(to, from, next) {
			// called before the route that renders this component is confirmed.
			// does NOT have access to `this` component instance,
			// because it has not been created yet when this guard is called!
            next(vm => {
                // access to component public instance via `vm`
				console.log(`Navigated from ${from.path} to ${to.path}`);
                return true;
            });
		},
		beforeRouteUpdate(to, from) {
			// called when the route that renders this component has changed, but this component is reused in the new route.
			// For example, given a route with params `/users/:id`, when we navigate between `/users/1` and `/users/2`,
			// the same `UserDetails` component instance will be reused, and this hook will be called when that happens.
			// Because the component is mounted while this happens, the navigation guard has access to `this` component instance.
		},
		beforeRouteLeave(to, from, next) {
			// called when the route that renders this component is about to be navigated away from.
			// As with `beforeRouteUpdate`, it has access to `this` component instance.
            //const answer = window.confirm('Do you really want to leave?');
            //if (!answer) return false;
            //else {
                  console.log(`Navigating to ${to.path} from ${from.path}`);
                  next();
            //}
		}
    };

</script>