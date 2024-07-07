export const dataGridInstances = {}

export const gridStateVars =
{
    minColumnWidth: 50,
    gridDotNetRef: undefined,
    isMouseDown: false,
    colMetaId: '',
    innerGridId: '',
    innerGridBodyTableId: '',
    visibleGridColumnCellId: '',
    hiddenGridColumnCellId: '',
    filterGridColumnCellId: '',
    filterMenuId: '',

    visibleHeaderEmptyColumnId: '',
    hiddenHeaderEmptyColumnId: '',
    filterHeaderEmptyColumnId: '',

    emptyColumnDictId: '',
    emptyColWidth: 0,
    emptyHeaderColWidth: 0,
    startMouseMoveX: 0,
    oldAbsoluteVisibleWidthValue: 0,
    fitColumnsToTableWidth: undefined,
    oldAbsoluteEmptyColVisibleWidthValue: 0,
    newVisibleAbsoluteWidthValue: 0,
    innerGridWidth: 0,
    outerGridWidth: 0,
    gridColWidthSum: 0,
    rightSideColumnId: null,
    rightSideCellId: null,
    rightSideFilterCellId: null,
    rightSideHiddenCellId: null,
    oldRightSideColumnWidth: 0,
    newRightSideColumnWidth: 0
}

export const windowStateVars =
{
    isCtrlKeyDown: false,
    isShiftKeyDown: false
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
    if (elementRef === null)
        return;

    elementRef.addEventListener('scroll', function () {
        let elmnt = document.getElementById(gridHeaderContainerId);
        if (elmnt !== null) {
            elmnt.scrollLeft = elementRef.scrollLeft;
        }
    });
}

export function startColumnWidthChangeMode(_gridDotNetRef, _gridColWidthSum, _colMetaId, _innerGridId, _innerGridBodyTableId, _visibleGridColumnCellId, _hiddenGridColumnCellId, _filterGridColumnCellId, _visibleEmptyColumnId, _hiddenEmptyColumnId, _filterEmptyColumnId, _emptyColumnDictId, _startMouseMoveX, _oldAbsoluteVisibleWidthValue, _fitColumnsToTableWidth, _oldAbsoluteEmptyColVisibleWidthValue, _rightSideColumnId, _rightSideCellId, _rightSideColumnWidth, _rightSideFilterCellId, _rightSideHiddenCellId, _outerGridId) {
    gridStateVars.gridDotNetRef = _gridDotNetRef;
    gridStateVars.isMouseDown = true;
    gridStateVars.gridColWidthSum = _gridColWidthSum;
    gridStateVars.colMetaId = _colMetaId;
    gridStateVars.innerGridId = _innerGridId;
    gridStateVars.innerGridBodyTableId = _innerGridBodyTableId;

    gridStateVars.visibleGridColumnCellId = _visibleGridColumnCellId;
    gridStateVars.hiddenGridColumnCellId = _hiddenGridColumnCellId;
    gridStateVars.filterGridColumnCellId = _filterGridColumnCellId;

    gridStateVars.visibleHeaderEmptyColumnId = _visibleEmptyColumnId;
    gridStateVars.hiddenHeaderEmptyColumnId = _hiddenEmptyColumnId;
    gridStateVars.filterHeaderEmptyColumnId = _filterEmptyColumnId;

    gridStateVars.emptyColumnDictId = _emptyColumnDictId;

    gridStateVars.startMouseMoveX = _startMouseMoveX;
    gridStateVars.oldAbsoluteVisibleWidthValue = _oldAbsoluteVisibleWidthValue;
    gridStateVars.newVisibleAbsoluteWidthValue = gridStateVars.oldAbsoluteVisibleWidthValue;

    gridStateVars.fitColumnsToTableWidth = _fitColumnsToTableWidth;

    gridStateVars.oldAbsoluteEmptyColVisibleWidthValue = _oldAbsoluteEmptyColVisibleWidthValue;

    gridStateVars.innerGridWidth = getElementWidth(_innerGridId);
    gridStateVars.outerGridWidth = getElementWidth(_outerGridId);

    //right side column
    gridStateVars.rightSideColumnId = _rightSideColumnId;
    gridStateVars.rightSideCellId = _rightSideCellId;
    gridStateVars.oldRightSideColumnWidth = _rightSideColumnWidth;
    gridStateVars.newRightSideColumnWidth = gridStateVars.oldRightSideColumnWidth;
    gridStateVars.rightSideFilterCellId = _rightSideFilterCellId;
    gridStateVars.rightSideHiddenCellId = _rightSideHiddenCellId;
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

    gridStateVars.visibleHeaderEmptyColumnId = '';
    gridStateVars.hiddenHeaderEmptyColumnId = '';
    gridStateVars.filterHeaderEmptyColumnId = '';

    gridStateVars.emptyColumnDictId = '';
    gridStateVars.emptyColWidth = 0;
    gridStateVars.startMouseMoveX = 0;

    gridStateVars.fitColumnsToTableWidth = undefined;

    gridStateVars.oldAbsoluteVisibleWidthValue = 0;
    gridStateVars.newVisibleAbsoluteWidthValue = 0;

    gridStateVars.oldAbsoluteEmptyColVisibleWidthValue = 0;
    gridStateVars.innerGridWidth = 0;
    gridStateVars.outerGridWidth = 0;

    gridStateVars.rightSideColumnId = '';
    gridStateVars.rightSideCellId = '';
    gridStateVars.oldRightSideColumnWidth = 0;
    gridStateVars.newRightSideColumnWidth = 0;
    gridStateVars.rightSideFilterCellId = '';
    gridStateVars.rightSideHiddenCellId = '';
}

//Mouse up
export function raise_Js_Window_MouseUp_OnDotNetRef(event) {
    if (gridStateVars.isMouseDown === true) {
        gridStateVars.gridDotNetRef.invokeMethodAsync('Js_Window_MouseUp',
            gridStateVars.colMetaId,
            gridStateVars.newVisibleAbsoluteWidthValue,
            gridStateVars.emptyColumnDictId,
            gridStateVars.emptyColWidth,
            gridStateVars.rightSideColumnId,
            gridStateVars.newRightSideColumnWidth,
            gridStateVars.emptyHeaderColWidth);
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

export function onWindowMouseMove(event) {
    if (gridStateVars.isMouseDown) {
        if (gridStateVars.fitColumnsToTableWidth === 'None') {
            return;
        }
        var delta = event.pageX - gridStateVars.startMouseMoveX;
        var newTriggerColumnWidth = 0;
        var newRightSideColumnWidth = 0;

        if (gridStateVars.oldAbsoluteVisibleWidthValue + delta < gridStateVars.minColumnWidth) {
            newTriggerColumnWidth = gridStateVars.minColumnWidth;
            delta = gridStateVars.newVisibleAbsoluteWidthValue - gridStateVars.oldAbsoluteVisibleWidthValue;
        }
        else {
            newTriggerColumnWidth = gridStateVars.oldAbsoluteVisibleWidthValue + delta;
        }

        if (gridStateVars.rightSideCellId !== null && gridStateVars.fitColumnsToTableWidth === 'Block') {
            newRightSideColumnWidth = gridStateVars.oldRightSideColumnWidth - delta;
            if (newRightSideColumnWidth < gridStateVars.minColumnWidth) {
                newRightSideColumnWidth = gridStateVars.minColumnWidth;
                newTriggerColumnWidth = gridStateVars.oldAbsoluteVisibleWidthValue + gridStateVars.oldRightSideColumnWidth - gridStateVars.minColumnWidth;
                delta = newTriggerColumnWidth - gridStateVars.oldVisibleAbsoluteWidthValue;
            }

            gridStateVars.newRightSideColumnWidth = newRightSideColumnWidth;
            document.getElementById(gridStateVars.rightSideCellId).style.width = gridStateVars.newRightSideColumnWidth + 'px';
            document.getElementById(gridStateVars.rightSideFilterCellId).style.width = gridStateVars.newRightSideColumnWidth + 'px';
            document.getElementById(gridStateVars.rightSideHiddenCellId).style.width = gridStateVars.newRightSideColumnWidth + 'px';
        }

        gridStateVars.newVisibleAbsoluteWidthValue = newTriggerColumnWidth;

        if (gridStateVars.fitColumnsToTableWidth === 'Sliding' && gridStateVars.gridColWidthSum + delta < gridStateVars.outerGridWidth) {
            gridStateVars.emptyHeaderColWidth = gridStateVars.outerGridWidth - (gridStateVars.gridColWidthSum + delta);
        }
        else {
            gridStateVars.emptyHeaderColWidth = 0;

        }

        document.getElementById(gridStateVars.visibleGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';
        document.getElementById(gridStateVars.hiddenGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';
        document.getElementById(gridStateVars.filterGridColumnCellId).style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';

        var visibleHeaderEmptyColumn = document.getElementById(gridStateVars.visibleHeaderEmptyColumnId);
        var filterHeaderEmptyColumn = document.getElementById(gridStateVars.filterHeaderEmptyColumnId);

        if (gridStateVars.emptyHeaderColWidth !== 0) {
            // set new style instance to visibleHeaderEmptyColumn
            visibleHeaderEmptyColumn.style.width = gridStateVars.emptyHeaderColWidth + 'px';
            filterHeaderEmptyColumn.style.width = gridStateVars.emptyHeaderColWidth + 'px';
        }
        else {
            visibleHeaderEmptyColumn.style.width = undefined;
            filterHeaderEmptyColumn.style.width = undefined;
        }
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
//export function raise_Js_InnerGrid_AfterScroll_OnDotNetRef(event) {
//    for (var dataGridId in dataGridInstances) {
//        dataGridInstances[dataGridId].invokeMethodAsync('JS_AfterWindowResize');
//    }
//}
export function add_GridInner_OnScroll_EventListener(innerGridId, dotNetRef) {
    document.getElementById(innerGridId).addEventListener('scroll', function (event) {
        var innerGridScrollTop = document.getElementById(innerGridId).scrollTop;
        dotNetRef.invokeMethodAsync("Js_InnerGrid_AfterScroll", innerGridScrollTop);
    })
}
export function remove_GridInner_OnScroll_EventListener(innerGridId, dotNetRef) {
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
    }
};