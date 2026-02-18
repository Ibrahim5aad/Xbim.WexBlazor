// Resize functionality for sidebar panels when docked
const resizeInstances = new Map();

export function startResize(panelElement, resizeHandle, isLeftSide, startX, startWidth, minWidth, maxWidth, dotNetRef) {
    if (!panelElement || !resizeHandle) return;

    let isResizing = true;
    let currentWidth = startWidth;

    function handleMouseMove(e) {
        if (!isResizing) return;

        e.preventDefault();
        e.stopPropagation();

        const deltaX = isLeftSide 
            ? e.clientX - startX  // For left side, dragging right increases width
            : startX - e.clientX; // For right side, dragging left increases width

        const newWidth = Math.max(minWidth, Math.min(maxWidth, startWidth + deltaX));

        if (newWidth !== currentWidth) {
            currentWidth = newWidth;
            panelElement.style.width = `${newWidth}px`;
            
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnResize', newWidth);
            }
        }
    }

    function handleMouseUp(e) {
        if (isResizing) {
            isResizing = false;
            
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('OnResizeEnd');
            }
        }

        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    }

    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
    document.body.style.cursor = isLeftSide ? 'ew-resize' : 'ew-resize';
    document.body.style.userSelect = 'none';
}

export function disposeResize(instanceId) {
    const cleanup = resizeInstances.get(instanceId);
    if (cleanup) {
        cleanup();
        resizeInstances.delete(instanceId);
    }
}
