

class StringHelper {

    public pad(number: number, length: number) {
        let str: string = '' + number;
        while (str.length < length) {
            str = '0' + str;
        }
        return str;
    }

}

export = StringHelper;