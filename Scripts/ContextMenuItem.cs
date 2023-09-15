using System;
using System.Collections.Generic;
using System.Text;

namespace StellaDragAndDropNS
{
    public abstract class ContextMenuItem
    {
        protected List<ContextMenuItem> _children = new List<ContextMenuItem>();
        protected string _textTerm;
        protected ITranslator _translator;
        protected Action<GameCard> _action;
        private string textTerm;
        private ITranslator translator;

        public ContextMenuItem(string textTerm, ITranslator translator, Action<GameCard> action)
        {
            _textTerm = textTerm;
            _translator = translator;
            _action = action;
        }

        public void AddChild(ContextMenuItem child)
        {
            _children.Add(child);
        }

        public string Text => _translator.Translate(_textTerm);

        public virtual bool IsSeparator => false;

        public virtual bool IsVisiable(GameCard card) => true;
        public virtual bool IsClickable(GameCard card) => IsVisiable(card) && !IsSeparator;
        public virtual bool HasChild => _children.Count > 0;

        public virtual void DoAction(GameCard target)
        {
            _action(target);
        }
    }

    public interface ITranslator
    {
        public string Translate(string term);
    }
}
