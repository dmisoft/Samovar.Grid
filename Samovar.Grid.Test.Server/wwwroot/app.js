var dimi = {
    var1: 1,
    var2: 'var2'
}


var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition
var SpeechGrammarList = SpeechGrammarList || webkitSpeechGrammarList
//var speech = SpeechGrammarList
var grammar = "#JSGF V1.0;"

var recognition = new SpeechRecognition();
var speechRecognitionList = new SpeechGrammarList();
speechRecognitionList.addFromString(grammar, 1);
recognition.grammars = speechRecognitionList;
recognition.lang = "de-DE";
recognition.interimResults = false;
recognition.continuous = true;

window.AppFunctions = {
    add_SpeechRecognition_EventListener: function (dotNetRef) {
        recognition.onresult = function (event) {
            let command = event.results[0][0].transcript;
            let isFinal = event.results[0].isFinal;
            //if (isFinal)
                dotNetRef.invokeMethodAsync("JS_SpeechRecognitionHandler", isFinal, command);
        }
        recognition.start();
    }
}