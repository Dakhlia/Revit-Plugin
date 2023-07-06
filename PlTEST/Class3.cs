using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MySql.Data.MySqlClient;

public static class DatabaseUtils
{

    public static void ReadDataFromDatabase(UIApplication uiApp)
    {
        Document doc = uiApp.ActiveUIDocument.Document;

        // Votre chaîne de connexion à la base de données
        string connectionString = "Server=127.0.0.1;Database=revitapi;Uid=root;Pwd=;";

        List<KeyValuePair<string, string>> dataList = new List<KeyValuePair<string, string>>();


        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string query = "SELECT * FROM produit";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Accéder aux valeurs des colonnes par leur nom ou leur indice
                            string Produit = reader.GetString(0); // Lire la première colonne en tant que chaîne de caractères
                            string Categorie = reader.GetString(2); // Lire la deuxième colonne en tant qu'entier
                            // ...

                            dataList.Add(new KeyValuePair<string, string>(Categorie, Produit));

                            // Faire quelque chose avec les valeurs lues
                          //  TaskDialog.Show("Database Data", $"prduit : {Produit}, Categorie : {Categorie}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Database Error", "Erreur de connexion ou de lecture des données : " + ex.Message);
            }
            foreach (KeyValuePair<string, string> item in dataList)
            {
                TaskDialog.Show("Database Data", $"Catégorie : {item.Key}, Produit : {item.Value}");
            }
        }
    }
}
