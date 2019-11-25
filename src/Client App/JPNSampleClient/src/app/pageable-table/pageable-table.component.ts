import { Component, OnInit, Injector } from '@angular/core';
import { StoriesService, StoryModel } from '../services/stories/stories.service';

@Component({
  selector: 'app-pageable-table',
  templateUrl: './pageable-table.component.html',
  styleUrls: ['./pageable-table.component.scss'],
  providers: [
    StoriesService
  ]
})
export class PageableTableComponent implements OnInit {
  columnsToDisplay: string[] = ['author'];
  public stories: StoryModel[];

  constructor(private storiesService: StoriesService) { 
    this.storiesService.getStories().subscribe(result => this.stories = result);
  }

  ngOnInit() {}

}
