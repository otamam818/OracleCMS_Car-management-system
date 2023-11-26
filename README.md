> **Note**: The words `UsedmiDNT` and `Des` are ciphered words to prevent any other candidates from finding this repository.
> The original word's vowels were substituted to the next vowel, and the consonants were substituted to the next consonant. 
> **For example:** the original word `Hello` would be converted into `Jimmu` using this method.

> This information is made available for the actual assessors of the company as proof that I worked on the task they requested.

# Des-management-system
A `Des` management system provided as a Coding Task from `UsedmiDNT`. This project uses .NET Core with Swagger to create a web API

## Usage
1. Clone this Repository
2. Open `WebApi/CarDeals/CarDeals.sln` in [Visual Studio](https://visualstudio.microsoft.com/vs/) (preferably version 22)
3. Press `CTRL+F5` to run the web API

## Implementation
This project implements authentication to prevent other dealers from retrieving others' data.
It does this by [encrypting passwords using hashing and a salt](https://www.okta.com/au/blog/2019/03/what-are-salted-passwords-and-password-hashing/).

Upon successful authentication, the user's `passwordHash` is returned and can be used to
get private data related to their account. Currently, it is just their stocks and address,
but the data has been designed so that it can be extended to other places.

[Sessions](https://roadmap.sh/guides/session-based-authentication) have also been implemented as a part of authentication. Every time a user uses
the `passwordHash` provided, the session refreshes, allowing the user to stay online into their
account. A session period of 30 minutes has been set as a constant so that it is easy to change in case a change is required.

A demonstration of its usage is planned using Svelte.

## Api Endpoints
```markdown
GET api/Cars
```
**Description:**  
Shows all available cars

**Request-body:** None

**Parameters:** None

**Returns:**  
An array of relevant car data, **each** containing the following fields:

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `Id`        | number    | the value used to identify the car     |
| `make`      | string    | the car's company's name               |
| `model`     | string    | the car's model name                   |
| `price`     | string    | how much the car costs                 |
---

```markdown
GET api/Cars/company/{passwordHash}
```
**Description:**  
Shows cars available from the dealer

**Request-body:** None

**Parameters:** None

**Returns:**  
Array of car data per company, **each** containing the following fields:

| Field name | Data type | Description                        |
|------------|-----------|------------------------------------|
| `carId`    | number    | the value used to identify the car |
| `make`     | string    | the car's company's name           |
| `price`    | number    | how much the car costs             |
| `model`    | string    | the car's model name               |
| `stock`    | number    | the quantity of cars available     |
---

```markdown
POST api/Cars/Search/Make
```
**Description:**  
Searches all available cars based on their make value

**Request-body:** None

**Parameters:**
- `passwordHash`: string
- `makeName`: string

**Returns:**  
Array of results found in the `makeName` represented as car data per company, **each** containing the following fields:

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `dealerId`  | number    | the value used to identify the dealer  |
| `companyId` | number    | the value used to identify the company |
| `carId`     | number    | the value used to identify the car     |
| `make`      | string    | the car's company's name               |
| `model`     | string    | the car's model name                   |
| `price`     | number    | how much the car costs                 |
---

```markdown
POST api/Cars/Search/Model
```
**Description:**  
Searches all available cars based on their model value

**Request-body:** None

**Parameters:**
- `passwordHash`: string
- `modelName`: string

**Returns:**  
Array of results found in the `modelName` represented as car data per company, **each** containing the following fields:

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `dealerId`  | number    | the value used to identify the dealer  |
| `companyId` | number    | the value used to identify the company |
| `carId`     | number    | the value used to identify the car     |
| `make`      | string    | the car's company's name               |
| `model`     | string    | the car's model name                   |
| `price`     | number    | how much the car costs                 |
---

```markdown
GET: api/Cars/{id}
```
**Description:**  
Shows an available car based on the `id` provided.

**Request-body:** None

**Parameters:**
- `id`: number

**Returns:**  
Array of results found in the `makeName` represented as car data per company, **each** containing the following fields:

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `carId`     | number    | the value used to identify the car     |
| `companyId` | number    | the value used to identify the company |
| `price`     | number    | how much the car costs                 |
| `model`     | string    | the car's model name                   |
---

```markdown
PUT api/Cars/{id}
```
**Description:**  
Updates a car with a new value based on the `id` and body values provided.

**Request-body:** 
- `Id`: number
- `CompanyId`: number
- `Price`: number
- `Model`: string

**Parameters:**
- `id`: number

**Returns:**  
- Nothing if successful.
- `NotFound` error if the id provided does not correspond to any existing car
---

```markdown
PUT api/Cars/Update/Stock/
```
**Description:**  
Updates the number of cars available from the dealer

**Request-body:** None

**Parameters:**
- `CarId`: number
- `NewStockValue`: number
- `PasswordHash`: string 

**Returns:**  
- An object with a `message` field saying `Success` if successful
- A `DbUpdateConcurrencyException` message if the data could not update successfully (possibly due to request conflicts)
---

```markdown
POST: api/Cars
```
**Description:**  
Creates a new Car or updates it if the `Id` is conflicting

**Request-body:**
- `Id`: number
- `CompanyId`: number
- `Price`: number
- `Model`: string

**Parameters:**
- `id`: number

**Returns:**
- Nothing if successful.
- `NotFound` error if the id provided does not correspond to any existing car
---

```markdown
DELETE api/Cars/5
```
**Description:**  
Deletes a car based on its id

**Request-body:** None

**Parameters:**
- `id`: number

**Returns:**
- Nothing if successful.
- `NotFound` error if the id provided does not correspond to any existing car
---

## Roadmap
### General Goals
- [X] Define the data model
- [X] Set up the Web API project
- [X] Create the database context
- [X] Create the controllers
- [ ] Create tests
- [ ] Submit code

### Stretch goals
- [ ] Make a Svelte UI for it
