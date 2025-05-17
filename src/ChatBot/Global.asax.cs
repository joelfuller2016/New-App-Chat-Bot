using System;
using System.Web;

namespace ChatBot
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            App_Code.DbManager.Initialize();
        }
    }
} 