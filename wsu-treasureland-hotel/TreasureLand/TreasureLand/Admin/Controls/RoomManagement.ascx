﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="~/Admin/Controls/RoomManagement.ascx.cs"
    Inherits="TreasureLand.Admin.Controls.RoomManagement" %>
<style type="text/css">
    .style1
    {
        height: 24px;
    }
</style>
<h1>
    Room Manager
</h1>
<asp:MultiView ID="MultiView_Rooms" runat="server" ActiveViewIndex="0">
    <asp:View ID="View_RoomsMain" runat="server">
        <asp:GridView ID="GridView_Rooms" runat="server" AllowPaging="True" AllowSorting="True"
            PageSize="5" AutoGenerateColumns="False" DataSourceID="LinqDataSource_Rooms"
            BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px"
            CellPadding="4" ForeColor="Black" GridLines="Vertical" DataKeyNames="RoomID"
            OnSelectedIndexChanged="GridView_Rooms_SelectedIndexChanged" 
            ShowFooter="True" onrowediting="GridView_Rooms_RowEditing" 
            onrowupdated="GridView_Rooms_RowUpdated" 
            onrowupdating="GridView_Rooms_RowUpdating" 
            onrowdeleting="GridView_Rooms_RowDeleting">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Room Number" SortExpression="RoomNumbers"><EditItemTemplate><asp:Label ID="Lable_RoomNumbers" runat="server" 
                            Text='<%# Bind("RoomNumbers") %>'></asp:Label></EditItemTemplate><ItemTemplate><asp:Label ID="Label_RoomNumber" runat="server" Text='<%# Bind("RoomNumbers") %>'></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Description" SortExpression="RoomDescription"><EditItemTemplate><asp:TextBox ID="TextBox_Description" runat="server" 
                            Text='<%# Bind("RoomDescription") %>' MaxLength="200"></asp:TextBox></EditItemTemplate><ItemTemplate><asp:Label ID="Label1" runat="server" Text='<%# Bind("RoomDescription") %>'></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Bed Configuration" SortExpression="RoomBedConfiguration"><EditItemTemplate><asp:TextBox ID="TextBox_RoomBedConfig" runat="server" 
                            Text='<%# Bind("RoomBedConfiguration") %>' MaxLength="30"></asp:TextBox><asp:RequiredFieldValidator 
                        ID="RFV_BedConfig" runat="server" ControlToValidate="TextBox_RoomBedConfig" SetFocusOnError="true"
                            ForeColor="Red" ErrorMessage="*">*</asp:RequiredFieldValidator></EditItemTemplate><ItemTemplate><asp:Label ID="Label_BedConfig" runat="server" Text='<%# Bind("RoomBedConfiguration") %>'></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Status" SortExpression="RoomStatus"><EditItemTemplate><asp:DropDownList ID="DropDownList1" runat="server" 
                            DataSourceID="LinqDataSource_Statuses" DataTextField="RoomStatusDescription" 
                            DataValueField="RoomStatus1" SelectedValue='<%# Bind("RoomStatus") %>'></asp:DropDownList></EditItemTemplate><ItemTemplate><asp:Label ID="Label_Status" runat="server" Text='<%# Bind("RoomStatus") %>'></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:CommandField ShowEditButton="True" ButtonType="Button" 
                    ShowDeleteButton="True" />
            </Columns>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
        <asp:Button runat="server" ID="Button_AddRoom" Text="New Room" OnClick="Button_AddRoom_Click" />
        <asp:Button runat="server" ID="Button_DeleteRoom" Text="Delete Room" 
            Enabled="false" onclick="Button_DeleteRoom_Click" />
        <br />
        
    </asp:View>
    <asp:View runat="server" ID="View_AddRoom" OnActivate="View_AddRoom_Activate">
        <h3>
            Enter New Room Details</h3>
        <table>
            <tr>
                <td>
                    Room Number
                </td>
                <td align="right">
                    <asp:TextBox ID="TextBox_RoomNumber" runat="server" 
                        ValidationGroup="VG_NewRoomData" MaxLength="5"></asp:TextBox>
                </td>
                <td colspan="2">
                    <asp:CheckBox ID="CheckBox_Smoking" runat="server" Text="Smoking" />
                    &nbsp;
                </td>
                <td>
                    <asp:CheckBox ID="CheckBox_Accessible" runat="server" Text="Accessible?" />
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RFV_RoomNumber" runat="server" ForeColor="Red" ErrorMessage="Please enter a room number."
                        SetFocusOnError="True" ControlToValidate="TextBox_RoomNumber" ValidationGroup="VG_NewRoomData"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    Type
                </td>
                <td>
                    <!--   <asp:DropDownList ID="DropDownList_RoomTypes" runat="server" DataSourceID="LinqDataSource_RoomTypes"
                        DataTextField="RoomType" DataValueField="HotelRoomTypeID" 
                        ValidationGroup="VG_NewRoomData">
                    </asp:DropDownList>-->
                    <asp:DropDownList ID="ddlRoomTypes" runat="server" 
                        DataSourceID="LinqDataSource_RoomTypes" DataTextField="RoomType" 
                        DataValueField="HotelRoomTypeID">
                    </asp:DropDownList>
                </td>
                <td colspan="2">
                    Status
                </td>
                <td>
                    <!--
                    <asp:DropDownList ID="DropDownList_RoomStatuses" runat="server" DataSourceID="LinqDataSource_Statuses"
                        DataTextField="RoomStatusDescription" DataValueField="RoomStatus1" 
                        ValidationGroup="VG_NewRoomData">
                    </asp:DropDownList>-->
                    <asp:DropDownList ID="ddlStatus" runat="server" 
                        DataSourceID="LinqDataSource_Statuses" DataTextField="RoomStatusDescription" 
                        DataValueField="RoomStatus1">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="6" class="style1">
                    Bed Configuration
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:TextBox ID="TextBox_BedConfig" runat="server" Width="95%" 
                        ValidationGroup="VG_NewRoomData" MaxLength="30"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_BedConfig" runat="server" ControlToValidate="TextBox_BedConfig"
                        ErrorMessage="Please enter a bed configuration." ForeColor="Red" SetFocusOnError="True"
                        ValidationGroup="VG_NewRoomData"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    Description
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:TextBox ID="TextBox_Description" runat="server" Width="95%" 
                        ValidationGroup="VG_NewRoomData" MaxLength="200"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_BedConfig0" runat="server" ControlToValidate="TextBox_Description"
                        ErrorMessage="Please enter a description." ForeColor="Red" SetFocusOnError="True"
                        ValidationGroup="VG_NewRoomData"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="right">
                    <asp:Button ID="btnBack" runat="server" CommandArgument="0" 
                        CommandName="SwitchViewByIndex" onclick="btnBack_Click" Text="Back" />
                </td>
                <td align="right" colspan="3">
                    <asp:Button ID="Room_Add" runat="server" CommandName="Insert" 
                        OnClick="Room_Add_Click" Text="Add" ValidationGroup="VG_NewRoomData" />
                </td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>
<asp:Panel ID="Panel_DataSourceBox" runat="server" Visible="false">    
    <asp:LinqDataSource ID="LinqDataSource_Rooms" runat="server" ContextTypeName="TreasureLand.DBM.TreasureLandDataClassesDataContext"
        TableName="Rooms" EnableDelete="True" EnableInsert="True" 
        EnableUpdate="True" onupdated="LinqDataSource_Rooms_Updated" 
        onupdating="LinqDataSource_Rooms_Updating">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="LinqDataSource_RoomTypes" runat="server" ContextTypeName="TreasureLand.DBM.TreasureLandDataClassesDataContext"
        EntityTypeName="" TableName="HotelRoomTypes">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="LinqDataSource_Statuses" runat="server" ContextTypeName="TreasureLand.DBM.TreasureLandDataClassesDataContext"
        EntityTypeName="" TableName="RoomStatus">
    </asp:LinqDataSource>
</asp:Panel>
<asp:Label ID="Label_StatusMsg" runat="server" ForeColor="Red" Font-Size="Large" />
