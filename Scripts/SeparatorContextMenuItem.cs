using System;
using System.Collections.Generic;
using System.Text;

namespace StellaDragAndDropNS
{
    public class SeparatorContextMenuItem : ContextMenuItem
    {
        public SeparatorContextMenuItem(ITranslator translator) : base("------", translator, DoNothing)
        {
        }

        private static void DoNothing(GameCard obj)
        {
        }

        public override bool IsSeparator => true;
    }
}
