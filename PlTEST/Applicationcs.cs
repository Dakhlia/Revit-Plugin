using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace PlTEST
{
    public class Applicationcs : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            List<RibbonPanel> panels = RibbonPanel(application);

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            foreach (RibbonPanel p in panels.Where(p => p.Name == "Info"))
            {
                

                if (p.AddItem(new PushButtonData("Info", "Info", thisAssemblyPath, "PlTEST.Class1"))
                    is PushButton button)
                {
                    button.ToolTip = "My First Plugin";

                    Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "info.png"));
                    int newWidth = 25; // Nouvelle largeur souhaitée
                    int newHeight = 25; // Nouvelle hauteur souhaitée
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = uri;
                    bitmapImage.DecodePixelWidth = newWidth;
                    bitmapImage.DecodePixelHeight = newHeight;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    button.LargeImage = bitmapImage;
                    button.Image = bitmapImage;

                }
            }
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Categorie"))
            {
                if (p.AddItem(new PushButtonData("Categorie", "Categorie", thisAssemblyPath, "PlTEST.Class2"))
                is PushButton button1)
                {
                    button1.ToolTip = "Categorie";


                  

                    Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "icon.png"));
                   // BitmapImage bitmapImage = new BitmapImage(uri);
                    int newWidth = 25; // Nouvelle largeur souhaitée
                    int newHeight = 25; // Nouvelle hauteur souhaitée
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = uri;
                    bitmapImage.DecodePixelWidth = newWidth;
                    bitmapImage.DecodePixelHeight = newHeight;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();



                    button1.LargeImage = bitmapImage;
                    // button1.Image = bitmapImage;
                }
            }

           



            return Result.Succeeded;
        }

        public List<RibbonPanel> RibbonPanel(UIControlledApplication a)
        {
            string tab = "Mepros";

            RibbonPanel ribbonPanel = null;

            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Info");
                RibbonPanel panel1 = a.CreateRibbonPanel(tab, "Categorie");
                //RibbonPanel panel2 = a.CreateRibbonPanel("Compléments", "Categorie");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Info"))
            {
                ribbonPanel = p;
            }

            return panels;


        }

       

    }
}
