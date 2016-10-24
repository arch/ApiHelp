export class PointModel{
    Summary:string;
    Response:Response;
    Request:any;
}

export class Response{
    Scaffold:any;
    Schema: any;
    SupportedMediaType: any;
}

export class ModelResponse{
    ModelType:string;
    Model:any;
}

export class DataModel{
    constructor(public name:string,public value){}
}

export class ValueModel{
    IsOptional:boolean;
    Summary:string;
    Type:string;
}
