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
        },
        message: ''
    },
    mounted() {
        this.get();
    },
    filters: {
        duration : function(value){
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
            if (this.model.url) {
                var response = await axios.post('/Repository/AddRepository', {
                    url: this.model.url
                });

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