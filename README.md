# Stock Options Scraper

This is a C# project to scrape stock options data from finance websites.

Description
The project utilizes a web scraping service to extract stock options information from finance sites. The data is then processed and exposed via API endpoints.

Features
Scrapes stock options data from finance sites
Processes and normalizes scraped data
Provides API endpoints to access stock options data
Swagger docs for API exploration
Docker support for containerization
Usage
The API endpoints can be accessed at /api/options. Swagger UI is available at /swagger to explore and test the API.

Stock options data can be filtered by ticker symbol, expiry date, strike price etc.

Installation
Clone the repo
Build the solution
Run the API app
Access Swagger docs at /swagger
Docker
A Dockerfile is provided to containerize the app. Build the image and run the container:

docker build -t stocks .
docker run -p 5000:80 stocks

Technologies
ASP.NET Core Web API
Entity Framework Core
Swagger/Swashbuckle
xUnit, Moq
Docker
License
MIT License

Contact
Contact the author at [admin@saintsnow.co.za]
