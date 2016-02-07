using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using todolist_completeService.DataObjects;
using todolist_completeService.Models;

using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Authentication;

using System.Security.Claims;
using System.Security.Principal;

using System.Web.Http.Tracing;

namespace todolist_completeService.Controllers
{
    [Authorize]
    public class TodoItemController : TableController<TodoItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            todolist_completeContext context = new todolist_completeContext();
            DomainManager = new EntityDomainManager<TodoItem>(context, Request);   
        }

        // GET tables/TodoItem
        public IQueryable<TodoItem> GetAllTodoItems()
        {            
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TodoItem> GetTodoItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TodoItem> PatchTodoItem(string id, Delta<TodoItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        //// Post
        //public async Task<IHttpActionResult> PostTodoItem(TodoItem item)
        //{
        //    TodoItem current = await InsertAsync(item);

        //    // Get the settings for the server project.
        //    HttpConfiguration config = this.Configuration;
        //    MobileAppSettingsDictionary settings =
        //        this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        //    // Get the Notification Hubs credentials for the Mobile App.
        //    string notificationHubName = settings.NotificationHubName;
        //    string notificationHubConnection = settings
        //        .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

        //    // Create the notification hub client.
        //    NotificationHubClient hub = NotificationHubClient
        //        .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

        //    // Define a WNS payload
        //    var windowsToastPayload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"
        //                            + item.Text + @"</text></binding></visual></toast>";
        //    try
        //    {
        //        // Send the push notification.
        //        var result = await hub.SendWindowsNativeNotificationAsync(windowsToastPayload);

        //        // Write the success result to the logs.
        //        config.Services.GetTraceWriter().Info(result.State.ToString());
        //    }
        //    catch (System.Exception ex)
        //    {
        //        // Write the failure result to the logs.
        //        config.Services.GetTraceWriter()
        //            .Error(ex.Message, null, "Push.SendAsync Error");
        //    }
        //    return CreatedAtRoute("Tables", new { id = current.Id }, current);
        //}

        // POST tables/TodoItem
        // Templates push
        public async Task<IHttpActionResult> PostTodoItem(TodoItem item)
        {
            TodoItem current = await InsertAsync(item);

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
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTodoItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}