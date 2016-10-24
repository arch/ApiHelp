import { Component, OnInit,Input} from '@angular/core';

import { PointComponent } from '../resource/point/point.component';
@Component({
    moduleId: module.id,
    selector: 'resource-new',
    templateUrl: 'resource.component.html',
    styleUrls:['resource.component.css'],
    directives:[PointComponent]
})
export class ResourceNewComponent implements OnInit {
    constructor() { }
    
    @Input() resourceList:any;
    ngOnInit() { 
        
    }

    point:any;
    name:any;
    current:number;//=0;
    toArray(object){
        var arr = new Array<any>();
        for (var key in object) {
            if (object.hasOwnProperty(key)) {
                var name = key;
                var value = object[key];
                arr.push(name);
            }
        }
        return arr;
    }

    bindPoint(item,index){
        this.point = item.point;
        this.name = item.name;
        this.current = index;
    }

    test(data){
        console.log(data)
    }
}