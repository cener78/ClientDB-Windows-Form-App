using DatabaseClient.Models;

namespace DatabaseClient
{
    public partial class Form1 : Form
    {
        // The db context object is tracking all the changes to the data
        // The scope of this db object is the entire class
        AdventureWorks2Context db = new AdventureWorks2Context();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetCustomers_Click(object sender, EventArgs e)
        {
            // Call ToList() to get all records from the DB table
            List<Customer>? list = db.Customers.ToList();

            dataGridView1.DataSource = list;
        }

        private void btnGetProducts_Click(object sender, EventArgs e)
        {
            // Define the query using LINQ:
            var query = from p in db.Products
                        where p.Color == cbxColors.Text
                        orderby p.ListPrice descending
                        select p;

            // Show the LINQ to SQL translation
            //MessageBox.Show(query.Expression

            // Execute the query by calling ToList()
            var products = query.ToList();

            // Show the result in the dgv
            dataGridView1.DataSource = products;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            var query = from p in db.Products
                        where p.Color != null
                        select p.Color;

            // Distinct() will remove all duplicates
            var colors = query.Distinct().ToList();

            cbxColors.DataSource = colors;

            // Now: get all categories

            // Use the DisplayMember to specify which property to show in the Combobox
            cbxCategories.DisplayMember = "Name";
            cbxCategories.DataSource = db.ProductCategories.ToList();
        }

        private void cbxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cbxCategories.SelectedItem as ProductCategory;

            // If the item is NOT a ProductCategory, then there is nothing to do here...
            if (item == null)
                return;

            
            var query = from p in db.Products
                        where p.ProductCategoryId == item.ProductCategoryId
                        select p;

            dataGridView1.DataSource = query.ToList();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            // Send all the changes to the DB server
            db.SaveChanges();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            var newCategory = new ProductCategory();
            newCategory.Name = txtNewName.Text;
            newCategory.ModifiedDate = DateTime.Now;
            newCategory.Rowguid = Guid.NewGuid();

            // Add the new object to the db context 
            // This is only client side!
            db.ProductCategories.Add(newCategory);

            // Send the new object to the DB server
            db.SaveChanges();

            cbxCategories.DataSource = db.ProductCategories.ToList();

            dataGridView1.DataSource = db.ProductCategories.ToList();
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row!");
                return;
            }

            var cat = dataGridView1.SelectedRows[0].DataBoundItem as ProductCategory;

            if (cat == null)
            {
                MessageBox.Show("Please select a ProductCategory! This is some other object!");
                return;
            }

            db.ProductCategories.Remove(cat);
            db.SaveChanges();

            // Refresh the combobox and the dgv
            var categories = db.ProductCategories.ToList();
            cbxCategories.DataSource = categories;
            dataGridView1.DataSource = categories;
        }

        private void btnGetCategories_Click(object sender, EventArgs e)
        {
            var categories = db.ProductCategories.ToList();
            cbxCategories.DataSource = categories;
            dataGridView1.DataSource = categories;
        }
    }
}

