var WebReturn = (function () {
    function WebReturn(result) {
        this.result = result;
        var firstPos = result.indexOf('|');
        this.success = result.substring(0, firstPos) == '0';
        var secondPos = result.indexOf('|', firstPos + 1);
        this.message = result.substring(firstPos + 1, secondPos);
        var thirdPos = result.indexOf('|', secondPos + 1);
        this.format = result.substring(secondPos + 1, thirdPos);
        this.content = result.substring(thirdPos + 1, result.length);
    }
    return WebReturn;
})();
//# sourceMappingURL=commonutil.js.map