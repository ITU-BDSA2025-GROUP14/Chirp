---
title: "_Chirp!_ Project Report"
subtitle: "ITU BDSA 2025 Group `14`"
author:
  - "Andreas Faber <afab@itu.dk>"
  - "Christian Dam <cacd@itu.dk>"
  - "Philip Nielsen <pann@itu.dk>"
  - "Peter Raasthøj <praa@itu.dk>"
  - "Søren Bräuner <brae@itu.dk>"
numbersections: true
---

- [Design and Architecture of _Chirp!_](#design-and-architecture-of-chirp)
  - [Domain model](#domain-model)
  - [Architecture — In the small](#architecture--in-the-small)
  - [Architecture of deployed application](#architecture-of-deployed-application)
  - [User activities](#user-activities)
  - [Sequence of functionality/calls trough _Chirp!_](#sequence-of-functionalitycalls-trough-chirp)
- [Process](#process)
  - [Build, test, release, and deployment](#build-test-release-and-deployment)
  - [Team work](#team-work)
  - [How to make _Chirp!_ work locally](#how-to-make-chirp-work-locally)
  - [How to run test suite locally](#how-to-run-test-suite-locally)
  - [Test suits](#test-suits)
    - [Chirp.PlaywrightTests](#chirpplaywrighttests)
    - [Chirp.Razor.Tests](#chirprazortests)
- [Ethics](#ethics)
  - [License](#license)
  - [LLMs, ChatGPT, CoPilot, and others](#llms-chatgpt-copilot-and-others)
# Design and Architecture of _Chirp!_

## Domain model
The Domain model illustrates the main concepts of the Chirp! project, namely `Author` and `Cheep`. We've implemented Likes as a core concept as well. The IdentityUser is connected to the Author by matching of username at runtime. When a logged-in user posts, the author name finds, or creates the author based on their username. The identity framework handle authentication, and the Author entity models the concept in the code, and are not formally related besides through username matching.

The Author entity contains information about a User, their relationship with Cheeps, as well as followers/followings, and posts they've liked.
The Cheep entity contains information of who wrote the cheep, as well as who's liked the cheep.
The Like entity contains information about who've liked which tweet, and when. It's the relationship between an author liking a cheep, and the cheep itself.

![image](./Images/Domain%20model.jpg)

Where the Author and Cheep contains lists of the other entities as they are one to many/many to many relations, the Like entity is a one-to-one relation between a Cheep and an Author.

### Validation and Constraints
- For the Cheep entity, validation is made to ensure a cheep doesn't exceed 160 characters.
-  When a new cheep is created a validation check to see if a user is logged in is made, as well as to check if the ApplicationUser has an Author created/associated. If not, a new Author is made and linked based on the username of the user.
-  Validation weather a user is logged in or not, when trying to like a message is also in place.

## Architecture — In the small
This diagram shows the architecture of our Chirp! project as orginized after onion architecture, creating seperation of concerns between each part of the project. The four layers depicted are as follows:

![image](./Images/Onion%20Architecture.jpg)

#### 1. Domain Layer (Core)
This layer contains the core business concepts of the project such as `Author`, `Cheep`, `Like` as wekk as the DTO's and repository interfaces which the next layer can interact with.
There's no EF Core, web application or database code on this layer.

#### 2. Repository Layer
This layer implements data access contracts from the domain, and this is the layer in which data access as well as data persistance is handled. Repositories for accessing data is required in this layer. The database also fits in this layer, as this is where the direct access to the database is made using the repositories.

#### 3. Service Layer
The service layer contains acts as a mediator between the application and repository layers. In this layer the CheepService lives, which has access to `AuthorRepository`, `CheepRepository` and `LikeRepository`, which the webserver uses (through the service layer) to get access to the database. This layer prepares data accessed through the Repository layer, and serves it to the application layer.

#### 4. Application and Test Layer
The last layer contains the webserver, as this is the presentation and entry point. This layer depends on services and repositories via Dependency injection. This is also the layer where we find the test suites for the webserver Razor tests, as well as the End-2-End UI tests using Playwright

## Architecture of deployed application
The Chirp! Application is hosted on an Azure webhost. When code is pushed to the main branch on Github, the Github Actions starts a workflow to deploy the code to the azure hosted webservice. The webservice is hosted using the free F1 plan. This is meant for learning and lightweight API's, but are unsuited for use in production due to the limited amount of memory available for use. We've run into a problem of runnning out of CPU available early on in the project, as we kept repopulating the entire database on azure using the DBinitializer class, which led to this only being run when the service is started locally.
 
A client can access the website using the link `https://bdsagroup14chirprazor.azurewebsites.net/` and will be able to see cheeps posted from other clients in real time. This is only possible because the project is so small, and only a small amount of requests/responses are send and recieved from the azure hosted webserver, every day. An SQLite database has also been configured for the webservice, and is accessible for read/write through the use of the Chirp! projected, leading to persistant data.

![image](./Images/Architecture%20deployed%20application.jpg)

## User activities
Illustrate typical scenarios of a user journey through your Chirp! application. That is, start illustrating the first page that is presented to a non-authorized user, illustrate what a non-authorized user can do with your Chirp! application, and finally illustrate what a user can do after authentication.

Make sure that the illustrations are in line with the actual behavior of your application.

## Sequence of functionality/calls trough _Chirp!_
Below is a UML sequence diagram visualizing a logged in user, creating a new cheep in the Chirp project. It starts with a HTTP post request, which triggers the OnPostAsync method, going through the `AuthorRepository` checking an author exists in the database, before creating the cheep, using the `CreateCheep` method in the `CheepService` going through the `CheepRepository` to create the new cheep in the database.

When the Cheep has been created the user will be redirected to the start page through a HTTP GET request, once again going through the `CheepService` and `CheepRepository` to get all cheeps posted and their authors from the database, and finally displaying all cheeps, as well as the newly created cheep, to the user, sending a 200 OK request to the browser from the webserver.
![image](./Images/Sequence%20of%20functionality.jpg)


# Process

## Build, test, release, and deployment
Illustrate with a UML activity diagram how your Chirp! applications are build, tested, released, and deployed. That is, illustrate the flow of activities in your respective GitHub Actions workflows.

Describe the illustration briefly, i.e., how your application is built, tested, released, and deployed.

## Team work
Show a screenshot of your project board right before hand-in. Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Every week we've read through all the requirements posed in the project work part of the lecture notes GitHub, and transformed each requirement into a GitHub issue.

We as a group went through each requirement individually and created success criteria for the requirement, as well as a user story and a small description to make sure each requirement would be implemented satisfactorily.

At the beginning we had neither a description or a user story, as we misinterpreted the way to create issues, but we've afterwards edited all issues to contain both things, and it made development a lot easier. Each issue was then issued to a single person, who would then work on the issue from start to completion. As the issue is assigned, we've moved it into "In development" in our project board.

 We made use of pair programming, making sure a lot of the slightly meatier issues, had two developers looking at it at once, and making sure knowledge was spread out throughout the group. We've tried to adhere to trunk-based development, branching out, and merging into the ‘main’ stream at the start and end of a day, but larger issues have been allowed to live for longer.
 
We've had issues creating continuous releases, as our tagging of branches and features has not been too carefully considered, which means we have a lot fewer releases, than one might expect to see from a project of this scale.

At the end of development of a feature, a pull request is opened, and someone who's not been a part of development is assigned to review the PR such that knowledge of the feature is shared across the group, without everyone having to actively be part of developing every feature. Once the pull request has been reviewed and accepted, given no merge conflicts, and the branch building with GitHub workflows, and all tests pass, the development branch will be merged into the ‘main’ branch, and the development branch is to be deleted.

 We have done our best to follow this flow, though there's been times where a branch has been forgotten, and thus lived longer, or we've felt it necessary to keep it alive, to easily access what changes had been made. Once everything is reviewed, accepted, and merged into ‘main’, the issue will be moved to done, and then closed.
 

## How to make _Chirp!_ work locally
To clone the project you need to have Git installed on your local machine
- [Install Git](https://git-scm.com/install/)

When Git is installed, run the following command in your terminal, or the Git CLI
```
git clone https://github.com/ITU-BDSA2025-GROUP14/Chirp.git
```

To run the Chirp project you need to have .NET 9 installed
- [Install .NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

After you have cloned the repository down to your local machine, navigate to the Chirp.Web folder of the project using following command
```
cd ./chirp/src/Chirp.web
```

Now run the following commands, to setup authentication secrets locally
```
dotnet user-secrets init
dotnet user-secrets set "Authentication:GitHub:ClientId" "Ov23licogNiZV33XuQcv"
dotnet user-secrets set "Authentication:GitHub:ClientSecret" "ff78c1b915997b82cc74bddf8e72248884954cb3"
```
Now you should be able to run the project using

```
dotnet run
```
The project will be running locally on http://localhost:5273. Navigating to this address should take you to the front page of the Chirp! application where you're able to see all posted cheeps, and you can login, to begin posting your own cheeps locally!

## How to run test suite locally
To run the test suite locally please have .NET 9 and Playwright installed. You need to use the terminal to run the commands needed for running the tests.
- Install Playwright
- Install .NET

To install .NET see the previous section of the report.

To install PlayWright, navigate to the `Chirp.PlayWrightTests` and install the required browsers using the following commands using powershell.
```
cd test\Chirp.PlayWrightTests
pwsh bin/Debug/net8.0/playwright.ps1 install
```


Then you can navigate back to the root folder `Chirp` and run the following commands in your terminal.

Navigate to root folder
```
cd ..\..\
```

Build the project and tests
```
dotnet build
```

Run the Chirp.Razor.Tests tests
```
dotnet test .\test\Chirp.Razor.Tests
```

To run the PlayWright tests, the server has to be running, so start the server using the commands as in the previous section from the `Chirp.Web` folder, and then from another terminal window run the command.
```
dotnet test .\test\Chirp.PlaywrightTests\
```

## Test suits
To prevent bugs and ensure requirements from issues are fulfilled, we have several test suites in our repository. namely we have Unit tests, Integration tests, and End-to-End tests and UI tests.

The structure of the tests are as follows
```
Project root
|-- test
    |-- Chirp.PlaywrightTests
    |-- Chirp.Razor.Tests
```

The Chirp.PlaywrightTests folder contains UI and end-to-end tests, the Chirp.Razor.Tests folder contains the remaining tests.

### Chirp.PlaywrightTests
This folder contains the following tests
- PlayWrightTestBase
- PostCheepFlowTests
- RegisterTest

The `PlayWrightTestBase` contains the base setup for the UI tests, settiung up a browser, giving it context, and settiung up playwright, whereas `PostCheepFlowTests` and `RegisterTest` each tests different UI functionality, with either logging in, and posting a cheep, or registering a new user. Many more UI tests could be made, but because of the time restrictions we've chosen these to make up the most important functionality to be tested in our project.

### Chirp.Razor.Tests

# Ethics
## License
We've chosen to use an MIT License as it gives us the most freedom for our project. It grants us the rights to modify and distribute our project as we see fit, and allows other to use our code in their own projects, as long as Attribution I.E the original copyright notice and license text is included in the software.

## LLMs, ChatGPT, CoPilot, and others
[ChatGPT ](https://chatgpt.com/) has been used during development. We've very early on set limitations on using LLM's and ChatGPT, as we've wanted to develop this project using out own intuition, and learn the proper tools for developing, as well as getting a feel for using C# as whole. We've been urged to make use of LLM's during development throughout the project though, and has since about halfway through the project, made use of mainly ChatGPT to create and help create tests for the different parts of the system. ChatGPT has also been used as a resource to bounce idea's off of and has been coauthered in every commit, where it's been used.

The use of LLMs significantly sped up development, as it has been quick to make a usefull testsuite, especially when given context, and guiding text as to how to create the tests. It's also helped when stuck on essential implementation or understanding issues in regards to using the Entity framework as well as when implementing 3rd party logins.
