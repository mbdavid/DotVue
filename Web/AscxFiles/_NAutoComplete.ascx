<%@ Control Language="C#" %>
<template>
    <div class="n-auto-complete" :class="{ 'modal': open }">
        <input type="text"
            ref="input"
            :placeholder="placeholder"
            :disabled="disabled"
            :value="text" 
            @focus="changed = false"
            @blur="onblur"
            @input="query($event.target.value)"
            @keydown.up.prevent="index = (index <= 0 ? 0 : index - 1)"
            @keydown.down.prevent="index = (index >= items.length - 1 ? items.length - 1: index + 1)"
            @keydown.enter="select(index < 0 ? null : items[index], $event)"
            @keydown.esc="clear"
        />
        <ul v-show="open && items.length > 0">
            <li v-for="(item, i) in items" 
                :class="{ 'sel': index == i }"
                @click="select(item, $event)">
                <slot :item="item">{{ item }}</slot>
            </li>
        </ul>
        <div v-show="open && items.length == 0">Nenhum resultado encontrado para "{{text}}"</div>
    </div>
</template>
<script>

    return {
        props: {
            value: [String, Object],
            prop: String,
            disabled: Boolean,
            placeholder: '',
            items: Array
        },
        data: function() {
            return {
                text: this.getText(this.value),
                open: false,
                index: -1,
                changed: false
            }
        },
        methods: {
            query: function(q) {
                this.text = q;
                this.open = true;
                this.changed = true;

                if(q.trim().length > 0) {
                    this.$emit('query', q);
                }
                else {
                    this.index = -1;
                    this.open = false;
                }
            },
            select: function(item, e) {

                if (item === null) return;

                this.text = this.getText(item);
                this.open = false;
                this.index = -1;
                this.changed = false;

                this.$emit('input', item);
                e.preventDefault();
            },
            clear: function() {
                this.text = this.getText(this.value);
                this.changed = false;
                this.open = false;
            },
            getText: function(item) {
                if (!item) return '';
                return typeof item === 'string' ? item : item[this.prop || Object.keys(item)[0]];
            },
            onblur: function(e) {
                setTimeout(() => {

                    if (this.changed == false) return;

                    if (this.text === '' || this.index == -1) {
                        this.text = null;
                        this.$emit('input', null);
                    }
                    else if(this.index >= 0 && this.open && this.items.length > 0) {
                        var item = this.items[this.index];
                        var text = this.getText(item);
                        if (this.text != text) {
                            this.text = text;
                            this.$emit('input', item);
                        }
                    }

                    this.open = false;
                }, 100);
            }
        },
        watch: {
            items: function() {
                this.index = this.items.length > 0 ? 0 : -1;
            }
        }
    }

</script>
<style>
    .n-auto-complete {
    }

    .n-auto-complete .sel {
        color: black;
        background-color: yellow !important;
    }
    .n-auto-complete > ul > li:hover {
        background-color: #f3f3f3;
    }

    .n-auto-complete.modal {
        position: absolute;
        border: 20px solid rgba(77, 77, 77, .5);
        top: 0px;
        left: 0px;
        right: 0px;
        bottom: 0px;
        padding: 20px;
        background-color: white;
    }

    .n-auto-complete.modal > input {
        width: 100%;
    }

</style>