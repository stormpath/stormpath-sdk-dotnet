# Stormpath .NET SDK #  [![Build Status](https://travis-ci.org/stormpath/stormpath-sdk-csharp.svg)](https://travis-ci.org/stormpath/stormpath-sdk-csharp)

*An advanced, reliable and easy-to-use user management API for .NET by security experts*

[Stormpath](https://stormpath.com) is a complete user management API.  This
library gives your ASP.NET web application access to all of Stormpath's features:

- Robust authentication and authorization.
- Schemaless user data and profiles.
- A hosted login subdomain, for easy Single Sign-On across your apps.
- Social login with Facebook and Google OAuth.
- Secure API key authentication for your service.
- Servlet support for Java web applications.

If you have feedback about this library, please get in touch and share your
thoughts! support@stormpath.com

## Current status

This library is currently under active development. If you would like to be notified when we publish our first release candidate, watch this repository or send an email to nate@stormpath.com.

## Installation Instructions

Coming soon.

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
 
 Save this file in a secure location, such as in a `.stormpath` directory in your home directory. (You can save it anywhere you like, but `%homedrive%%homepath\.stormpath\` is the default location the SDK checks.)
 
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
 2. Right-click on the new project in the Solution Explorer and choose **Manage Nuget Packages...**
 3. Search for Stormpath. Install the Stormpath SDK package.
   * Alternatively, you can simply execute `install-package Stormpath.SDK` from the Pacakge Manager Console.
 4. Add the following using statements to the top of your `Program.cs` file:
 
 ```
 using Stormpath.SDK;
 using Stormpath.SDK.Client;
 using Stormpath.SDK.Account;
 using Stormpath.SDK.Error
 ```
 
#### `await` a minute

The Stormpath SDK performs `await`able asynchronous operations by default. This makes it possible to make requests to the Stormpath API without tying up a worker thread - super cool for ASP.NET projects!

In the Console Application template, however, the entry point is `static void Main()`, so we can't use the `await` keyword. In an ASP.NET application, we could just change the method signature to `async Task`, but that doesn't work well in a console app. Let's get around that by writing a separate method instead:

```
static Task MainAsync(CancellationToken cancellationToken)
{
}
```

and calling it synchronously from `Main()`:

````
static void Main(string[] args)
{
    MainAsync(CancellationToken.None).GetAwaiter().GetResult();
}
````

If you are confused by all this `async` business, check out Stephen Cleary's [excellent introductory blog post](http://blog.stephencleary.com/2012/02/async-and-await.html) on the subject.

#### Let's make some requests!

Alright, now for the fun stuff. The first thing we need to connect to the Stormpath API is a `Client` object. In our new `MainAsync` method, let's create one:

````
// If you put the apiKey.properties file in the default location
// (%homedrive%%homepath\.stormpath\apiKey.properties), you can skip this!
IClientApiKey myApiKey = ClientApiKeys.Builder()
    .SetFileLocation("path\\to\\apiKey.properties")
    .Build();

// Create an IClient object. Everything starts here!
IClient client = Clients.Builder()
    .SetApiKey(myApiKey)
    .Build();
````

You can skip building the `IClientApiKey` object and using `.SetApiKey()` if you put your `apiKey.properties` file in the default location, because calling `.Build()` without specifying an API Key is smart enough to look for one there. :smile:



## Build Instructions

#### Building under Windows

Coming soon.

#### Building under Linux

Coming soon.

## Contributing

Contributions, bug reports and issues are very welcome. Stormpath regularly maintains this repository, and are quick to review pull requests and accept changes!

You can make your own contributions by forking the develop branch of this repository, making your changes, and issuing pull request on the develop branch.

## Copyright

Copyright &copy; 2013-2015 Stormpath, Inc. and contributors.

This project is open-source via the [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).

For all additional information, please see the full [Project Documentation](http://docs.stormpath.com/java/product-guide/).
