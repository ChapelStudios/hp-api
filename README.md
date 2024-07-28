# PlayerCharacter API

This API provides endpoints to manage the health of player characters in the DDB HealthCycle system.

---

## Table of Contents 

- [Intro](#Installation)
- [Installation](#Installation)
  - [Local SQL DB Setup](#Local-SQL-DB-Setup)
  - [Client Setup](#Client-Setup)
  - [Server Setup](#Server-Setup)
- [Usage](#Usage)
- [Endpoints](#Endpoints)
  - [Get Player Character](#Get-Player-Character)
  - [Heal Player Character](#Heal-Player-Character)
  - [Apply Temporary HP to Player Character](#Apply-Temporary-HP-to-Player-Character)
  - [Damage Player Character](#Damage-Player-Character)
    - [Accepted Damage Types](#Accepted-Damage-Types)
- [Error Handling](#Error-Handling)
- [Unit Testing](#Unit-Testing)
- [License](#License)

---
## Intro
Write stuff here jake....

## Installation
Instructions on how to install and set up the solution.

Start by cloning the repository:

```bash
  git clone https://github.com/yourusername/projectname.git
```

### Local SQL DB Setup
For development purposes, this project is configured to use a local SQL database. The database is set up to facilitate testing and development, but it does not contain any production configurations and should only be run locally.

The current setup will ensure that the database and all tables are created with the correct schema and does not require you to make any changes directly in SQL or run any Entity Framework migrations.

**Use these steps to configure your local database:**
1. Ensure you have SQL Server installed locally.
2. Ensure there is a the connection string named `PlayerCharacterContext` in `{Solution Directory}/DDB.HealthCycle.Server/appsettings.Development.json` to point to your local SQL Server instance.
   1. For example:
      ```json
      "ConnectionStrings": {
        "PlayerCharacterContext": "Server=(localdb)\\mssqllocaldb;Database=PlayerCharacterContext;Trusted_Connection=True;MultipleActiveResultSets=true"
      }
      ```

### Client Setup
The client is not currently set up and has been removed from project startup by removing the following properties from the PropertyGroup in the `DDB.HealthCycle.Server.csproj` file.

  ```xml
        <SpaRoot>..\ddb.healthcycle.client</SpaRoot>
        <SpaProxyLaunchCommand>npm run dev</SpaProxyLaunchCommand>
        <SpaProxyServerUrl>https://localhost:5173</SpaProxyServerUrl>
  ```
  
If the project still starts, ensure Visual Studio has DDB.HealthCycle.Server` set as the startup project and not `multiple projects`

When the client is reactivated, make sure to run `npm install` from the `ddb.healthcycle.client` directory.

### Server Setup
From the `DDB.HealthCycle.Server` directory in the solution directory, run `dotnet restore`.

---

## Usage
Instructions to run the API with or without the React frontend:

1. **Navigate to the DDB.HealthCycle.Server project directory:**
    ```bash
    cd DDB.HealthCycle.Server
    ```

2. **Run the server:**
    ```bash
    dotnet run
    ```

3. **Access the swagger page:**
    Open your browser and navigate to `https://localhost:7163/swagger/index.html` (or the port specified in your launch settings).

---

## Endpoints
### Get Player Character
Retrieves the JSON data for a player character by ID.

- URL: `/PlayerCharacter/{id}`
- Method: `GET`
- URL Params: `id=[string]`
- Success Response:
  - Code: 200
  - Content: `PlayerCharacter` object
- Error Responses:
  - Code: 204 (`PlayerCharacter` not found)
  - Code: 500 (Internal error)

### Heal Player Character
Heals the specified player character by a given amount.

- URL: `/PlayerCharacter/{id}/heal`
- Method: `POST`
- URL Params: `id=[string]`
- Query Params: `amount=[int]`
- Success Response:
  - Code: 200
  - Content: `PlayerCharacterHealthStats` object
- Error Responses:
  - Code: 204 (`PlayerCharacter` not found)
  - Code: 400 (Invalid amount)
  - Code: 500 (Internal error)

### Add Temporary HP
Applies a given amount of temporary hit points to the specified player character.

- URL: `/PlayerCharacter/{id}/add-temp-hp`
- Method: `POST`
- URL Params: `id=[string]`
- Query Params: `amount=[int]`
- Success Response:
  - Code: 200
  - Content: `PlayerCharacterHealthStats` object
- Error Responses:
  - Code: 204 (`PlayerCharacter` not found)
  - Code: 400 (Invalid amount)
  - Code: 500 (Internal error)

### Damage Player Character
Applies damage of a given type to the specified player character, taking into account any resistance defenses.

- URL: `/PlayerCharacter/{id}/damage`
- Method: `POST`
- URL Params: `id=[string]`
- Query Params: `amount=[int]`, `damageType=[DamageType]`
- Success Response:
  - Code: 200
  - Content: `PlayerCharacterHealthStats` object
- Error Responses:
  - Code: 204 (`PlayerCharacter` not found)
  - Code: 400 (Invalid amount)
  - Code: 500 (Internal error)

#### Accepted Damage Types
This is the list of currently accepted damage types. You can either specify the name below or call it by it's enum int value.

- None
- Bludgeoning
- Piercing
- Slashing
- Fire
- Cold
- Acid
- Thunder
- Lightning
- Poison
- Radiant
- Necrotic
- Psychic
- Force

## Error Handling
Common error responses include:

- 204: Resource not found
- 400: Invalid input
- 500: Internal server error

## Unit Testing

Unit tests are included for the `DDB.HealthCycle.Logic` library. There are probably a couple of other things that could be unit tested but these are the most important items. Due to the nature of this project I also skipped on testing to ensure Exceptions thrown from underlying components were raised.

All tests are written using the NUnit framework.

### Running Unit Tests
From the solution directory, run `dotnet test`.

The test results will be displayed in the terminal. Ensure all tests pass before committing any changes to the repository.

## Licensing
This solution is based on a coding challenge project for D&D Beyond / Wizards of the Coast / Hasbro that can be found [here](https://github.com/DnDBeyond/back-end-developer-challenge).

Any code provided past that provided by the original project is provided via use of the [MIT licensing agreement](https://opensource.org/license/mit).


