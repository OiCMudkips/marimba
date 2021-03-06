﻿namespace Marimba.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Media;

    using Marimba.Utility;

    public partial class SignUpForm : Form
    {
        int iSignup;
        //these three are used for multiple instruments
        private ListView lvInstruments;
        private ColumnHeader FName;
        List<ListViewItem> instrumentList;
        //this is for the "other" instrument category
        private TextBox txtOtherInstrument;
        private Label lblOtherInstrument;
        bool bOtherInstrument;
        public SignUpForm()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            bool bMissingInformation = false;
            Color defaultInfoColour = Color.FromName("ControlText");
            Color badInfoColour = Color.FromName("red");
            bool bIsUWStudent = cbClass.Text.Contains("UW") && cbClass.Text.Contains("Student");

            try
            {
                // mark missing information in red or restore added information to default
                if (txtFirstName.Text == "" || txtLastName.Text == "" || txtEmail.Text == "" ||
                    (bIsUWStudent && (txtStudentNumber.Text == "" || cbFaculty.Text == "")) ||
                    cbInstrument.Text == "" || (bOtherInstrument && txtOtherInstrument.Text == ""))
                {
                    if (txtFirstName.Text == "")
                        lblFirstName.ForeColor = badInfoColour;
                    else
                        lblFirstName.ForeColor = defaultInfoColour;

                    if (txtLastName.Text == "")
                        lblLastName.ForeColor = badInfoColour;
                    else
                        lblLastName.ForeColor = defaultInfoColour;

                    if (txtEmail.Text == "")
                        lblEmail.ForeColor = badInfoColour;
                    else
                        lblEmail.ForeColor = defaultInfoColour;

                    if (bIsUWStudent)
                    {
                        if (txtStudentNumber.Text == "")
                            lblStudentNumber.ForeColor = badInfoColour;
                        else
                            lblStudentNumber.ForeColor = defaultInfoColour;

                        if (cbFaculty.Text == "")
                            lblFaculty.ForeColor = badInfoColour;
                        else
                            lblFaculty.ForeColor = defaultInfoColour;
                    }
                    else
                    {
                        lblStudentNumber.ForeColor = defaultInfoColour;
                        lblFaculty.ForeColor = defaultInfoColour;
                    }

                    if (cbInstrument.Text == "")
                        lblInstrument.ForeColor = badInfoColour;
                    else
                        lblInstrument.ForeColor = defaultInfoColour;

                    if (bOtherInstrument && txtOtherInstrument.Text == "")
                        lblOtherInstrument.ForeColor = badInfoColour;
                    else if (bOtherInstrument)
                        lblOtherInstrument.ForeColor = defaultInfoColour;

                    bMissingInformation = true;
                }

                //check for missing information
                if (bMissingInformation)
                {
                    if (Properties.Settings.Default.playSounds)
                        Sound.Error.Play();
                    MessageBox.Show("Missing information. Please review the highlighted fields.");
                }
                else if ((bIsUWStudent) && (txtStudentNumber.Text.Trim().Length != 8))
                {
                    if (Properties.Settings.Default.playSounds)
                        Sound.Error.Play();
                    throw new FormatException();                    
                }
                else
                {
                    //prepare to provide information on what the "other" instrument is
                    string tempOtherInstrument;
                    if (bOtherInstrument)
                        tempOtherInstrument = txtOtherInstrument.Text;
                    else
                        tempOtherInstrument = "";
                    bool[] tempPlays = null;

                    //this section is only for handling members who have signed up with multiple instruments
                    if (lvInstruments != null)
                    {
                        //prepare to make a list of all the instruments the member plays
                        int iNumberofInstruments = Enum.GetValues(typeof(Member.Instrument)).Length;
                        tempPlays = new bool[iNumberofInstruments];
                        for (int i = 0; i < iNumberofInstruments; i++)
                            tempPlays[(int)Member.ParseInstrument(lvInstruments.Items[i].SubItems[0].Text)] = lvInstruments.Items[i].Checked;
                    }

                    //no missing info, then add the member!
                    if (!bIsUWStudent)
                            txtStudentNumber.Text = "0";
                    // remove whitespace
                    else
                        txtStudentNumber.Text = txtStudentNumber.Text.Trim();

                    if (cbMultiple.Checked && ClsStorage.currentClub.AddMember(txtFirstName.Text, txtLastName.Text, (Member.MemberType)cbClass.SelectedIndex,
                        Convert.ToUInt32(txtStudentNumber.Text), cbFaculty.SelectedIndex, cbInstrument.Text, tempOtherInstrument, txtEmail.Text,
                        txtOther.Text, cbShirtSize.SelectedIndex, tempPlays) || ClsStorage.currentClub.AddMember(txtFirstName.Text, txtLastName.Text, (Member.MemberType)cbClass.SelectedIndex,
                        Convert.ToUInt32(txtStudentNumber.Text), cbFaculty.SelectedIndex, cbInstrument.Text, tempOtherInstrument, txtEmail.Text,
                        txtOther.Text, cbShirtSize.SelectedIndex))
                    {
                        if (Properties.Settings.Default.playSounds)
                            Sound.Welcome.Play();
                        MessageBox.Show("Sign-Up Successful!");
                        iSignup++;
                        cleanup();
                    }
                    else //as of writing this comment, this cannot actually fail yet
                    {
                        if (Properties.Settings.Default.playSounds)
                            Sound.Success.Play();
                        MessageBox.Show("Sign-Up was not completed. Our records show you are already signed up.");
                    }

                    // restore text colour
                    foreach (Label l in this.Controls.OfType<Label>())
                    {
                        l.ForeColor = defaultInfoColour;
                    }
                }
            }
            catch (FormatException)
            {
                if (Properties.Settings.Default.playSounds)
                    Sound.Error.Play();

                MessageBox.Show("Bad input. Ensure that the student number is valid.");
                lblStudentNumber.ForeColor = badInfoColour;
            }
            catch (Exception)
            {
                if (Properties.Settings.Default.playSounds)
                    Sound.Error.Play();
                MessageBox.Show("Bad input. Please review the information.");
            }
        }

        private void cleanup()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtStudentNumber.Text = "";
            txtOther.Text = "";
            //UW Undergrad Student is the default
            cbClass.SelectedIndex = 0;
            cbFaculty.Text = "";
            cbInstrument.Text = "";
            txtFirstName.Focus();
            cbMultiple.Checked = false;
            cbShirtSize.Text = "";
            if (lvInstruments != null)
            {
                lvInstruments.Dispose();
                lvInstruments = null;
            }
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            //this is used for history keeping purposes
            iSignup = 0;
            //set default member type to UW Student
            cbClass.SelectedIndex = 0;

            //add the instruments to the combo box
            cbInstrument.BeginUpdate();
            List<string> listInstruments = new List<string>();
            foreach (Member.Instrument instrument in Enum.GetValues(typeof(Member.Instrument)))
                listInstruments.Add(Member.instrumentToString(instrument));
            listInstruments.Sort();
            cbInstrument.Items.AddRange(listInstruments.ToArray());
            cbInstrument.EndUpdate();
        }

        private void SignUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            //store history of signup
            if (iSignup > 0)
                ClsStorage.currentClub.AddHistory(Convert.ToString(iSignup), ChangeType.Signup);
        }

        private void cbMultiple_CheckedChanged(object sender, EventArgs e)
        {
            //if it is checked, then we need to add the listview to select additional instruments
            //also, resize the form and move the other controls
            //change tabindexes
            if(cbMultiple.Checked)
            {
                this.Height += 248;
                //create listview with instruments
                lvInstruments = new System.Windows.Forms.ListView();
                lvInstruments.CheckBoxes = true;
                FName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
                FName.Width = 350;
                lvInstruments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {FName});
                lvInstruments.Font = new System.Drawing.Font("Quicksand", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lvInstruments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
                lvInstruments.Location = new System.Drawing.Point(49, 350);
                lvInstruments.Name = "lvMembers";
                lvInstruments.Size = new System.Drawing.Size(489, 248);
                lvInstruments.Sorting = System.Windows.Forms.SortOrder.Ascending;
                lvInstruments.TabIndex = 7;
                lvInstruments.UseCompatibleStateImageBehavior = false;
                lvInstruments.View = System.Windows.Forms.View.Details;
                lvInstruments.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvInstruments_MouseClick);
                //this sets up the actual list
                lvInstruments.SmallImageList = Program.home.instrumentSmall;
                instrumentList = new List<ListViewItem>();
                lvInstruments.Items.Clear();
                instrumentList.Clear();
                foreach (Member.Instrument instrument in Enum.GetValues(typeof(Member.Instrument)))
                    instrumentList.Add(new ListViewItem(Member.instrumentToString(instrument), Member.instrumentIconIndex(instrument)));
                lvInstruments.Items.AddRange(instrumentList.ToArray());
                this.Controls.Add(lvInstruments);
                this.ResumeLayout(false);
                this.PerformLayout();
                if (cbInstrument.SelectedIndex != -1)
                    lvInstruments.Items[cbInstrument.SelectedIndex].Checked = true;
                //adding this command must be done last
                lvInstruments.ItemChecked += lvInstruments_ItemChecked;
             
                //move everything else
                if(bOtherInstrument)
                {
                    txtOtherInstrument.TabIndex++;
                    lblOtherInstrument.Location = new Point(lblOtherInstrument.Location.X, lblOtherInstrument.Location.Y + 248);
                    txtOtherInstrument.Location = new Point(txtOtherInstrument.Location.X, txtOtherInstrument.Location.Y + 248);
                }
                lblEmail.Location = new Point(lblEmail.Location.X, lblEmail.Location.Y + 248);
                txtEmail.Location = new Point(txtEmail.Location.X, txtEmail.Location.Y + 248);
                txtEmail.TabIndex++;
                lblOther.Location = new Point(lblOther.Location.X, lblOther.Location.Y + 248);
                txtOther.Location = new Point(txtOther.Location.X, txtOther.Location.Y + 248);
                txtOther.TabIndex++;
                lblShirtSize.Location = new Point(lblShirtSize.Location.X, lblShirtSize.Location.Y + 248);
                cbShirtSize.Location = new Point(cbShirtSize.Location.X, cbShirtSize.Location.Y + 248);
                cbShirtSize.TabIndex++;
                btnSignUp.Location = new Point(btnSignUp.Location.X, btnSignUp.Location.Y + 248);
                btnSignUp.TabIndex++;

            }
            else
            {
                //set the other item to be unchecked
                if (cbInstrument.SelectedIndex != 17)
                    lvInstruments.Items[17].Checked = false;

                this.Height -= 248;

                //move everything back
                if (bOtherInstrument)
                {
                    lblOtherInstrument.Location = new Point(lblOtherInstrument.Location.X, lblOtherInstrument.Location.Y - 248);
                    txtOtherInstrument.Location = new Point(txtOtherInstrument.Location.X, txtOtherInstrument.Location.Y - 248);
                    txtOtherInstrument.TabIndex--;
                }
                lblEmail.Location = new Point(lblEmail.Location.X, lblEmail.Location.Y - 248);
                txtEmail.Location = new Point(txtEmail.Location.X, txtEmail.Location.Y - 248);
                txtEmail.TabIndex--;
                lblOther.Location = new Point(lblOther.Location.X, lblOther.Location.Y - 248);
                txtOther.Location = new Point(txtOther.Location.X, txtOther.Location.Y - 248);
                txtOther.TabIndex--;
                lblShirtSize.Location = new Point(lblShirtSize.Location.X, lblShirtSize.Location.Y - 248);
                cbShirtSize.Location = new Point(cbShirtSize.Location.X, cbShirtSize.Location.Y - 248);
                cbShirtSize.TabIndex--;
                btnSignUp.Location = new Point(btnSignUp.Location.X, btnSignUp.Location.Y - 248);
                btnSignUp.TabIndex--;
                lvInstruments.Dispose();
                lvInstruments = null;
            }
        }

        void lvInstruments_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //just make sure the user's main instrument can't be unselected
            if(cbInstrument.SelectedIndex != -1)
                lvInstruments.Items[cbInstrument.SelectedIndex].Checked = true;

            if(e.Item.SubItems[0].Text == "Other")
            {
                //upon checking other, add the other instrument field and move everything else down
                if (e.Item.Checked && !bOtherInstrument)
                    showOther();
                //if deselected, move everything back
                else if (!e.Item.Checked && bOtherInstrument)
                    hideOther();
            }
        }

        private void lvInstruments_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvInstruments.SelectedItems.Count != 0)
            {
                lvInstruments.SelectedItems[0].Checked = !lvInstruments.SelectedItems[0].Checked;
                //lose focus of this item
                lvInstruments.Items[lvInstruments.SelectedItems[0].Index].Selected = false;
            }
            //do not allow the member to unselect their main instrument
            if(cbInstrument.SelectedIndex != -1)
                lvInstruments.Items[cbInstrument.SelectedIndex].Checked = true;           
        }

        void showOther()
        {
            //first create the textbox and label, then move everything out of the way
            this.Height += 45;
            txtOtherInstrument = new TextBox();
            lblOtherInstrument = new Label();
            lblOtherInstrument.AutoSize = true;
            lblOtherInstrument.Font = new System.Drawing.Font("Quicksand", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblOtherInstrument.Location = new System.Drawing.Point(lblEmail.Location.X, lblEmail.Location.Y);
            lblOtherInstrument.Size = new System.Drawing.Size(95, 18);
            lblOtherInstrument.Text = "Other Instrument";
            txtOtherInstrument.Font = new System.Drawing.Font("Quicksand", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txtOtherInstrument.Location = new Point(txtEmail.Location.X, txtEmail.Location.Y);
            txtOtherInstrument.Size = new System.Drawing.Size(263, 24);
            txtOtherInstrument.TabIndex = 8;

            //now, move everything out of the way
            //move everything else
            lblEmail.Location = new Point(lblEmail.Location.X, lblEmail.Location.Y + 45);
            txtEmail.Location = new Point(txtEmail.Location.X, txtEmail.Location.Y + 45);
            txtEmail.TabIndex++;
            lblOther.Location = new Point(lblOther.Location.X, lblOther.Location.Y + 45);
            txtOther.Location = new Point(txtOther.Location.X, txtOther.Location.Y + 45);
            txtOther.TabIndex++;
            lblShirtSize.Location = new Point(lblShirtSize.Location.X, lblShirtSize.Location.Y + 45);
            cbShirtSize.Location = new Point(cbShirtSize.Location.X, cbShirtSize.Location.Y + 45);
            txtOther.TabIndex++;
            btnSignUp.Location = new Point(btnSignUp.Location.X, btnSignUp.Location.Y + 45);
            btnSignUp.TabIndex++;

            //now add the two controls to the form
            this.Controls.Add(lblOtherInstrument);
            this.Controls.Add(txtOtherInstrument);

            //mark other has being selected
            bOtherInstrument = true;
        }

        void hideOther()
        {
            lblOtherInstrument.Dispose();
            txtOtherInstrument.Dispose();
            lblEmail.Location = new Point(lblEmail.Location.X, lblEmail.Location.Y - 45);
            txtEmail.Location = new Point(txtEmail.Location.X, txtEmail.Location.Y - 45);
            txtEmail.TabIndex--;
            lblOther.Location = new Point(lblOther.Location.X, lblOther.Location.Y - 45);
            txtOther.Location = new Point(txtOther.Location.X, txtOther.Location.Y - 45);
            txtOther.TabIndex--;
            lblShirtSize.Location = new Point(lblShirtSize.Location.X, lblShirtSize.Location.Y - 45);
            cbShirtSize.Location = new Point(cbShirtSize.Location.X, cbShirtSize.Location.Y - 45);
            cbShirtSize.TabIndex--;
            btnSignUp.Location = new Point(btnSignUp.Location.X, btnSignUp.Location.Y - 45);
            btnSignUp.TabIndex--;
            this.Height -= 45;

            //mark as other not being selected
            bOtherInstrument = false;
        }

        private void cbInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if "Other" is selected and it isn't already showing
            if (cbInstrument.SelectedIndex == 17 && !bOtherInstrument)
            {
                showOther();
            }
            // if an instrument besides "Other" is selected and "Other" is currently showing
            else if (bOtherInstrument)
            {
                // don't do anything if "Other" is selected in multiple instruments 
                if (lvInstruments != null && lvInstruments.Items[17].Checked)
                {
                    return;
                }

                hideOther();
            }
        }
    }
}
