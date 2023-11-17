export const dataGridInstances = {}

export const gridStateVars =
{
    gridDotNetRef: undefined,
    isMouseDown: false,
    colMetaId: '',
    innerGridId: '',
    innerGridBodyTableId: '',
    visibleGridColumnCellId: '',
    hiddenGridColumnCellId: '',
    filterGridColumnCellId: '',
    filterMenuId: '',

    visibleEmptyColumnId: '',
    hiddenEmptyColumnId: '',
    filterEmptyColumnId: '',

    emptyColumnDictId: '',
    emptyColWidth: 0,
    startMouseMoveX: 0,
    oldAbsoluteVisibleWidthValue: 0,
    fitColumnsToTableWidth: undefined,
    oldAbsoluteEmptyColVisibleWidthValue: 0,
    newVisibleAbsoluteWidthValue: 0,
    innerGridWidth: 0,
    gridColWidthSum: 0
}

export const windowStateVars =
{
    isCtrlKeyDown: false,
    isShiftKeyDown: false
}

export function measureScrollbar() {
    var scrollDiv = document.createElement('div');
    scrollDiv.className = 's-scrollbar-measure';
    document.body.appendChild(scrollDiv);

    // Get the scrollbar width
    var scrollbarWidth = scrollDiv.offsetWidth - scrollDiv.clientWidth;
    // Delete the div 
    document.body.removeChild(scrollDiv);

    return scrollbarWidth;
}
export function measureTableFilterHeight(tableClass, tableHeaderClass, filterToggleButtonClass, testId) {
    var table = document.createElement('table');
    table.setAttribute('style', 'margin:0;padding:0;table-layout:fixed;')
    table.className = tableClass;

    var thead = document.createElement('thead');
    thead.className = tableHeaderClass;
    table.appendChild(thead);

    var tr = document.createElement('tr');
    tr.id = testId;
    thead.appendChild(tr);

    var th = document.createElement('th');
    tr.appendChild(th);

    var div = document.createElement('div');
    div.setAttribute('style', 'display:flex !important;display:-ms-flexbox!important;-ms-flex-direction:row !important;flex-direction:row !important;');
    th.appendChild(div);

    var button = document.createElement('button');
    button.textContent = '*a*';
    button.className = filterToggleButtonClass;
    button.setAttribute('style', 'padding-right:3px;padding-left:3px;');
    div.appendChild(button);

    var input = document.createElement('input');
    input.className = 'form-control';
    div.appendChild(input);

    document.body.appendChild(table);

    // Get the scrollbar width
    var rowHeight = document.getElementById(testId).clientHeight;
    // Delete the table
    document.body.removeChild(table);

    return rowHeight;
    //<table style="margin:0;padding:0;table-layout:fixed;@(DataGrid.MinGridWidth > 0 ? " min-width:" + DataGrid.MinGridWidth.ToString(CultureInfo.InvariantCulture) + "px; " : "")" class="@DataGrid.TableTagClass" >
    //    <thead class="@DataGrid.TheadTagClass">
    //        <tr @ref="DataGrid.GridFilterRef">
    //        @foreach (ColumnMetadata colMeta in GridColumnService.Columns.Values.Where(md => md.ColumnType == Grid.Data.GridColumnType.Data).OrderBy(cm => cm.ColumnOrder))
    //        {
    //            <th
}

export function measureTableRowHeight(tableClass, testId) {
    let table = document.createElement('table');
    let tr = document.createElement('tr');
    let td = document.createElement('td');

    table.appendChild(tr);
    tr.id = testId;
    tr.appendChild(td);
    td.textContent = 'CONTENT';

    table.className = tableClass;
    document.body.appendChild(table);

    // Get the scrollbar width
    let rowHeight = document.getElementById(testId).clientHeight;

    // Delete the table 
    document.body.removeChild(table);

    return rowHeight;
}

export function getWindowIsCtrlKeyDown() {
    return windowStateVars.isCtrlKeyDown;
}

export function getWindowIsShiftKeyDown() {
    return windowStateVars.isShiftKeyDown;
}

export function getElementWidthByRef(element) {
    if (element === null)
        return 0;
    return element.clientWidth;
}

export function getElementWidth(elementId) {
    return document.getElementById(elementId).clientWidth;
}

export function synchronizeGridHeaderScroll(elementRef, gridHeaderContainerId) {
    elementRef.addEventListener('scroll', function () {
        let elmnt = document.getElementById(gridHeaderContainerId);
        if (elmnt !== null) {
            elmnt.scrollLeft = elementRef.scrollLeft;
        }
    });
}

//export function synchronizeGridInnerBodyScroll(elementRef, gridInnerBodyId) {
//    elementRef.addEventListener("scroll", function (event) {
//        translateYOffset = $("#" + gridInnerBodyId).attr("translate_y_offset");
//        if (isNaN(translateYOffset) || isNaN(elementRef.scrollTop)) {
//            return;
//        }
//        translateY = parseInt(translateYOffset) + parseInt(elementRef.scrollTop);
//        $("#" + gridInnerBodyId).css("transform", "translateY(" + translateY + "px)");
//    });
//}

export function isPartialInView(elementId, innerGridId) {
    //var elementTopValue = $("#" + elementId).offset().top;
    let elmnt = document.getElementById(elementId);
    let innerGrid = document.getElementById(innerGridId);
    if (elmnt !== null && innerGrid !== null) {
        let elementTopValue = elmnt.offsetTop;
        let viewportTopValue = innerGrid.offsetTop;

        let elementBottomValue = elementTopValue + elmnt.clientHeight;
        let viewportBottomValue = viewportTopValue + innerGrid.clientHeight;
        let probablyNotInView = Boolean(elementBottomValue > viewportBottomValue || elementTopValue < viewportTopValue);

        return JSON.stringify({ ProbablyNotInView: probablyNotInView, elementTop: elementTopValue, elementBottom: elementBottomValue, viewportTop: viewportTopValue, viewportBottom: viewportBottomValue });
    }

    //var elementTopValue = $("#" + elementId).offset().top;
    //var viewportTopValue = $("#" + innerGridId).offset().top;

    //var elementBottomValue = elementTopValue + $("#" + elementId).outerHeight();
    //var viewportBottomValue = viewportTopValue + $("#" + innerGridId).height();
    //var probablyNotInView = Boolean(elementBottomValue > viewportBottomValue || elementTopValue < viewportTopValue);

    //return JSON.stringify({ ProbablyNotInView: probablyNotInView, elementTop: elementTopValue, elementBottom: elementBottomValue, viewportTop: viewportTopValue, viewportBottom: viewportBottomValue });
}

export function isScrollbarVisible(innerGridId) {
    let innerGrid = document.getElementById(innerGridId);

    if (innerGrid !== null) {
        return Boolean(innerGrid.scrollHeight > innerGrid.clientHeight);
    }
}

export function getElementScrollTopByRef(element) {
    return element.scrollTop;
}

export function startColumnWidthChangeMode(_gridDotNetRef, _gridColWidthSum, _colMetaId, _innerGridId, _innerGridBodyTableId, _visibleGridColumnCellId, _hiddenGridColumnCellId, _filterGridColumnCellId, _visibleEmptyColumnId, _hiddenEmptyColumnId, _filterEmptyColumnId, _emptyColumnDictId, _startMouseMoveX, _oldAbsoluteVisibleWidthValue, _fitColumnsToTableWidth, _oldAbsoluteEmptyColVisibleWidthValue) {
    gridStateVars.gridDotNetRef = _gridDotNetRef;
    gridStateVars.isMouseDown = true;
    gridStateVars.gridColWidthSum = _gridColWidthSum;
    gridStateVars.colMetaId = _colMetaId;
    gridStateVars.innerGridId = _innerGridId;
    gridStateVars.innerGridBodyTableId = _innerGridBodyTableId;

    gridStateVars.visibleGridColumnCellId = _visibleGridColumnCellId;
    gridStateVars.hiddenGridColumnCellId = _hiddenGridColumnCellId;
    gridStateVars.filterGridColumnCellId = _filterGridColumnCellId;

    gridStateVars.visibleEmptyColumnId = _visibleEmptyColumnId;
    gridStateVars.hiddenEmptyColumnId = _hiddenEmptyColumnId;
    gridStateVars.filterEmptyColumnId = _filterEmptyColumnId;

    gridStateVars.emptyColumnDictId = _emptyColumnDictId;

    gridStateVars.startMouseMoveX = _startMouseMoveX;
    gridStateVars.oldAbsoluteVisibleWidthValue = _oldAbsoluteVisibleWidthValue;
    gridStateVars.fitColumnsToTableWidth = _fitColumnsToTableWidth;
    gridStateVars.oldAbsoluteEmptyColVisibleWidthValue = _oldAbsoluteEmptyColVisibleWidthValue;
    gridStateVars.newVisibleAbsoluteWidthValue = gridStateVars.oldAbsoluteVisibleWidthValue;

    gridStateVars.innerGridWidth = getElementWidth(_innerGridId);

    gridStateVars.emptyColWidth = 0;
    if (gridStateVars.gridColWidthSum < gridStateVars.innerGridWidth && gridStateVars.fitColumnsToTableWidth === false) {
        gridStateVars.emptyColWidth = gridStateVars.innerGridWidth - gridStateVars.gridColWidthSum - 1;
    }
}

export function toggleFilterPopupMenu(toggleFilterMenuButtonId, filterMenuContainerId, _filterMenuId, toogleToShow) {
    gridStateVars.filterMenuId = _filterMenuId;
    var filterMenu = document.getElementById(_filterMenuId);
    if (toogleToShow == true) {
        var filterMenuContainer = document.getElementById(filterMenuContainerId);
        var t = $('#' + toggleFilterMenuButtonId).offset().top + 33;
        var l = $('#' + toggleFilterMenuButtonId).offset().left;

        filterMenuContainer.style.top = t + 'px';
        filterMenuContainer.style.left = l + 'px';
    }
    filterMenu.classList.toggle('s-show');
}

export function stopColumnWidthChangeMode(dotNetRef) {
    gridStateVars.gridDotNetRef = undefined;
    gridStateVars.isMouseDown = false;
    gridStateVars.gridColWidthSum = 0;
    gridStateVars.colMetaId = '';
    gridStateVars.innerGridId = '';
    gridStateVars.innerGridBodyTableId = '';
    gridStateVars.visibleGridColumnCellId = '';
    gridStateVars.hiddenGridColumnCellId = '';
    gridStateVars.filterGridColumnCellId = '';

    gridStateVars.visibleEmptyColumnId = '';
    gridStateVars.hiddenEmptyColumnId = '';
    gridStateVars.filterEmptyColumnId = '';

    gridStateVars.emptyColumnDictId = '';
    gridStateVars.emptyColWidth = 0;
    gridStateVars.startMouseMoveX = 0;
    gridStateVars.oldAbsoluteVisibleWidthValue = 0;
    gridStateVars.fitColumnsToTableWidth = undefined;
    gridStateVars.oldAbsoluteEmptyColVisibleWidthValue = 0;
    gridStateVars.newVisibleAbsoluteWidthValue = 0;
    gridStateVars.innerGridWidth = 0;
}

//Mouse up
export function raise_Js_Window_MouseUp_OnDotNetRef(event) {
    if (gridStateVars.isMouseDown === true) {
        gridStateVars.gridDotNetRef.invokeMethodAsync('Js_Window_MouseUp', gridStateVars.colMetaId, gridStateVars.newVisibleAbsoluteWidthValue, gridStateVars.emptyColumnDictId, gridStateVars.emptyColWidth);
        stopColumnWidthChangeMode(gridStateVars.gridDotNetRef);
    }
}

export function add_Window_MouseUp_EventListener(dotNetRef) {
    window.removeEventListener('mouseup', raise_Js_Window_MouseUp_OnDotNetRef);
    window.addEventListener('mouseup', raise_Js_Window_MouseUp_OnDotNetRef);
}

export function remove_Window_MouseUp_EventListener() {
    window.removeEventListener('mouseup', raise_Js_Window_MouseUp_OnDotNetRef);
}

export function onWindowUpMove(event) {

}

//Mouse move event handling
export function raise_Js_Window_MouseMove_OnDotNetRef(event) {
    event.data.ref.invokeMethodAsync('JS_AfterWindowMouseMove', event.data.pageX, event.data.pageY);
}

export function add_Window_MouseMove_EventListener(dotNetRef) {
    window.removeEventListener('mousemove', onWindowMouseMove);
    window.addEventListener('mousemove', onWindowMouseMove);
}
export function onWindowMouseMove (event) {
    if (gridStateVars.isMouseDown) {
        var delta = event.pageX - gridStateVars.startMouseMoveX;
        if (gridStateVars.oldAbsoluteVisibleWidthValue + delta < 10) {
            gridStateVars.newVisibleAbsoluteWidthValue = 10;
            delta = gridStateVars.newVisibleAbsoluteWidthValue - gridStateVars.oldAbsoluteVisibleWidthValue;
        }
        else {
            gridStateVars.newVisibleAbsoluteWidthValue = gridStateVars.oldAbsoluteVisibleWidthValue + delta;
        }

        if (gridStateVars.gridColWidthSum + delta < gridStateVars.innerGridWidth && gridStateVars.fitColumnsToTableWidth === false) {
            gridStateVars.emptyColWidth = gridStateVars.innerGridWidth - (gridStateVars.gridColWidthSum + delta) - 1;
        }
        else {
            gridStateVars.emptyColWidth = 0;
        }

        document.getElementById(gridStateVars.visibleGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';
        document.getElementById(gridStateVars.hiddenGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';
        document.getElementById(gridStateVars.filterGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';

        document.getElementById(gridStateVars.visibleEmptyColumnId).style.width = gridStateVars.emptyColWidth + 'px';
        document.getElementById(gridStateVars.hiddenEmptyColumnId).style.width = gridStateVars.emptyColWidth + 'px';
        document.getElementById(gridStateVars.filterEmptyColumnId).style.width = gridStateVars.emptyColWidth + 'px';
    }
}

export function remove_Window_MouseMove_EventListener() {
    window.removeEventListener('mousemove', onWindowMouseMove);
}

export function scrollToTop(elmntId) {
    var elmnt = document.getElementById(elmntId);
    if (elmnt !== null) {
        elmnt.scrollIntoView(false); // Top
    }
    else {
        console.log('scrollToTop: elementid not found: ' + elmntId);
    }
}
export function scrollToBottom(elmntId) {
    var elmnt = document.getElementById(elmntId);
    if (elmnt !== null) {
        elmnt.scrollIntoView(false); // Bottom
    }
}

//Window resize event handling
export function raise_Js_Window_OnResize_OnDotNetRef(event) {
    for (var dataGridId in dataGridInstances) {
        dataGridInstances[dataGridId].invokeMethodAsync('JS_AfterWindowResize');
    }
}

export function add_Window_OnResize_EventListener(dataGridId, dataGridDotNetRef) {
    dataGridInstances[dataGridId] = dataGridDotNetRef;
    window.addEventListener('resize', raise_Js_Window_OnResize_OnDotNetRef);
}

export function disposeDataGridInstance(dataGridId) {
    delete dataGridInstances[dataGridId];
}

export function getElementHeight(elementId) {
    var element = document.getElementById(elementId);
    if (element !== null) {
        return element.clientHeight;
    }
    else {
        console.log('getElementHeight: elementid not found: ' + elementId);
        return 1;
    }
}

export function getElementHeightByRef(element) {
    if (element !== null) {
        return element.clientHeight;
    }
    else {
        console.log('getElementHeightByRef: elementid not found');
        return 1;
    }
}

export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}

//Inner grid scroll handling
export function raise_Js_InnerGrid_AfterScroll_OnDotNetRef(event) {
    //var innerGridScrollTop = $('#' + event.data.innerGridId).scrollTop();
    //event.data.ref.invokeMethodAsync("Js_InnerGrid_AfterScroll", innerGridScrollTop);
    for (var dataGridId in dataGridInstances) {
        dataGridInstances[dataGridId].invokeMethodAsync('JS_AfterWindowResize');
    }
}
export function add_GridInner_OnScroll_EventListener(innerGridId, dotNetRef) {
    //dataGridInstances[innerGridId] = dotNetRef;
    document.getElementById(innerGridId).addEventListener('scroll', function (event) {
        var innerGridScrollTop = document.getElementById(innerGridId).scrollTop;
        dotNetRef.invokeMethodAsync("Js_InnerGrid_AfterScroll", innerGridScrollTop);
    })
}
export function remove_GridInner_OnScroll_EventListener(innerGridId, dotNetRef) {
    //dataGridInstances[innerGridId] = dotNetRef;
    document.getElementById(innerGridId).removeEventListener('scroll', function (event) {
        var innerGridScrollTop = document.getElementById(innerGridId).scrollTop;
        dotNetRef.invokeMethodAsync("Js_InnerGrid_AfterScroll", innerGridScrollTop);
    })
}

export function scrollElementVerticalByValue(elementId, scrollValue) {
    document.getElementById(elementId).scrollTop = scrollValue;
}


export function dragElement(elmnt) {
    var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
    if (document.getElementById(elmnt.id + "header")) {
        // if present, the header is where you move the DIV from:
        document.getElementById(elmnt.id + "header").onmousedown = dragMouseDown;
    } else {
        // otherwise, move the DIV from anywhere inside the DIV:
        elmnt.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        e = e || window.event;
        e.preventDefault();
        // get the mouse cursor position at startup:
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        // call a function whenever the cursor moves:
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        // calculate the new cursor position:
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        // set the element's new position:
        elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
        elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        // stop moving when mouse button is released:
        document.onmouseup = null;
        document.onmousemove = null;
    }
}


window.addEventListener('click', function (event) {
    if (gridStateVars.filterMenuId !== '') {
        document.getElementById(gridStateVars.filterMenuId).classList.toggle('s-show');
        gridStateVars.filterMenuId = '';
    }
});

window.addEventListener('keydown', function (event) {
    windowStateVars.isCtrlKeyDown = event.ctrlKey;
    windowStateVars.isShiftKeyDown = event.shiftKey;
}, true);

window.addEventListener('keyup', function (event) {
    windowStateVars.isCtrlKeyDown = event.ctrlKey;
    windowStateVars.isShiftKeyDown = event.shiftKey;
}, true);

window.GridFunctions = {
    //Grid body keydown handling
    raise_Js_GridBody_KeyDown_OnDotNetRef: function (event) {
        event.data.ref.invokeMethodAsync(event.data.callbackName, event.originalEvent.key, event.data.elementId);
    },
    add_GridBody_KeyDown_EventListener: function (gridBodyId, dotNetRef, callbackFunction) {
        $('#' + gridBodyId).off('keydown');
        $('#' + gridBodyId).on('keydown', {
            elementId: gridBodyId,
            ref: dotNetRef,
            callbackName: callbackFunction
        }, GridFunctions.raise_Js_GridBody_KeyDown_OnDotNetRef);
    },
    remove_GridBody_KeyDown_EventListener: function (gridBodyId) {
        $('#' + gridBodyId).off('keydown', GridFunctions.raise_Js_GridBody_KeyDown_OnDotNetRef);
    },


    consoleOutput: function (msg) {
        console.log(msg);
        return true;
    },

    isElementVisible: function (el) {
        var rect = el.getBoundingClientRect(),
            vWidth = window.innerWidth || doc.documentElement.clientWidth,
            vHeight = window.innerHeight || doc.documentElement.clientHeight,
            efp = function (x, y) { return document.elementFromPoint(x, y) };
        return el.contains(efp(rect.left, rect.top)) || el.contains(efp(rect.right, rect.top)) || el.contains(efp(rect.right, rect.bottom)) || el.contains(efp(rect.left, rect.bottom));
    },

    isInView: function (elementId, innerGridId) {
        var elementTopValue = $('#' + elementId).offset().top;
        var elementBottomValue = elementTopValue + $('#' + elementId).outerHeight();
        var viewportTopValue = $('#' + innerGridId).offset().top;
        var viewportBottomValue = viewportTopValue + $('#' + innerGridId).height();
        var inView = Boolean(elementBottomValue > viewportTopValue && elementTopValue < viewportBottomValue);

        return JSON.stringify({ IsInView: inView, elementTop: elementTopValue, elementBottom: elementBottomValue, viewportTop: viewportTopValue, viewportBottom: viewportBottomValue });
    },

    scrollElementInView: function (elementId, innerGridId) {
        var elementTopValue = $('#' + elementId).offset().top;
        var elementBottomValue = elementTopValue + $('#' + elementId).outerHeight();
        var viewportTopValue = $('#' + innerGridId).offset().top;
        var viewportBottomValue = viewportTopValue + $('#' + innerGridId).height();
        var partialOnTopNotInView = Boolean(elementTopValue < viewportTopValue);
        var partialOnBottomNotInView = Boolean(elementBottomValue > viewportBottomValue);

        if (partialOnTopNotInView) {
            var actualScrollVal = $('#' + innerGridId).scrollTop();
            var newScrollVal = actualScrollVal - (viewportTopValue - elementTopValue);
            $('#' + innerGridId).scrollTop(newScrollVal);
        }
        else if (partialOnBottomNotInView) {
            var actualScrollVal = $('#' + innerGridId).scrollTop();
            var newScrollVal = actualScrollVal + (elementBottomValue - viewportBottomValue);
            $('#' + innerGridId).scrollTop(newScrollVal);
        }
    },

    getElementScrollTop: function (elementId) {
        return $('#' + elementId).scrollTop();
    },
    getElementScrollLeft: function (elementId) {
        return $('#' + elementId).scrollLeft();
    },
    getElementScrollLeftByRef: function (element) {
        return element.scrollLeft;
    }
};

window.dataGridScrollbar = {
    handleMouseDown: function (dotNetObjectRef) {
        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);

        function handleMouseMove(e) {
            dotNetObjectRef.invokeMethodAsync('HandleMouseMove', e.clientY);
            console.log("Test:" + e.clientY);
        }

        function handleMouseUp() {
            dotNetObjectRef.invokeMethodAsync('HandleMouseUp');
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        }
    },

    handleMouseMove: function (deltaY) {
        //var thumb = document.getElementById('customScrollbarThumb');
        //if (thumb && thumb.style) {
        //    thumb.style.top = Math.max(0, thumb.offsetTop + deltaY) + 'px';
        //}
    },

    handleMouseUp: function () {
        // Clean up any resources if needed
    }
};