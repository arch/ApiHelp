import { Component } from '@angular/core';

import './rxjs-operators';

import { HeaderComponent } from './components/header/header.component'; 
import { MainComponent } from './components/main/main.component';

@Component({
  selector: 'app',
  pipes: [],
  providers: [],
  // directives: [ ROUTER_DIRECTIVES ],
  directives:[HeaderComponent,MainComponent],
  templateUrl: './app.component.html',
})
export class App {
  constructor() {}
}
