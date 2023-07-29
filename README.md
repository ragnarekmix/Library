# Library

Task
You have a database representing library – it contains data about users, authors, books and books taken by users. To create a database populated with sample data please use script CreateDB-Library.sql provided to you.
Please implement the RESTful API service which exposes 3 endpoints:
Endpoint to search books (/api/search). The service can search books using following criteria:
by author
by text in book’s title or description
by user who’s currently holding the book
if more than one criterion specified - parameter defining how multiple criterions must be combined – either OR or AND condition.
Results: list of books which meet criteria
Endpoint to invert words in Title of the given book (/api/invertwords). Book is identified by ID. When called, service must invert all words in the Title of the book (words are sequences of characters and numbers separated by spaces or other signs like commas, semicolons etc.) 
Returns: book object

Endpoint to generate report (/api/report) – list of users with total count of books taken and total count of days this user holds all the books.
Returns: User details, Total books he/she is holding, Total days he/she holds them.
Application’s architecture, format of solution and data structures are completely up-to you.
What’s Expected
Please provide your code in form of MSVS solution (can be zip-file, link to Github repo etc.). Provide instructions on how to use your API (request formats) in readme file or Postman collection (preferable). If you feel any additional instructions are required to build and run your application – feel free to provide them too.
Notes
Use this task as an opportunity to demonstrate best of your skills, experience, favorite approaches and practices.
If you feel some information is missing or not complete - don’t hesitate to act on assumptions.

# To get app up and running do next steps:
0. Make sure you have VS and Docker for vindows installed on machine. Switch docker into linux containers mode.
1. Open Solution in VS.
2. Run docker compose project in VS.
3. Connect to MSSQL in docker with any prefered tool: 127.0.0.1:1434, user: sa, password: passw0rd!
4. Copy content of SQL/init.sql(Original sql with added commands to create DB and user for app) file to console window and execute it.
5. After that you can use Swagger page to call api.
6. Api documentation is available directly in swagger.

# What can be improved but was not implemented in the scope of the test task:
1. Full text search of book by Title and Description. Current search has simplified implementation.
2. Automatic DB creation on app start if it does not exist. Debatable approach so I desided not to do it.
3. I have used onion architecture for projects structure and dependencies. 
	Current structure is not optimal but I desided to left it at current state and not go further with it as I find that premature generalization brings more problems in the future.
4. There are no logs or metrics
5. Authentication and authorization
6. Api versioning
7. Health checks
8. Store db password in some key storage and not in config file
9. Integration tests that will run api in docker and tests will call it's endpoint 
10. Request validation

# Other Test tasks
https://github.com/ragnarekmix/HackerNews