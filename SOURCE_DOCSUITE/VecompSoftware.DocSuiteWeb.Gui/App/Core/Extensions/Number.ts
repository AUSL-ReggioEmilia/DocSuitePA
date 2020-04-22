
interface Number {
    padLeft(size: number, chToPad?: string): string;
}

define(() => {
    Number.prototype.padLeft = (size: number, chToPad: string = '0'): string => {
        let s = String(this);
        while (s.length < (size || 2)) { s = chToPad + s; }
        return s;
    }
});