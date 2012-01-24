﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TreasureLand.App_Code;
using System.Security;
using System.Web.Security;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using TreasureLand.DBM;

namespace TreasureLand.Clerk
{
    public partial class WebForm7 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnLocate.Focus();
            {
                if (!Page.IsPostBack)
                { }
               
            }
        }

        TreasureLandDataClassesDataContext db;

        #region eventhandlers
        /// <summary>
        /// When button is pressed, the textboxes are checked for value.
        /// If any of the textboxes are empty, default values are added.
        /// If all of the textboxes are empty, an error message is shown stating that
        /// at least one textbox must have data.
        /// When data has been properly entered, the gridview is populated with data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLocate_Click(object sender, EventArgs e)
        {

            if (txtFirstName.Text == "" && txtSurName.Text == "" && txtRoom.Text == "")
            {
                lblError.Text = "You must Enter information in at least one box";
            }
            else
            {
                //if there are not values entered, default values are added
                if (txtFirstName.Text == "")
                    txtFirstName.Text = "none";
                if (txtSurName.Text == "")
                    txtSurName.Text = "none";
                if (txtRoom.Text == "")
                    txtRoom.Text = "0";

                TreasureLandDataClassesDataContext db = new TreasureLandDataClassesDataContext();
                var guest = from g in db.Guests
                            join r in db.Reservations
                            on g.GuestID equals r.GuestID
                            join rd in db.ReservationDetails
                            on r.ReservationID equals rd.ReservationID
                            join ro in db.Rooms
                            on rd.RoomID equals ro.RoomID
                            where (ro.RoomNumbers == txtRoom.Text || g.GuestFirstName == txtFirstName.Text || g.GuestSurName == txtSurName.Text) && (r.ReservationStatus == 'A' || r.ReservationStatus == 'F')
                            select new { r.ReservationID, ro.RoomNumbers, g.GuestFirstName, g.GuestSurName, rd.ReservationDetailID, r.ReservationStatus, ro.RoomID };

                gvGuest.DataSource = guest.ToList();
                gvGuest.DataBind();


                //Gridview is populated with data
               
                //Clears the default values for the textboxes
                if (txtFirstName.Text == "none")
                    txtFirstName.Text = "";
                if (txtSurName.Text == "none")
                    txtSurName.Text = "";
                if (txtRoom.Text == "0")
                    txtRoom.Text = "";

                if (gvGuest.Rows.Count == 0)
                {
                    lblError.Text = "No data found";
                }
                else
                {
                    TreasureLand.DBM.BillingCategory billingCategory = new TreasureLand.DBM.BillingCategory();
                    IEnumerable<TreasureLand.DBM.BillingCategory> bill =
                                from g in db.BillingCategories
                                 select g;

                    ddlServices.DataSource = bill;
                    ddlServices.DataBind();

                    lblError.Text = "";
                }
            }
        }

        /// <summary>
        /// When the button is clicked, the gridview is checked to make sure that a customer is selected
        /// If nothing is selected, an error message is shown.
        /// If there is a selection, the view is changed and the textboxes are populated with the values from the textbox.
        /// The dropdown box is populated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            //checks for a selection in the gridview
            if (gvGuest.SelectedIndex > -1)
            {
                //switches to the next view
                mvViewGuest.ActiveViewIndex = 1;

                //Grabs the values from the gridview and populates the textboxes with the information
                txtShowReservation.Text = gvGuest.SelectedRow.Cells[0].Text;
                txtShowFirstName.Text = gvGuest.SelectedRow.Cells[1].Text;
                txtShowSurName.Text = gvGuest.SelectedRow.Cells[2].Text;
                txtShowRoom.Text = gvGuest.SelectedRow.Cells[6].Text;

                //gets the data for the drop down list
                TreasureLandDataClassesDataContext db = new TreasureLandDataClassesDataContext();
                TreasureLand.DBM.BillingCategory billingCategory = new TreasureLand.DBM.BillingCategory();
                IEnumerable<TreasureLand.DBM.BillingCategory> bill =
                            from g in db.BillingCategories
                            select g;
                ddlServices.DataSource = bill;
                ddlServices.DataTextField = "BillingCategoryDescription";
                ddlServices.DataValueField = "BillingCategoryID";
                ddlServices.DataBind();

                //gets the datasource for the guest room table
                //only grabs the room if it has an active status
                gvRoomCost.DataSource = GuestDB.getGuestRoom(Convert.ToInt32(txtShowRoom.Text), Convert.ToInt32(txtShowReservation.Text));
                gvRoomCost.DataBind();

                //gets the datasource for the services table 
                gvGuestServiesDataBind();

                updateGuestPriceTotals();

                //Shows the delete and edit button if the user is admin
                if (Roles.IsUserInRole("Admin") == true||Roles.IsUserInRole("Manager")==true)
                {
                    gvGuestServices.Columns[6].Visible = true;
                    gvGuestServices.Columns[7].Visible = true;
                    btnAdjustDiscount.Enabled = true;
                }
                //clears the error label
                lblError.Text = "";

                //disables the check out, add service, and adjust discount buttons if the reservation is not active
                if (gvGuest.SelectedRow.Cells[5].Text.ToString() != "A")
                {
                    btnAddService.Enabled = false;                    
                    btnGoToCheckOut.Enabled = false;
                }
                else
                {
                    btnAddService.Enabled = true;
                    
                    btnGoToCheckOut.Enabled = true;                }

                print();
            }
            else
            {
                //if no customer is selected, an error is shown
                lblError.Text = "You Must Select a customer before you can continue";
            }
        }

        /// <summary>
        /// When a guess is selected, the background is changed to yellow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGuest_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnNext.Visible = true;
            gvGuest.SelectedRow.BackColor = System.Drawing.Color.Yellow;
        }

        /// <summary>
        /// When the selected guest is changed, it changes the background color back to white
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGuest_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            if (gvGuest.SelectedIndex > -1)
            {
                gvGuest.SelectedRow.BackColor = System.Drawing.Color.White;
            }
        }

        /// <summary>
        /// Adds the service charge to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddService_Click(object sender, EventArgs e)
        {
            //clears the data from the gridview
            gvGuestServices.Dispose();

            //adds the entered service into the database
            db = new TreasureLandDataClassesDataContext();
            ReservationDetailBilling bill = new ReservationDetailBilling();
            bill.ReservationDetailID = Convert.ToInt16(gvGuest.SelectedRow.Cells[4].Text);
            bill.BillingAmount = Convert.ToDecimal(txtCostofService.Text);
            bill.BillingItemQty = Convert.ToByte(ddlQuantity.SelectedValue);
            bill.BillingCategoryID = Convert.ToByte(ddlServices.SelectedValue);
            bill.BillingDescription = ddlServices.SelectedItem.ToString();
            bill.BillingItemDate = DateTime.Now;
            bill.Comments = txtComments.Text.ToString();
            db.ReservationDetailBillings.InsertOnSubmit(bill);
            db.SubmitChanges();
            txtComments.Text = "";
            
            //Retrieves all services for the seleceted reservation detail ID and rebinds the data to the gridview
            gvGuestServiesDataBind();
            updateGuestPriceTotals();
        }

        /// <summary>
        /// used to return to the first page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            mvViewGuest.ActiveViewIndex = 0;
        }

        /// <summary>
        /// edits a row in the gridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGuestServices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvGuestServices.EditIndex = e.NewEditIndex;
            gvGuestServiesDataBind();
        }

        /// <summary>
        /// Updates the gridview row
        /// </summary>
        protected void gvGuestServices_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {   
            try
            {
                //Gets the values from the selected row and sends an update call to the database
                //Data is rebinded to gridview and the totals are updated
                    GridViewRow row = gvGuestServices.Rows[e.RowIndex];

                    TreasureLandDataClassesDataContext db = new TreasureLandDataClassesDataContext();
                    ReservationDetailBilling bill = new ReservationDetailBilling();
                    var query = from bills in db.ReservationDetailBillings
                                where bills.ReservationBillingID == (short)Convert.ToInt32((row.FindControl("lblTransactionID") as Label).Text)
                                select bills;
                    foreach (var bills in query)
                    {
                        bills.ReservationBillingID = (short)Convert.ToInt32((row.FindControl("lblTransactionID") as Label).Text);
                        bills.BillingItemQty = (byte)Convert.ToInt32((row.FindControl("txtQty") as TextBox).Text);
                        bills.BillingAmount = Convert.ToDecimal((row.FindControl("txtPrice") as TextBox).Text);
                        if ((row.FindControl("txtComments") as TextBox) != null)
                        {
                            bills.Comments = (row.FindControl("txtComments") as TextBox).Text;
                        }
                    }
                    db.SubmitChanges();                
                
                    gvGuestServices.EditIndex = -1;
                    gvGuestServiesDataBind();
                    updateGuestPriceTotals();                
            }
        catch (Exception)
            {
                lblErrorGuest.Text = "Cannot update information, insertion failed";
            }

        }

        /// <summary>
        /// Deletes the selected row from the database then rebinds the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGuestServices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            try
            {
                GridViewRow row = gvGuestServices.Rows[e.RowIndex];
                int transactionID = Convert.ToInt32((row.FindControl("lblTransactionID") as Label).Text);
                App_Code.GuestDB.deleteService(transactionID);

                gvGuestServiesDataBind();
                updateGuestPriceTotals();
            }
            catch (Exception)
            {
                lblErrorGuest.Text = "Deletion Failed";
            }
        }

        /// <summary>
        /// Cancels the editing of a row then rebinds the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvGuestServices_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvGuestServices.EditIndex = -1;
            gvGuestServiesDataBind();
        }

        protected void gvGuestServices_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            // Indicate whether the update operation succeeded.
            if (e.Exception == null)
            {
                lblErrorGuest.Text = "Row updated successfully.";
            }
            else
            {
                e.ExceptionHandled = true;
                lblErrorGuest.Text = "An error occurred while attempting to update the row.";
            }

            
        }
        #endregion

        #region methods
        /// <summary>
        /// binds the guest services gridview
        /// </summary>
        private void gvGuestServiesDataBind()
        {
            TreasureLandDataClassesDataContext db = new TreasureLandDataClassesDataContext();
            TreasureLand.DBM.ReservationDetailBilling guests = new TreasureLand.DBM.ReservationDetailBilling();
            IEnumerable<TreasureLand.DBM.ReservationDetailBilling> guest =
                        from g in db.ReservationDetailBillings
                        where g.ReservationDetailID == Convert.ToInt32(gvGuest.SelectedRow.Cells[4].Text)
                        select g;

            gvGuestServices.DataSource = guest;
            gvGuestServices.DataBind();
        }


        /// <summary>
        /// Updates the textboxes that diplay the sub total and the total price
        /// of the room and services
        /// </summary>
        private void updateGuestPriceTotals()
        {
            //makes a call to the database and totals all the selected guests cost of services
            txtServicesTotal.Text = string.Format("{0:0.00}",(GuestDB.getTotal(Convert.ToDecimal(gvGuest.SelectedRow.Cells[4].Text)).ToString()));

            //get the cost of the room
            txtRoomTotal.Text = (Convert.ToDecimal(gvRoomCost.Rows[0].Cells[2].Text)*Convert.ToDecimal(gvRoomCost.Rows[0].Cells[1].Text)).ToString();
            
            //get the discount
            ArrayList myArrList = new ArrayList();
            myArrList = App_Code.GuestDB.getGuestDiscount(Convert.ToInt32(gvGuest.SelectedRow.Cells[4].Text));
            //if there are no items in the arrayList then there is no discount
            if (myArrList.Count == 0)
            {
                txtDiscount.Text = "0";
            }
            else
            {
                //if this is true, the discount is a percent, otherwise it is a flat cost discount
                if (Convert.ToBoolean(myArrList[2]) == true)
                {
                    txtDiscount.Text = (((Convert.ToDecimal(txtServicesTotal.Text) + Convert.ToDecimal(txtRoomTotal.Text)) * (Convert.ToDecimal(myArrList[1]) / 100))).ToString();
                }
                else
                {
                    txtDiscount.Text = (Convert.ToDecimal(myArrList[1]).ToString());
                }
            }
            txtDiscount.Text = string.Format("{0:0.00}", Convert.ToDouble(txtDiscount.Text));
            
            //Displays the total cost
            txtTotal.Text = (Convert.ToDecimal(txtServicesTotal.Text) + Convert.ToDecimal(txtRoomTotal.Text) - Convert.ToDecimal(txtDiscount.Text)).ToString();
            print();
        }
        #endregion

        /// <summary>
        /// Checks out the guest.  Changes the ReservationDetail status, Reservation status and updateStatus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GoToCheckOut_Click(object sender, EventArgs e)
        {
            GuestDB.updateReservationDetail('F', Convert.ToInt32(gvGuest.SelectedRow.Cells[4].Text));

            if (App_Code.GuestDB.countActiveReservationDetail(Convert.ToInt32(gvGuest.SelectedRow.Cells[0].Text)) == 0)
            {
                App_Code.GuestDB.updateReservationStatus('F', Convert.ToInt32(gvGuest.SelectedRow.Cells[0].Text));
            }

                
            
                GuestDB.updateRoomStatus('H', Convert.ToInt32(txtShowRoom.Text));
                mvViewGuest.ActiveViewIndex = 2;
        }

        /// <summary>
        /// goes back to the choose guest page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBack_Click(object sender, EventArgs e)
        {
            mvViewGuest.ActiveViewIndex = 0;
        }

        /// <summary>
        /// When clicked all tables are hidden along with the add service feature.
        /// The aadjust discount is made visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdjustDiscount_Click(object sender, EventArgs e)
        {
            lblManagerUser.Visible = true;
            lblPassword.Visible = true;

            ddlDiscount.Visible = true;
            txtMangerUname0.Visible = true;
            txtManagerPword0.Visible = true;
            btnApply0.Visible = true;
            gvGuestServices.Visible = false;
            gvRoomCost.Visible = false;
            lblServies.Visible = false;
            lblQty.Visible = false;
            lblCost.Visible = false;
            ddlQuantity.Visible = false;
            ddlServices.Visible = false;
            btnAddService.Visible = false;
            txtCostofService.Visible = false;
            lblComments.Visible = false;
            txtComments.Visible = false;
            ddlDiscount.Items.Clear();
            SqlDataReader sqlDiscounts = sqlDiscounts = (SqlDataReader)App_Code.GuestDB.getAllDiscounts();
   
            while (sqlDiscounts.Read())
            {
                App_Code.Discount discount = new App_Code.Discount();
                

                discount.ID = Convert.ToInt32(sqlDiscounts["DiscountID"]);
                discount.description = (sqlDiscounts["DiscountDescription"]).ToString();
                discount.amountOfDiscount = Convert.ToDouble(sqlDiscounts["DiscountAmount"].ToString());
                discount.isPercent = Convert.ToBoolean(sqlDiscounts["IsPrecentage"]);
                ddlDiscount.Items.Add(new ListItem(discount.ToString(), discount.ID.ToString()));
                
            }
            
            ddlDiscount.DataBind();
            ddlDiscount.Items.Insert(0, new ListItem("No Discount", "-1"));
        }

        /// <summary>
        /// Makes the adjust discount fields invisible and
        /// makes everything else visible
        /// Sets the discount to the reservation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApply0_Click(object sender, EventArgs e)
        {
            if (Roles.IsUserInRole(txtMangerUname0.Text, "Manager") && Membership.ValidateUser(txtMangerUname0.Text, txtManagerPword0.Text))
            {
                lblManagerUser.Visible = false;
                lblPassword.Visible = false;
                ddlDiscount.Visible = false;
                txtMangerUname0.Visible = false;
                txtManagerPword0.Visible = false;
                btnApply0.Visible = false;
                gvGuestServices.Visible = true;
                gvRoomCost.Visible = true;

                lblServies.Visible = true;
                lblQty.Visible = true;
                lblCost.Visible = true;
                ddlQuantity.Visible = true;
                ddlServices.Visible = true;
                btnAddService.Visible = true;
                txtCostofService.Visible = true;
                lblComments.Visible = true;
                txtComments.Visible = true;

                try
                {
                    if ((Convert.ToInt32((ddlDiscount.SelectedItem.Value)) == -1))
                    {
                        lblErrorMessage.Text = "";
                    }
                    else
                    {
                        GuestDB.addDiscount(Convert.ToInt32(ddlDiscount.SelectedItem.Value), Convert.ToInt32(gvGuest.SelectedRow.Cells[4].Text));
                        updateGuestPriceTotals();
                        lblErrorMessage.Text = "";
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                lblErrorMessage.Text = "Must have a correct manager login";
            }
        }

        private void print()
        {
            if (Session["GuestInfo"] != null)
            {
                Session.Remove("GuestInfo");
                Session.Remove("RoomInfo");
                Session.Remove("Charges");
            }
            
            List<string> roomInfo = new List<string>();
            roomInfo.Add(gvGuest.SelectedRow.Cells[0].Text);
            roomInfo.Add(gvGuest.SelectedRow.Cells[1].Text);
            roomInfo.Add(gvGuest.SelectedRow.Cells[2].Text);
            roomInfo.Add(gvGuest.SelectedRow.Cells[6].Text);
            Session["GuestInfo"] = roomInfo;
            //Session["RoomInfo"] = (DataSet)gvRoomCost.DataSource;
            Session["Charges"] = gvGuest.SelectedRow.Cells[4].Text; 
        }
    }
}