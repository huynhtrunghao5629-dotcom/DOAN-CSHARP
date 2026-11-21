// SAWADE - Trạng thái giao diện & tương tác dùng chung (không thay đổi dữ liệu/API).
(() => {
    'use strict';

    const initialize = () => {
        const progress = document.getElementById('sawadeNavigationProgress');
        const offline = document.getElementById('sawadeOfflineBanner');
        const navbar = document.getElementById('sawadeMainNavbar');
        const backToTop = document.getElementById('sawadeBackToTop');

        const startedForms = new Set();
        const pendingButtons = new Map();
        let progressTimer = null;

        const setProgress = (active) => {
            if (!progress) return;
            progress.classList.toggle('is-active', active);
        };

        const scheduleProgress = () => {
            window.clearTimeout(progressTimer);
            // Không chớp thanh tải khi chuyển trang nhanh.
            progressTimer = window.setTimeout(() => setProgress(true), 180);
        };

        // 1. Theo dõi cuộn trang cho Navbar Overlay (chuyển kính mờ) & nút Lên đầu trang
        const handleScroll = () => {
            const y = window.scrollY || document.documentElement.scrollTop || 0;
            if (navbar) {
                navbar.classList.toggle('is-scrolled', y > 36);
            }
            if (backToTop) {
                backToTop.hidden = y < 420;
            }
        };

        handleScroll();
        window.addEventListener('scroll', handleScroll, { passive: true });

        if (backToTop) {
            backToTop.addEventListener('click', () => {
                window.scrollTo({ top: 0, behavior: 'smooth' });
            });
        }

        // 2. Cảnh báo trạng thái mạng trình duyệt
        const updateConnection = () => {
            if (offline) offline.hidden = navigator.onLine;
        };
        updateConnection();
        window.addEventListener('online', updateConnection);
        window.addEventListener('offline', updateConnection);

        // 3. Tự động đóng thông báo thành công sau 7 giây
        document.querySelectorAll('[data-sawade-auto-dismiss="true"]').forEach((alert) => {
            window.setTimeout(() => {
                if (!alert.isConnected) return;
                if (window.bootstrap?.Alert) {
                    bootstrap.Alert.getOrCreateInstance(alert).close();
                } else {
                    alert.remove();
                }
            }, 7000);
        });

        // 4. Chống bấm gửi form trùng lặp & hiển thị trạng thái đang xử lý
        document.addEventListener('submit', (event) => {
            const form = event.target;
            if (!(form instanceof HTMLFormElement) || !form.hasAttribute('data-sawade-busy')) return;
            if (event.defaultPrevented || !form.checkValidity()) return;
            if (startedForms.has(form)) {
                event.preventDefault();
                return;
            }
            startedForms.add(form);

            const submit = event.submitter instanceof HTMLButtonElement
                ? event.submitter
                : form.querySelector('button[type="submit"]');

            if (submit && !submit.disabled) {
                pendingButtons.set(submit, { html: submit.innerHTML, disabled: submit.disabled });
                submit.disabled = true;
                submit.innerHTML =
                    '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>' +
                    (form.dataset.sawadeBusy || 'Đang xử lý...');
            }
            scheduleProgress();
        });

        // 5. Khôi phục trạng thái form khi người dùng bấm Back/Forward từ bfcache
        window.addEventListener('pageshow', () => {
            startedForms.clear();
            pendingButtons.forEach((old, button) => {
                button.innerHTML = old.html;
                button.disabled = old.disabled;
            });
            pendingButtons.clear();
            setProgress(false);
            updateConnection();
            handleScroll();
            window.clearTimeout(progressTimer);
            progressTimer = null;
        });

        // 6. Hiển thị thanh tiến trình khi chuyển trang chậm (không chặn điều hướng native)
        document.addEventListener('click', (event) => {
            const anchor = event.target.closest?.('a[href]');
            if (
                !anchor || event.defaultPrevented || event.button !== 0 ||
                event.ctrlKey || event.metaKey || event.altKey || event.shiftKey ||
                anchor.hasAttribute('download') || anchor.hasAttribute('data-bs-toggle') ||
                (anchor.target && anchor.target !== '_self')
            ) return;

            let href;
            try {
                href = new URL(anchor.href, window.location.href);
            } catch {
                return;
            }

            if (
                !['http:', 'https:'].includes(href.protocol) ||
                href.origin !== window.location.origin ||
                (href.pathname === location.pathname && href.search === location.search && href.hash)
            ) return;

            scheduleProgress();
        });
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }
})();
