<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChatBot.LoginPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - Chat Bot</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .login-container {
            max-width: 400px;
            margin: 0 auto;
            padding-top: 100px;
        }
        .google-btn {
            background-color: #4285F4;
            color: white;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="login-container">
            <h2 class="text-center mb-4">Login to Chat Bot</h2>
            
            <form id="form1" runat="server">
                <asp:HiddenField ID="AntiForgeryToken" runat="server" />
                
                <div class="mb-3">
                    <asp:Label ID="lblUsername" runat="server" CssClass="form-label" Text="Username"></asp:Label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                        ControlToValidate="txtUsername" 
                        ErrorMessage="Username is required" 
                        CssClass="text-danger">
                    </asp:RequiredFieldValidator>
                </div>
                
                <div class="mb-3">
                    <asp:Label ID="lblPassword" runat="server" CssClass="form-label" Text="Password"></asp:Label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                        ControlToValidate="txtPassword" 
                        ErrorMessage="Password is required" 
                        CssClass="text-danger">
                    </asp:RequiredFieldValidator>
                </div>
                
                <div class="mb-3">
                    <asp:Button ID="btnLogin" runat="server" Text="Login" 
                        CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
                </div>
                
                <div class="mb-3">
                    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>
                </div>
                
                <div class="text-center">
                    <hr />
                    <p class="mb-3">Or sign in with</p>
                    <asp:Button ID="btnGoogleLogin" runat="server" 
                        Text="Google" 
                        CssClass="btn google-btn w-100" 
                        OnClick="btnGoogleLogin_Click" 
                        CausesValidation="false" />
                </div>
            </form>
        </div>
    </div>
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>