<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ChatBot.ErrorPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error - Chat Bot</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="container mt-5">
    <div class="row">
        <div class="col-md-6 mx-auto">
            <div class="card">
                <div class="card-header bg-danger text-white">
                    <h4>Error</h4>
                </div>
                <div class="card-body">
                    <p>An error occurred while processing your request. Please try again later.</p>
                    <p id="ErrorMessage" runat="server" visible="false"></p>
                    <hr />
                    <a href="~/Chat/Chat.aspx" runat="server" class="btn btn-primary">Return to Chat</a>
                    <a href="~/Login.aspx" runat="server" class="btn btn-secondary">Return to Login</a>
                </div>
            </div>
        </div>
    </div>
</body>
</html>