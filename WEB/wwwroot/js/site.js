// SAWADE - Trạng thái giao diện dùng chung (không thay đổi dữ liệu/API).
(() => {
    'use strict';
    const initialize = () => {
        const progress = document.getElementById('sawadeNavigationProgress');
        const offline = document.getElementById('sawadeOfflineBanner');
        const startedForms = new Set();
        const pendingButtons = new Map();
        let progressTimer = null;
        const scheduleProgress = () => {
            window.clearTimeout(progressTimer);
            // Không chớp thanh tải khi chuyển trang nhanh.
            progressTimer = window.setTimeout(() => setProgress(true), 180);
        };

        const setProgress = (active) => {
            if (!progress) return;
            progress.classList.toggle('is-active', active);
        };
        const updateConnection = () => {
            // navigator.onLine chỉ báo trạng thái mạng của trình duyệt, không khẳng định API hoạt động.
            if (offline) offline.hidden = navigator.onLine;
        };
        updateConnection();
        window.addEventListener('online', updateConnection);
        window.addEventListener('offline', updateConnection);

        document.querySelectorAll('[data-sawade-auto-dismiss="true"]').forEach(alert => {
            window.setTimeout(() => {
                if (!alert.isConnected) return;
                if (window.bootstrap?.Alert) bootstrap.Alert.getOrCreateInstance(alert).close();
                else alert.remove();
            }, 9000);
        });

        document.addEventListener('submit', event => {
            const form = event.target;
            if (!(form instanceof HTMLFormElement) || !form.hasAttribute('data-sawade-busy')) return;
            if (event.defaultPrevented || !form.checkValidity()) return;
            if (startedForms.has(form)) {
                event.preventDefault();
                return;
            }
            startedForms.add(form);
            const submit = event.submitter instanceof HTMLButtonElement
                ? event.submitter : form.querySelector('button[type="submit"]');
            if (submit && !submit.disabled) {
                pendingButtons.set(submit, { html: submit.innerHTML, disabled: submit.disabled });
                submit.disabled = true;
                submit.innerHTML = '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>' +
                    (form.dataset.sawadeBusy || 'Đang xử lý...');
            }
            scheduleProgress();
        });

        // Khi bấm Back/Forward, trình duyệt có thể khôi phục form từ bộ nhớ đệm.
        window.addEventListener('pageshow', () => {
            startedForms.clear();
            pendingButtons.forEach((old, button) => {
                button.innerHTML = old.html;
                button.disabled = old.disabled;
            });
            pendingButtons.clear();
            setProgress(false);
            updateConnection();
            // Bỏ hiệu ứng rời trang khi Back/Forward được khôi phục từ bfcache.
            window.clearTimeout(progressTimer);
            progressTimer = null;
        });

        // Điều hướng native: KHÔNG preventDefault, KHÔNG đợi 170ms.
        // Chrome/Edge tự crossfade nhờ CSS View Transitions; trình duyệt khác
        // chuyển ngay, ưu tiên phản hồi nhanh thay vì cố làm một animation chậm.
        document.addEventListener('click', event => {
            const anchor = event.target.closest?.('a[href]');
            if (!anchor || event.defaultPrevented || event.button !== 0 ||
                event.ctrlKey || event.metaKey || event.altKey || event.shiftKey ||
                anchor.hasAttribute('download') || anchor.hasAttribute('data-bs-toggle') ||
                (anchor.target && anchor.target !== '_self')) return;
            let href;
            try { href = new URL(anchor.href, window.location.href); }
            catch { return; }
            if (!['http:', 'https:'].includes(href.protocol) ||
                href.origin !== window.location.origin ||
                (href.pathname === location.pathname && href.search === location.search && href.hash)) return;
            scheduleProgress();
        });
        // Cố ý không dùng reveal theo IntersectionObserver: bản trước ẩn thẻ
        // rồi chạy nhiều animation cùng lúc, dễ giật khi cuộn danh sách phòng.
    };
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initialize);
    else initialize();
})();
