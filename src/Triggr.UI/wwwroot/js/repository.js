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
            valid: false,
            token: '',
            owner: '',
            name: '',
            webhook: false
        },
        message: ''
    },
    mounted() {
        this.get();
    },
    filters: {
        duration: function (value) {
            return moment(value).fromNow();
        }
    },
    methods: {
        async get() {
            var response = await axios.get('/Repository/GetRepositories');
            this.repositories = response.data;
        },
        clearMessage() {
            var that = this;
            setTimeout(() => {
                that.message = '';
            }, 2000);
        },
        closeModal() {
            this.showModal = false;
        },
        async addRepository() {
            if (this.model.url && this.model.token && this.model.owner && this.model.name) {
                var response = await axios.post('/Repository/AddRepository', this.model);

                let result = response.data;

                if (result && result === true) {
                    this.showModal = false;
                    this.message = "Repository is added."
                    this.clearMessage();
                    await this.get();
                }
                else {
                    this.showModal = false;
                    this.clearMessage();
                    this.message = "There is a problem with adding a repository"
                }
            }
            else {

            }
        },
        async removeRepository(id) {
            if (confirm('Are you sure ?')) {
                var response = await axios.post('/Repository/RemoveRepository', {
                    id: id
                });

                let result = response.data;

                if (result && result === true) {
                    this.message = "Repository is removed."
                    this.clearMessage();
                    await this.get();
                }
                else {
                    this.clearMessage();
                    this.message = "There is a problem with removing a repository"
                }
            }
        },
        async isValid() {
            var response = await axios.get('/Repository/GetValidation', {
                params: {
                    url: this.model.url
                }
            });

            let result = response.data;

            if (result !== "") {
                this.model.valid = result.valid;
                this.model.webhook = result.webhook;
            }
            else {
                this.model.valid = false;
                this.model.webhook = false;
            }
        }
    }
})