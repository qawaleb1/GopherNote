// template.js - управление выбором шаблонов

window.templateManager = {
    setupClickOutside: function(dropdownClass, buttonClass, dotnetHelper) {
        const handler = (event) => {
            const dropdown = document.querySelector(`.${dropdownClass}`);
            const button = document.querySelector(`.${buttonClass}`);

            if (dropdown && button && !dropdown.contains(event.target) && !button.contains(event.target)) {
                if (dotnetHelper) {
                    dotnetHelper.invokeMethodAsync('CloseTemplateMenu');
                }
            }
        };

        document.addEventListener('click', handler);

        // Сохраняем handler для возможности удаления
        this._currentHandler = handler;
    },

    removeClickOutside: function() {
        if (this._currentHandler) {
            document.removeEventListener('click', this._currentHandler);
            this._currentHandler = null;
        }
    }
};