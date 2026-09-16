document.addEventListener('DOMContentLoaded', () => {
    const flash = document.querySelector('.flash-message');
    if (flash) {
        setTimeout(() => {
            flash.style.transition = 'opacity .3s ease';
            flash.style.opacity = '0';
            setTimeout(() => flash.remove(), 300);
        }, 3000);
    }
});