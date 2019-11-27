import { Component } from '@angular/core';
import { StoriesService, StoryModel } from './services/stories/stories.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Sample Angular Client';

  constructor() { }
}
