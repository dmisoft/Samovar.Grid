﻿using System;

namespace Samovar.Blazor
{
    public partial class SmDataGridFilterHeaderScrollbar<TItem>
        : SmDesignComponentBase ,IDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public ILayoutService LayoutService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Dispose()
        {
        }
    }
}
