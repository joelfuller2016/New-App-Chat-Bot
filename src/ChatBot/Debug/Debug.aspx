<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Debug.aspx.cs" Inherits="ChatBot.Debug.DebugPage" %>
<!DOCTYPE html>
<html>
<head>
    <title>Debug</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" />
</head>
<body class="container mt-4">
    <h1>Debug Logs</h1>
    <pre id="Logs" runat="server" style="height:400px;overflow:auto;"></pre>
    <a href="../Chat/Chat.aspx">Back to Chat</a>
</body>
</html>
