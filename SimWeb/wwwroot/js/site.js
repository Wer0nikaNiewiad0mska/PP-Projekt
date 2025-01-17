// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('keydown', (event) => {
    const key = event.key.toLowerCase(); // Pobierz klawisz w formie małych liter
    let buttonId;
    switch (key) {
        case 'w':
            buttonId = 'move-up';
            break;
        case 'a':
            buttonId = 'move-left';
            break;
        case 's':
            buttonId = 'move-down';
            break;
        case 'd':
            buttonId = 'move-right';
            break;
        case 'e':
            buttonId = 'collect-key';
            break;
        case 'q':
            buttonId = 'open-lock';
            break;
        case 'i':
            buttonId = 'open-inventory';
            break;
        case 'u':
            buttonId = 'collect-potion';
            break;
        case 'r':
            buttonId = 'activate-follower';
            break;
        default:
            return; // Ignoruj inne klawisze
    }
    // Znajdź odpowiedni przycisk i wyślij zdarzenie kliknięcia
    const button = document.getElementById(buttonId);
    if (button) {
        button.click();
    }
});