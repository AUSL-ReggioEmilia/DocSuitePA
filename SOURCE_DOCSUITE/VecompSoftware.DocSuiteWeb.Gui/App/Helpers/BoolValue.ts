/*
 * Helper class desiged to convert 
 *  string: "false" "False" "true" "True" 
 *  numeric: 1 = true, other = false
 *  boolean : as is
 *  
 */
class BoolValue {
    private _true_number = 1;
    private _false_number = 0;
    private _true_string = "true";
    private _false_string = "false";

    private valueAsNumber: number;
    private valueAsString: string;
    private valueAsBoolean: boolean;

    private isValid: boolean;

    public get ValueAsNumber(): number {
        return this.valueAsNumber;
    }

    public get ValueAsString(): string {
        return this.valueAsString;
    }

    public get ValueAsBoolean(): boolean {
        return this.valueAsBoolean;
    }

    public get IsValid(): boolean {
        return this.isValid;
    }

    constructor(source: string | number | boolean, throwIfInvalid: boolean = true) {
        if (source === null || source === undefined) {
            throw new Error('The boolean value cannot be null');
        }
        var _source = source;

        if (typeof _source === 'string') {
            if (_source.toLowerCase() === this._true_string) {
                this.valueAsString = this._true_string;
                this.valueAsNumber = this._true_number;
                this.valueAsBoolean = true;
            }
            else if (_source.toLowerCase() === this._false_string) {
                this.valueAsString = this._false_string;
                this.valueAsNumber = this._false_number;
                this.valueAsBoolean = false;
            } else {
                try {
                    if (new RegExp('^[0-9]$').test(_source)) {
                        // will be caught in the numeric if
                        let _numeric = parseInt(_source, 10);
                        _source = _numeric;
                    }
                } catch {

                }
            }
        }

        if (typeof _source === 'number') {
            if (_source === this._true_number) {
                this.valueAsString = this._true_string;
                this.valueAsNumber = this._true_number;
                this.valueAsBoolean = true;
            } else {
                // any number other then 1 is false
                this.valueAsString = this._false_string;
                this.valueAsNumber = this._false_number;
                this.valueAsBoolean = false;
            }
        }
        if (typeof _source === "boolean") {
            if (_source === true) {
                this.valueAsString = this._true_string;
                this.valueAsNumber = this._true_number;
                this.valueAsBoolean = true;
            } else if (_source === false) {
                this.valueAsString = this._false_string;
                this.valueAsNumber = this._false_number;
                this.valueAsBoolean = false;
            }
        }

        if (this.valueAsString === null || this.valueAsString === undefined
            || this.valueAsNumber === null || this.valueAsNumber === undefined) {
            // happens when passed number not matching enum values
            // happens when passing a string not matching enum values
            this.isValid = false;
            if (throwIfInvalid) {
                throw new Error(`The provided boolean value \'${source}}\' is not valid`);
            }
        } else {
            this.isValid = true;
        }
    }

    public Equals(source: any): boolean {
        if (!this.isValid) {
            return false;
        }
        let other = new BoolValue(source, false);
        if (!other.isValid) {
            return false;
        }
        return this.valueAsBoolean === other.valueAsBoolean;
    }
}

export = BoolValue;