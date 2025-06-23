class CustomAutocomplete {
    constructor() {
        this.timer = null;
        this.observers = {};
        this.autocompleteId = null;
    }

    /**
     * Initializes the custom autocomplete component.
     * @param {string} autocompleteId - The ID of the autocomplete element.
     * @returns {Promise}
     */
    initialize(autocompleteId) {

        this.autocompleteId = autocompleteId;

        return new Promise((resolve, reject) => {
            var autocomplete = document.getElementById(this.autocompleteId);
            var chips = document.getElementById(`${this.autocompleteId}-chips`);

            if (!autocomplete) {
                console.error("Autocomplete element not found:", this.autocompleteId);
                reject("Autocomplete element not found");
                return;
            }

            if (!chips) {
                console.error("Chips container not found:", `${this.autocompleteId}-chips`);
                reject("Chips container not found");
                return;
            }

            // Create a wrapping container
            var wrappingContainer = document.createElement('div');

            // Apply necessary MudBlazor classes to ensure inner MudAutocomplete input element retains its styling
            wrappingContainer.className = "mud-input w-100 mud-input-text mud-input-text-with-label mud-input-adorned-end mud-shrink mud-typography-input mud-select-input";
            wrappingContainer.style.display = 'flex';
            wrappingContainer.style.flexWrap = 'wrap';
            wrappingContainer.style.alignItems = 'center';

            // Insert the wrapping container before the autocomplete
            var parentContainer = autocomplete.parentElement;
            parentContainer.insertBefore(wrappingContainer, autocomplete);

            // Move chips container and autocomplete into the wrappingContainer
            wrappingContainer.appendChild(chips);
            wrappingContainer.appendChild(autocomplete);

            // Make autocomplete grow to fill the remaining space
            autocomplete.style.flexGrow = '1';

            // Hide the autocomplete if there are chips present on the initial load
            if (chips.children.length != 0) {
                autocomplete.style.display = 'none'; // Set autocomplete to display none initially
            }

            // Set up MutationObserver to monitor changes in the chips div
            var observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    if (mutation.type === 'childList') {
                        if (chips.children.length === 0) {
                            autocomplete.style.display = 'block';
                            this.clearTimer();
                        } else {
                            // If no timer is active and chips have changed, they were likely changed externally,
                            // therefore, hide the input
                            if (!this.timer || this.timer <= 0) {
                                autocomplete.style.display = 'none';
                            }
                        }
                    }
                });
            });

            observer.observe(chips, { childList: true });
            this.observers[this.autocompleteId] = observer;

            // Add event listeners to reset the timer on interaction
            autocomplete.parentElement.parentElement.addEventListener('click', (event) => this.handleComponentAreaClicked(event));
            autocomplete.addEventListener('blur', () => this.startAutoHideTimer());

            resolve(true);
        });
    }

    /**
     * Handles the click event on the component area.
     * @param {Event} event - The event object.
     */
    handleComponentAreaClicked(event) {
        // If a mud-chip wasn't clicked
        if (!event.target.closest('.mud-chip')) {
            this.showInputWithAutoHide();
        }
    }

    /**
     * Shows the input element.
     */
    showInput() {
        var inputElement = document.getElementById(this.autocompleteId);
        if (inputElement) {
            inputElement.style.display = 'block';
            inputElement.focus();
        }
    }

    /**
     * Shows the input element with auto-hide functionality.
     */
    showInputWithAutoHide() {
        this.showInput();
        this.startAutoHideTimer();
    }

    /**
     * Starts the auto-hide timer.
     */
    startAutoHideTimer() {
        var inputElement = document.getElementById(this.autocompleteId);
        var chips = document.getElementById(`${this.autocompleteId}-chips`);

        // Clear any existing timer
        this.clearTimer();

        if (inputElement && chips) {
            // Set a timeout to hide the input if no longer focused
            this.timer = setTimeout(() => {
                if (document.activeElement !== inputElement && chips.hasChildNodes()) {
                    inputElement.style.display = 'none';
                } else {
                    // Restart the timer if the input is still focused
                    this.startAutoHideTimer();
                }
            }, 200); // Hide after
        }
    }

    /**
     * Clears the auto-hide timer.
     */
    clearTimer() {
        if (this.timer) {
            clearTimeout(this.timer);
            this.timer = null;
        }
    }

    /**
     * Disposes of the MutationObserver.
     */
    dispose() {
        var observer = this.observers[this.autocompleteId];
        if (observer) {
            observer.disconnect();
            delete this.observers[this.autocompleteId];
        }
        this.clearTimer();
    }
}

export function getCustomAutocompleteInstance() {
    return new CustomAutocomplete();
}