var app = angular.module('app.chat.signalr', ['angular-jwt']);
app.constant('$', window.jQuery);

app.factory('connectedUsers', function () {


    var whoIsTyping = "";
    var conectados = [];
    var rooms = [];
    var exists = function (room) {
        var ok=undefined ;
        rooms.forEach(function (r) {
            if (r.id == room)
                ok = r;
        })

        return ok;
    }

    return {
        getWhoIsTyping: function () {
            return whoIsTyping;
        },
        setWhoIsTyping: function (user) {
            whoIsTyping = user;
        },
        clear: function () {
            conectados = [];
        },
        createRoom: function (roomId) {
            var room = exists(roomId);
            if (!room) {
                room = {}; 
                room.id = roomId;
                room.users = [];
            
                rooms.push(room);
            }
        },
        initialize: function () {
            conectados = [];
            rooms = [];
        },
        add: function (usuario) {

          //  if (conectados.filter(function (idx) { return idx.ConnectionId == usuario.ConnectionId; }).length == 0)
            conectados.push(usuario);
        },
     
        all: function () {
            return conectados;
        },
        remove: function (usuario) {

           
            var idx = 0;
            conectados.forEach(function (i) {

                if (i.ConnectionId == usuario) {
                    conectados.splice(idx, 1);
                }
                idx++;
            })

            //var index = conectados.indexOf(usuario);
            //conectados.splice(index, 1);
        },
        room: function (roomid) {
            return exists(roomid);
        },
        allRooms: function () {
            return rooms;
        },

      
    }

});

app.factory('backendHubProxy', function ($, ENV, $rootScope) {

        function backendFactory(serverUrl, hubName) {
            var connection = $.hubConnection(ENV.backendServerUrl);
       
           var userId = 'unusuario';
           
            connection.qs = { "userId": userId};
            var proxy = connection.createHubProxy(hubName);
            return {
                getConnection:function()
                {
                    return connection.id;
                },
                disconnect: function () {
                    return connection.stop();
                },
                start: function () {
                
                    return connection.start(['websocket', 'longPolling', 'serverSentEvents', 'foreverFrame']);
                },
                on: function (eventName, callback) {

                    proxy.on(eventName, function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
                },
                sendMessage: function (a, b, c) {
                    return proxy.invoke("SendMessage", a, b,c);
                },
                invoke: function (methodName, callback, params,p2) {
                       proxy.invoke(methodName, params,p2).done(function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });


                    //  });
                }
            };
        };

        return backendFactory;
});

app.factory('chatsignalr', function (connectedUsers, backendHubProxy, $rootScope) {    
    var proxy;
    var disconnect = function () {
        proxy.disconnect();
    }

    var getProxy = function () {
        return proxy;
    }
    var initialize = function () {

        connectedUsers.initialize()
        proxy = new backendHubProxy(backendHubProxy.defaultServer, 'chatHub');
    
        proxy.on("AllConnectedInRoom", function (dto) {

        });


        return proxy.start();

    }

    var getConnectionId = function()
    {
        return proxy.getConnection();

    }
    return {
        initialize: initialize,
        disconnect: disconnect,
        getProxy: getProxy,
        getConnectionId:getConnectionId
    }


})

app.factory('authService', function ($http, ENV, jwtHelper, $window, $location) {



    var $tokenStorage = $window.localStorage['token'];
    var $tokenDecodedStorage = $window.localStorage['token-decoded'];
    var token;
    var userId;
    var tokenDecoded;

    if (token) {
        $window.localStorage['token'] = token;
        tokenDecoded = jwtHelper.decodeToken(token)
        $window.localStorage['token-decoded'] = tokenDecoded;
        userId = tokenDecoded.nameidentifier;

    }



    var getToken = function () {

        return token;
    }

    var getUserId = function () {
        if ($window.localStorage['token-decoded'] != undefined)
            return $window.localStorage['token-decoded'].nameidentifier;
        return undefined;
    }
    var auth = function (username, password) {
        return $http.post(ENV.apiEndpoint + '/auth', { Username: username, Password: password });
    }

    var enrol = function (username, password) {
        return $http.post(ENV.apiEndpoint + '/enrol', { Username: username, Password: password });
    }

    var getTokenDecoded = function () {
        return tokenDecoded;
    }

    var login = function (data) {
        token = data.token;
        $window.localStorage['token'] = token;
        tokenDecoded = jwtHelper.decodeToken(token)
        $window.localStorage['token-decoded'] = tokenDecoded;


    }

    var registro = function (data) {
        token = data.token;
        $window.localStorage['token'] = token;
        tokenDecoded = jwtHelper.decodeToken(token)
        $window.localStorage['token-decoded'] = tokenDecoded;


    }

    var reset = function () {
        token = undefined;
        userId = undefined;
        tokenDecoded = undefined;
        $window.localStorage['token'] = undefined;
        $window.localStorage['token-decoded'] = undefined;
        $window.localStorage.clear();
    }

    var logout = function () {
        reset();
        // $window.location.href = "#/login"
    }

    var isAuthenticated = function () {
        var tk = $window.localStorage['token'];
        return (!!tk && tk != undefined && !jwtHelper.isTokenExpired($window.localStorage['token']));

    }

    var loadTokenFromCookies = function () {
        //var tk = $cookies.get(ENV.appId);
        //  if (tk) securityToken=tk;
        securityToken = $window.localStorage['token'];

        if (securityToken && securityToken != "undefined") {

            if (jwtHelper.isTokenExpired(securityToken)) {

                this.logout();
            }
        }

    }


    return {
        loadTokenFromCookies: loadTokenFromCookies,
        auth: auth,
        enrol: enrol,
        login: login,
        registro: registro,
        isAuthenticated: isAuthenticated,
        getToken: getToken,
        getTokenDecoded: getTokenDecoded,
        logout: logout,
        getUserId: getUserId,
        reset: reset,

    };
});