<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="ChatBot.Admin.Admin" %>
<!DOCTYPE html>
<html>
<head>
    <title>Admin</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" />
</head>
<body class="container mt-4">
    <h1>Admin Configuration</h1>
    <form runat="server" class="mb-3">
        <div class="mb-3">
            <label for="ApiKey">API Key</label>
            <asp:TextBox ID="ApiKey" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="Model">Model</label>
            <asp:TextBox ID="Model" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="GoogleClientId">Google Client ID</label>
            <asp:TextBox ID="GoogleClientId" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="GoogleClientSecret">Google Client Secret</label>
            <asp:TextBox ID="GoogleClientSecret" runat="server" CssClass="form-control" TextMode="Password" />
        </div>
        <div class="mb-3">
            <label for="AdminUser">Admin Username</label>
            <asp:TextBox ID="AdminUser" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="AdminPassword">Admin Password</label>
            <asp:TextBox ID="AdminPassword" runat="server" CssClass="form-control" TextMode="Password" />
        </div>
        <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="SaveButton_Click" />
    </form>
    <a href="../Chat/Chat.aspx">Go to Chat</a>
</body>
</html>
