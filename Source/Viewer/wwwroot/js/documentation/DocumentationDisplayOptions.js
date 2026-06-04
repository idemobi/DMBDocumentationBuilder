window.DocumentationDisplayOptions = window.DocumentationDisplayOptions || (function () {
    const storageKey = "documentation_display_options";
    const memberKinds = ["constructor", "field", "property", "method", "event", "extension-method"];
    const accessKinds = ["public", "protected", "internal", "private"];

    let state = loadState();

    function buildDefaultState() {
        return {
            kinds: Object.fromEntries(memberKinds.map(function (kind) {
                return [kind, true];
            })),
            access: Object.fromEntries(accessKinds.map(function (access) {
                return [access, true];
            }))
        };
    }

    function cloneState(value) {
        return JSON.parse(JSON.stringify(value));
    }

    function loadState() {
        const defaults = buildDefaultState();
        const raw = localStorage.getItem(storageKey);

        if (!raw) {
            return defaults;
        }

        try {
            const parsed = JSON.parse(raw);
            return {
                kinds: Object.assign({}, defaults.kinds, parsed.kinds || {}),
                access: Object.assign({}, defaults.access, parsed.access || {})
            };
        } catch {
            return defaults;
        }
    }

    function saveState() {
        localStorage.setItem(storageKey, JSON.stringify(state));
    }

    function getAccessBuckets(accessibility) {
        const value = (accessibility || "").toLowerCase();

        if (value === "protected-internal") {
            return ["protected", "internal"];
        }

        if (value === "private-protected") {
            return ["private", "protected"];
        }

        if (accessKinds.includes(value)) {
            return [value];
        }

        return ["public"];
    }

    function memberIsVisible(member) {
        const kind = member.getAttribute("data-doc-member-kind") || "";
        const access = member.getAttribute("data-doc-member-accessibility") || "";
        const kindEnabled = state.kinds[kind] !== false;
        const accessEnabled = getAccessBuckets(access).some(function (bucket) {
            return state.access[bucket] !== false;
        });

        return kindEnabled && accessEnabled;
    }

    function updateGroupVisibility() {
        document.querySelectorAll("section[data-doc-section-kind]").forEach(function (section) {
            let previousHeading = null;
            const accessHeadingSelector = "[data-doc-access-heading], h6.mt-2";
            const groupHeadingSelector = "[data-doc-member-group-heading], h5.mt-3";

            section.querySelectorAll(accessHeadingSelector + ", ul.list-group").forEach(function (element) {
                if (element.matches(accessHeadingSelector)) {
                    previousHeading = element;
                    return;
                }

                const visibleMembers = element.querySelectorAll('[data-doc-member]:not([data-doc-filter-hidden="true"])').length;
                const hidden = visibleMembers === 0;
                element.setAttribute("data-doc-group-hidden", hidden ? "true" : "false");

                if (previousHeading) {
                    previousHeading.setAttribute("data-doc-group-hidden", hidden ? "true" : "false");
                }
            });

            section.querySelectorAll(groupHeadingSelector).forEach(function (heading) {
                let current = heading.nextElementSibling;
                let hasVisibleMembers = false;

                while (current && !current.matches(groupHeadingSelector)) {
                    if (current.querySelector('[data-doc-member]:not([data-doc-filter-hidden="true"])')) {
                        hasVisibleMembers = true;
                        break;
                    }

                    current = current.nextElementSibling;
                }

                heading.setAttribute("data-doc-group-hidden", hasVisibleMembers ? "false" : "true");
            });

            const visibleSectionMembers = section.querySelectorAll('[data-doc-member]:not([data-doc-filter-hidden="true"])').length;
            section.setAttribute("data-doc-section-hidden", visibleSectionMembers === 0 ? "true" : "false");
        });
    }

    function updateControls() {
        document.querySelectorAll("[data-doc-display-toggle]").forEach(function (control) {
            const group = control.getAttribute("data-doc-display-group");
            const value = control.getAttribute("data-doc-display-toggle");

            if (group === "kind") {
                control.checked = state.kinds[value] !== false;
            }

            if (group === "access") {
                control.checked = state.access[value] !== false;
            }
        });
    }

    function updateAvailability() {
        const members = Array.from(document.querySelectorAll("[data-doc-member]"));

        document.querySelectorAll("[data-doc-display-toggle]").forEach(function (control) {
            const group = control.getAttribute("data-doc-display-group");
            const value = control.getAttribute("data-doc-display-toggle");

            if (group === "kind") {
                control.disabled = document.querySelector('[data-doc-member-kind="' + value + '"]') === null;
            } else if (group === "access") {
                control.disabled = members.every(function (member) {
                    return !getAccessBuckets(member.getAttribute("data-doc-member-accessibility")).includes(value);
                });
            }
        });
    }

    function updateCounts() {
        const allMembers = document.querySelectorAll("[data-doc-member]");
        const visibleMembers = document.querySelectorAll('[data-doc-member]:not([data-doc-filter-hidden="true"])');
        const hasMembers = allMembers.length > 0;

        document.querySelectorAll("[data-doc-display-root]").forEach(function (root) {
            root.hidden = false;
            root.setAttribute("aria-disabled", hasMembers ? "false" : "true");

            if (hasMembers) {
                root.removeAttribute("data-doc-display-disabled");
            } else {
                root.setAttribute("data-doc-display-disabled", "true");
            }
        });

        document.querySelectorAll("[data-doc-display-action]").forEach(function (action) {
            if (hasMembers) {
                action.classList.remove("disabled");
                action.removeAttribute("aria-disabled");
            } else {
                action.classList.add("disabled");
                action.setAttribute("aria-disabled", "true");
            }
        });

        document.querySelectorAll("[data-doc-display-counter]").forEach(function (counter) {
            const badge = counter.querySelector(".badge");
            const text = visibleMembers.length + "/" + allMembers.length;

            if (badge) {
                badge.textContent = text;
            } else {
                counter.textContent = text;
            }
        });
    }

    function apply(persist) {
        document.querySelectorAll("[data-doc-member]").forEach(function (member) {
            member.setAttribute("data-doc-filter-hidden", memberIsVisible(member) ? "false" : "true");
        });

        updateGroupVisibility();
        updateControls();
        updateAvailability();
        updateCounts();

        if (persist) {
            saveState();
        }
    }

    function applyPreset(name) {
        const next = cloneState(state);

        if (name === "all") {
            memberKinds.forEach(function (kind) {
                next.kinds[kind] = true;
            });
            accessKinds.forEach(function (access) {
                next.access[access] = true;
            });
        }

        if (name === "public-api") {
            memberKinds.forEach(function (kind) {
                next.kinds[kind] = true;
            });
            accessKinds.forEach(function (access) {
                next.access[access] = access === "public" || access === "protected";
            });
        }

        state = next;
        apply(true);
    }

    function bindControls() {
        document.querySelectorAll("[data-doc-display-toggle]").forEach(function (control) {
            if (control.getAttribute("data-doc-display-bound") === "true") {
                return;
            }

            control.setAttribute("data-doc-display-bound", "true");
            control.addEventListener("change", function () {
                const group = control.getAttribute("data-doc-display-group");
                const value = control.getAttribute("data-doc-display-toggle");

                if (group === "kind") {
                    state.kinds[value] = control.checked;
                }

                if (group === "access") {
                    state.access[value] = control.checked;
                }

                apply(true);
            });
        });
    }

    function setKind(kind, enabled) {
        state.kinds[kind] = enabled === true || enabled === "true";
        apply(true);
    }

    function setAccess(access, enabled) {
        state.access[access] = enabled === true || enabled === "true";
        apply(true);
    }

    function init() {
        bindControls();
        apply(false);
    }

    return {
        init: init,
        setAccess: setAccess,
        setKind: setKind,
        setPreset: applyPreset
    };
})();

document.addEventListener("DOMContentLoaded", function () {
    DocumentationDisplayOptions.init();
});
