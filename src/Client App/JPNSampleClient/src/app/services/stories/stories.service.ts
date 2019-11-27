import { Inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Access-Control-Allow-Origin': '*'
  })
};

@Injectable({
  providedIn: 'root'
})
export class StoriesService {

  private baseUrl: string = environment.storiesApiBaseUrl;

  constructor(private httpClient: HttpClient) { }

  getStoryTypes(): Observable<StoryTypeResponseModel> {
    // Make call to story IDs endpoint
    return this.httpClient.get<StoryTypeResponseModel>(`${this.baseUrl}/api/types`, httpOptions);
  }

  getStories(): Observable<StoryModel[]> {
    // Get top stories from API service
    let topStoryIds: number[];
    let storiesIds = this.getStoryTypes().subscribe(result => {
      topStoryIds = result.newStories.items
    });

    if (topStoryIds === undefined)
      return null;

    // Convert to JSON for request post body
    const jsonBody = JSON.stringify(topStoryIds);

    // Make call to stories endpoint to get story content objects
    return this.httpClient.post<StoryModel[]>(`${this.baseUrl}/api/stories`, jsonBody, httpOptions);
  }
}

export interface StoryModel {
  id: number;
  type: string;
  author: string;
  title: string;
  url: string;
  score: number;
  createdAt: Date;
}

export interface StoryTypeModel {
  key: string;
  count: number;
  items: Array<number>;
}

export interface StoryTypeResponseModel {
  topStories: StoryTypeModel;
  newStories: StoryTypeModel;
  bestStories: StoryTypeModel;
}




