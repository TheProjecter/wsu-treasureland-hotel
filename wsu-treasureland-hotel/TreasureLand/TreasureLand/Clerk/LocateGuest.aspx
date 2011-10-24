﻿<%@ Page Title="Locate Guest" Language="C#" MasterPageFile="~/Clerk/ClerkMasterPage.master" AutoEventWireup="true" CodeBehind="LocateGuest.aspx.cs" Inherits="TreasureLand.Clerk.WebForm4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:MultiView ID="mvLocateGuest" runat="server">
        <asp:View ID="vLocateGuest" runat="server">
            <table class="style1">
                <tr>
                    <td class="style18">
                        Reservation Number:</td>
                    <td class="style24">
                        <asp:TextBox ID="txtReservationNum" runat="server"></asp:TextBox>
                    </td>
                    <td class="style12">
                        First Name:</td>
                    <td class="style22">
                        <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                    </td>
                    <td class="style13">
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" 
                            Text="You must enter a first/surname/email"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style19">
                        Room Number:</td>
                    <td class="style25">
                        <asp:TextBox ID="txtRoomNum" runat="server"></asp:TextBox>
                    </td>
                    <td class="style4">
                        Surname:</td>
                    <td class="style23">
                        <asp:TextBox ID="txtSurname" runat="server"></asp:TextBox>
                    </td>
                    <td class="style5">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="style19">
                        <asp:Button ID="btnLocateGuest" runat="server" onclick="btnLocateGuest_Click" 
                            Text="Locate Guest" ValidationGroup="lookup" />
                    </td>
                    <td class="style25">
                        &nbsp;</td>
                    <td class="style4">
                        Email:</td>
                    <td class="style23">
                        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                    </td>
                    <td class="style5">
                        &nbsp;</td>
                </tr>
            </table>
            <br />
            CHECK IN:<asp:GridView ID="gvGuest" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="ReservationID" DataSourceID="odsLocateGuest">
                <Columns>
                    <asp:BoundField DataField="ReservationID" HeaderText="ReservationID" 
                        InsertVisible="False" ReadOnly="True" SortExpression="ReservationID" />
                    <asp:BoundField DataField="GuestID" HeaderText="GuestID" 
                        SortExpression="GuestID" />
                    <asp:CommandField ButtonType="Button" ShowSelectButton="True" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="odsLocateGuest" runat="server" 
                ConnectionString="<%$ ConnectionStrings:TreasureLandDB %>" 
                SelectCommand="SELECT [ReservationID], [GuestID] FROM [Reservation]">
            </asp:SqlDataSource>
            <br />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnSelectGuest" runat="server" onclick="btnSelectGuest_Click" 
                Text="Select Guest" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" 
                Text="You must select a guest"></asp:Label>
        </asp:View>
        <asp:View ID="vCheckInGuest" runat="server">
            <table style="width: 78%">
                <tr>
                    <td style="width: 133px">
                        Reservation Number:</td>
                    <td class="style1" style="width: 156px">
                        <asp:TextBox ID="txtReservationNum1" runat="server"></asp:TextBox>
                    </td>
                    <td style="width: 99px">
                        First Name:</td>
                    <td style="width: 134px">
                        <asp:TextBox ID="txtFirstName0" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 133px">
                        Room Type:</td>
                    <td class="style1" style="width: 156px">
                        <asp:TextBox ID="txtRoomType" runat="server"></asp:TextBox>
                    </td>
                    <td style="width: 99px">
                        Surname:</td>
                    <td style="width: 134px">
                        <asp:TextBox ID="txtSurname0" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 133px">
                        Room Number:</td>
                    <td class="style1" style="width: 156px">
                        <asp:TextBox ID="txtRoomNumber" runat="server"></asp:TextBox>
                    </td>
                    <td style="width: 99px">
                        Phone Number:</td>
                    <td style="width: 134px">
                        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 133px">
                        Check out:</td>
                    <td class="style1" style="width: 156px">
                        <asp:TextBox ID="txtCheckOut" runat="server"></asp:TextBox>
                    </td>
                    <td style="width: 99px">
                        Email:</td>
                    <td style="width: 134px">
                        <asp:TextBox ID="txtEmail0" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 133px">
                        Number of Guests</td>
                    <td class="style1" style="width: 156px">
                        <asp:TextBox ID="txtNumGuests" runat="server"></asp:TextBox>
                    </td>
                    <td style="width: 99px">
                        &nbsp;</td>
                    <td style="width: 134px">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 133px">
                        &nbsp;</td>
                    <td class="style1" style="width: 156px">
                        &nbsp;</td>
                    <td style="width: 99px">
                        &nbsp;</td>
                    <td style="width: 134px">
                        &nbsp;</td>
                </tr>
            </table>
            <br />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnBack" runat="server" Text="Back" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCheckIn" runat="server" Text="Check in" />
        </asp:View>
    </asp:MultiView>
    <br />
    <br />
    </asp:Content>
