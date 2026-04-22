document.addEventListener('DOMContentLoaded', () => {
    document.documentElement.classList.add('js-enhanced');

    // Scroll progress bar
    const scrollProgress = document.createElement('div');
    scrollProgress.className = 'scroll-progress';
    document.body.appendChild(scrollProgress);

    const updateScrollProgress = () => {
        const windowHeight = document.documentElement.scrollHeight - window.innerHeight;
        const scrolled = windowHeight > 0 ? (window.scrollY / windowHeight) : 0;
        scrollProgress.style.transform = `scaleX(${Math.max(0, Math.min(1, scrolled))})`;
    };
    updateScrollProgress();
    window.addEventListener('scroll', updateScrollProgress, { passive: true });

    // Loading pulse
    const loading = document.createElement('div');
    loading.className = 'loading';
    loading.innerHTML = '<div class="loading-spinner"></div>';
    document.body.appendChild(loading);
    setTimeout(() => loading.classList.add('hide'), 700);

    // Smooth hash navigation
    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (!target) return;
            e.preventDefault();
            target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    });

    // Navbar scroll direction / polish
    const navbar = document.querySelector('.navbar');
    let lastScroll = 0;
    if (navbar) {
        window.addEventListener('scroll', () => {
            const currentScroll = window.pageYOffset;
            if (currentScroll <= 0) {
                navbar.classList.remove('scroll-up');
                return;
            }
            if (currentScroll > lastScroll && !navbar.classList.contains('scroll-down')) {
                navbar.classList.remove('scroll-up');
                navbar.classList.add('scroll-down');
            } else if (currentScroll < lastScroll && navbar.classList.contains('scroll-down')) {
                navbar.classList.remove('scroll-down');
                navbar.classList.add('scroll-up');
            }
            lastScroll = currentScroll;
        }, { passive: true });
    }

    // Section reveal animations (excluding PlayZone game pages)
    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (!prefersReduced) {
        const revealTargets = document.querySelectorAll(
            '.project-card-modern, .skill-section, .tool-card, .soft-skill, .stat-card, .contact-info-card, .contact-form-card, .timeline-item, .card'
        );
        revealTargets.forEach((el, i) => {
            el.classList.add('reveal-ready');
            // Keep stagger subtle so large skills grids appear quickly.
            el.style.setProperty('--reveal-delay', `${Math.min((i % 10) * 12, 108)}ms`);
        });

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.05, rootMargin: '0px 0px -10% 0px' });

        revealTargets.forEach((el) => observer.observe(el));
    }
});
