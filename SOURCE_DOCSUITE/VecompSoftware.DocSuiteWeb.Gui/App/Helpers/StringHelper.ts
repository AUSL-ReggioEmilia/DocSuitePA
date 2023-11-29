

class StringHelper {

    public format(value: string, ...content): string {
        // var testString : string = "{1}asdas{2:d}s{4:A.}das{asd:}sa{1222223:d.2}dasasdas{3}";
        const matchResult: string[] = value.match(/\{\d+(\:[a-zA-Z]([0-9]{0,}(\.{0,1}[1-9]){0,}))?\}/gi);
        const indices: Array<number> = [];
        let numericPart: number;
        let parts: Array<string>;
        let current: string;

        for (const result of matchResult) {
            current = result;
            parts = current.split(':');
            numericPart = parseInt(parts[0].replace(/\{?\}?/ig, ''), 10);
            if (numericPart < 0) {
                throw new Error('Negative identifier found');
            }
            if (indices.indexOf(numericPart) > -1) {
                throw new Error('Duplicate identifier found');
            }
            indices.push(numericPart);
        }
        let newString: string = value;
        for (let index = 0; index < content.length; index++) {
            newString = newString.replace(matchResult[index], content[indices[index]]);
        }
        return newString;
    };

    public pad(number: number, length: number) {
        let str: string = '' + number;
        while (str.length < length) {
            str = '0' + str;
        }
        return str;
    }

}

export = StringHelper;