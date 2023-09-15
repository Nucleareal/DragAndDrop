using System;
using System.Collections.Generic;
using System.Text;

namespace StellaDragAndDropNS
{
    public class DeleteContextMenuItem : ContextMenuItem
    {
        public DeleteContextMenuItem(ITranslator translator) : base("delete_term", translator, Delete)
        {
        }

        private static void Delete(GameCard obj)
        {
            obj.DestroyCard();
        }
    }
}
