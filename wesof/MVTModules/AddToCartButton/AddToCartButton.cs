using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wesof.MVTManager;

namespace wesof.MVTModules.AddToCartButton
{
    static class AddToCartButton
    {
        static private CommandLineInterface _CLI = null;

        
        static private string[] ColorPalette = { "#0f834d", "#43454b", "#3D9CD2", "#e2401c", "#ffffff"};
        static private string[] AddToCartTextCopies = { "Add to basket", "Add", "Buy" };


        static private List<AddToCartButtonVariation> Variations = new List<AddToCartButtonVariation>((ColorPalette.Length+1)*(AddToCartTextCopies.Length+1));

        static private AddToCartButtonVariation AbsoluteControl = null; //Represents the unchanged/vanilla site.
        static private AddToCartButtonVariation CurrentWinnerControl = null; //This is the latest winner, and used as the control for the next test. In the beginning, this is the absolute control.

        static private bool isInitialised = false;

        //Creates the permutations with IDs.
        //Id-s starting from 1. Zero is reserved for the no-change variation (absolute control / vanilla)
        public static void Initialise()
        {
            Configuration.MainController.GetCLI();
            int id = 1;


            //Initialise test variants
            for(int i = 0; i<ColorPalette.Length; i++)
            {
                for(int j = -1; j<AddToCartTextCopies.Length; j++)
                {
                    AddToCartButtonVariation tmp = new AddToCartButtonVariation();
                    tmp.id = id;
                    tmp.Type = TestTypesEnum.ADDTOCARTBUTTON;

                    if (i != -1)
                        tmp.colorPalette = ColorPalette[i];
                    else
                        tmp.colorPalette = null;

                    if (j != -1)
                        tmp.addToCartTextCopies = AddToCartTextCopies[j];
                    else
                        tmp.addToCartTextCopies = null;

                    Variations.Add(tmp);
                    id++;
                }
            }

            //Initialise absolute control
            AbsoluteControl = new AddToCartButtonVariation();
            AbsoluteControl.id = 0;
            AbsoluteControl.Type = TestTypesEnum.ADDTOCARTBUTTON;

            //Set the current winner control to be the absolute control
            CurrentWinnerControl = AbsoluteControl; 

            isInitialised = true;
        }


        public static void AddNewHypothesis(string ButtonCollor, string ButtonText)
        {
            int id = Variations.Count + 1;

            //Add the new combined variant
            if(ButtonCollor != null && ButtonText != null)
            {
                AddToCartButtonVariation tmp = new AddToCartButtonVariation();
                tmp.id = id;
                tmp.Type = TestTypesEnum.ADDTOCARTBUTTON;
                tmp.colorPalette = ButtonCollor;
                tmp.addToCartTextCopies = ButtonText;

                Variations.Add(tmp);
                id++;
            }

            //Combine new colour with existing copies
            if(ButtonCollor != null)
            {
                for(int i = 0; i < AddToCartTextCopies.Length; i++)
                {
                    AddToCartButtonVariation tmp = new AddToCartButtonVariation();
                    tmp.id = id;
                    tmp.Type = TestTypesEnum.ADDTOCARTBUTTON;
                    tmp.colorPalette = ButtonCollor;
                    tmp.addToCartTextCopies = AddToCartTextCopies[i];

                    Variations.Add(tmp);
                    id++;
                }

                Array.Resize(ref ColorPalette, ColorPalette.Length + 1);
                ColorPalette[ColorPalette.Length - 1] = ButtonCollor;
            }

            //Combine new copy with existing colours
            if(ButtonText != null)
            {
                for (int i = 0; i < ColorPalette.Length; i++)
                {
                    AddToCartButtonVariation tmp = new AddToCartButtonVariation();
                    tmp.id = id;
                    tmp.Type = TestTypesEnum.ADDTOCARTBUTTON;
                    tmp.colorPalette = ColorPalette[i];
                    tmp.addToCartTextCopies = ButtonText;

                    Variations.Add(tmp);
                    id++;
                }

                Array.Resize(ref AddToCartTextCopies, AddToCartTextCopies.Length + 1);
                AddToCartTextCopies[AddToCartTextCopies.Length - 1] = ButtonText;
            }               
                  
        }

        internal static VariationType GetNextUntestedVariant()
        {     
            for(int i = 0; i < Variations.Count; i++)
            {
                if(Variations[i].hasBeenTested == false)
                {
                    Variations[i].hasBeenTested = true;
                    return Variations[i];
                }
            }
            return null;
        }

        internal static void ApplyChangesBasedOnLatestWinner(ref WesofHtmlTag[] tags, ref ServerResponse sRep)
        {
            int idIndex = 0;
            for (idIndex = 0; idIndex < Variations.Capacity; idIndex++)
            {
                if (Variations[idIndex].id == CurrentWinnerControl.id)
                    break;
            }


            foreach (WesofHtmlTag tag in tags)
            {
                GenerateVariant(idIndex, tag.fullTagString);
                sRep.ModifiedBody = sRep.ModifiedBody.Replace(CurrentWinnerControl.UnModified, CurrentWinnerControl.Modified);                
            }
        }

        public static bool IsAllVariationTested()
        {
            bool result = true;
            for(int i = 0; i < Variations.Count; i++)
            {
                if(Variations[i].hasBeenTested == false)
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }


        public static AddToCartButtonVariation GetVariant(int id, ref string original)
        {
            //Check if the id is 0. If it is, this is considered to be the absolute control. 
            if(id == 0)
            {
                //Keep the modified and unmodified text the same. This is has some performance hit.
                AbsoluteControl.UnModified = original;
                AbsoluteControl.Modified = original;
                return AbsoluteControl;
            }



            int idIndex = 0;
            for(idIndex = 0; idIndex < Variations.Capacity; idIndex++)
            {
                if (Variations[idIndex].id == id)
                    break;
            }

            //Could not find variation with the ID number
            if (idIndex == Variations.Capacity)
            {
                if (Configuration.IsDebugModeOn)
                    _CLI.Out("AddToCartButton: Cannot find the variant with id: " + id.ToString() + "):\n" , true, false);

                return null;
            }

            GenerateVariant(idIndex, original);
            return Variations[idIndex];
        }

        private static void GenerateVariant(int id, string sourceText)
        {
            Variations[id].UnModified = sourceText;
            string modified = sourceText;


            if(Variations[id].colorPalette != null && Variations[id].addToCartTextCopies != null)
            {
                ChangeCopyAndColour(id);
                return;
            }

            if(Variations[id].colorPalette != null)
            {
                //Change the colour of the button
                ColorChange(id);
            }

            if(Variations[id].addToCartTextCopies != null)
            {
                //Change the copy of the button's text
                CopyChanger(id);
            }
        }


        private static void ChangeCopyAndColour(int id)
        {
            //First change the color
            string original = "<a";
            string modified = original + " style =\"background-color:" + Variations[id].colorPalette + "\" ";
            Variations[id].Modified = Variations[id].UnModified.Replace(original, modified);

            //Change the copy on the variant where the color already changed
            string original2 = ">Add to cart<";
            string modified2 = ">" + Variations[id].addToCartTextCopies + "<";
            Variations[id].Modified = Variations[id].Modified.Replace(original2, modified2);
        }


        /// <summary>
        /// Changes the coloour of the button based on the unmodified string
        /// </summary>
        /// <param name="id"></param>
        private static void ColorChange(int id)
        {
            string original = "<a";
            string modified = original + " style =\"background-color:" + Variations[id].colorPalette + "\" ";

            Variations[id].Modified = Variations[id].UnModified.Replace(original, modified);            
        }

        /// <summary>
        /// Changes the text copy of the button based on the unmodified string
        /// </summary>
        /// <param name="id"></param>
        private static void CopyChanger(int id)
        {
            string original = ">Add to cart<";
            string modified = ">" + Variations[id].addToCartTextCopies + "<";

            Variations[id].Modified = Variations[id].UnModified.Replace(original, modified);
        }


        /// <summary>
        /// Called when a addToCartButtonTest completes, and a new winner found. 
        /// </summary>
        /// <param name="id"></param>
        public static void SetNewWinnerControl(int id)
        {
            if(id == 0)
            {
                CurrentWinnerControl = AbsoluteControl;
            }
            else
            {
                for (int i = 0; i < Variations.Count; i++)
                {
                    if(Variations[i].id == id)
                    {
                        CurrentWinnerControl = Variations[i];
                        return;
                    }
                }
            }
        }

        public static AddToCartButtonVariation GetCurrentWinnderControl()
        {
            return CurrentWinnerControl;
        }


    }
}
