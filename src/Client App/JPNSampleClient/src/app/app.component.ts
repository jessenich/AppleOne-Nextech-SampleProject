import { Component, OnInit } from "@angular/core";
import { MatPaginator, MatSort, MatTableDataSource } from "@angular/material";
import { StoriesService, StoryModel } from "./stories.service";
import { delay } from "q";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"]
})
export class AppComponent implements OnInit {
  title = "Sample Angular Client";
  columns: string[] = ["id", "author", "title", "score", "url"];
  stories: StoryModel[];
  storiesDataSource: MatTableDataSource<StoryModel>;

  // @ViewChild(MatPaginator) paginator: MatPaginator;
  // @ViewChild(MatSort) sort: MatSort;

  constructor(private storiesService: StoriesService) {
    this.storiesService = storiesService;
    let observer = this.storiesService
      .getStories()
      .toPromise()
      .then(result => {
        if (result === undefined || result === null) return;

        this.stories = result;
      });
  }

  ngOnInit(): void {
    for (var i = 0; i < 3; i++) {
      if (this.stories == null) {
        delay(3000);
        console.debug("waiting on stories result...");
      }
    }

    this.storiesDataSource = new MatTableDataSource(this.stories);
    this.storiesDataSource.connect().complete();

    console.debug("ngOnInit complete");
  }
}
