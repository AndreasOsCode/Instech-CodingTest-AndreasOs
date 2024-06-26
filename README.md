# Read this first!
This repository is a template repository for our technical interview, so create your own project using this guide:

[GitHub - Creating a repository from a template](https://docs.github.com/en/repositories/creating-and-managing-repositories/creating-a-repository-from-a-template).

An alternative is to download the code and create a new repository using a different VCS provider (gitlab / Azure repos). **Do not fork this repository.**

When you have completed the tasks, please share the repository link with us. We will review your submission before the interview.

Good luck! 😊

# Prerequisites

- Your favourite IDE to code in C# 😊
- _Optional_ - an Azure Subscription. You can demo this API by hosting it in Azure. If that is not an option for you, you can run the demo having a locally running instance. If you select a different cloud provider, that is fine for us.
- `Docker` and `docker-compose`

Use `$ docker-compose up --detach` to start the containers with dependencies. The existing code base is preconfigured to work with these containers.
There are no volumes setup for any of the storage, so when you `docker-compose down` these storage media *WILL NOT BE PERSISTED*.

> **Disclaimer**
> 
> As you can see - a DB password is committed to the `appsettings.json` file. However, these secrets are just for development dependencies. If you deploy
> the application into the cloud, we expect that you make use of an alternate method of storing secrets.

# Programming Task
Complete the backend for a multi-tier application for Insurance Claims Handling.
The use case is to maintain a list of insurance claims. The user should be able to create, delete and read claims.
## Task 1
The codebase is messy:
* The controller has too much responsibility. 
* Introduce proper layering within the codebase. 
* Documentation is missing.
* ++

Adhere to the SOLID principles.

### Task 2
As you can see, the API supports some basic REST operations. But validation is missing. The basic rules are:

* Claim
  * DamageCost cannot exceed 100.000
  * Created date must be within the period of the related Cover
* Cover
  * StartDate cannot be in the past
  * total insurance period cannot exceed 1 year

## Task 3
Auditing. The basics are there, but the execution of the DB command (INSERT & DELETE) blocks the processing of the HTTP request. How can this be improved? Look into some asynchronous patterns. It is ok to introduce an Azure managed service to help you with this (ServiceBus/EventGrid/Whatever), but that is not required. Whatever you can manage to get working which is in-memory is also ok.

## Task 4
One basic test is included, please add other (mandatory) unit tests. Note: If you start on this task first, you will find it hard to write proper tests. Some refactoring of the Claims API will be needed. 

## Task 5
Cover premium computation formula evolved over time. Fellow developers were lazy and omitted all tests. Now there are a couple of bugs. Can you fix them? Can you make the computation more readable?

Desired logic: 
* Premium depends on the type of the covered object and the length of the insurance period. 
* Base day rate was set to be 1250.
* Yacht should be 10% more expensive, Passenger ship 20%, Tanker 50%, and other types 30%
* The length of the insurance period should influence the premium progressively:
  * First 30 days are computed based on the logic above
  * Following 150 days are discounted by 5% for Yacht and by 2% for other types
  * The remaining days are discounted by additional 3% for Yacht and by 1% for other types


## My own notes
Firstly, I'm very appreciative of the opportunity to attempt this test, and enjoyed it a lot.

Some immediate disclaimers, I wanted to clean up more around the auditing. I believe I've fixed the blocking issue and it should now be async, but I was unable to do everything I wanted to solve the issues with dependency inversion and the open-closed principle. I had a fully made separation with interfaces and registered dependencies, but ran into trouble with the autogenerated migrations. I reached out but I think it was just too late in the day, so I was unable to resolve that issue. Since I wanted to split the auditcontext into one for claims and one for covers, I couldn't find what to do about the migrationsfile requiring typeof(AuditContext). I would be really grateful if you could provide me with how I could regenerate it, or modify, or just do what needs to be done to make it work.

Further, I have not added very much documentation. I believe that apart from the premium calculation, and the places I was not able to refactor due to the above issue, the code to be fairly self explanatory. I tried to make generic what I could for easier expansion and reuse, I attempted to write tests as well but here lies my second issue of this task. I considered setting up a moq or similar to make testing completely disconnected from the main program functionality, but decided it was a bit out of scope. So there aren't as much unit testing as there are integration tests. I did add tests specifically for the premium calculation in task 5, and the validation in task 2, and the basic functions of the API in task 1.

I believe this task overall took me around 8 hours, although I did admittedly fall into something of a rabbit hole when I got hit with an exception I've never seen before, but that was due to me mistakenly creating two mongodb clients, and thus overloading the service provider. I would also really love to discuss this task with the team in a meeting and I hope you all feel the same.
