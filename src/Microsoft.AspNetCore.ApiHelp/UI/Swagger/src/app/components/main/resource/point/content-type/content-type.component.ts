import { Component, OnInit,Input } from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'content-type',
    templateUrl: 'content-type.component.html',
    styleUrls:['content-type.component.css']
})
export class ContentTypeComponent implements OnInit {
    constructor() { }
    @Input() type_text:string;
    @Input() type_data:any;

    ngOnInit() { }

}