window.blazorExtentions = {
    SetCookie: function (name, value) {
        document.cookie = name + "=" + value + "; path=/";
    },

    RemoveCookie: function (name) {
        document.cookie = name + "=; Path=/; Expires=;";
    }
}