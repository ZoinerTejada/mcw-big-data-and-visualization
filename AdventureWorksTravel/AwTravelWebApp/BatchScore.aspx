<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchScore.aspx.cs" Inherits="PBIWebApp.BatchScore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Batch Scoring</title>
</head>
<body>
    <h1>Batch Score Weather Delays with Machine Learning</h1>
    <form id="form1" runat="server">

        <div>
            <h2>Submit Job</h2>
            <asp:Label ID="Label1" runat="server" Text="Relative Location of File to Score"></asp:Label>
            <asp:TextBox ID="txtSourceRelativeLocation" runat="server" Text="sol-exp-spark/flights/FlightsAndWeather.csv" Width="600"></asp:TextBox>
        </div>

        <div>
            <asp:Label ID="Label2" runat="server" Text="Relative Location of Scored Output File"></asp:Label>
            <asp:TextBox ID="txtDestRelativeLocation" runat="server" Text="sol-exp-spark/flights/Scored_FlightsAndWeather.csv" Width="600"></asp:TextBox>
        </div>

        <div>
            <asp:Button ID="btnSubmitJob" runat="server" Text="Submit Job" OnClick="btnSubmitJob_Click" />
        </div>

        <asp:Panel ID="Panel1" runat="server">
            <div>
                <h2>Start Job</h2>
                <asp:Label ID="Label3" runat="server" Text="Job ID:"></asp:Label>
                <asp:TextBox ID="txtJobID" runat="server"></asp:TextBox>
            </div>
            <div>
                <asp:Button ID="btnStartJob" runat="server" Text="Start Job" OnClick="btnStartJob_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel2" runat="server">
            <div>
                <h2>Check Job Status</h2>
                <asp:Button ID="btnCheckJobStatus" runat="server" Text="Check Job Status" OnClick="btnCheckJobStatus_Click"  />
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel3" runat="server">
            <div>
                <asp:Label ID="Label4" runat="server" Text="Job Status:"></asp:Label>
                <asp:TextBox ID="txtJobStatus" runat="server"></asp:TextBox>
            </div>
        </asp:Panel>
    </form>


</body>
</html>
