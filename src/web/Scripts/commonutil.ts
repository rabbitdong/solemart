class WebReturn {
    success: boolean;
    message: string;
    format: string;
    content: string;
    constructor(public result: string) {
        var firstPos = result.indexOf('|');
        this.success = result.substring(0, firstPos) == '0';
        var secondPos = result.indexOf('|', firstPos + 1);
        this.message = result.substring(firstPos + 1, secondPos);
        var thirdPos = result.indexOf('|', secondPos + 1);
        this.format = result.substring(secondPos + 1, thirdPos);
        this.content = result.substring(thirdPos + 1, result.length);
    }
}

class Validator {
    public static ValidatePhone(phone: string): Boolean {
        if ((phone.length == 12 && phone.indexOf("0591") == 0)
            || phone.length == 11)
            return true;
        return false;
    }

    public static ValidateEmail(email: string): Boolean {
        var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        return re.test(email);
    }
}
