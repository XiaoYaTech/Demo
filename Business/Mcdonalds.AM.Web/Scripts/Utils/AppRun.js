validateUser = function ($window, messager) {
    setTimeout(function () {
        if (!$window.currentUser) {
            $window.parent.location.href = Utils.ServiceURI.AppUri + "Error/LoginError.html";
        }
    }, 1000);
}