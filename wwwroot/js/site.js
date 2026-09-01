document.addEventListener('DOMContentLoaded', () => {
    document.documentElement.classList.add('js-enhanced');

    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const isDemoPage = /Demo|PlayZone|GymCrm|Liquor|Cake|ArtStudio|Snake|Maze|WP/i.test(window.location.pathname);

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

    // Floating background orbs (Shopify-style ambient motion)
    if (!prefersReduced && !isDemoPage) {
        document.body.classList.add('motion-enabled');
        const motionBg = document.createElement('div');
        motionBg.className = 'motion-bg';
        motionBg.setAttribute('aria-hidden', 'true');
        motionBg.innerHTML = `
            <span class="motion-orb motion-orb--amber"></span>
            <span class="motion-orb motion-orb--violet"></span>
            <span class="motion-orb motion-orb--mint"></span>
        `;
        document.body.prepend(motionBg);
    }

    // Smooth hash navigation with navbar offset
    const navbarHeight = () => {
        const navbar = document.querySelector('.navbar');
        return navbar ? navbar.offsetHeight : 78;
    };

    document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (!target) return;
            e.preventDefault();
            const top = target.getBoundingClientRect().top + window.scrollY - navbarHeight() - 12;
            window.scrollTo({ top, behavior: prefersReduced ? 'auto' : 'smooth' });
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

    // Subtle parallax on inner page headers
    const parallaxHeader = document.querySelector('.page-header .container');
    if (!prefersReduced && parallaxHeader) {
        window.addEventListener('scroll', () => {
            const offset = Math.min(window.scrollY * 0.05, 36);
            parallaxHeader.style.transform = `translate3d(0, ${offset}px, 0)`;
        }, { passive: true });
    }

    if (!prefersReduced) {
        const revealSelector = [
            '.project-card-modern',
            '.skill-section',
            '.skill-category',
            '.skill-section__header',
            '.tool-card',
            '.soft-skill',
            '.stat-card',
            '.contact-info-card',
            '.contact-form-card',
            '.timeline-item',
            '.card',
            '.experience-stat',
            '.experience-stats',
            '.skills-stats',
            '.page-header .container > *',
            '.footer-brand',
            '.footer-links',
            '.footer-socials'
        ].join(', ');

        const revealTargets = document.querySelectorAll(revealSelector);
        revealTargets.forEach((el, i) => {
            el.classList.add('reveal-ready');
            el.style.setProperty('--reveal-delay', `${Math.min((i % 12) * 45, 360)}ms`);
        });

        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.08, rootMargin: '0px 0px -8% 0px' });

        revealTargets.forEach((el) => observer.observe(el));

        // Count-up stats (Skills / Experience)
        const countObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting) return;
                const el = entry.target;
                const text = el.textContent.trim();
                const match = text.match(/^(\d+)(\+?)$/);
                if (!match) {
                    countObserver.unobserve(el);
                    return;
                }

                const target = parseInt(match[1], 10);
                const suffix = match[2] || '';
                const duration = 1400;
                const start = performance.now();
                el.classList.add('is-counting');

                const step = (now) => {
                    const progress = Math.min(1, (now - start) / duration);
                    const eased = 1 - Math.pow(1 - progress, 4);
                    el.textContent = `${Math.round(target * eased)}${suffix}`;
                    if (progress < 1) {
                        requestAnimationFrame(step);
                    }
                };

                requestAnimationFrame(step);
                countObserver.unobserve(el);
            });
        }, { threshold: 0.5 });

        document.querySelectorAll('.stat-number').forEach((el) => countObserver.observe(el));

        // 3D tilt on project cards (Shopify-style hover depth)
        document.querySelectorAll('.project-card-modern').forEach((card) => {
            card.addEventListener('mousemove', (event) => {
                const rect = card.getBoundingClientRect();
                const x = (event.clientX - rect.left) / rect.width - 0.5;
                const y = (event.clientY - rect.top) / rect.height - 0.5;
                card.classList.add('is-tilting');
                card.style.transform = `perspective(900px) rotateX(${y * -5}deg) rotateY(${x * 6}deg) translateY(-8px) scale(1.02)`;
            });

            card.addEventListener('mouseleave', () => {
                card.classList.remove('is-tilting');
                card.style.transform = '';
            });
        });
    }
});
