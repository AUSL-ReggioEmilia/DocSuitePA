(function () {

    String.prototype.format = function () {
        var s = this,
            i = arguments.length;

        while (i--) {
            s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
        }
        return s;
    };

    Number.prototype.padLeft = function (n, str) {
        return Array(n - String(this).length + 1).join(str || '0') + this;
    }
})();