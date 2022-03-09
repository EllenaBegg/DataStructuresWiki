using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

/*
 * Name/ID: Ellena Begg, 30040389
 * Date: 2022
 * Created on 9/3/2022
 * Program Name: Data Structures Wiki
 * Description: A small wiki for Data Structure types.
 *              A uniform definition and cataloguing of this information.
 *              Users can add and delete from the wiki, and edit items. 
 *              Users can open from and save to a Binary file.
 */

namespace DataStructuresWiki
{
    public partial class DataStructureWikiForm : Form
    {
        public DataStructureWikiForm()
        {
            InitializeComponent();
        }

        // 8.1 use 2D string array, and static variables for array dimensions
        static int rowSize = 12;  //x
        static int colSize = 4;  //y
        static int max = 12;
        static int attributes = 4;
        static string[,] myWikiArray = new string[max, attributes];

        // 8.8 and 8.9 file name for save/load
        string fileName = "definitions.bin";

        // Valid Categories
        string[] validCategories = new string[] { "Array", "List", "Tree", "Graph", "Abstract", "Hash" };
        // Valid Structures
        string[] validStructures = new string[] { "Linear", "Non-linear" };

        private void DisplayArray()
        {
            listViewNameCategory.Items.Clear();
            for (int x = 0; x < rowSize; x++)//iterate rows of 2D string array
            {
                ListViewItem listViewItem = new ListViewItem(myWikiArray[x, 0]);//add first column ("name")
                listViewItem.SubItems.Add(myWikiArray[x, 1]);//add second column ("category")
                listViewNameCategory.Items.Add(listViewItem);// add to listView to render
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //for testing
            myWikiArray[0, 0] = "Array";
            myWikiArray[0, 1] = "Array";
            myWikiArray[0, 2] = "Linear";
            myWikiArray[0, 3] = "Brief description";
            myWikiArray[1, 0] = "Two Dimension Array";
            myWikiArray[1, 1] = "Array";
            myWikiArray[1, 2] = "Linear";
            myWikiArray[1, 3] = "Brief description";
            myWikiArray[2, 0] = "List";
            myWikiArray[2, 1] = "List";
            myWikiArray[2, 2] = "Linear";
            myWikiArray[2, 3] = "Brief description";
            myWikiArray[3, 0] = "Linked List";
            myWikiArray[3, 1] = "List";
            myWikiArray[3, 2] = "Linear";
            myWikiArray[3, 3] = "Brief description";
            myWikiArray[4, 0] = "Self-Balance Tree";
            myWikiArray[4, 1] = "Tree";
            myWikiArray[4, 2] = "Non-Linear";
            myWikiArray[4, 3] = "Brief description";
            myWikiArray[5, 0] = "Heap";
            myWikiArray[5, 1] = "Tree";
            myWikiArray[5, 2] = "Non-Linear";
            myWikiArray[5, 3] = "Brief description";
            myWikiArray[6, 0] = "Binary Search Tree";
            myWikiArray[6, 1] = "Tree";
            myWikiArray[6, 2] = "Non-Linear";
            myWikiArray[6, 3] = "Brief description";
            myWikiArray[7, 0] = "Graph";
            myWikiArray[7, 1] = "Graph";
            myWikiArray[7, 2] = "Non-Linear";
            myWikiArray[7, 3] = "Brief description";
            myWikiArray[8, 0] = "Set";
            myWikiArray[8, 1] = "Abstract";
            myWikiArray[8, 2] = "Non-Linear";
            myWikiArray[8, 3] = "Brief description";
            myWikiArray[9, 0] = "Queue";
            myWikiArray[9, 1] = "Abstract";
            myWikiArray[9, 2] = "Linear";
            myWikiArray[9, 3] = "Brief description";
            myWikiArray[10, 0] = "Stack";
            myWikiArray[10, 1] = "Abstract";
            myWikiArray[10, 2] = "Linear";
            myWikiArray[10, 3] = "Brief description";
            myWikiArray[11, 0] = "Hash Table";
            myWikiArray[11, 1] = "Hash";
            myWikiArray[11, 2] = "Non-Linear";
            myWikiArray[11, 3] = "Brief description";
            DisplayArray();
        }

        #region Form, TextBox and ListBox Events
        // 8.7 User can select a definition (Name) from the Listbox and all the information is displayed in the appropriate Textboxes
        private void listViewNameCategory_MouseClick(object sender, MouseEventArgs e)
        {
            //TODO, check array has items, has been displayed (is in memory?
            //TODO don't need to clear textboxes first, it repopulates by itself. overwrites whatever was there when select new item

            textBoxName.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 0];
            textBoxCategory.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 1];
            textBoxStructure.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 2];
            textBoxDefinition.Text = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 3];
        }

        //	A double mouse click in the Search text box will clear the Search input box
        private void textBoxSearch_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBoxSearch.Clear();
        }

        // 8.3	Create a CLEAR method to clear the four text boxes so a new definition can be added
        private void textBoxName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBoxName.Clear();
            textBoxCategory.Clear();
            textBoxStructure.Clear();
            textBoxDefinition.Clear();
        }


        private void DataStructureWikiForm_Load(object sender, EventArgs e)
        {
            textBoxSearch.Focus();
        }
        #endregion

        #region BUtton Click Events
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Check user has entered text into TextBox
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
            }
            else
            {
                //TODO MsgTextDisplayArea.Text = "NOTE: No Registration Plate entered.";
            }
            textBoxSearch.Clear();
            textBoxSearch.Focus();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // check for nulls in 4 x textboxes
            if ((string.IsNullOrWhiteSpace(textBoxName.Text)) || (string.IsNullOrWhiteSpace(textBoxCategory.Text))
                || (string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
            {
                toolStripStatusLabel.Text = "NOTE: All four attributes are necessary to Add an item. One or more is empty.";
            }
            else
            {
                // confirm Add with dialog
                DialogResult dr = MessageBox.Show("Do you want to Add this Data Structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                {
                    toolStripStatusLabel.Text = "Add cancelled.";
                }
                else
                {
                    if (!IsValidCategory(textBoxCategory.Text) || !IsValidStructure(textBoxStructure.Text))
                    { return; }
                    else
                    {
                        for (int x = 0; x < rowSize; x++)
                        {
                            if (myWikiArray[x, 0] == "~")
                            {
                                myWikiArray[x, 0] = textBoxName.Text;
                                myWikiArray[x, 1] = textBoxCategory.Text;
                                myWikiArray[x, 2] = textBoxStructure.Text;
                                myWikiArray[x, 3] = textBoxDefinition.Text;
                                break;
                            }
                            else
                            {
                                toolStripStatusLabel.Text = "Insufficient space. Delete an existing Data Structure to add a new one.";
                            }
                        }

                        //TODO sort, does it itself automatically?
                        DisplayArray();
                        
                        toolStripStatusLabel.Text = "Item successfully added.";
                    }
                }
            }// end confirmation with dialog box to add
            ClearTextBoxes();
        }// end buttonAdd_Click()

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count == 0)
            {
                toolStripStatusLabel.Text = "NOTE: Select from the List the item to edit, then update the fields, then click 'Edit'.";
            }
            else
            {
                // check for nulls in 4 x textboxes
                if ((string.IsNullOrWhiteSpace(textBoxName.Text)) || (string.IsNullOrWhiteSpace(textBoxCategory.Text))
                    || (string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
                {
                    toolStripStatusLabel.Text = "NOTE: All four attributes are necessary to Edit an item. One or more is empty.";
                }
                else
                {
                    // confirm Edit with dialog
                    DialogResult dr = MessageBox.Show("Do you want to edit this Data Structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr != DialogResult.Yes)
                    {
                        toolStripStatusLabel.Text = "Edit cancelled.";
                    }
                    else
                    {
                        if (!IsValidCategory(textBoxCategory.Text) || !IsValidStructure(textBoxStructure.Text))
                        { return; }
                        else
                        {
                            int currentRecord = listViewNameCategory.SelectedIndices[0];
                            myWikiArray[currentRecord, 0] = textBoxName.Text;
                            myWikiArray[currentRecord, 1] = textBoxCategory.Text;
                            myWikiArray[currentRecord, 2] = textBoxStructure.Text;
                            myWikiArray[currentRecord, 3] = textBoxDefinition.Text;

                            //TODO sort, does it itself automatically?
                            DisplayArray();

                            toolStripStatusLabel.Text = "Item successfully updated.";
                        }
                    }
                }//end check for null in TextBoxes
            } //end check have Selected Item in ListView
            ClearTextBoxes();
        } //end buttonEdit_Click()

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count == 0)
            {
                toolStripStatusLabel.Text = "NOTE: No Data Structure selected to Delete. Select a Data Structure first.";
            }
            else
            {
                // confirm Delete with dialog
                DialogResult dr = MessageBox.Show("Do you want to delete this Data Structure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                {
                    toolStripStatusLabel.Text = "Delete cancelled.";
                }
                else
                {
                    int currentRecord = listViewNameCategory.SelectedIndices[0];
                    string oldName = myWikiArray[listViewNameCategory.SelectedItems[0].Index, 0];

                    myWikiArray[currentRecord, 0] = "~";
                    myWikiArray[currentRecord, 1] = "";
                    myWikiArray[currentRecord, 2] = "";
                    myWikiArray[currentRecord, 3] = "";


                    //TODO sort, does it itself automatically?
                    DisplayArray();

                    toolStripStatusLabel.Text = oldName + " successfully deleted.";

                }
            } // end if have an item selected in ListView
        }// end buttonDelete_Click()

        // 8.3	Create a CLEAR method to clear the four text boxes so a new definition can be added
        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        // 8.9 Create a LOAD button that will read the information from a binary file
        // called "definitions.dat" into the 2D array
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            //use dialog box
            //string fileName = "definitions.bin"; //default file name
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.Filter = "Binary File | *.bin";
            openFileDialog.Title = "Select a Binary File";

            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            if (dr == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;

                OpenBinaryFile(fileName);
            }
        }

        // 8.8 Create a SAVE button so the information from the 2D array can be written into
        // a binary file called "definitions.dat" which is sorted by Name
        private void buttonSave_Click(object sender, EventArgs e)
        {
            //TODO check empty array?
            //TODO array SORTED by Name

            //use dialog box
            //string fileName = "definitions.bin"; //default file name
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            saveFileDialog.Filter = "Binary File | *.bin";

            DialogResult dr = saveFileDialog.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            if (dr == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;

                SaveBinaryFile(fileName);
            }
        }
        #endregion

        #region Utilities
        private void SaveBinaryFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    for (int y = 0; y < colSize; y++)
                    {
                        for (int x = 0; x < rowSize; x++)
                        {
                            bin.Serialize(stream, myWikiArray[x, y]);
                        }
                    }
                }
                toolStripStatusLabel.Text = "File Saved.";//TODO check what works???
                //TODO MsgTextDisplayArea.Text = "File Saved.";
            }
            catch (IOException ex)
            {
                //For testing TODO swap commenting to show message to user
                MessageBox.Show(ex.Message);
                //MessageBox.Show("File could not be saved.");
            }
            DisplayArray();
        }

        private void OpenBinaryFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    for (int y = 0; y < colSize; y++)
                    {
                        for (int x = 0; x < rowSize; x++)
                        {
                            myWikiArray[x, y] = (string)bin.Deserialize(stream);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                //MessageBox.Show("File could not be opened.");
            }
            DisplayArray();
        }

        private bool IsValidCategory(string categoryValue)
        {

            //(validCategories.Any(s => categoryValue.Contains(s))) alternate code, same as below
            //if (validCategories.Any(categoryValue.Contains))
            // cater for case invariance
            if (!validCategories.Any(s => s.IndexOf(categoryValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                //TODO use MsgBox, or StatusStrip? When to put up msg (in coordination with outer Method)?
                MessageBox.Show("Invalid Category entered.\nValid Categories: 'Array', 'List', 'Tree', 'Grpah', 'Abstract' and 'Hash'", "Invalid Category");
                textBoxCategory.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsValidStructure(string structureValue)
        {
            // cater for case invariance
            if (!validStructures.Any(s => s.IndexOf(structureValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                //TODO use MsgBox, or StatusStrip? When to put up msg (in coordination with outer Method)?
                MessageBox.Show("Invalid Structure entered.\nValid Structures: 'Linear' and 'Non-Linear'", "Invalid Structure");
                textBoxStructure.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearTextBoxes()
        {
            textBoxName.Clear();
            textBoxCategory.Clear();
            textBoxStructure.Clear();
            textBoxDefinition.Clear();
        }
        #endregion
    }//end Form
}// end namespace
