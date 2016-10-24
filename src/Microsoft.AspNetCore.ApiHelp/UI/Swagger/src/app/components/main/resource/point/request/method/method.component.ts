import { Component, OnInit, Input } from '@angular/core';

import { MethodEnum } from '../../method.enum';
import { JsonHandleComponent } from '../../../../jsonHandle/json-handle.component';
import { ContentTypeComponent } from '../../content-type/content-type.component';
@Component({
    moduleId: module.id,
    selector: 'method',
    templateUrl: 'method.component.html',
    styleUrls:['method.component.css'],
    directives:[JsonHandleComponent,ContentTypeComponent]
})
export class MethodComponent implements OnInit {
    constructor() { }

    @Input() type: MethodEnum;
    @Input() parameters: any;
    formatValue:string="";
    ngOnInit() { }

    jsonHandle(object) {
        var item = [];
        if (typeof object == 'object') {
            if (object instanceof Array) {

            } else {
                for (var key in object) {
                    if (object.hasOwnProperty(key)) {
                        if(key!="SupportedMediaType")
                        {
                            var name = key;
                            var value = object[key];
                            item.push({ name, value });
                        }
                    }
                }
            }
        }
        return item;
    }

    getDataType(data) {
        if (typeof data == 'object') {
            if (data instanceof Array) {
                return 'array';
            }
            else {
                return 'object'
            }
        } else {
            return typeof data;
        }
    }
    
    getJsonString(json){
        var jsonString = JSON.stringify(json);
        return jsonString;
    }
    
    getValue(){
        // console.log(123123);
    }
    
    onGetValue(json:string){
        this.formatValue =json;
    }
    

    test(data) {
        alert(123);
        console.log(data);
        debugger;
    }
}