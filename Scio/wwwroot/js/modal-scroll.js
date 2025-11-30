// Modal scroll management
export function preventScroll() {
    document.body.style.overflow = 'hidden';
}

export function allowScroll() {
    document.body.style.overflow = '';
}
