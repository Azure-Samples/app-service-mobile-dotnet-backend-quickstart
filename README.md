---
services: app-service\mobile
platforms: dotnet
author: ggailey777
---
# App Service Mobile completed quickstart for .NET backend
This repository contains a .NET backend Mobile App project based on the App Service Mobile Apps quickstart project, which you can download from the [Azure portal](https://portal.azure.com). This project has been enhanced by the addition of offline sync, authentication, and push notification functionality. This sample demonstrates how to best integrate the various Mobile Apps features. This readme topic contains the following information to help you run the sample app project and to better understand the design decisions.

+ [Overview](#overview)
+ [Create a new .NET backend Mobile App](#create-a-new-net-backend-mobile-app)
+ [Configure authentication](#configure-authentication)
+ [Configure push notifications](#configure-push-notifications)
+ [Publish the project to Azure](#publish-the-project-to-azure)
+ [Implementation notes](#implementation-notes)
	+ [Push to users](#push-to-users)
	+ [Template push notification registration](#template-push-notification-registration)
	+ [Client-added push notification tags](#client-added-push-notification-tags)

To learn more about a Mobile Apps .NET backend project, see [Work with the .NET backend server SDK for Azure Mobile Apps](https://azure.microsoft.com/documentation/articles/app-service-mobile-dotnet-backend-how-to-use-server-sdk/).

##Overview
The project in this repository is equivalent to downloading the quickstart .NET backend project from the portal and then completing the following Mobile Apps tutorials:

+ [Enable offline sync for your Windows app](https://azure.microsoft.com/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-offline-data/)
+ [Add authentication to your Windows app](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-users/)
+ [Add push notifications to your Windows app](https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-push/) 

## Create a new .NET backend Mobile App 

The first step is to create a new Mobile App backend in Azure. You can do this either by completing the [quickstart tutorial](https://azure.microsoft.com/documentation/articles/app-service-mobile-windows-store-dotnet-get-started/) or by [following these steps](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-dotnet-backend-create-new-service.md).

## Configure authentication

Because both the client and backend are configured to use authentication, you must define an authentication provider for your app and register it with your Mobile App backend in the [portal](https://portal.azure.com).

Follow the instructions in the topic to configure the Mobile App backend to use one of the following authentication providers:

+ [AAD](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-active-directory-authentication/)
+ [Facebook](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-facebook-authentication/)
+ [Google](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-google-authentication/)
+ [Microsoft account](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-microsoft-authentication/)
+ [Twitter](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-twitter-authentication/)

The `[Authorize]` attribute applied to the controllers means that requests must contain an App Service authentication header (*X-ZUMO-AUTH*) with a valid token. To test authentication when running locally, you must also set the following app settings in the Web.config file:

- **SigningKey** - obtained from
- **ValidAudience** - defaults to the URL of your Mobile App backend.
- **ValidIssuer** - defaults to the URL of your Mobile App backend.

## Configure push notifications

You need to configure push notifications by registering your Windows app with the Windows Store then storing the app's package SID and client secret in the Mobile App backend. These credentials are used by Azure to connect to Windows Notification Service (WNS) to send push notifications. Complete the following sections of the push notifications tutorial to configure push notifications:

1. [Create a Notification Hub](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-create-notification-hub.md)
2. [Register your app for push notifications](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-register-wns.md)
3. [Configure the backend to send push notifications](https://github.com/Azure/azure-content-pr/blob/master/includes/app-service-mobile-configure-wns.md)

For local testing, you must also replace the following settings values in the Web.config file with values from the notification hub used by your Mobile App backend:

- **MS_NotificationHubName** - the name of the notification hub.
- **MS_NotificationHubConnectionString** - the Notification Hubs connection string for the *DefaultFullSharedAccessSignature* key. 


## Publish the project to Azure

To be able to test this project with one of the corresponding client apps, you need to publish the project to your Mobile App backend in Azure: 

1. In Solution Explorer, right-click the project and click **Publish**.
2. In **Profile**, click **Microsoft Azure App Service**, select your subscription, locate the Mobile App backend you created either by browsing or by searching for the name, then click **OK**.
3. Validate the connection, then click **Publish**.

Now you are ready to test. Because authentication is enabled on this project, much harder to test using a REST client since you will need to present an X-ZUMO-AUTH header in the request that contains a valid access token.

## Implementation notes 
This section highlights changes made to the original tutorial samples and other design decisions were made when implementing all of the features or Mobile Apps in the same client app. 

###Push to users
The push notification tutorial sends broadcast push notifications to all registrations. Because authentication is enabled in the backed, all push notification registration requests handled by the backend from Mobile Apps clients get a userId tag added to the registration automatically. This tag can then be used to send push notifications to a specific user. The code below gets the userID for the logged in user and uses it to send a notification to only that user.

	// Get the settings for the server project.
    HttpConfiguration config = this.Configuration;
    MobileAppSettingsDictionary settings =
        this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

    // Get the Notification Hubs credentials for the Mobile App.
    string notificationHubName = settings.NotificationHubName;
    string notificationHubConnection = settings
        .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

    // Create the notification hub client.
    NotificationHubClient hub = NotificationHubClient
        .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

    // Get the current user SID and create a tag for the current user.
    var claimsPrincipal = this.User as ClaimsPrincipal;
    string sid = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;
    string userTag = "_UserId:" + sid;

    // Build a dictionary for the template with the item message text.
    var notification = new Dictionary<string, string> { { "message", item.Text } };

    try
    {
        // Send a template notification to the user ID.
        await hub.SendTemplateNotificationAsync(notification, userTag);
    }
    catch (System.Exception)
    {
        throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
    }

If the user has registered on multiple devices, each device will get a notification.

###Template push notification registration
The original push notification tutorial used native registrations. This sample has been changed to use a template registration, which makes it easier to send push notifications to users on multiple clients from a single **send** method call. You can see in the above code that the **SendTemplateNotificationAsync()** method is called, which sends a notification to all platforms.

For more information, see [How to: Register push templates to send cross-platform notifications](https://azure.microsoft.com/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/#how-to-register-push-templates-to-send-cross-platform-notifications).

###Client-added push notification tags
When a mobile app registers for push notifications using an Azure App Service Mobile Apps backend, there are two default tags that can get added to the registration in Azure Notification Hubs: the installation ID, which is unique to the app on a given device, and the user ID, which is only added when the user has been previously authenticated. Any other tags that get supplied by the client are ignored, which is by design. (Note that this differs from Mobile Services, where the client could supply any tag and there were hooks into the registration process on the backend to validate tags on incoming registrations.) 

Because the client can’t add tags and at the same time there are no service-side hooks into the push notification registration process, the client needs to do the work of adding new tags to a given registration. In this sample, there is an **UpdateTagsController** that defines an `/updatetags` endpoint to enable clients to add tags to their push registration. The client calls that endpoint with its *installationId* to create new tags. 

The following code updates and installation to add user-supplied tags:

    // Verify that the tags are a valid JSON array.
    var tags = JArray.Parse(message);
               
    // Define a collection of PartialUpdateOperations. Note that 
    // only one '/tags' path is permitted in a given collection.
    var updates = new List<PartialUpdateOperation>();

    // Add a update operation for the tag.
    updates.Add(new PartialUpdateOperation
    {
        Operation = UpdateOperationType.Add,
        Path = "/tags",
        Value = tags.ToString()
    });

    // Add the requested tag to the installation.
    await hubClient.PatchInstallationAsync(Id, updates);

For more information, see [Adding push notification tags from an Azure Mobile Apps client](http://blogs.msdn.com/b/writingdata_services/archive/2016/01/22/adding-push-notification-tags-from-an-azure-mobile-apps-client.aspx).
