# App Service Mobile completed quickstart for .NET backend
This repository contains a .NET backend Mobile App projects based on the App Service Mobile Apps quickstart project, which you can download from the [Azure portal](https://portal.azure.com). This project has been enhanced by the addition of offline sync, authentication, and push notification functionality. This demonstrates how to best integrate the various Mobile Apps features. To learn how to download the Windows quickstart app backend project from the portal, see [Create a Windows app](https://azure.microsoft.com/documentation/articles/app-service-mobile-windows-store-dotnet-get-started/). This readme topic contains the following information to help you run the sample app project and to better understand the design decisions.

+ [Overview](#overview)
+ [Configure the Mobile App backend](#configure-the-mobile-app-backend)
+ [Configure the Windows app](#configure-the-windows-app)
	+ [Configure authentication](#configure-authentication)
	+ [Configure push notifications](#configure-push-notifications)
+ [Running the app](#running-the-app)
+ [Implementation notes](#implementation-notes)
	+ [Template push notification registration](#template-push-notification-registration)
	+ [Client-added push notification tags](#client-added-push-notification-tags)
	+ [Authenticate first](#authenticate-first)

##Overview
The project in this repository is equivalent to downloading the quickstart .NET backend project from the portal and then completing the following Mobile Apps tutorials:

+ [Enable offline sync for your Windows app](https://azure.microsoft.com/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-offline-data/)
+ [Add authentication to your Windows app](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-users/)
+ [Add push notifications to your Windows app](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-push/) 

## Configure the .NET backend project

The first step is to create a new Mobile App backend in Azure and configure and publish this project to that backend. 

## Configure authentication

Because both the client and backend are configured to use authentication, you must define an authentication provider for your app and register it with your Mobile App backend in the [portal](https://portal.azure.com).

Follow the instructions in the topic to configure the Mobile App backend to use one of the following authentication providers:

+ [AAD](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-active-directory-authentication/)
+ [Facebook](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-facebook-authentication/)
+ [Google](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-google-authentication/)
+ [Microsoft account](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-microsoft-authentication/)
+ [Twitter](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-twitter-authentication/)

The `[Authorize]` attribute applied to the controllers means that requests must contain an App Service authentication header (*X-ZUMO-AUTH*) with a valid token.

## Configure push notifications

You need to configure push notifications by registering your Windows app with the Windows Store then storing the app's package SID and client secret in the Mobile App backend. These credentials are used by Azure to connect to Windows Notification Service (WNS) to send push notifications. Complete the following sections of the push notifications tutorial to configure push notifications:

1. [Create a Notification Hub](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-create-notification-hub.md)
2. [Register your app for push notifications](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-register-wns.md)
3. [Configure the backend to send push notifications](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-configure-wns.md)


## Implementation notes 
This section highlights changes made to the original tutorial samples and other design decisions were made when implementing all of the features or Mobile Apps in the same client app. 

###Template push notification registration

TBD


For more information, see [How to: Register push templates to send cross-platform notifications](https://azure.microsoft.com/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/#how-to-register-push-templates-to-send-cross-platform-notifications).

###Client-added push notification tags

TBD

For more information, see [Adding push notification tags from an Azure Mobile Apps client](http://blogs.msdn.com/b/writingdata_services/archive/2016/01/22/adding-push-notification-tags-from-an-azure-mobile-apps-client.aspx).

###Authenticate first
TBD

