// ظهور العناصر (.reveal) تدريجياً لما تدخل الشاشة
(function () {
    var items = document.querySelectorAll('.reveal');
    if (!items.length) return;

    if (!('IntersectionObserver' in window)) {
        items.forEach(function (el) { el.classList.add('is-visible'); });
        return;
    }

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('is-visible');
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.12 });

    items.forEach(function (el) { observer.observe(el); });
})();

// الـ Navbar بيصغر وظله بيزيد لما الصفحة تتحرك
(function () {
    var nav = document.querySelector('.evo-nav');
    if (!nav) return;

    function onScroll() {
        nav.classList.toggle('is-scrolled', window.scrollY > 12);
    }

    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
})();
// "إظهار كلمة المرور": أي checkbox عليه data-toggle-password بيتحكم في الحقول اللي في الـ selector
document.querySelectorAll('[data-toggle-password]').forEach(function (box) {
    box.addEventListener('change', function () {
        document.querySelectorAll(box.dataset.togglePassword).forEach(function (input) {
            input.type = box.checked ? 'text' : 'password';
        });
    });
});