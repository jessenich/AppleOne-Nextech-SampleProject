import { TestBed } from '@angular/core/testing';
import { StoriesService } from './stories.service';

describe('StoriesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: StoriesService = TestBed.get(StoriesService);
    expect(service).toBeTruthy();
  });

  it('should be called', () => {
    const service: StoriesService = TestBed.get(StoriesService);
    service.getStories().subscribe(result => console.log(JSON.stringify(result)));
  });

});
