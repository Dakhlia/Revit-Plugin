using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MySql.Data.MySqlClient;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;

public class CategoryProductForm : Form
{
    private System.Windows.Forms.ComboBox categoryComboBox;
    private ListBox productListBox;

    private Dictionary<string, List<string>> data;

    public CategoryProductForm()
    {
        InitializeComponents();
        LoadData();
    }

    private void InitializeComponents()
    {
        this.Text = "Liste des Catégories et Produits";
        this.Size = new System.Drawing.Size(300, 200);

        categoryComboBox = new ComboBox();
        categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        categoryComboBox.SelectedIndexChanged += CategoryComboBox_SelectedIndexChanged;

        productListBox = new ListBox();

        TableLayoutPanel panel = new TableLayoutPanel();
        panel.Dock = DockStyle.Fill;
        panel.ColumnCount = 2;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        panel.RowCount = 1;
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        panel.Controls.Add(categoryComboBox, 0, 0);
        panel.Controls.Add(productListBox, 1, 0);

        this.Controls.Add(panel);
    }

    private void LoadData()
    {
        data = new Dictionary<string, List<string>>();

        // Votre chaîne de connexion à la base de données
        string connectionString = "Server=127.0.0.1;Database=revitapi;Uid=root;Pwd=;";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "SELECT Categorie, Nom FROM produit";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader.GetString(0);
                            string product = reader.GetString(1);

                            if (!data.ContainsKey(category))
                                data[category] = new List<string>();

                            data[category].Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion ou de lecture des données : " + ex.Message);
            }
        }

        categoryComboBox.Items.AddRange(data.Keys.ToArray());
    }

    private void CategoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedCategory = categoryComboBox.SelectedItem.ToString();
        List<string> products = data[selectedCategory];

        productListBox.Items.Clear();
        productListBox.Items.AddRange(products.ToArray());
    }
}

public static class Class4
{
    public static void ShowCategoryProductForm(UIApplication uiApp)
    {
        CategoryProductForm form = new CategoryProductForm();
        Application.Run(form);
    }
}
