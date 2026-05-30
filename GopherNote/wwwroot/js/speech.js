// JS-скрипт для использования встроенного в браузер WebSpeechAPI и передачи текста Шарпу
// Находится здесь, т. к. является статическим JS-файлом
// Здесь мы через JS-интероп можем напрямую запускать JS-код
window.speechInterop = {
    recognition: null,

    start: function (dotNetHelper, language) {
        // Поддержка разных браузеров (Chrome, Edge, Safari)
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

        if (!SpeechRecognition) {
            alert("Ваш браузер не поддерживает голосовой ввод. Используйте Chrome или Edge.");
            return false;
        }

        this.recognition = new SpeechRecognition();
        this.recognition.lang = language || 'ru-RU';
        this.recognition.continuous = true; // Слушать непрерывно
        this.recognition.interimResults = true; // Показывать промежуточные результаты

        this.recognition.onresult = function (event) {
            let finalTranscript = '';
            let interimTranscript = '';

            for (let i = event.resultIndex; i < event.results.length; ++i) {
                if (event.results[i].isFinal) {
                    finalTranscript += event.results[i][0].transcript;
                } else {
                    interimTranscript += event.results[i][0].transcript;
                }
            }

            // Отправляем текст обратно в C#
            dotNetHelper.invokeMethodAsync('OnSpeechRecognized', finalTranscript, interimTranscript);
        };

        this.recognition.onerror = function (event) {
            dotNetHelper.invokeMethodAsync('OnSpeechError', event.error);
        };

        this.recognition.onend = function () {
            dotNetHelper.invokeMethodAsync('OnSpeechEnded');
        };

        this.recognition.start();
        return true;
    },

    stop: function () {
        if (this.recognition) {
            this.recognition.stop();
        }
    }
};