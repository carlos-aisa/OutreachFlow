(function () {
    const STORAGE_KEY = "outreachflow_theme";
    const THEMES = ["system", "light", "dark"];
    const mediaQuery = window.matchMedia ? window.matchMedia("(prefers-color-scheme: dark)") : null;

    function normalize(theme) {
        if (!theme || typeof theme !== "string") {
            return "system";
        }

        const normalized = theme.toLowerCase();
        return THEMES.includes(normalized) ? normalized : "system";
    }

    function resolveAppliedTheme(theme) {
        const normalized = normalize(theme);
        if (normalized === "system") {
            return mediaQuery && mediaQuery.matches ? "dark" : "light";
        }

        return normalized;
    }

    function applyTheme(theme) {
        const normalized = normalize(theme);
        const applied = resolveAppliedTheme(normalized);
        document.documentElement.dataset.themePreference = normalized;
        document.documentElement.dataset.theme = applied;
        document.documentElement.style.colorScheme = applied;
        return normalized;
    }

    function getTheme() {
        const storedTheme = normalize(localStorage.getItem(STORAGE_KEY));
        applyTheme(storedTheme);
        return storedTheme;
    }

    function setTheme(theme) {
        const normalized = normalize(theme);
        localStorage.setItem(STORAGE_KEY, normalized);
        applyTheme(normalized);
        return normalized;
    }

    if (mediaQuery) {
        const refreshSystemTheme = function () {
            const currentTheme = normalize(localStorage.getItem(STORAGE_KEY));
            if (currentTheme === "system") {
                applyTheme(currentTheme);
            }
        };

        if (typeof mediaQuery.addEventListener === "function") {
            mediaQuery.addEventListener("change", refreshSystemTheme);
        } else if (typeof mediaQuery.addListener === "function") {
            mediaQuery.addListener(refreshSystemTheme);
        }
    }

    applyTheme(localStorage.getItem(STORAGE_KEY));

    window.themeHelper = {
        getTheme,
        setTheme
    };
})();
