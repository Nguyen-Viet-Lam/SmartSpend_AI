(() => {
    if (window.smartSpendTheme?.init) {
        window.smartSpendTheme.init();
    }

    const path = window.location.pathname.toLowerCase();
    const links = document.querySelectorAll(".nav-link[data-nav-match]");
    links.forEach((link) => {
        const match = (link.getAttribute("data-nav-match") || "").toLowerCase();
        const isHome = match === "/";
        const isActive = isHome
            ? path === "/" || path === "/home" || path === "/home/index"
            : path.startsWith(match);

        link.classList.toggle("is-active", isActive);
    });
})();
