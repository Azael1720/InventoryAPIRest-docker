const loader = document.getElementById('page-loader');
window.addEventListener('beforeunload', () => loader?.classList.remove('d-none'));
window.addEventListener('pageshow', () => loader?.classList.add('d-none'));

document.addEventListener('submit', function (event) {
    const form = event.target;
    if (!(form instanceof HTMLFormElement) || !form.hasAttribute('data-loading')) return;

    if (event.defaultPrevented) return;

    const button = form.querySelector('button[type="submit"]');
    if (!button) return;

    const label = button.dataset.loadingText || 'Procesando...';
    setTimeout(function () {
        button.disabled = true;
        button.innerHTML =
            '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>' + label;
    }, 0);
});
