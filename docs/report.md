---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group `14`
author:
- "Andreas Faber <afab@itu.dk>"
- "Christian Dam <cacd@itu.dk>"
- "Philip Nielsen <pann@itu.dk>"
- "Peter Raasthøj <praa@itu.dk>"
- "Søren Bräuner <brae@itu.dk>"
  
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model
Provide an illustration of your domain model. Make sure that it is correct and complete. In case you are using ASP.NET Identity, make sure to illustrate that accordingly.
Here comes a description of our domain model.

![Illustration of the _Chirp!_ data model as UML class diagram.](docs/images/domain_model.png)

## Architecture — In the small
Illustrate the organization of your code base. That is, illustrate which layers exist in your (onion) architecture. Make sure to illustrate which part of your code is residing in which layer.

## Architecture of deployed application
Illustrate the architecture of your deployed application. Remember, you developed a client-server application. Illustrate the server component and to where it is deployed, illustrate a client component, and show how these communicate with each other.

## User activities
Illustrate typical scenarios of a user journey through your Chirp! application. That is, start illustrating the first page that is presented to a non-authorized user, illustrate what a non-authorized user can do with your Chirp! application, and finally illustrate what a user can do after authentication.

Make sure that the illustrations are in line with the actual behavior of your application.

## Sequence of functionality/calls trough _Chirp!_
With a UML sequence diagram, illustrate the flow of messages and data through your Chirp! application. Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.

Make sure that your illustration is complete. That is, likely for many of you there will be different kinds of "calls" and responses. Some HTTP calls and responses, some calls and responses in C# and likely some more. (Note the previous sentence is vague on purpose. I want that you create a complete illustration.)

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
