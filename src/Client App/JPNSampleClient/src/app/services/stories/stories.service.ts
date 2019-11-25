import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {JsonObject, JsonProperty, JsonConvert, ValueCheckingMode, OperationMode} from 'json2typescript';
import { stringify } from 'querystring';

const httpOptions = {
  headers: new HttpHeaders('Content-Type: application/json')
};

@JsonObject('StoryModel')
export class StoryModel {

  @JsonProperty('author', String)
  author: string;

  @JsonProperty('title', String)
  title: string;

  @JsonProperty('url', String)
  url: string;

  @JsonProperty('score', Number)
  score: number;

  @JsonProperty('createdAt', Date)
  createdAt: Date;
}

@JsonObject('StoryTypeModel')
export class StoryTypeModel {

  @JsonProperty('key', String)
  key: string;

  @JsonProperty('count', Number)
  count: number;

  @JsonProperty('items', [Number])
  items: number[];
}

@JsonObject('StoryTypeResponseModel')
export class StoryTypeResponseModel {

  @JsonProperty('top', StoryTypeModel)
  topStories: StoryTypeModel;

  @JsonProperty('new', StoryTypeModel)
  newStories: StoryTypeModel;

  // constructor(top: StoryTypeModel, newStories: StoryTypeModel) {

  // }

}

@Injectable({
  providedIn: 'root'
})
export class StoriesService {

  private jsonConvert: JsonConvert;

  constructor(private baseUrl: string, private http: HttpClient) {
    this.jsonConvert = new JsonConvert();
    this.jsonConvert.ignorePrimitiveChecks = false;
    this.jsonConvert.valueCheckingMode = ValueCheckingMode.ALLOW_OBJECT_NULL;
    this.jsonConvert.operationMode = OperationMode.ENABLE;
  }

  getStoryTypes(): StoryTypeResponseModel {
    const typesResponse = this.http.get(`${this.baseUrl}/api/types`, httpOptions);
    return this.jsonConvert.deserialize(typesResponse, StoryTypeResponseModel);
  }

  getStories() {
    const storiesResponse = this.http.post(`${this.baseUrl}/api/stories`, JSON.stringify(this.getStoryTypes().top), httpOptions);
    return this.jsonConvert.deserialize(storiesResponse, [StoryModel]);
  }
}





