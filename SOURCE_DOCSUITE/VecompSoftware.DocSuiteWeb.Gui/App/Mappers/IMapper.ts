interface IMapper<T> {

    Map(source: any) : T;
}

export = IMapper;