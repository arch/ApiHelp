import { Component, OnInit, Input } from '@angular/core';

import { PointService } from './point.service';
import { ModelSchemaComponent } from './schema/model-schema.component';
import { RequestComponent } from './request/request.component';
import { JsonHandleComponent } from '../../jsonHandle/json-handle.component';
import { ContentTypeComponent } from './content-type/content-type.component';

import { PointModel, Response, ModelResponse, DataModel } from './pointModel';
import { MethodEnum } from './method.enum';
@Component({
    moduleId: module.id,
    selector: 'point',
    templateUrl: 'point.component.html',
    styleUrls: ['point.component.css'],
    directives: [ModelSchemaComponent,RequestComponent,JsonHandleComponent,ContentTypeComponent],
    providers: [PointService]
})
export class PointComponent implements OnInit {
    constructor(private pointService: PointService) { }

    modelType: number = 0;
    modelSelected: boolean = false;

    @Input() api: string;
    method: string;
    titleClass: string;
    apiName: string;
    open1: boolean;
    @Input() summary:string;
    response: Response;
    Data: Response;
    Schema: Response;
    SupportedMediaType:any;
    request:any;

    enum:MethodEnum;
    point: PointModel;
    ngOnInit() {
        this.open1 = false;
        this.bindData();
    }
    selectTab(i: number) {
        this.modelType = i;
        if (i == 0) {
            this.modelSelected = true;
        } else {
            this.modelSelected = false;
        }
    }


    bindData() {
        var model = this.api.split(' ');
        this.titleClass = model[0].toLowerCase();
        this.method = model[0];
        this.apiName = model[1];
        
    }

    Open() {
        this.open1 = !this.open1;
        var para = `RelativePath=${this.apiName}&HttpMethod=${this.method}`;
        this.enum = MethodEnum[this.method];
        if (this.open1) {
            this.pointService.GetData(para).subscribe(
                data => this.bindModel(data)
            );
        }
    }

    bindModel(data) {
        var point: PointModel = data[this.api];
        
        // this.summary = point.Summary;
        //repose
        this.response = point.Response;
        
        var modelData = this.clone(this.response);
        delete modelData.Schema;
        delete modelData.SupportedMediaType;
        this.Data = modelData;

        var schemaData = this.clone(this.response);
        delete schemaData.Scaffold;
        delete schemaData.SupportedMediaType
        this.Schema = schemaData;
        
        this.SupportedMediaType = this.response.SupportedMediaType;

        //request
        this.request = point.Request;

        
    }
    
    


    jsonHandle(object) {
        var item = [];
        if (object instanceof Array) {
            for (var i = 0, len = object.length; i < len; i++) {
                if (typeof object[i] == object)
                    this.jsonHandle(object[i]);
                else {
                    item = object[i];
                }
            }
        } else {
            if (typeof object == 'object') {
                if (object) {
                    for (var key in object) {
                        var name = key;
                        var value = this.jsonHandle(object[key]);
                        item.push({ name, value });
                    }
                }
            } else {
                item = object;
            }
        }
        return item;
    }

    clone(obj) {
        var o;
        switch (typeof obj) {
            case 'undefined': break;
            case 'string': o = obj + ''; break;
            case 'number': o = obj - 0; break;
            case 'boolean': o = obj; break;
            case 'object':
                if (obj === null) {
                    o = null;
                } else {
                    if (obj instanceof Array) {
                        o = [];
                        for (var i = 0, len = obj.length; i < len; i++) {
                            o.push(this.clone(obj[i]));
                        }
                    } else {
                        o = {};
                        for (var k in obj) {
                            o[k] = this.clone(obj[k]);
                        }
                    }
                }
                break;
            default:
                o = obj; break;
        }
        return o;
    }


    test(){
        console.log(this.response.SupportedMediaType);
    }
}