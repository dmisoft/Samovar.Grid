// customScrollbar.js

window.customScrollbar = {
    handleMouseDown: function (dotNetObjectRef) {
        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);

        function handleMouseMove(e) {
            dotNetObjectRef.invokeMethodAsync('HandleMouseMove', e.clientY);
            console.log(e.clientY);
        }

        function handleMouseUp() {
            dotNetObjectRef.invokeMethodAsync('HandleMouseUp');
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        }
    },

    handleMouseMove: function (deltaY) {
        var thumb = document.getElementById('customScrollbarThumb');
        if (thumb && thumb.style) {
            thumb.style.top = Math.max(0, thumb.offsetTop + deltaY) + 'px';
        }
    },

    handleMouseUp: function () {
        // Clean up any resources if needed
    }
};
