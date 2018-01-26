"use strict"

Vue.component('modal', {
    template: '#modal-template'
})
var app = new Vue({
    el: "#app",
    data: {
        repositories: [],
        showModal: false,
        model: {
            url: '',
            valid: false
        }
    },
    mounted() {
        this.get();
    },
    methods: {
        async get() {
            var response = await axios.get('/Repository/GetRepositories');
            this.repositories = response.data;
        },
        closeModal() {
            this.showModal = false;
        },
        async addRepository() {
            var response = await axios.post('/Repository/AddRepository', {
                url : this.model.url
            });
            
            let result = response.data;

            console.log(result);
        },
        async isValid() {
            var response = await axios.get('/Repository/GetProvider', {
                params: {
                    url: this.model.url
                }
            });

            let result = response.data;

            if (result !== "")
                this.model.valid = true;
            else
                this.model.valid = false;
        }
    }
})