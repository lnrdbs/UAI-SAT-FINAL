var app= angular
.module('chat', [   
    'chat.app',
    'ui.bootstrap',
    'angular-jwt'
])

//'angular-jwt',

app.constant('$', window.jQuery);

app.controller('mainController', function ($scope, $timeout, ENV, $uibModal, authService, chatsignalr, connectedUsers, $rootScope, inmuebleService) {

    var vm = this;
    vm.message = "";
    vm.messages = [];
    //vm.inmuebles = {};
    vm.roomid = undefined;
    vm.isEnrol = false;
    vm.localuser = "";


    $scope.$watch('vm.message', function () {
        if (angular.isUndefined(vm.message) == true || vm.message == "")
            return;

        chatsignalr.getProxy().invoke("SayWhoIsTyping", function (x) {
            
        }, vm.roomid, vm.userid);
    });

    vm.getWhoIsTyping = function () {
        var u = connectedUsers.getWhoIsTyping();

        if (angular.isUndefined(u) == true || u == "")
            return "";

        if (!angular.isUndefined(vm.timerSayWhoIsTyping) && vm.timerSayWhoIsTyping != null)
            $timeout.cancel(vm.timerSayWhoIsTyping);

        vm.timerSayWhoIsTyping = $timeout(function () {
            vm.timerSayWhoIsTyping = null;
            connectedUsers.setWhoIsTyping("");
        }, 2000);

        return u + " esta escribiendo...";
    }

    vm.isAuthenticated = function () {
        return authService.isAuthenticated();
    }

    vm.IsAdmin = function () {
        return (authService.getUserId()=="admin");
    }
    
    vm.logout = function () {
        authService.logout();
    }
    vm.login = function () {
        authService.auth(vm.Username, vm.Password, vm.IP).then(function (resp) {
            authService.login(resp.data);
            connectedUsers.clear();
            chatsignalr.getProxy().invoke("JoinRoom", function (x) {
            }, 'Inmobiliaria', vm.Username);
        }, function () {
            alert("Usuario y/o password incorrectas.");
        })
    }

    vm.crearInmueble = function () {
        var Inmueble = new Object();  
        Inmueble.Barrio = vm.Barrio;
        Inmueble.Descripcion = vm.Descripcion;
        Inmueble.Id = vm.Id;
        Inmueble.Titulo = vm.Titulo;
        Inmueble.Abierto = '1';
        Inmueble.Imagen = vm.Imagen;
        Inmueble.Valoracion = '';
        Inmueble.VotosPositivos = 0;
        Inmueble.VotosNegativos = 0;
        
        authService.NuevoInmueble(Inmueble).then(function (resp) {
            alert("Registro creado con exito!!");
            //chatsignalr.getProxy().showNewPublish('Inmobiliaria', vm.getUserId(), vm.Titulo, vm.Id)
            chatsignalr.getProxy().sendMessage('Nueva Publicacion Nro ' + vm.Id + ' !!! Puede votar el Barrio: ' + vm.Barrio, 'Inmobiliaria', vm.getUserId()).done(function () {  
            });
        }, function () {
            alert("Registro Duplicado!!");
        })
    }

    // Nuevo registro
    vm.registro = function () {
        authService.enrol(vm.Username, vm.Password).then(function (resp) {
            authService.registro(resp.data);
            alert("Usuario Registrado con Exito");
            vm.isEnrol = false;
        }, function () {
            alert("El nombre de usuario se encuentra registrado, seleccione otro.");
        })
    }

    vm.getInmuebles = function () {
        if (vm.isAuthenticated()) {
            inmuebleService.all().then(function (resp) {
                vm.inmuebles = resp.data;
                $scope.inmuebleslocal = resp.data;
            }, function (a) { vm.error = "unauthorized"; });
        }
    }

    vm.EnrolClic = function () {
        vm.isEnrol = true;
        return vm.isEnrol;
    }

    vm.Back = function () {
        window.history.back();
    }

    vm.LoginClic = function () {
        vm.isEnrol = false;
        return vm.isEnrol;
    }

    vm.getUserId = function () {
        return authService.getUserId();
    }

    vm.room = function () {
        var x = connectedUsers.all();
        return x;
    }

    vm.send = function () {
        chatsignalr.getProxy().sendMessage(vm.message, vm.roomid, vm.userid).done(function () {
            vm.message = "";
        });
    }

    vm.disconnect = function () {
       
        chatsignalr.getProxy().invoke("LeaveRoom", function (x) {
        
        }, vm.roomid, vm.userid);
        
    }

    $rootScope.$on("ReceiveMessage", function (evt, xx) {
        vm.messages.push(xx);
    })

    $rootScope.$on("ReceiveVoteMessage", function (evt, xx) {
        $scope.inmuebleslocal[xx.index].votosPositivos += xx.pos; 
        $scope.inmuebleslocal[xx.index].votosNegativos += xx.neg;
        $scope.inmuebleslocal.push($scope.inmuebleslocal);
    })

    // #### Trabajar con la vista
    //  Cuando se crear un nuevo inmueble
    //  -> chatsignalr.getProxy().showNewPublish(room, alias, titulo, idPublish)
    //  
    //  Cuando se cierra la publicación
    //  -> chatsignalr.getProxy().showPublishClosed(room, alias, titulo, idPublish)

    // #### Este evento hay que trabajar la vista
    $rootScope.$on("ShowNewPublish", function (evt, xx) {
        // #### Diseñar vista
        // --------------------------------------
        // Marcos (alias) 
        // Casa en pilar (titulo)
        // ¿Que onda el barrio?
        //  Bueno (usar id)       Malo (usar id)
        // --------------------------------------
        vm.messages.push(xx);
    })

    // #### Este evento hay que trabajar la vista
    $rootScope.$on("ShowPublishClosed", function (evt, xx) {
        // #### Diseñar vista
        // --------------------------------------
        // Encuesta cerrada
        // Marcos (alias)
        // Casa en pilar (titulo)
        // ¿Que onda el barrio?
        // Bueno: 20vts     Malo: 5vts  
        // --------------------------------------
        vm.messages.push(xx);
    })


    vm.rooms = function () {
        var x = connectedUsers.allRooms();
        return x;
    }

    vm.openModalConfig = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            size: 'lg',
            ariaLabelledBy: 'modal-title',
            ariaDescribedBy: 'modal-body',
            templateUrl: '/app/views/config.html',
            controller: function ($scope, $uibModalInstance, chatsignalr, connectedUsers) {

                $scope.ok = function () {
                  
                    vm.messages = [];
                    connectedUsers.clear();
                    chatsignalr.getProxy().invoke("JoinRoom", function (x) {

                        $uibModalInstance.close({ $value: { room: $scope.room, user: $scope.alias } });

                    }, $scope.room, $scope.alias);

                };
                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }
        });

        modalInstance.result.then(function (e) {
            vm.roomid = e.$value.room;
            vm.userid = e.$value.user;

        }, function () {

        });
    }

    vm.votar = function (id, voto, index) {
        inmuebleService.votar(id, voto).then(function (resp) {
            var pos = (voto == 1) ? 1 : 0;
            var neg = (voto == 0) ? 1 : 0;
           
            chatsignalr.getProxy().votoMessage(index, pos, neg, 'Inmobiliaria').done(function () {
            });
        }, function (a) { vm.error = "unauthorized"; });
    }

    vm.cerrarvotacion = function (id, titulo, barrio) {
        inmuebleService.cerrar(id).then(function (resp) {
            var a = vm.getInmuebles();
            alert("Votacion cerrada con exito!!");
            chatsignalr.getProxy().sendMessage('Votacion Cerrada!!! Pubicacion Nro: ' + id + ' - Barrio: ' + barrio, 'Inmobiliaria', vm.getUserId()).done(function () {
            });
            
        }, function (a) { vm.error = "unauthorized"; });
    }
})
