import { Component, OnInit } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { StoriesService, StoryModel } from '../../services/stories/stories.service';

@Component({
  selector: "app-data-table",
  templateUrl: "./data-table.component.html",
  styleUrls: ["./data-table.component.scss"]
})
export class DataTableComponent implements OnInit {
  columns: string[] = [ 'author', 'title', 'score' ];
  stories: StoryModel[];
  storiesDataSource: MatTableDataSource<StoryModel>;

  constructor(private storiesService: StoriesService) {
    let observer = storiesService.getStories().subscribe(result => {
      if (result === undefined || result === null)
        return;

      this.stories = result;
      this.storiesDataSource = new MatTableDataSource(result)
    });
  }

  ngOnInit() {}
}
