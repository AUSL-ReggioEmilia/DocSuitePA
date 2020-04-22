
interface StringConstructor {
    isNullOrEmpty(value?: string): boolean;
    format(value: string, ...params: any[]): string;
}

define(() => {
    String.isNullOrEmpty = (value?: string): boolean => {
        return value == null || value == '';
    }

    String.format = (value: string, ...params: any[]): string => {
        return value.replace(/{(\d+)}/g, function (match: string, number: number) {
            return typeof params[number] != 'undefined' ? params[number] : match;
        });
    }
});