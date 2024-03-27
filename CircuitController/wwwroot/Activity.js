window.userActivity = {
    setupActivityTracker: function (dotNetReference, timeout) {
        let isActive = true; // Track whether the user is currently active
        let lastCallback = Date.now();
        console.log(lastCallback);

        function resetTimer() {
            // Ensure that the user was inactive before, and the user has been active in the last 100ms.
            if (!isActive && Date.now() > lastCallback + 100) {
                // If the user was inactive, notify Blazor that they are now active.
                dotNetReference.invokeMethodAsync('NotifyUserActive', true);
                isActive = true; // Update the state to active
                lastCallback = Date.now();
            }
            startTimer(); // Reset the inactivity timer
        }

        // Events that should trigger the user activity
        const events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'];

        let timer;
        function startTimer() {
            clearTimeout(timer);
            timer = setTimeout(() => {
                if (isActive) {
                    console.log("Inactive");
                    // If the user was active, notify Blazor that they are now inactive.
                    dotNetReference.invokeMethodAsync('NotifyUserInactive', false);
                    isActive = false; // Update the state to inactive
                }
            }, timeout);
        }

        // Listen for any of the events in the `events` array, reset the timer on any of these events
        events.forEach(function (name) {
            document.addEventListener(name, resetTimer, true);
        });

        startTimer(); // Start the inactivity timer initially
        console.log("Setup tracker done");
    },

    removeActivityTracker: function () {
        // Implement logic to remove listeners if necessary
        // This might include removing the event listeners added in setupActivityTracker and clearing the timeout
    }
};