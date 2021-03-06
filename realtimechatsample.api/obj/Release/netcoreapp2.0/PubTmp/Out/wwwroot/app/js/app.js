var app= angular
.module('chat', [   
    'chat.app',
    'ui.bootstrap',
    'angular-jwt'
])

//'angular-jwt',

app.constant('$', window.jQuery);

app.controller('mainController', function ($scope, $timeout, ENV, $uibModal, authService, chatsignalr, connectedUsers,$rootScope) {

    var vm = this;
    vm.message = "";
    vm.messages = [];
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
    vm.logout = function () {
        authService.logout();
    }
    vm.login = function () {
        authService.auth(vm.Username, vm.Password).then(function (resp) {
            authService.login(resp.data);
            
        }, function () {
            alert("Usuario y/o password incorrectas.");
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

    vm.EnrolClic = function () {
        vm.isEnrol = true;
        return vm.isEnrol;
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

    $rootScope.$on("ReceiveMessage", function (evt,xx) {
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

                    }, $scope.room, $scope.alia);

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

})
