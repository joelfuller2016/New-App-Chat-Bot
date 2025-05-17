using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using App_Code;
using ChatBot.Data.Models;
using ChatBot.Data.Repositories;
using ChatBot.Services.OpenAI;
using ChatBot.Services.Logging;
using ChatBot.Services.Auth;
using Newtonsoft.Json;
using System.Text;

namespace ChatBot.Chat
{
    public partial class ChatPage : Page
    {
        // Add the control declarations
        protected TextBox UserMessage;
        protected HtmlGenericControl JsonResponse;
        protected HtmlGenericControl ChatContainer;
        protected HtmlGenericControl DebugPanel;
        protected Button SendButton;

        // Get services from dependency resolver
        private readonly IOpenAIService _openAIService;
        private readonly IErrorLogger _errorLogger;
        private readonly IRepository<ConfigModel> _configRepository;
        private readonly IRepository<ChatModel> _chatRepository;
        private readonly IAuthManager _authManager;

        public ChatPage()
        {
            _openAIService = DependencyResolver.Resolve<IOpenAIService>();
            _errorLogger = DependencyResolver.Resolve<IErrorLogger>();
            _configRepository = DependencyResolver.Resolve<IRepository<ConfigModel>>();
            _chatRepository = DependencyResolver.Resolve<IRepository<ChatModel>>();
            _authManager = DependencyResolver.Resolve<IAuthManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated
            if (!_authManager.IsAuthenticated())
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Display debug panel for admins
            DebugPanel.Visible = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];

            if (!IsPostBack)
            {
                // Load chat history for this user
                LoadChatHistory();
            }
        }

        protected async void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string userMessage = UserMessage.Text;
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    return;
                }

                // Add user message to chat UI
                AddMessageToChat(userMessage, true);

                // Get model preference from configuration
                var modelConfig = _configRepository.Find(c => c.Key == "OpenAIModel").FirstOrDefault();
                string model = modelConfig?.Value ?? "gpt-3.5-turbo";

                // Send message to OpenAI
                var response = await _openAIService.GetChatCompletionAsync(userMessage, model);
                
                // Parse the response
                dynamic parsedResponse = JsonConvert.DeserializeObject(response);
                string aiMessage = parsedResponse.choices[0].message.content.ToString();

                // Display the AI response in the chat
                AddMessageToChat(aiMessage, false);
                
                // Display raw JSON for admins
                if (DebugPanel.Visible)
                {
                    JsonResponse.InnerText = JsonConvert.SerializeObject(parsedResponse, Formatting.Indented);
                }

                // Store the chat in the repository
                var chat = new ChatModel
                {
                    UserId = Session["UserId"] as int? ?? null,
                    Message = userMessage,
                    Response = aiMessage,
                    RawRequest = userMessage,
                    RawResponse = response,
                    DateCreated = DateTime.UtcNow
                };

                // Save to database
                _chatRepository.Add(chat);

                // Clear the input field
                UserMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                _errorLogger.Log(ex);
                AddMessageToChat("Sorry, I encountered an error: " + ex.Message, false);
                
                if (DebugPanel.Visible)
                {
                    JsonResponse.InnerText = "Error: " + ex.Message + "\n" + ex.StackTrace;
                }
            }
        }

        private void LoadChatHistory()
        {
            // Get chat history for this user
            int? userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                var chatHistory = _chatRepository.Find(c => c.UserId == userId);
                foreach (var chat in chatHistory.OrderBy(c => c.DateCreated).Take(50)) // Limit to last 50 messages
                {
                    AddMessageToChat(chat.Message, true);
                    AddMessageToChat(chat.Response, false);
                }
            }
        }

        private void AddMessageToChat(string message, bool isUser)
        {
            var div = new HtmlGenericControl("div");
            div.Attributes["class"] = isUser ? "message-bubble message-user" : "message-bubble message-bot";
            div.InnerHtml = message.Replace("\n", "<br/>");

            ChatContainer.Controls.Add(div);

            // Add script to scroll to bottom after rendering
            ScriptManager.RegisterStartupScript(this, GetType(), "ScrollToBottom", "setTimeout(function() { scrollToBottom(); }, 100);", true);
        }
    }
}
