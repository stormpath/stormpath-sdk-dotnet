# Stormpath .NET SDK  [![Build Status](https://travis-ci.org/stormpath/stormpath-sdk-dotnet.svg?branch=develop)](https://travis-ci.org/stormpath/stormpath-sdk-dotnet)

*An advanced, reliable and easy-to-use user management API for .NET by security experts*

[Stormpath](https://stormpath.com) is a complete user management API.  This
library gives your ASP.NET, C#, or VB.NET application access to all of Stormpath's features:

- Robust authentication and authorization.
- Schemaless user data and profiles.
- A hosted login subdomain, for easy Single Sign-On across your apps.
- Social login with Facebook and Google OAuth.
- Secure API key authentication for your service.

In addition to these core Stormpath features, this SDK provides:

- Support for .NET 4.5 and later
- Support for Linux and OS X via Mono
- LINQ to Stormpath for readable, expressive queries
- Fully dual-stack design for native asynchronous and native synchronous access

If you have feedback about this library, please get in touch and share your
thoughts! support@stormpath.com

## Current status

This library is currently under active development. If you would like to be notified when we publish releases, watch this repository or send an email to support@stormpath.com.

## Installation instructions

#### Using the Nuget Package Manager

 1. Right-click on the new project in the Solution Explorer and choose **Manage Nuget Packages...**
 2. Search for Stormpath. Install the Stormpath.SDK package.

#### Using the Package Manager Console

Simply run `install-package Stormpath.SDK`. Done!

#### Using Mono

It's as easy as `mono nuget.exe install Stormpath.SDK`.

## Quickstart

In this quickstart, you'll use Visual Studio to make a simple console application that creates a user and logs them into an application.

#### Get an API Key

All API requests to Stormpath require a valid API Key and Secret. The Stormpath .NET SDK does this automatically; all you have to do is generate an API Key in the Stormpath Admin Console.

 1. If you haven't already, [sign up for Stormpath here](https://api.stormpath.com/register), and click the link in the verification email.
 2. Log in to the [Stormpath Admin Console](https://api.stormpath.com/).
 3. Click the **Create API Key** or **Manage Existing Keys** under Developer Tools on the right side of the page.
 4. Scroll down to Security Credentials and click **Create API Key**. This will generate your API Key and download it to your computer as an `apiKey.properties` file.
 
We recommend you store the API Key and Secret values as environment variables, or place the downloaded file into a hidden folder in your home directory. (In a production environment, environment variables are highly preferred!)

* To store the API Key and Secret as secure environment variables, open the `apiKey.properties` file in a text editor and execute these commands in a terminal window (Command Prompt or PowerShell both work)

```
setx STORMPATH_API_KEY_ID "[value-from-properties-file]"
setx STORMPATH_API_KEY_SECRET "[value-from-properties-file]"
```

* To store the file in a hidden home directory folder, execute these commands instead:

```
cd [folder-you-downloaded-the-file-to]
mkdir %homedrive%%homepath%\.stormpath
copy apiKey.properties %homedrive%%homepath%\.stormpath\
```

Or, in PowerShell:

```
cd [folder-you-downloaded-the-file-to]
mkdir ~\.stormpath
copy apiKey.properties ~\.stormpath\
```

#### Creating a project

Next, create a simple Visual Studio project and add the Stormpath SDK.

 1. Create a new Console Application project.
 2. Use the [instructions above](#installation-instructions) to install the Stormpath SDK into your project.
 3. Add the following using statements to the top of your `Program.cs` file:
 
 ```csharp
 using Stormpath.SDK;
 using Stormpath.SDK.Client;
 using Stormpath.SDK.Error;
 ```
 
#### `await` a minute

The Stormpath SDK performs `await`able asynchronous operations by default. This makes it possible to make requests to the Stormpath API without tying up a worker thread - super cool for ASP.NET projects!

In the Console Application template, however, the entry point is `static void Main()`, so you can't use the `await` keyword. (In an ASP.NET application, you could just change the method signature to `async Task`, but that doesn't work well in a console app.)

You can work around this by writing a separate method instead:

```csharp
static Task MainAsync()
{
}
```

and calling it synchronously from `Main()`:

```csharp
static void Main(string[] args)
{
    MainAsync().GetAwaiter().GetResult();
}
```

If you are confused by all this `async` business, check out Stephen Cleary's [excellent introductory blog post](http://blog.stephencleary.com/2012/02/async-and-await.html) on the subject.

#### Creating a client

The first thing you need to connect to the Stormpath API is a `IClient` object. Create one in your new `MainAsync` method:

```csharp
IClientApiKey myApiKey = ClientApiKeys.Builder()
    .SetFileLocation("path\\to\\apiKey.properties")
    .Build();

// Create an IClient object. Everything starts here!
IClient client = Clients.Builder()
    .SetApiKey(myApiKey)
    .Build();
```

> :bulb: You can skip building the `IClientApiKey` object and the call to `.SetApiKey()` if you put your API Key and Secret in environment variables, or stored the `apiKey.properties` file in the default location. Calling `client.Build()` without specifying an API Key will check the default location. :thumbsup:

Once you have built a `IClient`, keep it around! You should only need to create it **once** per application. It's thread-safe, so you can safely reuse it, even in an ASP.NET application.

#### Retrieving your application

Now that you have a `IClient`, you need to get an `IApplication` object that represents an Application in Stormpath. You should have a default application called "My Application" in your Stormpath account. It's easy to retrieve:

```csharp
var myApp = await client.GetApplications()
    .Where(x => x.Name == "My Application")
    .SingleAsync();
```

You can use this `IApplication` object to create user accounts, log in user accounts, and all sorts of other cool stuff.

#### Creating a user account

You can create a new user by calling `CreateAccountAsync`:

```csharp
var joe = await myApp.CreateAccountAsync("Joe", "Stormtrooper",
                                         "tk421@galacticempire.co",
                                         "Changeme123!",
                                         new { isAtPost = false });
Console.WriteLine("User " + joe.FullName + " created");
```

The returned `IAccount` object will contain all the account's data. Easy! :smile: 

#### Logging in

Now that you have a valid user in the Stormpath application, you can perform a login request.

```csharp
// Imagine that we got these values from a web request:
var usernameOrEmail = "tk421@galacticempire.co";
var password = "Changeme123!";

try
{
     var loginResult = await myApp.AuthenticateAccountAsync(usernameOrEmail, password);
     var loggedInAccount = await loginResult.GetAccountAsync();
     var accountCustomData = await loggedInAccount.GetCustomDataAsync();
     Console.WriteLine("User {0} logged in. Is at post: {1}",
                       loggedInAccount.FullName,
                       accountCustomData["isAtPost"]);
}
catch (ResourceException rex)
{
    Console.WriteLine("Could not log in. Error: " + rex.Message);
}
```

:bulb: You can also use the `TryAuthenticateAccountAsync(username, password)` overload to get a `bool` result if you don't need to access the account's data after login.

#### Clean up

Cleaning up your Stormpath application is easy:

```csharp
try
{
    await joe.DeleteAsync();
}
catch (ResourceException rex)
{
    Console.WriteLine("Unexpected error when deleting " + joe.Email + ". Error: " + rex.Message);
}
Console.WriteLine("Done!");
```

As a final touch, you can add this line at the end so the console window doesn't disappear immediately:

```csharp
Console.ReadKey(false);
```

#### Other things you can do with the SDK

 * Search for any resource using LINQ 
 * Create and delete entire applications
 * Create and manage directories
 * Create and manage groups and account membership in groups
 * Store arbitrary custom data for any resource
 * Easily perform social login against Google, Facebook, Github, and LinkedIn
 * Reset an account's password
 * Verify an account's email address

## Build Instructions

#### Building with Visual Studio

Building the SDK requires Visual Studio 2015.

 1. Use `git clone` or the Visual Studio GitHub extension to clone the branch you want to build (`master` or `develop`).
 2. Open `stormpath-sdk-csharp\Stormpath.SDK\Stormpath.SDK.sln` in Visual Studio.
 3. On first build, the required NuGet packages will resore. If not, [enable automatic package restore](http://stackoverflow.com/a/29708080/3191599).

#### Building with MSBuild

 1. Clone the repository: `git clone https://github.com/stormpath/stormpath-sdk-csharp.git`
 2. `cd stormpath-sdk-csharp\Stormpath.SDK`
 2. Assuming you have [nuget.exe](https://docs.nuget.org/consume/installing-nuget#command-line-utility) in your `PATH`: `nuget restore Stormpath.SDK.sln`
 3. `msbuild Stormpath.SDK.sln`

#### Building with Mono

Building with Mono requires Mono 4.0 or greater installed on your machine.

 1. Clone the repository: `git clone https://github.com/stormpath/stormpath-sdk-csharp.git`
 2. `cd stormpath-sdk-csharp\Stormpath.SDK`
 3. If you don't have NuGet available otherwise, use your favorite command line tool to download NuGet: `wget https://nuget.org/nuget.exe`
 4. `mono nuget.exe restore Stormpath.SDK.sln`
 5. `xbuild Stormpath.SDK.sln`

## Contributing

Contributions, bug reports and issues are very welcome. Stormpath regularly maintains this repository, and are quick to review pull requests and accept changes!

You can make your own contributions by forking the develop branch of this repository, making your changes, and issuing pull request on the develop branch.

## Copyright

Copyright &copy; 2013-2015 Stormpath, Inc. and contributors.

This project is open-source via the [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).
