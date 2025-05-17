using System;
using System.Web.UI;

namespace ChatBot
{
    public partial class ErrorPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only show detailed error messages to admins
            if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
            {
                string errorMessage = Request.QueryString["msg"];
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ErrorMessage.InnerText = errorMessage;
                    ErrorMessage.Visible = true;
                }
            }
        }
    }
}