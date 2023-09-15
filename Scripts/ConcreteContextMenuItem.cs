using System;
using System.Collections.Generic;
using System.Text;

namespace StellaDragAndDropNS
{
    public class ConcreteContextMenuItem : ContextMenuItem
    {
        public ConcreteContextMenuItem(string textTerm, ITranslator translator, Action<GameCard> action) : base(textTerm, translator, action)
        {
        }
    }
}
