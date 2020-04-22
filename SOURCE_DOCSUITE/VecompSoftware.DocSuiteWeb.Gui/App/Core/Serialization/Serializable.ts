import ClassMapping = require('App/Core/Serialization/ClassMapping');

/**
 * Classe astratta che permette per la serializzazione/deserializzazione
 * di oggetti JSON e che permette di ricreare l'istanza della classe in maniera ricorsiva.
 */
abstract class Serializable {

    private _classMappings: Array<ClassMapping>;

    /**
     * Costruttore
     */
    constructor() {
        this._classMappings = new Array<ClassMapping>();
        this.initClassMapping();
    }

    /**
     * Permette di deserializzare un oggetto JSON nella classe specifica
     * @param JSO
     */
    fromJson(JSO: string)
    fromJson(JSO: any)
    fromJson(JSO?: any): void {
        this.makeObject(JSO, this);
    }

    /**
     * Serializza la classe in un oggetto JSON
     */
    toJson(): string {
        return JSON.stringify(this);
    }

    /**
     * Aggiunge la mappatura delle proprietà figlie da deserializzare in maniera ricorsiva
     * @param tclass
     * @param attributeName
     */
    addChildClassMapping<TClass extends Serializable>(tclass: { new (): TClass; }, attributeName: string): void {
        let classMap: ClassMapping = <ClassMapping>{};
        classMap.attributeName = attributeName;
        classMap.className = (<any>tclass).name;
        classMap.classObject = tclass;
        this._classMappings.push(classMap);
    }

    /**
     * Implementazione mappatura figlie
     */
    abstract initClassMapping(): void;

    /**
     * Dato un attribute_name ritorna la relativa mappatura se definita
     * @param attrName
     */
    private getClassMappingFor(attrName: string): ClassMapping {
        let props: Array<ClassMapping> = $.grep(this._classMappings, (classMap: ClassMapping, index: number) => {
            return classMap.attributeName == attrName;
        });

        if (props.length > 0) {
            return props[0];
        }

        return null;
    };

    /**
     * Crea l'istanza delle proprietà figlie se definite nella mappatura
     * @param prop
     * @param obj
     */
    private makeInstance(prop, obj): any {
        let classMap: ClassMapping = this.getClassMappingFor(prop);
        if (classMap != undefined && classMap.classObject instanceof Function) {
            let instance: any = new classMap.classObject();
            if (instance instanceof Serializable) {
                instance.fromJson(obj);
                return instance;
            }
            return obj;
        } else {
            return obj;
        }
    }

    /**
     * Ritorna l'elemento verificando se creare una nuova istanza o ritornare l'elemento stesso
     * @param obj
     * @param propName
     */
    private resolveElement(obj: any, propName: string) {
        if (obj == undefined)
            return obj;

        if (this.isArray(obj)) {
            let propCollection: Array<any> = new Array<any>();
            $.each(obj, (index: number, prop: any) => {
                propCollection.push(this.resolveElement(prop, propName));
            });            
            return propCollection;
        } else if (this.isObject(obj)) {
            return this.makeInstance(propName, obj);
        } else {
            return obj;
        }
    }

    /**
     * Crea il nuovo oggetto
     * @param JSO
     * @param thisx
     */
    private makeObject(JSO: any, thisx: any): void {
        JSO = (this.isObject(JSO)) ? JSO : JSON.parse(JSO);
        for(let prop in JSO) {
            let attrName: string = prop;
            if ((this.isArray(JSO[prop]) || this.isObject(JSO[prop])) && JSO[prop] != undefined) {
                let classMap: ClassMapping = this.getClassMappingFor(prop);
                attrName = classMap ? classMap.attributeName : attrName;
            }

            thisx[prop] = this.resolveElement(JSO[prop], attrName);
        };
    }

    /**
     * Verifica se l'elemento passato è un array
     * @param obj
     */
    private isArray(obj: any): boolean {
        return $.isArray(obj);
    }

    /**
     * Verifica se l'elemento passato è un oggetto
     * @param obj
     */
    private isObject(obj: any) {
        return $.isPlainObject(obj);
    }
}

export = Serializable;