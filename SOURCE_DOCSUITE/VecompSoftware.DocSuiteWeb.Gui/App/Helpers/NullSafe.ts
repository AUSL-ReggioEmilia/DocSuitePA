class NullSafe {
    static Do<T>(func : { () : T } , defaultValue : T): T
    {
        try {
            let result = func();

            if (result == undefined || result == null) {
                return defaultValue;
            }

            return result;

        } catch (e) {
            return defaultValue;
        }
    }
}

export = NullSafe;