<%@ Application Language="C#" %>
<%@ Import Namespace="ChatBot" %>
<%@ Import Namespace="ChatBot.Data" %>
<%@ Import Namespace="ChatBot.Services.Logging" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">
    void Application_Start(object sender, EventArgs e)
    {
        // Initialize dependency resolver
        DependencyResolver.Initialize();
        
        // Register routes
        RegisterRoutes(RouteTable.Routes);
    }
    
    void RegisterRoutes(RouteCollection routes)
    {
        routes.MapPageRoute(
            "Home",
            "",
            "~/Chat/Chat.aspx"
        );
        
        routes.MapPageRoute(
            "Login",
            "login",
            "~/Login.aspx"
        );
        
        routes.MapPageRoute(
            "Admin",
            "admin",
            "~/Admin/Admin.aspx"
        );
        
        routes.MapPageRoute(
            "Debug",
            "debug",
            "~/Debug/Debug.aspx"
        );
    }
    
    void Application_Error(object sender, EventArgs e)
    {
        // Get the exception
        Exception ex = Server.GetLastError();
        
        // Log the error using our error logger
        try
        {
            var errorLogger = DependencyResolver.Resolve<IErrorLogger>();
            errorLogger.Log(ex);
        }
        catch
        {
            // If dependency resolution fails, log to the application log
            System.Diagnostics.Debug.WriteLine("Application Error: " + ex.Message);
        }
        
        // Clear the error
        Server.ClearError();
        
        // Redirect to error page
        Response.Redirect("~/Error.aspx");
    }
</script>
