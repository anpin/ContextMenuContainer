// MIT License
// Copyright (c) 2021 Pavel Anpin

using System;
using System.Collections.ObjectModel;

namespace APES.MAUI
{
    public class ContextMenuItems : ObservableCollection<BaseContextMenuItem>
    {
        public ContextMenuItem this[string text] => FindTextIndex(text);

        private ContextMenuItem FindTextIndex(string text)
        {
            foreach (var item in Items)
            {
                if (item is ContextMenuItem contextMenuItem && contextMenuItem.Text == text)
                    return contextMenuItem;
            }

            throw new ArgumentOutOfRangeException(nameof(text), $"Item with text {text} was not present");
        }
    }
}
