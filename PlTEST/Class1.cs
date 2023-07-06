using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlTEST
{
    [TransactionAttribute(TransactionMode.ReadOnly)]

    public class Class1 : IExternalCommand
    {
         public Result OnStartup(UIControlledApplication application) 
        {



                return Result.Succeeded; }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
           
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            try
            {
                Reference selectedElement = uIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                if (selectedElement != null)
                {
                    Element element = document.GetElement(selectedElement);


                    ElementType elementType = document.GetElement(element.GetTypeId()) as ElementType;

                    TaskDialog.Show("Element",
                        $"Id:{element.Id}{Environment.NewLine}" +
                        $"Instance: {element.Name}{Environment.NewLine}" +
                        $"Type:{elementType.Name}{Environment.NewLine}" +
                        $"Family:{elementType.FamilyName}{Environment.NewLine}" +
                        $"Category :{element.Category.Name}");



                    return Result.Succeeded;
                }
                else
                {
                    message = "empty";
                    return Result.Failed;
                }
            }catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
