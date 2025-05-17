<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head>
    <title>Login</title>
</head>
<body>
    <form runat="server">
        <asp:Button ID="LoginButton" runat="server" Text="Login with Google" OnClick="LoginButton_Click" />
    </form>
</body>
</html>
<script runat="server">
    protected void LoginButton_Click(object sender, EventArgs e)
    {
        App_Code.AuthManager.SignIn(Response);
    }
</script>
