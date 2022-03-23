using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

/*
 * Name/ID: Ellena Begg, 30040389
 * Date: 23 March 2022
 * Version 1.2 Complete ready for release.
 * Program Name: Data Structures Wiki
 * Description: C# Assessment Task 1
 *              A small wiki for Data Structure types.
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

        #region Form, TextBox and ListBox Events
        // 8.7 User can select a definition (Name) from the Listbox and all the information is displayed in the appropriate Textboxes
        private void listViewNameCategory_MouseClick(object sender, MouseEventArgs e)
        {
            int currentRecord = listViewNameCategory.SelectedIndices[0];
            textBoxName.Text = myWikiArray[currentRecord, 0];
            textBoxCategory.Text = myWikiArray[currentRecord, 1];
            textBoxStructure.Text = myWikiArray[currentRecord, 2];
            textBoxDefinition.Text = myWikiArray[currentRecord, 3];
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
            buttonSave.Enabled = false;
        }
        #endregion

        #region Button Click Events
        // 8.5 Binary Search for the Name in the 2D array and display the information in the other textboxes when found
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            // Check user has entered text into TextBox
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                SortArray();
                DisplayArray();

                bool found = false;
                string target = textBoxSearch.Text;
                int upperBound = max - 1;
                int lowerBound = 0;
                int midpoint = 0;

                while (lowerBound <= upperBound)
                {
                    midpoint = (lowerBound + upperBound) / 2;
                    int result = target.CompareTo(myWikiArray[midpoint, 0]);
                    if (result == 0)
                    {
                        // Found
                        found = true;
                        break;
                    }
                    else if (result > 0)
                    {
                        // target is after value
                        lowerBound = midpoint + 1;
                    }
                    else
                    {
                        // (result is < 0)
                        // target is before value
                        upperBound = midpoint - 1;
                    }
                }

                if (found)
                {
                    toolStripStatusLabel.Text = "\'" + target + "\' found";
                    listViewNameCategory.Items[midpoint].Selected = true; // Set as selected item in ListView

                    // Display the information in the other textboxes
                    textBoxName.Text = myWikiArray[midpoint, 0];
                    textBoxCategory.Text = myWikiArray[midpoint, 1];
                    textBoxStructure.Text = myWikiArray[midpoint, 2];
                    textBoxDefinition.Text = myWikiArray[midpoint, 3];
                }
                else
                {
                    toolStripStatusLabel.Text = "\'" + target + "\' not found";
                }
            }
            else
            {
                toolStripStatusLabel.Text = "NOTE: Enter a Data Structure NAME to search for.";

            }
            textBoxSearch.Clear();
            textBoxSearch.Focus();
        }  // end buttonSearch_Click()

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // check for no nulls in 4 x textboxes
            if ((!string.IsNullOrWhiteSpace(textBoxName.Text)) || (!string.IsNullOrWhiteSpace(textBoxCategory.Text))
                || (!string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (!string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
            {
                // confirm Add with dialog
                DialogResult dr = MessageBox.Show("Do you want to Add this Data Structure?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    if (IsNotDuplicate(textBoxName.Text))
                    {
                        if (IsValidCategory(textBoxCategory.Text) || IsValidStructure(textBoxStructure.Text))
                        {
                            for (int x = 0; x < rowSize; x++)
                            {
                                // If there is a free space in the array
                                if (myWikiArray[x, 0] == "~")
                                {
                                    myWikiArray[x, 0] = textBoxName.Text; // write data to Array
                                    myWikiArray[x, 1] = textBoxCategory.Text;
                                    myWikiArray[x, 2] = textBoxStructure.Text;
                                    myWikiArray[x, 3] = textBoxDefinition.Text;

                                    SortArray();
                                    DisplayArray();
                                    toolStripStatusLabel.Text = "Item successfully added.";
                                    break;
                                }
                                else
                                {
                                    toolStripStatusLabel.Text = "Insufficient space. Delete an existing Data Structure to add a new one.";
                                }
                            }
                        } // end check for Valid data
                        else
                        {
                            return; // when either Category or Structure is not valid
                        }
                    } // end check for no duplicates
                    else
                    {
                        return; // when there is a duplicate. No duplicates permitted.
                    }
                } // end confirmation with dialog box to add
                else
                {
                    toolStripStatusLabel.Text = "Add cancelled.";
                    return;
                }
            } // end check for no nulls in TextBoxes
            else
            {
                toolStripStatusLabel.Text = "NOTE: All four attributes are necessary to Add an item. One or more is empty.";
            }
            ClearTextBoxes();
        } // end buttonAdd_Click()

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count > 0)
            {
                // check for no nulls in 4 x textboxes
                if ((!string.IsNullOrWhiteSpace(textBoxName.Text)) || (!string.IsNullOrWhiteSpace(textBoxCategory.Text))
                    || (!string.IsNullOrWhiteSpace(textBoxStructure.Text)) || (!string.IsNullOrWhiteSpace(textBoxDefinition.Text)))
                {
                    // confirm Edit with dialog
                    DialogResult dr = MessageBox.Show("Do you want to edit this Data Structure?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        if (IsValidCategory(textBoxCategory.Text) || IsValidStructure(textBoxStructure.Text))
                        {
                            int currentRecord = listViewNameCategory.SelectedIndices[0];
                            myWikiArray[currentRecord, 0] = textBoxName.Text; // write new data over old
                            myWikiArray[currentRecord, 1] = textBoxCategory.Text;
                            myWikiArray[currentRecord, 2] = textBoxStructure.Text;
                            myWikiArray[currentRecord, 3] = textBoxDefinition.Text;

                            SortArray();
                            DisplayArray();

                            toolStripStatusLabel.Text = "Item successfully updated.";
                        }
                        else
                        {
                            return; // when either Category or Structure is not valid
                        }
                    } // end confirmation with dialog box to edit
                    else
                    {
                        toolStripStatusLabel.Text = "Edit cancelled.";
                        return;
                    }
                } // end check for no nulls in TextBoxes
                else
                {
                    toolStripStatusLabel.Text = "NOTE: All four attributes are necessary to Edit an item. One or more is empty.";
                }
            } //end check have Selected Item in ListView
            else
            {
                toolStripStatusLabel.Text = "NOTE: Select from the List the item to edit, then update the fields, then click 'Edit'.";
            }
            ClearTextBoxes();
        } //end buttonEdit_Click()

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Check an item is selected in ListView
            if (listViewNameCategory.SelectedItems.Count > 0)
            {
                // confirm Delete with dialog
                DialogResult dr = MessageBox.Show("Do you want to delete this Data Structure?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int currentRecord = listViewNameCategory.SelectedIndices[0];
                    string oldName = myWikiArray[currentRecord, 0];

                    myWikiArray[currentRecord, 0] = "~"; // clear data from Array
                    myWikiArray[currentRecord, 1] = "";
                    myWikiArray[currentRecord, 2] = "";
                    myWikiArray[currentRecord, 3] = "";

                    SortArray();
                    DisplayArray();

                    toolStripStatusLabel.Text = oldName + " successfully deleted.";
                } // end confirmation with dialog box to delete
                else
                {
                    toolStripStatusLabel.Text = "Delete cancelled.";
                    return;
                }
            } //end check have Selected Item in ListView
            else
            {
                toolStripStatusLabel.Text = "NOTE: No Data Structure selected to Delete. Select a Data Structure first.";
            }
            ClearTextBoxes();
        } // end buttonDelete_Click()

        // 8.3	Create a CLEAR method to clear the four text boxes so a new definition can be added
        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        // 8.9 Create a LOAD button that will read the information from a binary file
        // called "definitions.bin" into the 2D array
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            //use dialog box
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
                buttonSave.Enabled = true;
            }
        } // end buttonOpen_Click()

        // 8.8 Create a SAVE button so the information from the 2D array can be written into
        // a binary file called "definitions.bin" which is sorted by Name
        private void buttonSave_Click(object sender, EventArgs e)
        {
            //use dialog box
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            saveFileDialog.InitialDirectory = Application.StartupPath;
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

        // 8.6	Create a display method that will show the following information in a List box: Name and Category
        #region Utilities
        /// <summary>
        /// Render the contents of the 2D array to screen, in the ListView.
        /// </summary>
        private void DisplayArray()
        {
            listViewNameCategory.Items.Clear();
            for (int x = 0; x < rowSize; x++) // iterate rows of 2D string array
            {
                ListViewItem listViewItem = new ListViewItem(myWikiArray[x, 0]); // add first column ("name")
                listViewItem.SubItems.Add(myWikiArray[x, 1]); // add second column ("category")
                listViewNameCategory.Items.Add(listViewItem); // add to listView to render
            }
        }

        /// <summary>
        /// Save the contents of the 2D array to a Binary file.
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveBinaryFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    for (int y = 0; y < colSize; y++) // iterate columns
                    {
                        for (int x = 0; x < rowSize; x++) // iterate rows
                        {
                            bin.Serialize(stream, myWikiArray[x, y]); // 
                        }
                    }
                }
                toolStripStatusLabel.Text = "File Saved.";
            }
            catch (IOException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("File could not be saved.\n" + ex.Message);
            }
            //DisplayArray();
        }

        /// <summary>
        /// Open a Binary file and write contents to the 2D array.
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenBinaryFile(string fileName)
        {
            try
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    for (int y = 0; y < colSize; y++) // iterate columns
                    {
                        for (int x = 0; x < rowSize; x++) // iterate rows
                        {
                            myWikiArray[x, y] = (string)bin.Deserialize(stream);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("File could not be opened.\n" + ex.Message);
            }
            DisplayArray();
        }

        /// <summary>
        /// Confirm the Data Structure CATEGORY value is in one of the valid ones.
        /// </summary>
        /// <param name="categoryValue">Data Structure CATEGORY value</param>
        /// <returns>true when is a valid Category value. false when it is not a valid Category value.</returns>
        private bool IsValidCategory(string categoryValue)
        {
            //(validCategories.Any(s => categoryValue.Contains(s))) alternate code, same as below
            //if (validCategories.Any(categoryValue.Contains))
            // cater for case invariance
            if (!validCategories.Any(s => s.IndexOf(categoryValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                MessageBox.Show("Invalid Category entered.\nValid Categories: 'Array', 'List', 'Tree', 'Graph', \n'Abstract' and 'Hash'", "Invalid Category");
                textBoxCategory.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Confirm the Data Structure STRUCTURE value is in one of the valid ones.
        /// </summary>
        /// <param name="structureValue">Data Structure STRUCTURE value</param>
        /// <returns>true when is a valid Structure value. false when it is not a valid Structure value.</returns>
        private bool IsValidStructure(string structureValue)
        {
            // cater for case invariance
            if (!validStructures.Any(s => s.IndexOf(structureValue, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                MessageBox.Show("Invalid Structure entered.\nValid Structures: 'Linear' and 'Non-Linear'", "Invalid Structure");
                textBoxStructure.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Confirm we do not already have this Data Structure NAME in the current list.
        /// </summary>
        /// <param name="name">Data Structure NAME value</param>
        /// <returns>true when is not a duplicate of an existing Name value. false when it is a duplicate of an existing Name value.</returns>
        private bool IsNotDuplicate(string name)
        {
            for (int i = 0; i < max - 1; i++)
            {
                if (string.CompareOrdinal(myWikiArray[i, 0], name) == 0)
                {
                    MessageBox.Show("That Data Structure already exists.");
                    textBoxName.Focus();
                    return false;
                }
            }
            return true;
        }

        private void ClearTextBoxes()
        {
            textBoxName.Clear();
            textBoxCategory.Clear();
            textBoxStructure.Clear();
            textBoxDefinition.Clear();
            listViewNameCategory.SelectedItems.Clear();
        }

        // 8.4 Bubble Sort method to sort the 2D array by "Name" ascending
        /// <summary>
        /// Bubble Sort the 2D Array by "Name" value, ascending
        /// </summary>
        private void SortArray()
        {
            for (int i = 0; i < rowSize; i++) // iterate rows (i)
            {
                for (int j = 0; j < rowSize - 1; j++) // iterate rows (j)
                {
                    // check if next row (j + 1) is alphabetically before this row (j) 
                    if (string.CompareOrdinal(myWikiArray[j, 0], myWikiArray[j + 1, 0]) > 0)
                    {
                        // if it is, swap the two rows in Array
                        for (int k = 0; k < colSize; k++) // iterating all columns to swap the rows
                        {
                            SwapForSort(j, k);
                        }
                    }
                }
            }
        }

        // 8.4 Use a separate swap method that passes (by reference) the array element to be swapped 
        private void SwapForSort(int row, int column)
        {
            string temp = myWikiArray[row, column];
            myWikiArray[row, column] = myWikiArray[row + 1, column];
            myWikiArray[row + 1, column] = temp;
        }
        #endregion

        //BUTTON1_CLICK METHOD IS FOR TESTING AND PRE-START ONLY
        private void button1_Click(object sender, EventArgs e)
        {
            //for testing
            myWikiArray[0, 0] = "Array";
            myWikiArray[0, 1] = "Array";
            myWikiArray[0, 2] = "Linear";
            myWikiArray[0, 3] = "Arrays are collections of data items that are of the same type, stored together in adjoining memory locations. Each data item is known as an “element.” Data is stored in a linear sequence. Arrays can be seen as data elements organised in a row.";
            myWikiArray[1, 0] = "Two Dimension Array";
            myWikiArray[1, 1] = "Array";
            myWikiArray[1, 2] = "Linear";
            myWikiArray[1, 3] = "A two-dimension array is an array which technically has one row of elements, however, each row has a bunch of elements defined by itself. It can be visualised as a grid (or table) with rows and columns. It is an array of arrays, or an array that has multiple levels.";
            myWikiArray[2, 0] = "List";
            myWikiArray[2, 1] = "List";
            myWikiArray[2, 2] = "Linear";
            myWikiArray[2, 3] = "A List is an ordered sequence of data. Each data stored in the list is called an item and item can be of any data type. It is a collection of data in which items do not need to be searched, added or removed in any sorted order, like a To-Do list. It is a sequential set of elements to which you can add new elements and remove or change existing ones.";
            myWikiArray[3, 0] = "Linked List";
            myWikiArray[3, 1] = "List";
            myWikiArray[3, 2] = "Linear";
            myWikiArray[3, 3] = "Linked lists store item collections in a linear order. Each element in a linked list contains a data item and a link, or reference, to the subsequent item on the same list.";
            myWikiArray[4, 0] = "Self-Balance Tree";
            myWikiArray[4, 1] = "Tree";
            myWikiArray[4, 2] = "Non-Linear";
            myWikiArray[4, 3] = "The Self-Balance Tree is a type of Binary Search Tree, but is self-balancing. Each node stores a value called a balanced factor, which is the difference in the height of the left sub-tree and right sub-tree. All the nodes in the Self-Balancing tree must have a balance factor of - 1, 0, and 1.";
            myWikiArray[5, 0] = "Heap";
            myWikiArray[5, 1] = "Tree";
            myWikiArray[5, 2] = "Non-Linear";
            myWikiArray[5, 3] = "A Heap is a special Tree-based data structure in which the tree is a complete binary tree. The tree in a heap is always balanced. Generally, Heaps can be of two types: Max-Heap - where the key present in the root node is the greatest among all keys. Min-Heap - where the key present in the root node is the smallest among all keys.";
            myWikiArray[6, 0] = "Binary Search Tree";
            myWikiArray[6, 1] = "Tree";
            myWikiArray[6, 2] = "Non-Linear";
            myWikiArray[6, 3] = "Tree data structures have a hierarchical structure like a family tree. They have one ‘root’ node. This can have 2 ‘child’ nodes. Each child node can have subsequent ‘child’ nodes etc. Binary trees can have at most two child nodes, which are called the left child and the right child. The value of a left child node of the tree should be less than or equal to the parent node value of the tree. And the value of the right child node should be greater than or equal to the parent value.";
            myWikiArray[7, 0] = "Graph";
            myWikiArray[7, 1] = "Graph";
            myWikiArray[7, 2] = "Non-Linear";
            myWikiArray[7, 3] = "Graphs are a nonlinear pictorial representation of element sets. Graphs consist of finite node sets, also called vertices, connected by links, alternately called edges.";
            myWikiArray[8, 0] = "Set";
            myWikiArray[8, 1] = "Abstract";
            myWikiArray[8, 2] = "Non-Linear";
            myWikiArray[8, 3] = "A set is a data structure that can store any number of unique values in any order you so wish. Sets only allow non-repeated, unique values within them.";
            myWikiArray[9, 0] = "Queue";
            myWikiArray[9, 1] = "Abstract";
            myWikiArray[9, 2] = "Linear";
            myWikiArray[9, 3] = "Queues store item collections sequentially like stacks, but the operation order must be “first in, first out” only, meaning the element which gets inserted in a list at first gets removed at first. Queues are linear lists. In a queue, insertion is performed at one end (“Rear”), and the removal is performed at the opposite end (“Front”). ";
            myWikiArray[10, 0] = "Stack";
            myWikiArray[10, 1] = "Abstract";
            myWikiArray[10, 2] = "Linear";
            myWikiArray[10, 3] = "Stacks store collections of items in a linear order and accompanies a principle known as LIFO (Last In First Out) or FILO (First In Last Out). Visually you make a pile of elements. You can only access the top most element. You can only add to the top of the pile. To access any other element, you need to remove the one(s) on top of it.";
            myWikiArray[11, 0] = "Hash Table";
            myWikiArray[11, 1] = "Hash";
            myWikiArray[11, 2] = "Non-Linear";
            myWikiArray[11, 3] = "Hash tables, also called hash maps, can be used as either a linear or nonlinear data structure, though they favour the former. This structure is normally built using arrays. Hash tables map keys to values.";
            DisplayArray();
        }

    }//end Form
}// end namespace
