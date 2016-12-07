using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wesof.MVTManager
{
    class PostParser
    {

        private CommandLineInterface _CLI;
        private LinkedList<WesofHtmlTag> _Tags;
        private ServerResponse _sRep;

        public PostParser(CommandLineInterface cli, ref LinkedList<WesofHtmlTag> tags, ref ServerResponse sRep)
        {
            _CLI = cli;
            _Tags = tags;
            _sRep = sRep;
        }

        /// <summary>
        /// This method should work like a hub to match the object types to the appropriate handling modules.
        /// However, this now only looks for the AddToCartButton type, as no other will be implemented. 
        /// (This method will need change when new modules/test types are implemented)
        /// </summary>
        public void SendTagsToMatchingMvtModules()
        {
            if (_Tags.First == null)
                return;

            WesofHtmlTag[] addToCartButtons;

            //Count up the number of testable "add to cart" CTA objects
            int countEnabledAddToButtonTypeObjects = 0;
            LinkedListNode<WesofHtmlTag> iterator = _Tags.First;
            while(iterator != null)
            {
                if(iterator.Value.testingEnabled == true && iterator.Value.objectType.ToUpper() == Configuration.MVT_MODULES_LIST[0].ToUpper())
                {
                    countEnabledAddToButtonTypeObjects++;
                }
                iterator = iterator.Next;
            }

            //Instantiate an array for testable "add to cart" CTA objects
            if(countEnabledAddToButtonTypeObjects > 0)
            {
                addToCartButtons = new WesofHtmlTag[countEnabledAddToButtonTypeObjects];
                int index = 0;

                //Iterate through the LL again and add the tags to the array
                iterator = _Tags.First;
                while(iterator != null)
                {
                    if (iterator.Value.testingEnabled == true && iterator.Value.objectType.ToUpper() == Configuration.MVT_MODULES_LIST[0].ToUpper())
                    {
                        addToCartButtons[index] = iterator.Value;
                        index++;
                    }
                    iterator = iterator.Next;
                }

                //We have all testable addToCartButtons in an array. Let's pass them to the test manager.
                Configuration.MainController.GetTestManager().HandleAddToCartCTATags(addToCartButtons, ref _sRep);
            }
            else
            {
                return;
            }

        }

    }
}
