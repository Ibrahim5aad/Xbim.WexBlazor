// Drag functionality for properties panel
const dragInstances = new Map();

export function initializeDrag(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const header = element.querySelector('.properties-header');
    if (!header) return;

    let isDragging = false;
    let currentX;
    let currentY;
    let initialX;
    let initialY;
    let xOffset = 0;
    let yOffset = 0;

    // Get initial position from CSS
    const rect = element.getBoundingClientRect();
    xOffset = rect.left;
    yOffset = rect.top;

    header.style.cursor = 'move';
    header.classList.add('draggable-header');

    function dragStart(e) {
        if (e.type === "touchstart") {
            initialX = e.touches[0].clientX - xOffset;
            initialY = e.touches[0].clientY - yOffset;
        } else {
            initialX = e.clientX - xOffset;
            initialY = e.clientY - yOffset;
        }

        if (e.target.closest('.btn-header-action, .btn-close-panel')) {
            return;
        }

        if (header.contains(e.target)) {
            isDragging = true;
            element.classList.add('dragging');
        }
    }

    function dragEnd(e) {
        initialX = currentX;
        initialY = currentY;
        isDragging = false;
        element.classList.remove('dragging');
    }

    function drag(e) {
        if (!isDragging) return;

        e.preventDefault();

        if (e.type === "touchmove") {
            currentX = e.touches[0].clientX - initialX;
            currentY = e.touches[0].clientY - initialY;
        } else {
            currentX = e.clientX - initialX;
            currentY = e.clientY - initialY;
        }

        xOffset = currentX;
        yOffset = currentY;

        setTranslate(currentX, currentY, element);
    }

    function setTranslate(xPos, yPos, el) {
        el.style.transform = `translate(${xPos}px, ${yPos}px)`;
        el.style.left = '';
        el.style.right = '';
        el.style.top = '';
        el.style.bottom = '';
        el.classList.remove('position-left', 'position-right', 'position-bottom-left', 'position-bottom-right');
        el.classList.add('position-dragged');
    }

    function cleanup() {
        header.removeEventListener('mousedown', dragStart);
        header.removeEventListener('touchstart', dragStart);
        document.removeEventListener('mousemove', drag);
        document.removeEventListener('touchmove', drag);
        document.removeEventListener('mouseup', dragEnd);
        document.removeEventListener('touchend', dragEnd);
        header.style.cursor = '';
        header.classList.remove('draggable-header');
        element.classList.remove('dragging', 'position-dragged');
        dragInstances.delete(elementId);
    }

    header.addEventListener('mousedown', dragStart);
    header.addEventListener('touchstart', dragStart);
    document.addEventListener('mousemove', drag);
    document.addEventListener('touchmove', drag);
    document.addEventListener('mouseup', dragEnd);
    document.addEventListener('touchend', dragEnd);

    // Store cleanup function
    dragInstances.set(elementId, cleanup);
}

export function disposeDrag(elementId) {
    const cleanup = dragInstances.get(elementId);
    if (cleanup) {
        cleanup();
    }
}
