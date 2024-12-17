---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2024 Group `19`
author:
- "Lluc Paret Aguer <llpa@itu.dk>"
- "Gustav Hejgaard <ghej@itu.dk>"
- "Ronas Jacob Coban Olsen <rono@itu.dk>"
- "Jacob Sponholtz <adho@itu.dk>"
- "Jakob Sønder <jakso@itu.dk>"
numbersections: true
papersize: a4
geometry:
- left=30mm
- right=30mm
toc: true
---

\pagebreak

# Design and Architecture of _Chirp!_

## Domain model

The two main entities of _Chirp!_ are `Author` and `Cheep`. 

- An `Author` represents one of our users. This type is implemented as an extension of the default `IdentityUser` type.[^1] Authors can follow each other on _Chirp!_ 
- A `Cheep` is a message posted to _Chirp!_ by an author. Additionally for this project, users are able to "like" Cheeps.

These entity types are saved to a database using _Enitiy Framework Core_.[^2] EF Core supports saving these entities in a relational database for the application, and is able to apply _migrations_ to this database as the entities change and features are added. 

In addition to the main entities, we have derived types for data transfer and page model types. Data transfer objects such as `CheepDto` are used where data leaves `Chirp.Infrastructure` for other services. In this way we have greater control over data leaving our infrastructure layer. Example: Each `Cheep` has an `Author` object reference to the writer. This entity has fields with email data, password hashes, etc. A `CheepDto` passed to another service only contains the author name and profile picture link.

In addition, `Chirp.Web` uses a `CheepViewModel` to represent the same data when prepared for display on the website. This design choice allows `CheepDto` to also be used for other services such as serving Cheeps as JSON. 

[^1]: _IdentityUser Class Reference_ https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identityuser

[^2]: Lock, Chapter 12: _Saving data with Entity Framework Core_ 

![Illustration of the _Chirp!_ data model as a UML class diagram.](images/domain_model.png)

## Architecture — In the small

![Illustration of the _Chirp!_ code architecture as a UML class diagram.](images/code-architecture.png)

Above is an illustration of the organization of our code base.
We have implemented the 'Onion Architecture'. The four colours in the diagram refer to the four layers of the architectural pattern[^3].
Our code base is divided into `Chirp.Web`, `Chirp.Architecture`, and `Chirp.Core`.
It is worth noting that `Chirp.Infrastructure` contains both the Service and Repository layer.
The diagram shows that our code base only has inward dependencies, in compliance with the 'Onion Architecture'. Thereby no inner layer has any knowledge of outer layers.
 
[^3]: _Colouring of the 'Onion Architecture' inspiration_ https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_07/images/onion_architecture.webp

## Architecture of deployed application

![Illustration of the _Chirp!_ deployment as a UML package diagram.](images/deployment-diagram.png)

## User activities

![Illustration of _Chirp!_ user activities as a UML activity diagram.](images/user_activities.png)

## Sequence of functionality/calls trough _Chirp!_

![Illustration of a sequence of calls through _Chirp! as a UML sequence diagram.](images/sequence_diagram.png)

# Process

## Build, test, release, and deployment

![Illustration of _Chirp! build, test and deployment workflows as a UML activity diagram.](images/github-actions.png)
Above is an illustration of how our two workflows interact with different github action triggers.
We have two workflows:
- _`build_and_test`_ makes sure the program builds and tests locally.
- _`bdsa2024group19chirprazordeploy`_ handles deployment to Azure.

If a pull request is triggered our build_and_test workflow is activated.
If a push to main is triggered both workflows are activated sequentially.
Both of our workflows run on a local Ubuntu instance, that is created at the start of the workflow.
Both the workflows checks out our code base, sets up .NET8 and runs a dotnet restore, build and test.
The deployment workflow then deploys the newly build application to Azure. Note that this workflow is only triggered on a push to main.

## Team work

![Illustration of the workflow when working on _Chirp!  as a UML activity diagram.](images/workflow.png)
When a new task arises, an issue is created on github using it's build-in ticket system.
The issue is then created based on a set of rules shown in our `README.md` file, including:
- Issue Title Format: "('Session week number', 'issue number') 'Title of the issue at hand'"
- User Story Format: "As a (user type), I want to (task) so that (goal)." (Please write in issue description)
- Acceptance Criteria: Follow a point format of the intended outcomes of the issue.

After the issue has been created, a development branch is added. If we identify the task as a feature, the branch name has the prefix _feature/_, thereby creating a feature directory.
Otherwise the prefix should be _issue/_.
Afterwards, development begins and runs iteratively until all acceptance criteria are satisfied.
If the developer thinks the task is done, they create a pull request.
If another group member believes changes are necessary, a new development cycle begins.
The contents are only pushed to main when the pull request gets a minimum of one approval.


## How to make _Chirp!_ work locally

- Environment variables for the database connection string and the Azure storage connection string depends of if you run the sourcecode directly, which is seen as development state, or run from the published project. If not in development state, enivorment variables are needed to be set.
- If in development state, database will be stored directly in memory and the default stock image will be shown, instead of those provided by the Azure cloud storage. Environment variables are not needed to be set, but can still be set in development state.
- Examples of these environment variables is: `$env:CHIRPDBPATH=":memory:"`, `$env:CHIRPDBPATH='C:/Temp/db.db'`, `$env:AZURE_STORAGE_CONNECTION_STRING="DefaultEndpointsProtocol=https;AccountName=chirpstorage;AccountKey=yourkey;EndpointSuffix=core.windows.net"` etc.

- Warnings of deprecated or old dependencies can be fixed by deleting old packages from the running machines NuGet cache. This can be done by running `dotnet nuget locals all --clear` in the terminal. Or manually deleting the packages from `C:\Users\<INSERT_YOUR_OWN_USERNAME>\.nuget\packages` and delete the packages that are no longer needed.
- Example of this issue could be the .nuget\packages\system.text.json that is being relied on in the project by the package Azure.Storage.Blobs. This states that System.Text.Json ≥ 6.0.10 is needed, but this allows for packages that are older and have warnings and secuirity issues.

- If above steps are followed, the project should be able to run with the simple `dotnet run` command.
## How to run test suite locally

# Ethics

## License

## LLMs, ChatGPT, CoPilot, and others
