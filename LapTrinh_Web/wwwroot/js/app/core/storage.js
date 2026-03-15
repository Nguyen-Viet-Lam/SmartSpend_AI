window.smartSpendStorage = (() => {
    const read = (key, fallback) => {
        try {
            const raw = localStorage.getItem(key);
            return raw ? JSON.parse(raw) : fallback;
        } catch {
            return fallback;
        }
    };

    const write = (key, value) => {
        localStorage.setItem(key, JSON.stringify(value));
    };

    return { read, write };
})();
