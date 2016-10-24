import { Component, OnInit, Input,Output,EventEmitter } from '@angular/core';

import { PointComponent } from './point/point.component';

@Component({
    moduleId: module.id,
    selector: 'resource-api',
    templateUrl: 'resource.component.html',
    styleUrls: ['resource.component.css'],
    directives: [PointComponent]

})
export class ResourceComponent implements OnInit {
    constructor() { }

    @Input() name: string;
    @Input() point: any;
    @Output() setCurrent = new EventEmitter<boolean>();
    open0: boolean = false;
    ngOnInit() {
    }

    Open() {
        this.open0= !this.open0;
        this.setCurrent.emit(this.open0);
    }

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
}

