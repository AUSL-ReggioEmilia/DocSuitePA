
export abstract class BaseMapper<T, TResult> {

    constructor() { }

    abstract map(source: T): TResult;
}
