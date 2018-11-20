"use strict";
angular.module('app.chat.environment', [])

    .constant('ENV', {
        name: 'localdev',
        apiEndpoint: 'http://localhost:5000/api',
        backendServerUrl: 'http://localhost:5000'
    })

