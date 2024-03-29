﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TreasureLand.App_Code;

namespace TreasureLand.Manager
{
    public partial class Inventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtPurchaseDate.Text = System.DateTime.Now.ToShortDateString();
            }
        }

        protected void btnAddLongTermAsset_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtLongTermName.Text == "")
                {
                    txtLongTermName.Text = "None";
                }

                if (txtLongTermLocation.Text == "")
                {
                    txtLongTermLocation.Text = "None";
                }

                LongTermAssetDB.AddTransaction(txtLongTermName.Text, txtLongTermLocation.Text, Convert.ToDecimal(txtCost.Text), cbLongTermInUse.Checked, Convert.ToDateTime(txtPurchaseDate.Text));
                gvLongTermAsset.DataBind();

                if (txtLongTermName.Text == "None")
                {
                    txtLongTermName.Text = "";
                }

                if (txtLongTermLocation.Text == "None")
                {
                    txtLongTermLocation.Text = "";
                }

                txtLongTermName.Text = "";

                txtLongTermLocation.Text = "";

                txtCost.Text = "";
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}