
/**
 * EnumValue is a helper class intented to be used in a property with a getter.
 * The models from the api can provide in an enum propery a number or a string. In all cases we want to
 * be able to handle all of them without caring.
 * 
 * Usage:
 * 
 * enum TemplateCollaborationRepresentationType {
      FixedTemplates = 0,
      Template = 1,
      Folder = 2
  }
  let v1 = new EnumValue('FixedTemplates', TemplateCollaborationRepresentationType);
  let v2 = new EnumValue('Template', TemplateCollaborationRepresentationType);

   v1 === v2; v1 === 0; v1 === '0'; v2 === 'FixedTemplates'
 **/

class EnumValue {
    private valueAsNumber: number;
    private valueAsString: string;
    private isValid: boolean;
    private enumType: any;

    public get ValueAsNumber(): number {
        return this.valueAsNumber;
    }

    public get ValueAsString(): string {
        return this.valueAsString;
    }

    public get IsValid(): boolean {
        return this.isValid;
    }

    constructor(source: string | number, enumType: any, throwIfInvalid: boolean = true) {
        this.enumType = enumType;
        if (source === null || source === undefined) {
            throw new Error('The enumeration class cannot have a null value');
        }

        if (enumType === null || source === undefined) {
            throw new Error('An invalid enum type was passed');
        }

        if (typeof source === 'string') {
            this.valueAsString = source;
            this.valueAsNumber = enumType[this.valueAsString];
        }
        else if (typeof source === 'number') {
            this.valueAsNumber = source;
            this.valueAsString = enumType[this.valueAsNumber];
        } else {
            throw new Error('The provided enum value was not recognized');
        }

        if (this.valueAsString === null || this.valueAsString === undefined
            || this.valueAsNumber === null || this.valueAsNumber === undefined) {
            // happens when passed number not matching enum values
            // happens when passing a string not matching enum values
            this.isValid = false;
            if (throwIfInvalid) {
                throw new Error(`The provided enum value \'${source}}\' does not match enum values`);
            }
        } else {
            this.isValid = true;
        }
    }

    public Equals(source: any): boolean {
        if (!this.isValid) {
            return false;
        }
        let other = new EnumValue(source, this.enumType, false);
        if (!other.isValid) {
            return false;
        }
        return this.valueAsNumber === other.valueAsNumber;
    }
}

export = EnumValue;