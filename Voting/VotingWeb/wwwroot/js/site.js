var app = angular.module('VotingApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('VotingAppController', ['$rootScope', '$scope', '$http', '$timeout', 'notificationService',
    function ($rootScope, $scope, $http, $timeout, notificationService) {

    $scope.load = function () {
        $http.get('api/Votes?c=' + new Date().getTime())
            .then(function (data, status) {
                $scope.characters = data;
            }, function (data, status) {
                $scope.characters = undefined;
                notificationService.displayError('Can not load characters!');
            });
    };

    $scope.remove = function (item) {
        $http.delete('api/Votes/' + item)
            .then(function (data, status) {
                $scope.load();
            },
            function () {
                notificationService.displayError('Can not delete this character!');
            }
        )
    };

    $scope.vote = function (item) {
        var fd = new FormData();
        fd.append('item', item);
        $http.put('api/Votes/' + item, fd, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        })
            .then(function (data, status) {
                $scope.load();
                $scope.item = undefined;
            },
            function () {
                notificationService.displayError('The vote was not saved!');
            }
        )
    };
}]);