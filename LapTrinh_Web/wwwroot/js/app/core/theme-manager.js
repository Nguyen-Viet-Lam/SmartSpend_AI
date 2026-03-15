window.smartSpendTheme = (() => {
    const storageKey = "smartspend-theme";

    const apply = (theme) => {
        const root = document.documentElement;
        root.setAttribute("data-theme", theme);

        const toggle = document.getElementById("themeToggle");
        if (toggle) {
            toggle.textContent = theme === "dark" ? "Light" : "Dark";
        }
    };

    const init = () => {
        const savedTheme = localStorage.getItem(storageKey);
        if (savedTheme === "dark" || savedTheme === "light") {
            apply(savedTheme);
        } else {
            apply("light");
        }

        const toggle = document.getElementById("themeToggle");
        if (!toggle) {
            return;
        }

        toggle.addEventListener("click", () => {
            const current = document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "light";
            const next = current === "dark" ? "light" : "dark";
            localStorage.setItem(storageKey, next);
            apply(next);
        });
    };

    return { init };
})();