angular.module('app.chat.controllers', []);
angular.module('app.chat.services', [])

var app = angular.module('chat.app', [
    'app.chat.environment',
    'app.chat.controllers',
    'app.chat.services',
    'app.chat.signalr'
    
]);


app.run(function ($rootScope, $http, chatsignalr, connectedUsers, $q) {



    var initialize = function () {
        
        var deferred = $q.defer();
            
                chatsignalr.initialize().done(function (xx) {
                

                    chatsignalr.getProxy().on('NewRoom', function (room) {
                        if (room != undefined) {
                           
                                connectedUsers.createRoom(room)
                            

                        }


                    });



                    chatsignalr.getProxy().on('GetAllRooms', function (rooms) {
                        if (rooms != undefined) {
                            rooms.forEach(function(room){
                                connectedUsers.createRoom(room)
                            })
                       
                        }
                      

                    });

                    chatsignalr.getProxy().on("LeaveRoom", function (dto) {
                        //AGREGO A UN ROOM
                        connectedUsers.remove(dto);
                        
                    });

                    chatsignalr.getProxy().on("JoinRoom", function (dto) {
                            //AGREGO A UN ROOM
                            connectedUsers.add(dto);
                            
                        });

                    chatsignalr.getProxy().on("SayWhoIsTyping", function (dto) {
                        connectedUsers.setWhoIsTyping(dto);

                    });

                    chatsignalr.getProxy().on("ShowNewPublish", function (dto) {
                        $rootScope.$emit("ShowNewPublish", dto);
                    });

                    chatsignalr.getProxy().on("ShowPublishClosed", function (dto) {
                        $rootScope.$emit("ShowPublishClosed", dto);
                    });

                    chatsignalr.getProxy().on('AllConnectedInRoom', function (msg) {
                       
                        msg.users.forEach(function (i) {
                            connectedUsers.add(i);
                        })
                    });

                    chatsignalr.getProxy().on('ReceiveMessage', function (xx) {
                     
                        $rootScope.$emit("ReceiveMessage", xx);
                    });

                    chatsignalr.getProxy().on('ReceiveVoteMessage', function (xx) {

                        $rootScope.$emit("ReceiveVoteMessage", xx);
                    });
                  
                    chatsignalr.getProxy().invoke("AllRooms", function (x) { connectedUsers.clear(); });
                  
                    deferred.resolve();
               })

            
     
        return deferred.promise;

    }

   
    initialize().then(function () {
     

    })
   

});

app.factory('authHttpResponseInterceptor', function ($q,
    $location, $rootScope, ENV) {

    return {

        request: function (config) {

            return config || $q.when(config);
        },


        requestError: function (rejection) {

            return $q.reject(rejection);
        },
        response: function (response) {

            var r = response;


            return response || $q.when(response);
        },
        responseError: function (rejection) {

            return $q.reject(rejection);
        }
    }
})
    .config(function ($httpProvider, jwtOptionsProvider, jwtInterceptorProvider) {

        jwtOptionsProvider.config({
            whiteListedDomains: ['localhost', 'www.uai.edu.ar']
        });
        jwtInterceptorProvider.tokenGetter = ['authService', function (authService) {
            var token = authService.getToken();

            return token;
        }];


        $httpProvider.defaults.useXDomain = true;
        $httpProvider.interceptors.push('authHttpResponseInterceptor');
        $httpProvider.interceptors.push('jwtInterceptor');

        //initialize get if not there
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }



    })




