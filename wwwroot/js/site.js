document.addEventListener('DOMContentLoaded', () => {
    document.documentElement.classList.add('js-enhanced');

    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const isDemoPage = /Demo|PlayZone|GymCrm|Liquor|Cake|ArtStudio|Snake|Maze|WP/i.test(window.location.pathname);

    const escapeHtml = (value) => value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');

    const revealAfterTypewriter = (el) => {
        const scope = el.closest('.hero-section, .page-header') || document;
        scope.querySelectorAll('[data-motion-after-typewriter]').forEach((node, index) => {
            node.classList.add('motion-after-typewriter');
            node.style.setProperty('--motion-delay', `${index * 110 + 90}ms`);
        });

        const floatTarget = document.querySelector('[data-motion-float]');
        if (floatTarget) {
            floatTarget.classList.add('motion-float-in');
        }
    };

    const revealStaggeredMotion = (scope = document) => {
        scope.querySelectorAll('[data-motion-after-typewriter]').forEach((node, index) => {
            node.classList.add('motion-after-typewriter');
            node.style.setProperty('--motion-delay', `${index * 110 + 90}ms`);
        });

        const floatTarget = scope.querySelector('[data-motion-float]');
        if (floatTarget) {
            floatTarget.classList.add('motion-float-in');
        }
    };

    const initRotatingWords = () => {
        document.querySelectorAll('[data-rotate-words]').forEach((container) => {
            const words = (container.getAttribute('data-rotate-words') || '')
                .split('|')
                .map((word) => word.trim())
                .filter(Boolean);

            if (!words.length) return;

            const headline = container.closest('.hero-headline');
            const wordSample = container.querySelector('.hero-rotate__word') || headline;
            const measure = document.createElement('span');
            measure.setAttribute('aria-hidden', 'true');
            measure.style.cssText = 'position:absolute;visibility:hidden;white-space:nowrap;pointer-events:none;';
            if (wordSample) {
                const styles = window.getComputedStyle(wordSample);
                measure.style.font = styles.font;
                measure.style.letterSpacing = styles.letterSpacing;
                measure.style.fontWeight = styles.fontWeight;
            }
            document.body.appendChild(measure);

            const setRotateWidth = () => {
                let maxWidth = 0;
                words.forEach((word) => {
                    measure.textContent = word;
                    maxWidth = Math.max(maxWidth, measure.offsetWidth);
                });
                container.style.width = `${Math.ceil(maxWidth * 1.03) + 10}px`;
            };

            setRotateWidth();

            let index = 0;
            const firstWord = container.querySelector('.hero-rotate__word');
            if (firstWord) {
                firstWord.textContent = words[0];
            }

            if (prefersReduced || words.length <= 1) return;

            window.addEventListener('resize', setRotateWidth, { passive: true });

            window.setInterval(() => {
                const current = container.querySelector('.hero-rotate__word.is-active');
                if (!current) return;

                current.classList.remove('is-active');
                current.classList.add('is-exiting');

                window.setTimeout(() => {
                    index = (index + 1) % words.length;
                    current.remove();

                    const next = document.createElement('span');
                    next.className = 'hero-rotate__word is-active';
                    next.textContent = words[index];
                    container.appendChild(next);
                }, 520);
            }, 3600);
        });
    };

    const initHomeHeroMotion = () => {
        if (!document.body.classList.contains('page-home')) return;

        window.setTimeout(() => {
            const hero = document.querySelector('.hero-section');
            if (hero) revealStaggeredMotion(hero);
        }, prefersReduced ? 0 : 480);
    };

    const initPreviewReveal = () => {
        const previews = document.querySelectorAll('.project-card__preview');
        if (!previews.length) return;

        const revealPreview = (preview) => {
            preview.classList.add('is-revealed');
        };

        previews.forEach((preview, index) => {
            preview.classList.add('preview-reveal');
            preview.style.setProperty('--preview-reveal-delay', `${Math.min(index % 8, 7) * 70}ms`);
        });

        if (prefersReduced) {
            previews.forEach(revealPreview);
            return;
        }

        const isInViewport = (el) => {
            const rect = el.getBoundingClientRect();
            return rect.top < window.innerHeight * 0.92 && rect.bottom > 0;
        };

        const previewObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting) return;
                revealPreview(entry.target);
                previewObserver.unobserve(entry.target);
            });
        }, { threshold: 0.05, rootMargin: '0px 0px 8% 0px' });

        previews.forEach((preview) => {
            if (isInViewport(preview)) {
                revealPreview(preview);
            } else {
                previewObserver.observe(preview);
            }
        });
    };

    const initTypewriters = () => {
        document.querySelectorAll('[data-typewriter]').forEach((el, index) => {
            const text = (el.getAttribute('data-typewriter-text') || el.textContent || '').trim();
            const speed = parseInt(el.getAttribute('data-typewriter-speed') || '52', 10);
            const delay = parseInt(el.getAttribute('data-typewriter-delay') || String(220 + index * 60), 10);

            if (!text) return;

            el.textContent = '';

            if (prefersReduced) {
                el.textContent = text;
                el.classList.add('typewriter-complete');
                revealAfterTypewriter(el);
                return;
            }

            el.classList.add('typewriter-host');
            el.setAttribute('aria-label', text);
            el.innerHTML = `
                <span class="typewriter-measure" aria-hidden="true">${escapeHtml(text)}</span>
                <span class="typewriter-line">
                    <span class="typewriter-output"></span><span class="typewriter-cursor" aria-hidden="true"></span>
                </span>
            `;

            const output = el.querySelector('.typewriter-output');
            const cursor = el.querySelector('.typewriter-cursor');
            let charIndex = 0;

            const typeNext = () => {
                if (charIndex < text.length) {
                    output.textContent = text.slice(0, charIndex + 1);
                    charIndex += 1;
                    const currentChar = text.charAt(charIndex - 1);
                    const pause = currentChar === '.' || currentChar === '—' ? speed * 2.8
                        : currentChar === ',' || currentChar === ' ' ? speed * 1.35
                        : speed;
                    window.setTimeout(typeNext, pause + Math.random() * 16);
                    return;
                }

                cursor.classList.add('is-done');
                el.classList.add('typewriter-complete');
                window.setTimeout(() => cursor.remove(), 1200);
                revealAfterTypewriter(el);
            };

            window.setTimeout(() => requestAnimationFrame(typeNext), delay);
        });

        document.querySelectorAll('[data-motion-before-typewriter]').forEach((el) => {
            if (prefersReduced) {
                el.classList.add('motion-before-typewriter');
                return;
            }
            window.setTimeout(() => el.classList.add('motion-before-typewriter'), 80);
        });
    };

    const initMagneticButtons = () => {
        if (prefersReduced) return;

        const magneticSelector = '.hero-btn-group .btn, .hero-section .social-link, .page-header .btn, .filter-btn, .btn-primary';
        document.querySelectorAll(magneticSelector).forEach((btn) => {
            btn.addEventListener('mousemove', (event) => {
                const rect = btn.getBoundingClientRect();
                const x = event.clientX - rect.left - rect.width / 2;
                const y = event.clientY - rect.top - rect.height / 2;
                btn.style.setProperty('--magnetic-x', `${x * 0.16}px`);
                btn.style.setProperty('--magnetic-y', `${y * 0.2}px`);
                btn.classList.add('is-magnetic');
            });

            btn.addEventListener('mouseleave', () => {
                btn.classList.remove('is-magnetic');
                btn.style.removeProperty('--magnetic-x');
                btn.style.removeProperty('--magnetic-y');
            });
        });
    };

    const initScrollTextFade = () => {
        if (prefersReduced) return;

        document.querySelectorAll('.page-header__lead:not([data-motion-after-typewriter])').forEach((lead) => {
            const header = lead.closest('.page-header');
            if (!header) return;

            window.addEventListener('scroll', () => {
                const rect = header.getBoundingClientRect();
                const progress = Math.min(1, Math.max(0, (120 - rect.top) / 180));
                lead.style.opacity = String(1 - progress * 0.35);
                lead.style.transform = `translateY(${progress * 10}px)`;
            }, { passive: true });
        });
    };

    initTypewriters();
    initRotatingWords();
    initHomeHeroMotion();
    initMagneticButtons();
    initScrollTextFade();
    initPreviewReveal();

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
    setTimeout(() => loading.classList.add('hide'), 350);

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
