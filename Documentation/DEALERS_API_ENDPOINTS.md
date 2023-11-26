# `Dealers` Api Endpoints
These Endpoints are for all requests that start with `api/Dealers`
### `GetDealers`
```markdown
GET api/Dealers
```
**Description:**  
Shows all available dealers

**Request-body:** None

**Parameters:** None

**Returns:**  
A list of strings containing the names of all dealers.
---

### `GetDealerName`
```markdown
GET api/Dealers/Name/{passwordHash}
```
**Description:**  
Gets the name of the dealer associated with their `passwordHash` value

**Request-body:** None

**Parameters:**
- `passwordHash`: string

**Returns:**  
A string representing the dealer name
---

### `SignInStatus`
```markdown
GET api/Dealers/SignInStatus/
```
**Description:**  
Gets the status of whether the current user is signed in, based on their `passwordHash` value

**Request-body:** None

**Parameters:** None

**Returns:**  
- `{ status = "SessionExpired" }` if the session has lasted longer than the set time (currently set to 30 minutes)
- `{ status = "SessionExists" }` if the session is still active
- `{ status = "NotSignedIn" }` if there was no previous session and the user isn't signed in
---

### `SignUpDealer`
```markdown
POST api/Dealers/SignUp
```
**Description:**  
Creates an account for a new dealer

**Request-body:** None

**Parameters:** None

**Returns:**
- `hashValue`: string | to be used by the client to reference all other `passwordHash` requiring functions
-  `status`: number | denoting the status code of the API call. Defaults to 200

---
### `SignInDealer`
```markdown
POST api/Dealers/SignIn
```
**Description:**  
Signs in to an already-created account for an existing dealer

**Request-body:** None

**Parameters:** 
- `username`: string
- `password`: string

**Returns:**
- `hashValue`: string | to be used by the client to reference all other `passwordHash` requiring functions
-  `status`: number | denoting the status code of the API call. Defaults to 200
---

### `SignOutDealer`
```markdown
POST api/Dealers/SignOut
```
**Description:**  
Signs out a logged-in account for an existing dealer

**Request-body:** None

**Parameters:**
- `passwordHash`: string

**Returns:**
-  `status`: number | denoting the status code of the API call. Defaults to 200
---

### `DeleteDealer`
```markdown
DELETE api/Dealers/5
```
**Description:**  
Deletes an existing dealer

**Request-body:** None

**Parameters:**
- `passwordHash`: string

**Returns:**
-  Nothing if successful
---
