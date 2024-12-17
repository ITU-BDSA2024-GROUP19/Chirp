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

[^1]: _IdentityUser Class Reference_ https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identityuser

[^2]: Lock, Chapter 12: _Saving data with Entity Framework Core_ 

![Illustration of the _Chirp!_ data model as a UML class diagram.](images/domain_model.png)

## Architecture — In the small

![Illustration of the _Chirp!_ code architecture as a UML class diagram.](images/code-architecture.png)

Above is an illustration of the organization of our code base.
We have implemented the 'Onion Architecture'. The four colours in the diagram refer to the four layers of the architectural pattern.
Our code base is divided into Web, Architecture, and Core.
It is worth noting that Chirp.Infrastructure contains both the Service and Repository layer.
The diagram shows that our code base only has inward dependencies, in compliance with the 'Onion Architecture'. Thereby no inner layer has any knowledge of outer layers.

## Architecture of deployed application

![Illustration of the _Chirp!_ deployment as a UML package diagram.](images/deployment-diagram.png)

## User activities

![Illustration of _Chirp!_ user activities as a UML activity diagram.](images/user_activities.png)

## Sequence of functionality/calls trough _Chirp!_

![Illustration of a sequence of calls through _Chirp! as a UML sequence diagram.](images/sequence_diagram.png)

# Process

## Build, test, release, and deployment

![Illustration of _Chirp! build, test and deployment workflows as a UML activity diagram.](images/github-actions.png)
If a pull request is triggered our build_and_test workflow is activated.
If a push to main is triggered both workflows are activated sequentially.
Both of our workflows run on a local Ubuntu instance, that is created at the start of the workflow.
Both the workflows checks out our code base, sets up .NET8 and runs a dotnet restore, build and test.
The deployment workflow then deploys the newly build application to Azure. Note that this workflow is only triggered on a push to main.


## Team work

![Illustration of the workflow when working on _Chirp!  as a UML activity diagram.](images/workflow.png)

## How to make _Chirp!_ work locally

## How to run test suite locally

# Ethics

## License

## LLMs, ChatGPT, CoPilot, and others
