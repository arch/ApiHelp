import { Component, OnInit,Input } from '@angular/core';

import { MethodComponent } from './method/method.component';
import { MethodEnum } from '../method.enum';
@Component({
    moduleId: module.id,
    selector: 'request',
    templateUrl: 'request.component.html',
    directives:[MethodComponent]
})
export class RequestComponent implements OnInit {
    constructor() { }
    
    @Input() method:MethodEnum;
    @Input() request:any;
    ngOnInit() { 
    }

}