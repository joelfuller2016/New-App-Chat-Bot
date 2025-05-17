<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="ChatBot.Chat.ChatPage" %>
<!DOCTYPE html>
<html>
<head>
    <title>Chat</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" />
</head>
<body class="container mt-4">
    <h1>Chat</h1>
    <form runat="server" class="mb-3">
        <div class="mb-3">
            <asp:TextBox ID="UserMessage" runat="server" CssClass="form-control" />
        </div>
        <asp:Button ID="SendButton" runat="server" Text="Send" CssClass="btn btn-primary" OnClick="SendButton_Click" />
    </form>
    <pre id="JsonResponse" runat="server"></pre>
    <a href="../Admin/Admin.aspx">Admin</a> |
    <a href="../Debug/Debug.aspx">Debug</a>
</body>
</html>
