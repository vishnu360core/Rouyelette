mergeInto(LibraryManager.library, {
  Copy: function(text) 
      {
         var copyText = Pointer_stringify(text);   

        // Create a new text area field
        var field = document.createElement('textarea');
        field.innerText = copyText;
        document.body.appendChild(field);

        // Select the text and copy it
        field.select();
        document.execCommand('copy');

        // Remove the field from the documentprivate static extern void copyText(string text);
        document.body.removeChild(field);
     },
CopyPasteReader: function(gObj, vName) {
                var gameObjectName = UTF8ToString(gObj);
                var voidName = UTF8ToString(vName);
                console.log(gameObjectName, "gameObjectName");
                console.log(voidName, "voidName");
                navigator.clipboard.readText().then(function(data) {
                    SendMessage(gameObjectName,"Paste", data);
                }, function() {
                    SendMessage(gameObjectName, "Paste", "no text available in clipboard");
                })
            }
     
});
