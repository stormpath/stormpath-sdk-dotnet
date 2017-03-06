#Stormpath is Joining Okta
We are incredibly excited to announce that [Stormpath is joining forces with Okta](https://stormpath.com/blog/stormpaths-new-path?utm_source=github&utm_medium=readme&utm-campaign=okta-announcement). Please visit [the Migration FAQs](https://stormpath.com/oktaplusstormpath?utm_source=github&utm_medium=readme&utm-campaign=okta-announcement) for a detailed look at what this means for Stormpath users.

We're available to answer all questions at [support@stormpath.com](mailto:support@stormpath.com).

# Stormpath .NET SDK [![Slack Status](https://talkstormpath.shipit.xyz/badge.svg)](https://talkstormpath.shipit.xyz) [![NuGet](https://img.shields.io/nuget/v/Stormpath.SDK.svg)](https://www.nuget.org/packages/Stormpath.SDK)

[Stormpath](https://stormpath.com) is a complete user management API.  This
library gives your ASP.NET, C#, or Visual Basic application access to all of Stormpath's features:

- Robust authentication and authorization.
- Schemaless user data and profiles.
- A hosted login subdomain, for easy Single Sign-On across your apps.
- External login with social providers like Facebook and Google, or SAML IdPs.
- Secure API key authentication for your service.

In addition to these core Stormpath features, this SDK provides:

- Support for .NET 4.5 and later.
- Support for .NET Core and Mono.
- LINQ-to-Stormpath for readable, expressive queries.
- Fully dual-stack design for native asynchronous and native synchronous API calls.

## Installation

#### Using the Nuget Package Manager

 1. Right-click on your project in the Solution Explorer and choose **Manage Nuget Packages...**
 2. Search for Stormpath. Install the `Stormpath.SDK` package.

#### Using the Package Manager Console

Simply run `install-package Stormpath.SDK`. Done!

## Quickstart

To learn how to use the Stompath .NET SDK in a simple project, follow our easy quickstart:

* [C# Quickstart](http://docs.stormpath.com/csharp/product-guide/latest/quickstart.html)
* [Visual Basic Quickstart](http://docs.stormpath.com/vbnet/product-guide/latest/quickstart.html)

## Build Instructions

#### Building with Visual Studio

Building the SDK requires Visual Studio 2015 or later.

 1. Use `git clone` or the Visual Studio GitHub extension to check out the `develop` branch.
 2. Open `stormpath-sdk-dotnet\Stormpath.SDK.sln` in Visual Studio.
 3. On first build, the required NuGet packages will resore. If not, [enable automatic package restore](http://stackoverflow.com/a/29708080/3191599).

#### Building with `dotnet`

If you have the `dotnet` tool installed (included with the [.NET Core SDK](https://www.microsoft.com/net/core)), you can build on any platform supported by .NET Core:

```
git clone https://github.com/stormpath/stormpath-sdk-dotnet.git
cd stormpath-sdk-dotnet
git checkout develop

cd src\Stormpath.SDK.Abstractions
dotnet restore
dotnet build

cd ..\Stormpath.SDK.Core
dotnet restore
dotnet build
```

## Contributing

Contributions, bug reports, and issues are very welcome! Stormpath regularly maintains this repository, and are quick to review pull requests and accept changes.

You can make your own contributions by forking the develop branch of this repository, making your changes, and issuing pull requests against the `develop` branch.

## Copyright

Copyright &copy; 2013-2016 Stormpath, Inc. and contributors.

This project is open-source via the [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0).
