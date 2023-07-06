
using Form = System.Windows.Forms.Form;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = System.Windows.Forms.View;
using Autodesk.Revit.DB.Structure;
using System.Linq;



namespace PlTEST
{
    [Transaction(TransactionMode.Manual)]
    public class Class2 : IExternalCommand
    {
        private List<CategorieProduits> categories;
        private ListView categorieListView;
        private ListView produitListView;
        private Document doc;
        private UIDocument uiDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Récupérer l'application Revit active
            UIApplication uiApp = commandData.Application;
            uiDoc = uiApp.ActiveUIDocument;
            Application app = uiApp.Application;
            doc = uiDoc.Document;

            Class4.ShowCategoryProductForm(uiApp);

           /* try
            {
                // Demander à l'utilisateur d'entrer les catégories et produits
                categories = ObtenirCategoriesProduits();

                // Afficher la liste des catégories dans une boîte de dialogue
                using (Form form = new Form())
                {
                    form.Text = "Liste des catégories";
                    form.Size = new System.Drawing.Size(400, 400);

                    // Créer une ListView pour afficher les catégories
                    categorieListView = new ListView();
                    categorieListView.Dock = DockStyle.Fill;
                    categorieListView.View = View.Details;
                    categorieListView.FullRowSelect = true;
                    categorieListView.MultiSelect = false;
                    categorieListView.Columns.Add("Catégorie");

                    // Ajouter les catégories à la ListView
                    foreach (CategorieProduits categorie in categories)
                    {
                        ListViewItem item = new ListViewItem(categorie.Nom);
                        categorieListView.Items.Add(item);
                    }

                    // Gérer l'événement de sélection d'une catégorie
                    categorieListView.SelectedIndexChanged += (sender, e) =>
                    {
                        if (categorieListView.SelectedItems.Count > 0)
                        {
                            ListViewItem selectedItem = categorieListView.SelectedItems[0];
                            string categorieSelectionnee = selectedItem.Text;

                            // Afficher la liste des produits de la catégorie sélectionnée
                            AfficherProduits(categorieSelectionnee);
                        }
                    };

                    form.Controls.Add(categorieListView);

                    // Afficher la boîte de dialogue
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                message = ex.Message;
                return Result.Failed;
            }
           */
            return Result.Succeeded;
        }

        private void AfficherProduits(string categorieSelectionnee)
        {
            using (Form form = new Form())
            {
                form.Text = "Liste des produits";
                form.Size = new System.Drawing.Size(400, 400);

                // Créer une ListView pour afficher les produits
                produitListView = new ListView();
                produitListView.Dock = DockStyle.Fill;
                produitListView.View = View.Details;
                produitListView.FullRowSelect = true;
                produitListView.MultiSelect = false;
                produitListView.Columns.Add("Produit");

                // Récupérer les produits de la catégorie sélectionnée
                CategorieProduits categorie = categories.Find(c => c.Nom == categorieSelectionnee);
                List<string> produits = categorie.Produits;

                // Ajouter les produits à la ListView
                foreach (string produit in produits)
                {
                    ListViewItem item = new ListViewItem(produit);
                    produitListView.Items.Add(item);
                }

                // Gérer l'événement de sélection d'un produit
                produitListView.SelectedIndexChanged += (sender, e) =>
                {
                    if (produitListView.SelectedItems.Count > 0)
                    {
                        ListViewItem selectedItem = produitListView.SelectedItems[0];
                        string produitSelectionne = selectedItem.Text;

                        // Insérer le produit dans le projet
                        InsererProduit(doc, uiDoc, produitSelectionne);
                    }
                };

                form.Controls.Add(produitListView);

                // Afficher la boîte de dialogue
                form.ShowDialog();
            }
        }

        private void InsererProduit(Document doc, UIDocument uiDoc, string produit)
        {
            if (doc.IsModifiable)
            {
                using (Transaction transaction = new Transaction(doc, "Insert Product"))
                {
                    try
                    {
                        transaction.Start();

                        // Charger la famille dans le projet (assurez-vous d'avoir le chemin d'accès correct)
                        string famillePath = "C:\\Program Files\\Autodesk\\Revit 2019\\Samples\\rst_basic_sample_family.rfa";
                        Family famille = LoadFamily(doc, famillePath);

                        // Rechercher le symbole de famille correspondant au produit
                        FamilySymbol familleSymbol = famille.GetFamilySymbolIds()
                            .Select(id => doc.GetElement(id))
                            .OfType<FamilySymbol>()
                            .FirstOrDefault(symbol => symbol.Name == produit);

                        // Vérifier si le symbole de famille est valide et actif
                        if (familleSymbol != null && familleSymbol.IsActive)
                        {
                            // Demander à l'utilisateur de sélectionner un point d'insertion dans le document Revit
                            XYZ insertionPoint = null;
                            try
                            {
                                // Demander à l'utilisateur de sélectionner un point d'insertion dans le document Revit
                                XYZ pickedPoint = uiDoc.Selection.PickPoint();
                                insertionPoint = new XYZ(pickedPoint.X, pickedPoint.Y, pickedPoint.Z);
                            }
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                                // L'utilisateur a annulé la sélection du point d'insertion
                                transaction.RollBack();
                                return;
                            }

                            // Créer une instance de famille à l'emplacement sélectionné
                            FamilyInstance familleInstance = doc.Create.NewFamilyInstance(insertionPoint, familleSymbol, StructuralType.NonStructural);
                            familleInstance.Name = produit;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur during the transaction
                        transaction.RollBack();
                        TaskDialog.Show("Error", ex.Message);
                    }
                }
            }
            else
            {
                // Show an error message if the document is not modifiable
                TaskDialog.Show("Error", "The document is not modifiable.");
            }
        }

        private Family LoadFamily(Document doc, string famillePath)
        {
            // Vérifier si la famille est déjà chargée dans le document
            Family famille = GetLoadedFamily(doc, famillePath);
            if (famille != null)
                return famille;

            // Charger la famille dans le document en utilisant LoadFamily
            using (Transaction transaction = new Transaction(doc, "Load Family"))
            {
                transaction.Start();
                doc.LoadFamily(famillePath, out Family familleLoaded);
                famille = familleLoaded;
                transaction.Commit();
            }

            return famille;
        }


        private Family GetLoadedFamily(Document doc, string famillePath)
        {
            // Parcourir toutes les familles chargées dans le document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Family));
            foreach (Family famille in collector)
            {
                if (famille.Name == famillePath)
                    return famille;
            }
            return null;
        }

        private List<CategorieProduits> ObtenirCategoriesProduits()
        {
            List<CategorieProduits> categories = new List<CategorieProduits>();

            // Catégorie 1: Equipment électrique
            CategorieProduits categorie1 = new CategorieProduits();
            categorie1.Nom = "Equipment électrique";
            categorie1.Produits = new List<string>()
            {
                "M_Lighting and Appliance Panelboard - 480V MCB - Surface",
                "Produit 2",
                "Produit 3"
            };
            categories.Add(categorie1);

            return categories;
        }

        private class CategorieProduits
        {
            public string Nom { get; set; }
            public List<string> Produits { get; set; }
        }
    }
}
