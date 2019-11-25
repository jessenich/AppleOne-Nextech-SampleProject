import { Inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StoriesService {

  private baseUrl: string;

  constructor(private httpClient: HttpClient) {
    this.baseUrl = environment.baseUrl;
    this.httpClient = httpClient;
  }

  getStoryTypes(): Observable<StoryTypeResponseModel> {

    const headers = new HttpHeaders();
    headers.append('Access-Control-Allow-Origin', '*');

    const options = {
      headers
    };

    // Make call to story IDs endpoint
    return this.httpClient.get<StoryTypeResponseModel>(`${this.baseUrl}/api/types`, options);
  }

  getStories(): Observable<StoryModel[]> {
    // Get top stories from API service
    let topStoryIds: StoryTypeModel = null;
    this.getStoryTypes().subscribe(result => topStoryIds = result.topStories);

    // Convert to JSON for request post body
    const jsonBody = JSON.stringify(topStoryIds);

    // Make call to stories endpoint to get story content objects
    return this.httpClient.post<StoryModel[]>(`${this.baseUrl}/api/stories`, jsonBody);
  }
}

export interface StoryModel {
  author: string;
  title: string;
  url: string;
  score: number;
  createdAt: Date;
}

interface StoryTypeModel {
  key: string;
  count: number;
  items: Array<number>;
}

interface StoryTypeResponseModel {
  topStories: StoryTypeModel;
  newStories: StoryTypeModel;
}





