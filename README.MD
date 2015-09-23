# Stormpath .NET SDK  [![Build Status](https://travis-ci.org/stormpath/stormpath-sdk-csharp.svg)](https://travis-ci.org/stormpath/stormpath-sdk-csharp)

*An advanced, reliable and easy-to-use user management API for .NET by security experts*

[Stormpath](https://stormpath.com) is a complete user management API.  This
library gives your ASP.NET or C# application access to all of Stormpath's features:

- Robust authentication and authorization.
- Schemaless user data and profiles.
- A hosted login subdomain, for easy Single Sign-On across your apps.
- Social login with Facebook and Google OAuth.
- Secure API key authentication for your service.
- Servlet support for Java web applications.

In addition to these core Stormpath features, this SDK provides:

- Support for .NET 4.5 and later
- Support for Mono on Linux
- Easy installation using NuGet
- Fully asynchronous design using the [TPL](https://msdn.microsoft.com/en-us/library/dd460717(v=vs.110).aspx)

If you have feedback about this library, please get in touch and share your
thoughts! support@stormpath.com

## Current status

This library is currently under active development. If you would like to be notified when we publish releases, watch this repository or send an email to support@stormpath.com.

## Installation instructions

The Stormpath C# SDK consists of three packages: `Stormpath.SDK.Core`, `Stormpath.SDK.JsonNetSerializer`, and `Stormpath.SDK.RestSharpClient`. The latter two contain dependencies on external libraries ([JSON.NET](https://github.com/JamesNK/Newtonsoft.Json) and [RestSharp](https://github.com/restsharp/RestSharp)). These are required for the SDK to run properly.

For convenience, the meta-package `Stormpath.SDK` contains everything you need to get up an running quickly. :thumbsup:

#### Using the Nuget Package Manager

 1. Right-click on the new project in the Solution Explorer and choose **Manage Nuget Packages...**
 2. Search for Stormpath. Install the Stormpath.SDK package.

#### Using the Package Manager Console

Simply run `install-package Stormpath.SDK`. Done!

#### Using Mono

It's as easy as `mono nuget.exe install Stormpath.SDK`.

## Quickstart

In this quickstart, we'll use Visual Studio to make a simple console application that creates a user and logs them into an application.

#### Get an API Key

 1. If you haven't already, [sign up for Stormpath here](https://api.stormpath.com/register). Check your email for a verification link.
 2. Click the link in the verification email.
 3. Log in to the [Stormpath Admin Console](https://api.stormpath.com/) using the email address and password you used to register with Stormpath.
 4. Click the **Create API Key** or **Manage Existing Keys** button in the middle of the page.
 5. Under **Security Credentials**, click **Create API Key**. This will generate your API Key and download it to your computer as an `apiKey.properties` file. If you open the file in a text editor, you will see something similar to the following:
 ```
 apiKey.id = 144JVZINOF5EBNCMG9EXAMPLE
 apiKey.secret = lWxOiKqKPNwJmSldbiSkEbkNjgh2uRSNAb+AEXAMPLE
 ```
 
 Save this file in a secure location, such as a `.stormpath` directory in your home directory. (This is the default location the SDK checks, but you can specify a different location if you'd like.)
 
 For example:
 
 ```
 C:\>cd %homedrive%%homepath%
 C:\Users\yourname>mkdir .stormpath
 C:\Users\yourname>cd .stormpath
 C:\Users\yourname\.stormpath>copy C:\Users\yourname\Downloads\apiKey.properties apiKey.Properties
 ```

The `apiKey.properties` file holds your API key information, and can be used to easily authenticate with the Stormpath SDK. Treat it as a sensitive file!

#### Creating a project

Next, let's create a simple Visual Studio project and add the Stormpath SDK.

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

In the Console Application template, however, the entry point is `static void Main()`, so we can't use the `await` keyword. In an ASP.NET application, we could just change the method signature to `async Task`, but that doesn't work well in a console app. Let's get around that by writing a separate method instead:

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

The first thing we need to connect to the Stormpath API is a `IClient` object. In our new `MainAsync` method, let's create one:

```csharp
// If you put the apiKey.properties file in the default location
// (%homedrive%%homepath\.stormpath\apiKey.properties), you can skip this!
IClientApiKey myApiKey = ClientApiKeys.Builder()
    .SetFileLocation("path\\to\\apiKey.properties")
    .Build();

// Create an IClient object. Everything starts here!
IClient client = Clients.Builder()
    .SetApiKey(myApiKey)
    .Build();
```

You can skip building the `IClientApiKey` object and the call to `.SetApiKey()` if you put your `apiKey.properties` file in the default location. Calling `.Build()` without specifying an API Key will check the default location. :thumbsup:

Once you have built a `IClient`, keep it around! You should only need to create it **once** per application. It's thread-safe, so you can safely reuse it, even in an ASP.NET application.

#### Retrieving your application

Now that we have a `IClient`, we need to get an `IApplication` object that represents an Application in Stormpath. You should have a default application called "My Application" in your Stormpath account. Let's retrieve it:

```csharp
var myApp = await client.GetApplications()
    .Where(x => x.Name == "My Application")
    .SingleAsync();
```

We can use this `IApplication` object to create user accounts, log in user accounts, and all sorts of other cool stuff.

#### Creating a user account

Let's create a new user!

```csharp
var joe = await myApp.CreateAccountAsync("Joe", "Stormtrooper",
                                         "tk421@galacticempire.co",
                                         "Changeme123!");
Console.WriteLine("User " + joe.FullName + " created");
```

The returned `IAccount` object will contain all the account's data. Easy! :smile: 

#### Logging in

Now that we have a valid user in the application, we can perform a login request.

```csharp
// Imagine that we got these values from a web request:
var usernameOrEmail = "tk421@galacticempire.co";
var password = "Changeme123!";

try
{
     var loginResult = await myApp.AuthenticateAccountAsync(usernameOrEmail, password);
     var loggedInAccount = await loginResult.GetAccountAsync();
     Console.WriteLine("User {0} logged in", loggedInAccount.FullName);
}
catch (ResourceException rex)
{
    Console.WriteLine("Could not log in. Error: " + rex.Message);
}
```

:bulb: You can also use the `TryAuthenticateAccountAsync(username, password)` overload to get a `bool` result if you don't need to access the account's data after login.

#### Clean up

Cleaning up your Stormpath application after our demo is easy:

```csharp
try
{
    await joe.DeleteAsync();
}
catch (ResourceException rex)
{
    Console.WriteLine("Unexpected error when deleting " + joe.Email + ". Error: " + rex.Message);
}
```

As a final touch, we can add this line at the end so the console window doesn't disappear immediately:

```csharp
Console.WriteLine("Done!")
Console.ReadKey(false);
```

#### Other things you can do with the SDK

 * Create and delete entire applications
 * Search for accounts using LINQ
 * Modify and save account data
 * Reset an account's password

## Build Instructions

#### Building with Visual Studio

Building the SDK requires Visual Studio 2015.

 1. Use `git clone` or the Visual Studio GitHub extension to clone the branch you want to build (`master` or `develop`).
 2. Open `stormpath-sdk-csharp\Stormpath.SDK\Stormpath.SDK.sln` in Visual Studio.
 3. On first build, the required NuGet packages should resore. If not, [enable automatic package restore](http://stackoverflow.com/a/29708080/3191599).

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

For all additional information, please see the full [Project Documentation](http://docs.stormpath.com/java/product-guide/).
