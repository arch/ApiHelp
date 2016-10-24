import { Component, OnInit, Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'model-schema',
    templateUrl: 'model-schema.component.html',
    styleUrls:['model-schema.component.css'],
    directives:[ModelSchemaComponent]
})
export class ModelSchemaComponent implements OnInit {
    constructor() { }
    @Input() schema: any;
    ngOnInit() {
    }
    

    jsonHandle(object){
        var item =[];
        if(object instanceof Array){
            // for(var i=0,len = object.length;i<len;i++){
            //     if(typeof object[i]==object)
            // 	    this.jsonHandle(object[i]);
            //     else{
            //         item = object[i];
            //     }
            // }
            return object;
        }else{
            if(typeof object=='object')
            {
                if(object){
                    for(var key in object){
                        var name = key;
                        var value = this.jsonHandle(object[key]);
                        item.push({name,value});
                    }
                }
            }else{
                item = object;
            }
        }
        return item;
    }

     getDataType(data){
        if(typeof data =='object'){
            if(data instanceof Array){
                return 'array';
            }
            else{
                return 'object'
            }
        }else{
             return typeof data;
        }
    }

    test(data){
        var jsonData = this.jsonHandle(data);
        console.log(data);
        console.log(jsonData);
    }
}