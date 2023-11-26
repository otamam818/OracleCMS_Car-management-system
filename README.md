> **Note**: The commit history would show that I used the words `UsedmiDNT` and `Des` in the past. These are ciphered words I used to
> prevent any other candidates from finding this repository.

> The original word's vowels were substituted to the next vowel, and the consonants were substituted to the next consonant. 
> **For example:** the original word `Hello` would be converted into `Jimmu` using this method.

> This information is made available for the actual assessors of OracleCMS as proof that I worked on the task they requested.

# Car-management-system
A `Car` management system provided as a Coding Task from `OracleCMS`. This project uses .NET Core with Swagger to create a web API

## Usage
### Running the Web API
1. Clone this Repository
2. Open `WebApi/CarDeals/CarDeals.sln` in [Visual Studio](https://visualstudio.microsoft.com/vs/) (preferably version 22)
3. Press `CTRL+F5` to run the web API

### Using the Svelte UI
1. Make sure that the Web API is running
2. Open `CarDealersFrontEnd` as a root folder
3. Run the following commands:

```bash
npm install # run this only once
npm run dev
```

4. Open your browser and paste the following in your URL bar:
```markdown
http://localhost:5173/
```

The app should be running now.

## Implementation
This project implements authentication to prevent other dealers from retrieving others' data.
It does this by [encrypting passwords using hashing and a salt](https://www.okta.com/au/blog/2019/03/what-are-salted-passwords-and-password-hashing/).

Upon successful authentication, the user's `passwordHash` is returned and can be used to
get private data related to their account. Currently, it is just their stocks and address,
but the data has been designed so that it can be extended to other places.

[Sessions](https://roadmap.sh/guides/session-based-authentication) have also been implemented as a part of authentication. Every time a user uses
the `passwordHash` provided, the session refreshes, allowing the user to stay online into their
account. A session period of 30 minutes has been set as a constant so that it is easy to change in case a change is required.

A demonstration of its usage is made using Svelte.

## API Endpoints
The following are documentation on the API implemented for multiple dealers to use.
- [`/api/Dealers` API Endpoints](Documentation/DEALERS_API_ENDPOINTS.md): Focusing on login functionality and dealer-specific functionality
- [`/api/Cars` API Endpoints](Documentation/CARS_API_ENDPOINTS.md): Focusing on car data management, including the interaction of cars with dealers

For a programmatic example of how to use them, check out the [api.ts](./CarDealersFrontEnd/src/lib/api.ts) and the [prepareDB.ts](./CarDealersFrontEnd/src/lib/prepareDB.ts) files

## Roadmap
### General Goals
- [X] Define the data model
- [X] Set up the Web API project
- [X] Create the database context
- [X] Create the controllers
- [X] Create tests
- [X] Submit code

### Stretch goals
- [X] Make a Svelte UI for it
