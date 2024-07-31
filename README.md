# D&D Beyond Health Cycle API Test Solution

This API provides endpoints to manage the health of player characters in the DDB HealthCycle system.

---

## Table of Contents 

- [Intro](#Intro)
  - [Design](#The-Design)
  - [Code](#The-Code)
  - [Roadmap](#Roadmap)
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
This is the start of an API for a slightly larger project now. It was based on a coding challenge by the D&D Beyond
software development team found [here](https://github.com/DnDBeyond/back-end-developer-challenge).

### The Design
Right now this is just a simple API that can be used to Heal, Damage or apply Temp HP to a Player Character. You can also gather the player character's overall data. 

> There is currently no creation available and the only sample character is Briv, (ID: A0771105-CAE2-44FE-9AEB-CBA2D1DFA29B).


The intent is to add support for weapon item types that add attacks and a sample set of monsters. This will lead into a game that will work in 2 main phases:
1. **The game starts you at a base before you begin your dungeon crawl**
    1. Here you can select from items obtained to give you a variety of attacks to use in the dungeon.
    2. All adventurers start with a base weapon
2. **You enter a dungeon and fight random monsters, gaining a new item at the end**
    1. Each fight the monsters each have randomly selected defenses from the system currently in place.
    2. Monsters have more defenses the more fights you've completed in that dungeon.
    3. After a final boss battle, you gain an item and return to your base to start again

### The Code
This is setup to run on a SQL backend to store Player Character data with a C# .Net 8 Core API to interact with it.
As this is intended to largely mimic a real enterprise level solution, the API has been split into 3 layers, Repository (Data Access), Manager (Logic) and Controller.

Furthermore, the code is separating into the following libraries:
1. **ddb.healthCycle.client**
    1. This is defunct for the moment and will be used in future updates
2. **DDB.HealthCycle.Data**
    1. This houses code for the Entity Framework context as well as test data initialization.
    2. There are some larger changes from the original test json data to allow for a more robust system
        1. Please see the notes in the `{Solution Directory}/DDB.HealthCycle.Data/TestData/briv.json` file for more insight.
2. **DDB.HealthCycle.DataAccess**
    1. This contains repos and other data providers
3. **DDB.HealthCycle.Logic**
    1. This contains all the logic for the API. (In many repositories this may be called DDB.HealthCycle.Business)
4. **DDB.HealthCycle.Models**
    1. Contains all model types for the API:
        1. DataModels: 1-for1 Table Match models used by the Context
            1. Some People might put these in the DDB.HealthCycle.Data project itself   ¯\\\_(ツ)_/¯
        2. Data Transfer Objects (DTO)
        3. Enums
            1. I heavily considered putting this in Data because it can feel like static data more so than a part of a larger data.   ¯\\\_(ツ)_/¯
5. **DDB.HealthCycle.Server**
    1. The actual API server for this project.
    2. Run this to run the entire solution. (Including the client when it is reintegrated into the project.)

### Roadmap
After the mechanics layed out in the design section are in place and a basic set of monsters and weapons have been added, these are the next ideas in no particular order
* Leveling / HP gain Mechanics
* Classes
  * I'd like to look to add classes with simple abilities tied to them
  * This will likely effect HP mechanic above
* Abilities effecting attacks
* Items that effect abilities, defenses and health
* Additional Monsters / Loot

## Installation
Instructions on how to install and set up the solution.

Start by cloning the repository:

```bash
  git clone https://github.com/ChapelStudios/hp-api.git
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
Make sure to run `npm install` from the `ddb.healthcycle.client` directory.

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
    > You can use ID `A0771105-CAE2-44FE-9AEB-CBA2D1DFA29B` to interact with the sample character *Briv*.

4. **Access the client:**
    Open your browser and navigate to `https://localhost:707/swagger/index.html`.

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

### Apply Temporary HP to Player Character
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

> All C# tests are written using the NUnit framework.

### Running Unit Tests
From the solution directory, run `dotnet test`.

The test results will be displayed in the terminal. Ensure all tests pass before committing any changes to the repository.

## Licensing
This solution is based on a coding challenge project for D&D Beyond / Wizards of the Coast / Hasbro that can be found [here](https://github.com/DnDBeyond/back-end-developer-challenge).

Any code past that provided by the original project is provided via use of the [MIT licensing agreement](https://opensource.org/license/mit).
