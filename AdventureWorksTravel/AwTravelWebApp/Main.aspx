<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="PBIWebApp.Main" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">

        <!-- Embed tiles / map -->
        <div class="col-md-6">
            <div class="panel panel-default shadow-z-2">
                <div class="panel-body" style="height:450px;">
                    
                    <asp:Button ID="signInButton" class="btn btn-primary" runat="server" OnClick="signInButton_Click" Text="Sign in to Power BI" />
                    
                    <asp:Panel ID="mapPanel" runat="server" Visible="false">
                        <iframe id="iFrameEmbedTile" src="" frameborder="0" style="border:0" allowfullscreen=""></iframe>
                    </asp:Panel>

                    <asp:Panel ID="signInStatus" runat="server" Visible="false">
                        <div class="form-group m-b-30">
                            <p>Signed in as</p>
                            <asp:Label ID="userLabel" runat="server"></asp:Label>
                        </div>
                        <div class="form-group m-b-30" style="visibility:hidden">
                            <p>Access Token</p>
                            <asp:TextBox ID="accessTokenTextbox" class="form-control" runat="server" Width="586px" ></asp:TextBox>
                        </div>
                        <div class="form-group m-b-30" style="visibility:hidden">
                            <p>iFrame URL</p>
                            <asp:TextBox ID="txtIFrameURL" class="form-control" runat="server" Width="586px" ClientIDMode="Static"></asp:TextBox>
                        </div>

                    </asp:Panel>

                </div>
            </div>
        </div>
        
        <div class="col-md-3">
            <%--<div class="panel panel-default no-bd">--%>
            <div class="panel panel-transparent no-bd">

                <div class="panel-body bg-white">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Panel ID="PanelPredictInputs" runat="server" Visible="true">
                                <div class="form-group m-b-30">
                                    <p>From</p>
                                    <asp:DropDownList ID="ddlOriginAirportCode" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="form-group m-b-30">
                                    <p>To</p>
                                    <asp:DropDownList ID="ddlDestAirportCode" class="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Date</p>
                                    <asp:TextBox runat="server" class="form-control" type="text" id="txtDepartureDate" value="11/10/2015" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Time</p>
                                    <asp:TextBox runat="server" class="form-control" type="text" id="txtDepartureHour" value="19" />
                                </div>
                                <div class="form-group m-b-30">
                                    <p>Carrier</p>
                                    <asp:TextBox runat="server" class="form-control" type="text" id="txtCarrier" value="DL" />
                                </div>
                                <asp:Button ID="btnPredictDelays" class="btn btn-primary" runat="server" Text="Predict Delay" OnClick="btnPredictDelays_Click" />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-3">
            <div class="panel panel-transparent no-bd">
                <div class="panel-body bg-white">
                    <asp:Panel ID="PanelPredictResults" runat="server" Visible="true">
                        <div class="row">
                            <div class="col-md-12">
                                <h3>Weather Forecast</h3>
                                <asp:Image ID="weatherForecast" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <h3>Delay Prediction</h3>
                                <div class="prediction prediction-mid shadow-z-1">
                                    <div id="predictionDelay">
                                        <p>
                                            <asp:Label ID="lblPrediction" runat="server" Text=""></asp:Label>
                                        </p>
                                        (<asp:Label ID="lblConfidence" runat="server" Text=""></asp:Label>% confidence)
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>

    </div>

</asp:Content>