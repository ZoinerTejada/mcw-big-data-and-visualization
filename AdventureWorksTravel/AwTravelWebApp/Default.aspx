<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PBIWebApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1><i class="fa fa-send-o text-primary"></i>&nbsp;Smart Travel Booking</h1>
        <p>Predict travel delays based upon our SmartBooking&reg; system</p>
    </div>

    <div class="row">

        <div class="col-md-3">
            <%--<div class="panel panel-default no-bd">--%>
            <div class="panel panel-transparent no-bd">

                <div class="panel-header bg-dark">
                    <h3 class="panel-title"><strong>Exercise 2-5:</strong> Inputs</h3>
                </div>

                <div class="panel-body bg-white">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="PanelPredictInputs" runat="server" Visible="true">

                                <div class="form-group m-b-30">
                                    <p>From</p>
                                    <input class="form-control" type="text" id="tb_from" value="SAN" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>To</p>
                                    <input class="form-control" type="text" id="tb_to" value="SEA" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Date</p>
                                    <input class="form-control" type="text" id="tb_date" value="11/15/2015" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Time</p>
                                    <input class="form-control" type="text" id="tb_time" value="5:00 PM" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Carrier</p>
                                    <input class="form-control" type="text" id="tb_carrier" placeholder="Enter a carrier" />
                                </div>
                                <input class="btn btn-primary" type="button" id="bPredictDelay" value="Predict Delay" />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-3">
            <div class="panel panel-transparent no-bd">

                <div class="panel-header bg-dark">
                    <h3 class="panel-title"><strong>Exercise 2,3:</strong> Prediction</h3>
                </div>

                <div class="panel-body bg-white">
                    <asp:Panel ID="PanelPredictResults" runat="server" Visible="true">
                        <div class="row">
                            <div class="col-md-12">
                                <h3>Weather Forcast</h3>
                                <img id="weatherForecast" src="http://icons.wxug.com/i/c/k/partlycloudy.gif" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <h3>Delay Prediction</h3>
                                <div class="prediction prediction-mid shadow-z-1">
                                    <span id="predictionDelay">expect delays (73% confidence)</span>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>

        <div class="col-md-3">

            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-transparent no-bd">

                        <div class="panel-header bg-dark">
                            <h3 class="panel-title"><strong>Exercise 4:</strong> Batch Scoring</h3>
                        </div>

                        <div class="panel-body bg-white">
                            <asp:Panel ID="Panel1" runat="server" Visible="true">
                                <div class="form-group m-b-30">
                                    <p>URI to blob for scoring</p>
                                    <input class="form-control" type="text" id="tb_blobToScore" value="" />
                                </div>
                                <input type="button" class="btn btn-primary" id="bBatchScore" value="Batch Score File" />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Sign in -->
            <div class="row">
                <div class="col-md-12">
                    <div class="panel panel-transparent no-bd">

                        <div class="panel-header bg-dark">
                            <h3 class="panel-title"><strong>Exercise 5:</strong> Sign in</h3>
                        </div>

                        <div class="panel-body bg-white">
                            Sign in to your Power BI account to link your account to this web application
                        <p>
                            <asp:Button ID="signInButton" class="btn btn-primary" runat="server" OnClick="signInButton_Click" Text="Sign in to Power BI" />
                        </p>
                            <asp:Panel ID="signInStatus" runat="server" Visible="false">
                                <div class="form-group m-b-30">
                                    <p>Signed in as</p>
                                    <asp:Label ID="userLabel" runat="server"></asp:Label>
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Access Token</p>
                                    <asp:TextBox ID="accessTokenTextbox" class="form-control" runat="server" Width="586px"></asp:TextBox>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>

    <div class="row">

        <div class="col-md-6">

            <!-- Get dashboards -->
            <asp:Panel ID="GroupsResults" runat="server" Visible="true">
                <div class="panel panel-transparent no-bd">

                    <div class="panel-header bg-dark">
                        <h3 class="panel-title"><strong>Exercise 5:</strong> Get groups from your account</h3>
                    </div>

                    <div class="panel-body bg-white">
                        <p>
                            <asp:Button ID="Button_GetGroups" CssClass="btn btn-primary" runat="server" OnClick="getGroupsButton_Click" Text="Get Groups" />
                        </p>
                        <asp:TextBox ID="tb_GroupsResults" runat="server" Height="200px" Width="100%" TextMode="MultiLine" Wrap="False"></asp:TextBox>
                    </div>
                </div>
            </asp:Panel>

        </div>

        <div class="col-md-6">

            <!-- Get datasets -->
            <asp:Panel ID="PBIPanel" runat="server" Visible="true">
                <div class="panel panel-transparent no-bd">

                    <div class="panel-header bg-dark">
                        <h3 class="panel-title"><strong>Step 2:</strong> Get your datasets</h3>
                    </div>

                    <div class="panel-body bg-white">
                        <p>
                            <asp:Button ID="getDatasetsButton" CssClass="btn btn-primary" runat="server" OnClick="getDatasetsButton_Click" Text="Get Datasets" />
                        </p>
                        <asp:TextBox ID="resultsTextbox" runat="server" Height="200px" Width="100%" TextMode="MultiLine" Wrap="False"></asp:TextBox>
                    </div>
                </div>
            </asp:Panel>

        </div>

    </div>


    <!-- Get dashboards -->
    <div>
        <asp:Panel ID="PanelDashboards" runat="server" Visible="true">
            <p><b class="step">Exercise 5</b>: Get dashboards from your account.</p>
            <table>

                <tr>
                    <td>
                        <asp:Button ID="Button2" runat="server" OnClick="getDashboardsButton_Click" Text="Get Dashboards" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="tb_dashboardsResult" runat="server" Height="200px" Width="586px" TextMode="MultiLine" Wrap="False"></asp:TextBox></td>
                </tr>

            </table>
        </asp:Panel>
    </div>

    <!-- Get tiles -->
    <div>
        <asp:Panel ID="TilesStep" runat="server" Visible="true">
            <p><b class="step">Exercise 5</b>: Get tiles from your dashboards.</p>
            <table>
                <tr>
                    <td>Enter a dashboard id from step 3:<asp:TextBox ID="inDashboardID" runat="server" TextMode="SingleLine" Wrap="false"></asp:TextBox></td>
                </tr>

                <tr>
                    <td>
                        <asp:Button ID="Button1" runat="server" OnClick="getTilesButton_Click" Text="Get Tiles" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="tb_tilesResult" runat="server" Height="200px" Width="1024px" TextMode="MultiLine" Wrap="False"></asp:TextBox></td>
                </tr>

            </table>
        </asp:Panel>
    </div>

    <!-- Embed Tile-->
    <div>
        <asp:Panel ID="PanelEmbedTile" runat="server" Visible="true">
            <p><b class="step">Exercise 5</b>: Embed a tile</p>
            <table>
                <tr>
                    <td>Enter an embed url for a tile from Step 4 (starts with https://):
                    <input type="text" id="tb_EmbedURL" />
                        <%--<asp:TextBox ID="tb_EmbedURL" TextMode="SingleLine" runat="server" Wrap="false"></asp:TextBox></td></tr>--%>
                    <tr>
                        <td><%--<asp:Button ID="bEmbedTileAction" runat="server" Text="Embed Tile" />--%>
                            <input type="button" id="bEmbedTileAction" value="Embed Tile" />
                        </td>
                    </tr>
                <tr>
                    <td>
                        <iframe id="iFrameEmbedTile" src="" height="500px" width="500px" frameborder="0" seamless></iframe>
                    </td>
                </tr>
                <tr>
                    <%--<td><asp:TextBox ID="TextBox2" runat="server" Height="200px" Width="1024px" TextMode="MultiLine" Wrap="False"></asp:TextBox></td>--%>
                </tr>

            </table>
        </asp:Panel>


    </div>



</asp:Content>
