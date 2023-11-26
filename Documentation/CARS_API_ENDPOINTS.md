# `Cars` Api Endpoints
These Endpoints are for all requests that start with `api/Cars`
### `GetCars`
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

### `GetCarsByCompany`
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

### `SearchByMake`
```markdown
POST api/Cars/Search/Make
```
**Description:**  
Case insensitive. Searches all available cars based on their make value.

**Request-body:** None

**Parameters:**
- `passwordHash`: string
- `makeName`: string

**Returns:**  
Array of results found from the `makeName` represented as car data per company, **each** containing the following fields:

| Field name | Data type | Description                        |
|------------|-----------|------------------------------------|
| `carId`    | number    | the value used to identify the car |
| `make`     | string    | the car's company's name           |
| `price`    | number    | how much the car costs             |
| `model`    | string    | the car's model name               |
| `stock`    | number    | the quantity of cars available     |
---

### `SearchByModel`
```markdown
POST api/Cars/Search/Model
```
**Description:**  
Case insensitive. Searches all available cars based on their model value

**Request-body:** None

**Parameters:**
- `passwordHash`: string
- `modelName`: string

**Returns:**  
Array of results found in the `modelName` represented as car data per company, **each** containing the following fields:

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `companyId` | number    | the value used to identify the company |
| `carId`     | number    | the value used to identify the car     |
| `make`      | string    | the car's company's name               |
| `price`     | number    | how much the car costs                 |
| `model`     | string    | the car's model name                   |
| `stock`     | number    | the quantity of cars available         |
---

### `GetCar`
```markdown
GET api/Cars/{id}
```
**Description:**  
Shows an available car based on the `id` provided.

**Request-body:** None

**Parameters:**
- `id`: number

**Returns:**  
A `Car` object containing the following fields

| Field name  | Data type | Description                            |
|-------------|-----------|----------------------------------------|
| `carId`     | number    | the value used to identify the car     |
| `companyId` | number    | the value used to identify the company |
| `price`     | number    | how much the car costs                 |
| `model`     | string    | the car's model name                   |
---

### `UpdateCar`
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

### `UpdateStock`
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

### `AddCar`
```markdown
POST api/Cars
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

### `AddStock`
```markdown
POST api/Cars/Stock
```
**Description:**  
Adds information about how many cars of each model a dealer has

**Request-body:** None

**Parameters:**
- `CarId`: number
- `passwordHash`: string
- `Stock`: int

**Returns:**
- Status code `200` if successful.
- `ConflictObjectResult` error if the dealer tries to add a car whose stock is already being tracked
---

### `AddCompany`
```markdown
POST api/Cars/Company
```
**Description:**  
Inserts a new car company into the database

**Request-body:** None

**Parameters:**
- `Name`: string
- `Address`: (Optional) string

**Returns:**
- Status code `200` if successful.
- `ConflictObjectResult` error if a Car company with the same name exists
---

### `DeleteCar`
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
