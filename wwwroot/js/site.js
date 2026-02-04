// ============================================
// LocalServicesBooking - Site JavaScript
// ============================================

// ============================================
// Theme Management
// ============================================
const ThemeManager = {
    STORAGE_KEY: 'theme',
    DARK: 'dark',
    LIGHT: 'light',

    init() {
        const savedTheme = localStorage.getItem(this.STORAGE_KEY) || this.DARK;
        this.setTheme(savedTheme);
        this.bindToggle();
    },

    setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(this.STORAGE_KEY, theme);
        this.updateToggleIcon(theme);
    },

    toggle() {
        const current = document.documentElement.getAttribute('data-theme') || this.DARK;
        const newTheme = current === this.DARK ? this.LIGHT : this.DARK;
        this.setTheme(newTheme);
    },

    updateToggleIcon(theme) {
        const toggle = document.getElementById('theme-toggle');
        if (toggle) {
            const icon = toggle.querySelector('i');
            if (icon) {
                icon.className = theme === this.DARK ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
            }
        }
    },

    bindToggle() {
        const toggle = document.getElementById('theme-toggle');
        if (toggle) {
            toggle.addEventListener('click', () => this.toggle());
        }
    }
};

// ============================================
// Navbar Scroll Effect
// ============================================
const NavbarScroll = {
    init() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;

        window.addEventListener('scroll', () => {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }
};

// ============================================
// Smooth Scroll
// ============================================
const SmoothScroll = {
    init() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (href === '#') return;

                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }
};

// ============================================
// Scroll Animations (Intersection Observer)
// ============================================
const ScrollAnimations = {
    init() {
        const animatedElements = document.querySelectorAll('[data-animate]');
        if (animatedElements.length === 0) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const animation = entry.target.dataset.animate;
                    entry.target.classList.add(`animate-${animation}`);
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });

        animatedElements.forEach(el => {
            el.style.opacity = '0';
            observer.observe(el);
        });
    }
};

// ============================================
// Counter Animation
// ============================================
const CounterAnimation = {
    init() {
        const counters = document.querySelectorAll('[data-counter]');
        if (counters.length === 0) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateCounter(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(counter => observer.observe(counter));
    },

    animateCounter(element) {
        const target = parseInt(element.dataset.counter);
        const duration = 2000;
        const step = target / (duration / 16);
        let current = 0;

        const update = () => {
            current += step;
            if (current < target) {
                element.textContent = Math.floor(current).toLocaleString();
                requestAnimationFrame(update);
            } else {
                element.textContent = target.toLocaleString();
                const suffix = element.dataset.suffix || '';
                if (suffix) element.textContent += suffix;
            }
        };

        requestAnimationFrame(update);
    }
};

// ============================================
// Form Validation Enhancement
// ============================================
const FormValidation = {
    init() {
        const forms = document.querySelectorAll('form[data-validate]');
        forms.forEach(form => {
            form.addEventListener('submit', (e) => {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                form.classList.add('was-validated');
            });

            // Real-time validation
            const inputs = form.querySelectorAll('input, textarea, select');
            inputs.forEach(input => {
                input.addEventListener('blur', () => {
                    input.classList.remove('is-valid', 'is-invalid');

                    if (input.checkValidity()) {
                        // Only mark as valid if it has a value (don't mark empty optional fields)
                        if (input.value.trim() !== '') {
                            input.classList.add('is-valid');
                        }
                    } else {
                        input.classList.add('is-invalid');
                    }
                });
            });
        });
    }
};

// ============================================
// Password Strength Indicator
// ============================================
const PasswordStrength = {
    init() {
        const passwordInputs = document.querySelectorAll('[data-password-strength]');
        passwordInputs.forEach(input => {
            const wrapper = input.closest('.password-toggle-wrapper');
            const target = wrapper || input.parentNode;

            const indicator = document.createElement('div');
            indicator.className = 'password-strength-indicator mt-2';
            indicator.innerHTML = `
                <div class="strength-bar">
                    <div class="strength-fill"></div>
                </div>
                <small class="strength-text text-secondary"></small>
            `;

            if (wrapper) {
                target.parentNode.insertBefore(indicator, target.nextSibling);
            } else {
                target.appendChild(indicator);
            }

            input.addEventListener('input', () => this.checkStrength(input, indicator));
        });
    },

    checkStrength(input, indicator) {
        const password = input.value;
        let strength = 0;
        let text = '';

        if (password.length >= 8) strength++;
        if (password.length >= 12) strength++;
        if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
        if (/\d/.test(password)) strength++;
        if (/[^a-zA-Z\d]/.test(password)) strength++;

        const fill = indicator.querySelector('.strength-fill');
        const textEl = indicator.querySelector('.strength-text');

        const colors = ['#ef4444', '#f97316', '#eab308', '#22c55e', '#10b981'];
        const texts = ['Very Weak', 'Weak', 'Fair', 'Strong', 'Very Strong'];

        if (password.length === 0) {
            fill.style.width = '0%';
            textEl.textContent = '';
        } else {
            fill.style.width = `${(strength / 5) * 100}%`;
            fill.style.background = colors[strength - 1] || colors[0];
            textEl.textContent = texts[strength - 1] || texts[0];
        }
    }
};

// ============================================
// Toast Notifications
// ============================================
const Toast = {
    container: null,

    init() {
        this.container = document.createElement('div');
        this.container.className = 'toast-container';
        document.body.appendChild(this.container);
    },

    show(message, type = 'info', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;

        const icons = {
            success: 'bi-check-circle-fill',
            error: 'bi-x-circle-fill',
            warning: 'bi-exclamation-triangle-fill',
            info: 'bi-info-circle-fill'
        };

        toast.innerHTML = `
            <i class="bi ${icons[type] || icons.info}"></i>
            <span>${message}</span>
            <button class="toast-close btn btn-link p-0 ms-auto">
                <i class="bi bi-x"></i>
            </button>
        `;

        this.container.appendChild(toast);

        toast.querySelector('.toast-close').addEventListener('click', () => {
            this.dismiss(toast);
        });

        setTimeout(() => this.dismiss(toast), duration);
    },

    dismiss(toast) {
        toast.style.animation = 'slideOutRight 0.3s ease forwards';
        setTimeout(() => toast.remove(), 300);
    }
};

// ============================================
// Loading State Management
// ============================================
const LoadingState = {
    show(button) {
        if (!button) return;
        button.disabled = true;
        button.dataset.originalText = button.innerHTML;
        button.innerHTML = `<span class="spinner me-2"></span> Loading...`;
    },

    hide(button) {
        if (!button || !button.dataset.originalText) return;
        button.disabled = false;
        button.innerHTML = button.dataset.originalText;
    }
};

// ============================================
// Mobile Menu Toggle
// ============================================
const MobileMenu = {
    init() {
        const toggler = document.querySelector('.navbar-toggler');
        const collapse = document.querySelector('.navbar-collapse');

        if (!toggler || !collapse) return;

        // Close menu when clicking outside
        document.addEventListener('click', (e) => {
            if (!collapse.contains(e.target) && !toggler.contains(e.target)) {
                const bsCollapse = bootstrap.Collapse.getInstance(collapse);
                if (bsCollapse) bsCollapse.hide();
            }
        });

        // Close menu when clicking a nav link
        collapse.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', () => {
                const bsCollapse = bootstrap.Collapse.getInstance(collapse);
                if (bsCollapse) bsCollapse.hide();
            });
        });
    }
};

// ============================================
// View Toggle (Grid/List)
// ============================================
const ViewToggle = {
    init() {
        const toggles = document.querySelectorAll('[data-view-toggle]');
        toggles.forEach(toggle => {
            toggle.addEventListener('click', () => {
                const targetId = toggle.dataset.viewToggle;
                const target = document.getElementById(targetId);
                const viewType = toggle.dataset.view;

                if (target) {
                    target.dataset.view = viewType;

                    // Update active state
                    toggles.forEach(t => {
                        if (t.dataset.viewToggle === targetId) {
                            t.classList.toggle('active', t === toggle);
                        }
                    });
                }
            });
        });
    }
};

// ============================================
// OTP Input Handler
// ============================================
const OTPInput = {
    init() {
        const otpContainers = document.querySelectorAll('.otp-input-container');
        otpContainers.forEach(container => {
            const inputs = container.querySelectorAll('input');

            inputs.forEach((input, index) => {
                input.addEventListener('input', (e) => {
                    const value = e.target.value;
                    if (value.length === 1 && index < inputs.length - 1) {
                        inputs[index + 1].focus();
                    }
                });

                input.addEventListener('keydown', (e) => {
                    if (e.key === 'Backspace' && !e.target.value && index > 0) {
                        inputs[index - 1].focus();
                    }
                });

                input.addEventListener('paste', (e) => {
                    e.preventDefault();
                    const paste = (e.clipboardData || window.clipboardData).getData('text');
                    const chars = paste.split('').slice(0, inputs.length);
                    chars.forEach((char, i) => {
                        if (inputs[i]) inputs[i].value = char;
                    });
                    if (chars.length > 0) {
                        inputs[Math.min(chars.length, inputs.length) - 1].focus();
                    }
                });
            });
        });
    }
};

// ============================================
// Initialize Everything
// ============================================
document.addEventListener('DOMContentLoaded', () => {
    ThemeManager.init();
    NavbarScroll.init();
    SmoothScroll.init();
    ScrollAnimations.init();
    CounterAnimation.init();
    FormValidation.init();
    PasswordStrength.init();
    Toast.init();
    MobileMenu.init();
    ViewToggle.init();
    OTPInput.init();
});

// Expose Toast globally for use in other scripts
window.Toast = Toast;
window.LoadingState = LoadingState;
