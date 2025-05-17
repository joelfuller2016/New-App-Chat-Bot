<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="ChatBot.Chat.ChatPage" %>
<!DOCTYPE html>
<html>
<head>
    <title>AI Chat</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" />
    <style>
        .chat-container {
            height: 70vh;
            overflow-y: auto;
            border: 1px solid #ddd;
            border-radius: 0.25rem;
            padding: 1rem;
            margin-bottom: 1rem;
            background-color: #f8f9fa;
        }
        .message-bubble {
            max-width: 75%;
            padding: 0.5rem 1rem;
            border-radius: 1rem;
            margin-bottom: 1rem;
            position: relative;
        }
        .message-user {
            background-color: #007bff;
            color: white;
            margin-left: auto;
        }
        .message-bot {
            background-color: #e9ecef;
            color: #212529;
        }
        .typing-indicator {
            display: none;
            background-color: #e9ecef;
            color: #6c757d;
            padding: 0.5rem 1rem;
            border-radius: 1rem;
            margin-bottom: 1rem;
            width: fit-content;
        }
        .typing-indicator span {
            animation: ellipsis 1.5s infinite;
        }
        @keyframes ellipsis {
            0% { content: '.'; }
            33% { content: '..'; }
            66% { content: '...'; }
        }
        .debug-panel {
            margin-top: 1rem;
            padding: 1rem;
            border: 1px solid #ddd;
            border-radius: 0.25rem;
        }
    </style>
</head>
<body class="container mt-4">
    <nav class="navbar navbar-expand-lg navbar-light bg-light mb-4">
        <div class="container-fluid">
            <a class="navbar-brand" href="#">AI Chat</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item">
                        <a class="nav-link" href="../Admin/Admin.aspx">Admin</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="../Debug/Debug.aspx">Debug</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="../Login.aspx?logout=true">Logout</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>

    <div class="row">
        <div class="col-md-12">
            <div id="ChatContainer" class="chat-container" runat="server">
                <!-- Messages will be populated here -->
            </div>
            
            <div id="TypingIndicator" class="typing-indicator">
                <span>AI is typing</span>
            </div>
            
            <form runat="server" class="mb-3">
                <div class="input-group">
                    <asp:TextBox ID="UserMessage" runat="server" CssClass="form-control" placeholder="Type your message..." />
                    <asp:Button ID="SendButton" runat="server" Text="Send" CssClass="btn btn-primary" OnClick="SendButton_Click" />
                </div>
            </form>
        </div>
    </div>
    
    <div class="row debug-panel" runat="server" id="DebugPanel" visible="false">
        <div class="col-12">
            <h5>Debug Information</h5>
            <pre id="JsonResponse" runat="server" style="max-height: 300px; overflow: auto;"></pre>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        // Function to scroll chat to bottom
        function scrollToBottom() {
            var chatContainer = document.getElementById('<%= ChatContainer.ClientID %>');
            chatContainer.scrollTop = chatContainer.scrollHeight;
        }
        
        // Scroll to bottom when page loads
        window.onload = function() {
            scrollToBottom();
        };
        
        // Show typing indicator when form submits
        var form = document.querySelector('form');
        var typingIndicator = document.getElementById('TypingIndicator');
        
        form.addEventListener('submit', function() {
            typingIndicator.style.display = 'block';
            setTimeout(function() {
                typingIndicator.style.display = 'none';
            }, 10000); // Timeout after 10 seconds in case of error
        });
    </script>
</body>
</html>
