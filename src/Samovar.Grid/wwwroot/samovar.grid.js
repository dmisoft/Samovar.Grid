export const dataGridInstances = {}

export const gridStateVars =
{
    triggerColumnMinWidth: 50,
    rightSideColumnMinWidth: 50,
    gridDotNetRef: undefined,
    isMouseDown: false,
    colMetaId: '',
    innerGridId: '',
    innerGridBodyTableId: '',
    visibleGridColumnCellId: '',
    hiddenGridColumnCellId: '',
    filterGridColumnCellId: '',
    summaryGridColumnCellId: '',
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
    rightSideSummaryCellId: null,
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

export function getElementBoundingRect(element) {
    if (element === null)
        return { top: 0, left: 0, width: 0, height: 0 };
    const rect = element.getBoundingClientRect();
    return { top: rect.top, left: rect.left, width: rect.width, height: rect.height };
}

export function getElementScrollLeft(element) {
    if (element === null) return 0;
    return element.scrollLeft;
}

export function setElementScrollLeft(element, value) {
    if (element === null) return;
    element.scrollLeft = value;
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

export function startColumnWidthChangeMode(_gridDotNetRef, _gridColWidthSum, _colMetaId, _innerGridId, _innerGridBodyTableId, _visibleGridColumnCellId, _hiddenGridColumnCellId, _filterGridColumnCellId, _summaryGridColumnCellId, _visibleEmptyColumnId, _hiddenEmptyColumnId, _filterEmptyColumnId, _emptyColumnDictId, _startMouseMoveX, _oldAbsoluteVisibleWidthValue, _fitColumnsToTableWidth, _oldAbsoluteEmptyColVisibleWidthValue, _rightSideColumnId, _rightSideCellId, _rightSideColumnWidth, _rightSideFilterCellId, _rightSideHiddenCellId, _rightSideSummaryCellId, _outerGridId, _triggerColumnMinWidth, _rightSideColumnMinWidth) {
    gridStateVars.gridDotNetRef = _gridDotNetRef;
    gridStateVars.isMouseDown = true;
    gridStateVars.gridColWidthSum = _gridColWidthSum;
    gridStateVars.colMetaId = _colMetaId;
    gridStateVars.innerGridId = _innerGridId;
    gridStateVars.innerGridBodyTableId = _innerGridBodyTableId;

    gridStateVars.visibleGridColumnCellId = _visibleGridColumnCellId;
    gridStateVars.hiddenGridColumnCellId = _hiddenGridColumnCellId;
    gridStateVars.filterGridColumnCellId = _filterGridColumnCellId;
    gridStateVars.summaryGridColumnCellId = _summaryGridColumnCellId;

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
    gridStateVars.rightSideSummaryCellId = _rightSideSummaryCellId;

    gridStateVars.triggerColumnMinWidth = _triggerColumnMinWidth;
    gridStateVars.rightSideColumnMinWidth = _rightSideColumnMinWidth;
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
    gridStateVars.summaryGridColumnCellId = '';

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
    gridStateVars.rightSideSummaryCellId = '';

    gridStateVars.triggerColumnMinWidth = 50;
    gridStateVars.rightSideColumnMinWidth = 50;
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

        if (gridStateVars.oldAbsoluteVisibleWidthValue + delta < gridStateVars.triggerColumnMinWidth) {
            newTriggerColumnWidth = gridStateVars.triggerColumnMinWidth;
            delta = gridStateVars.newVisibleAbsoluteWidthValue - gridStateVars.oldAbsoluteVisibleWidthValue;
        }
        else {
            newTriggerColumnWidth = gridStateVars.oldAbsoluteVisibleWidthValue + delta;
        }

        if (gridStateVars.rightSideCellId !== null && gridStateVars.fitColumnsToTableWidth === 'Block') {
            newRightSideColumnWidth = gridStateVars.oldRightSideColumnWidth - delta;
            if (newRightSideColumnWidth < gridStateVars.rightSideColumnMinWidth) {
                newRightSideColumnWidth = gridStateVars.rightSideColumnMinWidth;
                newTriggerColumnWidth = gridStateVars.oldAbsoluteVisibleWidthValue + gridStateVars.oldRightSideColumnWidth - gridStateVars.rightSideColumnMinWidth;
                delta = newTriggerColumnWidth - gridStateVars.oldVisibleAbsoluteWidthValue;
            }

            gridStateVars.newRightSideColumnWidth = newRightSideColumnWidth;
            document.getElementById(gridStateVars.rightSideCellId).style.width = gridStateVars.newRightSideColumnWidth + 'px';
            var rightSideFilterCell = document.getElementById(gridStateVars.rightSideFilterCellId);
            if (rightSideFilterCell) rightSideFilterCell.style.width = gridStateVars.newRightSideColumnWidth + 'px';
            document.getElementById(gridStateVars.rightSideHiddenCellId).style.width = gridStateVars.newRightSideColumnWidth + 'px';
            var rightSideSummaryCell = document.getElementById(gridStateVars.rightSideSummaryCellId);
            if (rightSideSummaryCell) rightSideSummaryCell.style.width = gridStateVars.newRightSideColumnWidth + 'px';
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
        var triggerFilterCell = document.getElementById(gridStateVars.filterGridColumnCellId);
        if (triggerFilterCell) triggerFilterCell.style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';
        var triggerSummaryCell = document.getElementById(gridStateVars.summaryGridColumnCellId);
        if (triggerSummaryCell) triggerSummaryCell.style.width = gridStateVars.newVisibleAbsoluteWidthValue + 'px';

        var visibleHeaderEmptyColumn = document.getElementById(gridStateVars.visibleHeaderEmptyColumnId);
        var filterHeaderEmptyColumn = document.getElementById(gridStateVars.filterHeaderEmptyColumnId);

        if (gridStateVars.emptyHeaderColWidth !== 0) {
            visibleHeaderEmptyColumn.style.width = gridStateVars.emptyHeaderColWidth + 'px';
            if (filterHeaderEmptyColumn) filterHeaderEmptyColumn.style.width = gridStateVars.emptyHeaderColWidth + 'px';
        }
        else {
            visibleHeaderEmptyColumn.style.width = undefined;
            if (filterHeaderEmptyColumn) filterHeaderEmptyColumn.style.width = undefined;
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

// Custom scrollbar state per grid instance
const scrollbarInstances = new Map();

export function initCustomScrollbars(contentElement, vTrack, hTrack, vThumb, hThumb) {
    if (!contentElement) return;

    const state = {
        contentElement,
        vTrack, hTrack, vThumb, hThumb,
        isDragging: false,
        dragAxis: null,
        dragStartPos: 0,
        dragStartScroll: 0,
        hideTimer: null,
        rafId: null,
        resizeObserver: null,
        mutationObserver: null,
        lastScrollHeight: 0,
        lastScrollWidth: 0,
        disposed: false
    };

    function updateScrollbars() {
        if (state.disposed) return;
        const el = state.contentElement;

        // Vertical
        const needsVertical = el.scrollHeight > el.clientHeight;
        if (needsVertical) {
            const thumbHeight = Math.max(20, (el.clientHeight / el.scrollHeight) * el.clientHeight);
            const maxScroll = el.scrollHeight - el.clientHeight;
            const scrollRatio = maxScroll > 0 ? el.scrollTop / maxScroll : 0;
            const maxThumbTop = el.clientHeight - thumbHeight;
            vThumb.style.height = thumbHeight + 'px';
            vThumb.style.transform = 'translateY(' + (scrollRatio * maxThumbTop) + 'px)';
            vTrack.style.display = '';
        } else {
            vTrack.style.display = 'none';
        }

        // Horizontal
        const needsHorizontal = el.scrollWidth > el.clientWidth;
        if (needsHorizontal) {
            const thumbWidth = Math.max(20, (el.clientWidth / el.scrollWidth) * el.clientWidth);
            const maxScroll = el.scrollWidth - el.clientWidth;
            const scrollRatio = maxScroll > 0 ? el.scrollLeft / maxScroll : 0;
            const maxThumbLeft = el.clientWidth - thumbWidth;
            hThumb.style.width = thumbWidth + 'px';
            hThumb.style.transform = 'translateX(' + (scrollRatio * maxThumbLeft) + 'px)';
            hTrack.style.display = '';
        } else {
            hTrack.style.display = 'none';
        }
    }

    function showScrollbars() {
        vTrack.classList.add('sm-scrollbar-visible');
        hTrack.classList.add('sm-scrollbar-visible');
        clearTimeout(state.hideTimer);
        state.hideTimer = setTimeout(function () {
            if (!state.isDragging) {
                vTrack.classList.remove('sm-scrollbar-visible');
                hTrack.classList.remove('sm-scrollbar-visible');
            }
        }, 1000);
    }

    function onScroll() {
        scheduleUpdate();
        showScrollbars();
    }

    function onThumbMouseDown(axis, e) {
        e.preventDefault();
        e.stopPropagation();
        state.isDragging = true;
        state.dragAxis = axis;
        state.dragStartPos = axis === 'vertical' ? e.clientY : e.clientX;
        state.dragStartScroll = axis === 'vertical' ? contentElement.scrollTop : contentElement.scrollLeft;

        const thumb = axis === 'vertical' ? vThumb : hThumb;
        thumb.classList.add('sm-scrollbar-dragging');

        window.addEventListener('mousemove', onDragMove);
        window.addEventListener('mouseup', onDragEnd);
    }

    function onDragMove(e) {
        if (!state.isDragging) return;
        e.preventDefault();

        const el = state.contentElement;
        if (state.dragAxis === 'vertical') {
            const trackHeight = el.clientHeight;
            const thumbHeight = Math.max(20, (el.clientHeight / el.scrollHeight) * trackHeight);
            const maxThumbTop = trackHeight - thumbHeight;
            const maxScroll = el.scrollHeight - el.clientHeight;
            const delta = e.clientY - state.dragStartPos;
            const scrollDelta = maxThumbTop > 0 ? (delta / maxThumbTop) * maxScroll : 0;
            el.scrollTop = state.dragStartScroll + scrollDelta;
        } else {
            const trackWidth = el.clientWidth;
            const thumbWidth = Math.max(20, (el.clientWidth / el.scrollWidth) * trackWidth);
            const maxThumbLeft = trackWidth - thumbWidth;
            const maxScroll = el.scrollWidth - el.clientWidth;
            const delta = e.clientX - state.dragStartPos;
            const scrollDelta = maxThumbLeft > 0 ? (delta / maxThumbLeft) * maxScroll : 0;
            el.scrollLeft = state.dragStartScroll + scrollDelta;
        }
    }

    function onDragEnd() {
        state.isDragging = false;
        vThumb.classList.remove('sm-scrollbar-dragging');
        hThumb.classList.remove('sm-scrollbar-dragging');
        window.removeEventListener('mousemove', onDragMove);
        window.removeEventListener('mouseup', onDragEnd);
        showScrollbars();
    }

    function onTrackClick(axis, e) {
        if (e.target === vThumb || e.target === hThumb) return;
        e.preventDefault();

        const el = state.contentElement;
        if (axis === 'vertical') {
            const trackRect = vTrack.getBoundingClientRect();
            const clickRatio = (e.clientY - trackRect.top) / trackRect.height;
            el.scrollTop = clickRatio * el.scrollHeight - el.clientHeight / 2;
        } else {
            const trackRect = hTrack.getBoundingClientRect();
            const clickRatio = (e.clientX - trackRect.left) / trackRect.width;
            el.scrollLeft = clickRatio * el.scrollWidth - el.clientWidth / 2;
        }
    }

    // Touch support for thumb drag
    function onThumbTouchStart(axis, e) {
        e.preventDefault();
        e.stopPropagation();
        const touch = e.touches[0];
        state.isDragging = true;
        state.dragAxis = axis;
        state.dragStartPos = axis === 'vertical' ? touch.clientY : touch.clientX;
        state.dragStartScroll = axis === 'vertical' ? contentElement.scrollTop : contentElement.scrollLeft;

        const thumb = axis === 'vertical' ? vThumb : hThumb;
        thumb.classList.add('sm-scrollbar-dragging');

        window.addEventListener('touchmove', onTouchDragMove, { passive: false });
        window.addEventListener('touchend', onTouchDragEnd);
    }

    function onTouchDragMove(e) {
        if (!state.isDragging) return;
        e.preventDefault();
        const touch = e.touches[0];

        const el = state.contentElement;
        if (state.dragAxis === 'vertical') {
            const trackHeight = el.clientHeight;
            const thumbHeight = Math.max(20, (el.clientHeight / el.scrollHeight) * trackHeight);
            const maxThumbTop = trackHeight - thumbHeight;
            const maxScroll = el.scrollHeight - el.clientHeight;
            const delta = touch.clientY - state.dragStartPos;
            const scrollDelta = maxThumbTop > 0 ? (delta / maxThumbTop) * maxScroll : 0;
            el.scrollTop = state.dragStartScroll + scrollDelta;
        } else {
            const trackWidth = el.clientWidth;
            const thumbWidth = Math.max(20, (el.clientWidth / el.scrollWidth) * trackWidth);
            const maxThumbLeft = trackWidth - thumbWidth;
            const maxScroll = el.scrollWidth - el.clientWidth;
            const delta = touch.clientX - state.dragStartPos;
            const scrollDelta = maxThumbLeft > 0 ? (delta / maxThumbLeft) * maxScroll : 0;
            el.scrollLeft = state.dragStartScroll + scrollDelta;
        }
    }

    function onTouchDragEnd() {
        state.isDragging = false;
        vThumb.classList.remove('sm-scrollbar-dragging');
        hThumb.classList.remove('sm-scrollbar-dragging');
        window.removeEventListener('touchmove', onTouchDragMove);
        window.removeEventListener('touchend', onTouchDragEnd);
        showScrollbars();
    }

    // Bind events
    contentElement.addEventListener('scroll', onScroll, { passive: true });
    contentElement.addEventListener('mouseenter', showScrollbars);

    vThumb.addEventListener('mousedown', function (e) { onThumbMouseDown('vertical', e); });
    hThumb.addEventListener('mousedown', function (e) { onThumbMouseDown('horizontal', e); });

    vTrack.addEventListener('mousedown', function (e) { onTrackClick('vertical', e); });
    hTrack.addEventListener('mousedown', function (e) { onTrackClick('horizontal', e); });

    vThumb.addEventListener('touchstart', function (e) { onThumbTouchStart('vertical', e); }, { passive: false });
    hThumb.addEventListener('touchstart', function (e) { onThumbTouchStart('horizontal', e); }, { passive: false });

    // Schedule a non-duplicate update on next animation frame
    function scheduleUpdate() {
        if (state.disposed) return;
        if (!state.rafId) {
            state.rafId = requestAnimationFrame(function () {
                state.rafId = null;
                updateScrollbars();
            });
        }
    }

    // Observe container size changes (grid height/width set, window resize)
    state.resizeObserver = new ResizeObserver(function () {
        scheduleUpdate();
    });
    state.resizeObserver.observe(contentElement);
    // Also observe the scroll-container parent for grid height changes
    if (contentElement.parentElement) {
        state.resizeObserver.observe(contentElement.parentElement);
    }

    // Observe DOM mutations: row filtering, page size change, column style changes
    state.mutationObserver = new MutationObserver(function () {
        scheduleUpdate();
    });
    state.mutationObserver.observe(contentElement, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ['style', 'class']
    });

    // Initial update
    updateScrollbars();

    // Store for cleanup
    state.onScroll = onScroll;
    state.showScrollbars = showScrollbars;
    scrollbarInstances.set(contentElement, state);
}

export function disposeCustomScrollbars(contentElement) {
    if (!contentElement) return;
    const state = scrollbarInstances.get(contentElement);
    if (!state) return;

    state.disposed = true;
    clearTimeout(state.hideTimer);
    if (state.rafId) cancelAnimationFrame(state.rafId);
    if (state.resizeObserver) state.resizeObserver.disconnect();
    if (state.mutationObserver) state.mutationObserver.disconnect();
    contentElement.removeEventListener('scroll', state.onScroll);
    contentElement.removeEventListener('mouseenter', state.showScrollbars);
    scrollbarInstances.delete(contentElement);
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

let _filterMenuDismissClickHandler = null;
let _filterMenuDismissKeyHandler = null;

export function addFilterMenuDismissHandlers(element, dotNetRef) {
    removeFilterMenuDismissHandlers();

    _filterMenuDismissClickHandler = function (event) {
        if (element && !element.contains(event.target)) {
            dotNetRef.invokeMethodAsync('CloseFromJs');
        }
    };

    _filterMenuDismissKeyHandler = function (event) {
        if (event.key === 'Escape') {
            dotNetRef.invokeMethodAsync('CloseFromJs');
        }
    };

    // Defer so the click that opened the menu doesn't immediately close it
    setTimeout(function () {
        window.addEventListener('mousedown', _filterMenuDismissClickHandler);
        window.addEventListener('keydown', _filterMenuDismissKeyHandler, true);
    }, 0);
}

export function removeFilterMenuDismissHandlers() {
    if (_filterMenuDismissClickHandler) {
        window.removeEventListener('mousedown', _filterMenuDismissClickHandler);
        _filterMenuDismissClickHandler = null;
    }
    if (_filterMenuDismissKeyHandler) {
        window.removeEventListener('keydown', _filterMenuDismissKeyHandler, true);
        _filterMenuDismissKeyHandler = null;
    }
}

export function downloadFile(fileName, contentType, data) {
    const blob = new Blob([new Uint8Array(data)], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

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