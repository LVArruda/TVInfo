# TV Info

This solution goal is to get TV shows and cast for each TV show from a public API, [TV Maze](http://www.tvmaze.com/api) by scraping it and once we have the data, we provide it to our consumers trough our own API. 
TV Maze API has a [rate limit](https://www.tvmaze.com/api#rate-limiting) (20 calls every 10s) and has pagination (250 per page).
To accomplish this, the solution has two applications inside:
- TVInfo.Console.Scraper
  
  Console application that wil run every day(TBD) and will fetch TV shows page by page, per page then start to get all cast for all Tv shows in that page, once finished for that page, proceed with next page until the TV Maze API returns a 404 signing that there is no page left
- TVInfo.API
  
  Just one HTTP GET endpoint, `api/show`, to retrieve TV shows already with cast data. The results are paginated 50 per page.

## Note
This was created with the intention to have a MVP working and running for a while, to have something that domain and tech can lean on and build from there. For cases that I have more time and the business have more rules I recommend making use of DDD and Clean Architecture. That is the architecture that I have plenty experience, both on strategic and tactical design. 

## Requirements
- .Net 6 installed
- Docker installed

## How to run

- Open terminal and go to the root folder where the solution was cloned (the place that has the .sln file), you should see a `docker-compose.yml` file.
-  Run
    ``` 
    docker-compose up 
    ```
- If there is no error, you should have a MongoDB on port `27017`.
- You can now open the solution and run `TVInfo.Console.Scraper` application.
- After it is done, you can run `TVInfo.API` to fetch TV shows information

## Running the tests

The solution has two types of tests, unit tests and integration tests. Unit test you can run without any previous action but integration tests needs to have the MongoDB running (the same one used by production code), even though this is far from ideal, was necessary because time constraint. Integration tests are checking the pagination, json format and business requirement to order cast within the TV show.

## Shortcomings - points that could be improved

- Logs
- Test Coverage, specially on worker logic
- Exception handling
- Health checks
- Database optimization. Some queries and possibly a fine tuning on its server. Perhaps move to a different database technology
- If TV shows with status `ended` does not need to be updated, we can skip it once initial load is done
- Change TVInfo.API project to be a azure function that is triggered by http get call
- Authentication?
- Remove MongoDBService. It was included because MongoDBClient followed implementation recommendation from  Microsoft and MongoDB but when creating the tests was too difficult to use it, again because of time constraint, decided to create repository to move sort logic to it and test it
- Architecture was created and layers but DOES NOT follow Clean Architecture and DDD, if it is known that it will start to get more and more business rules, we should reconsider the architecture. Models project has public getters and setters and it is used by persistence and for external dependency (TV Maze API), resulting in a clogged with annotation class.
