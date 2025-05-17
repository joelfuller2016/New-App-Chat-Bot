<%@ Application Language="C#" %>
<script runat="server">
    void Application_Start(object sender, EventArgs e)
    {
        App_Code.DbManager.Initialize();
    }
</script>
