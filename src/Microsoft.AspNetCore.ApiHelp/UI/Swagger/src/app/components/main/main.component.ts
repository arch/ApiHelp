import { Component, OnInit } from '@angular/core';

import { ResourceComponent } from './resource/resource.component';
import { ResourceNewComponent } from './resourceNav/resource.component';
import { ResourceService } from './resourceService.ts';
import { ResourceModel } from './resourceModel.ts';
@Component({
    moduleId: module.id,
    selector: 'main-pager',
    templateUrl: 'main.component.html',
    styleUrls:['main.component.css'],
    directives: [ResourceComponent,ResourceNewComponent],
    providers: [ResourceService]
})
export class MainComponent implements OnInit {
    constructor(private resourceService: ResourceService) { }
    list: Array<ResourceModel> = new Array<ResourceModel>();
    open:boolean=false;
    current:boolean=false;
    ngOnInit() {
        this.resourceService.GetData()
            .subscribe(
            data => this.toArray(data)
            );
    }

    toArray(data: any) {
        for (var prop in data) {
            var item: ResourceModel = new ResourceModel(prop, data[prop]);
            this.list.push(item);
        }
    }
    
    onSetCurrent(current){
        if(current){
            return "resource active";
        }
        else{
            return "resource";
        }
    }

    test(data){
        console.log(data);
    }
}