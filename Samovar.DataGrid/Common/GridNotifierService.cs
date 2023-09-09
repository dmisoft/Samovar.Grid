using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal class GridNotifierService
    {
        internal async Task AfterEditingEnd()
        {
            if (NotifyAfterEditingEnd != null)
            {
                await NotifyAfterEditingEnd.Invoke();
            }
        }

        internal async Task AfterPagingChange()
        {
            if (NotifyAfterPagingChange != null)
            {
                await NotifyAfterPagingChange.Invoke();
            }
        }
        internal async Task AfterSort()
        {
            if (NotifyAfterSort != null)
            {
                await NotifyAfterSort.Invoke();
            }
        }
        internal async Task AfterFilter()
        {
            if (NotifyAfterFilter != null)
            {
                await NotifyAfterFilter.Invoke();
            }
        }
        internal async Task AfterScroll()
        {
            if (NotifyAfterScroll != null)
            {
                await NotifyAfterScroll.Invoke();
            }
        }

        internal async Task AfterKeyDown(KeyboardEventArgs args)
        {
            if (NotifyAfterKeyDown != null)
            {
                await NotifyAfterKeyDown.Invoke(args);
            }
        }
        internal async Task AfterGridResize()
        {
            if (NotifyAfterGridResize != null)
            {
                await NotifyAfterGridResize.Invoke();
            }
        }
        internal async Task AfterWindowMouseMove(int windowX, int windowY)
        {
            if (NotifyAfterWindowMouseMove != null)
            {
                await NotifyAfterWindowMouseMove.Invoke(windowX, windowY);
            }
        }

        internal async Task OnFilterModeChange(byte filterMode, string targetFilterMenuContainerId)
        {
            if (NotifyOnFilterModeChange != null)
            {
                await NotifyOnFilterModeChange.Invoke(filterMode, targetFilterMenuContainerId);
            }
        }

        internal async Task OnClearFilter()
        {
            if (NotifyOnClearFilter != null)
            {
                await NotifyOnClearFilter.Invoke();
            }
        }

        //public event Func<Dictionary<Guid, ColumnMetadata>, Task> Notify;
        internal event Func<Task> NotifyAfterEditingEnd;
        internal event Func<Task> NotifyAfterPagingChange;
        internal event Func<Task> NotifyAfterSort;
        internal event Func<Task> NotifyAfterFilter;
        internal event Func<Task> NotifyAfterScroll;
        internal event Func<KeyboardEventArgs, Task> NotifyAfterKeyDown;
        internal event Func<Task> NotifyAfterGridResize;
        internal event Func<int, int, Task> NotifyAfterWindowMouseMove;
        internal event Func<Task> NotifyOnClearFilter;

        /// <summary>
        /// Parameters
        /// 1. byte - filter mode
        /// 2. string - TargetFilterMenuContainerId
        /// </summary>
        internal event Func<byte, string, Task> NotifyOnFilterModeChange;
    }
}
