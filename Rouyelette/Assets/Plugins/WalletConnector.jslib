mergeInto(LibraryManager.library, {
 ConnectWalletAndRetrieveAddress: function () {
   
    if (typeof window.ethereum !== 'undefined') {
      window.ethereum.request({ method: 'eth_requestAccounts' })
        .then(accounts => {
          if (accounts.length === 0) {
            console.log('Please connect to your wallet.');
            return;
          }
          const account = accounts[0];
          // Request the balance of the wallet
          window.ethereum.request({
            method: 'eth_getBalance',
            params: [account, 'latest'],
          }).then(balance => {
            // Convert the balance from wei to ether using ethers.js
            //const balanceInEther = ethers.utils.formatEther(balance);
            // Send the wallet address and balance back to Unity
            if (typeof nethereumUnityInstance !== 'undefined') {
              nethereumUnityInstance.SendMessage('WalletConnector', 'ReceiveWalletAddressAndBalance', account + ',' + balance);
            } else {
              
              console.error('nethereumUnityInstance is not defined');
            }
          }).catch(err => {
            console.error(err);
            // Handle errors by sending an error message to Unity
            if (typeof nethereumUnityInstance !== 'undefined') {
             // nethereumUnityInstance.SendMessage('WalletConnector', 'OnConnectError', err.message);
            } else {
              console.error('nethereumUnityInstance is not defined');
            }
          });
        })
        .catch(err => {
          console.error(err);
          // Handle errors by sending an error message to Unity
          if (typeof nethereumUnityInstance !== 'undefined') {
           // nethereumUnityInstance.SendMessage('WalletConnector', 'OnConnectError', err.message);
          } else {
            console.error('nethereumUnityInstance is not defined');
          }
        });
    } else {
      console.log('Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
      // Handle the case where the Ethereum object is not found
      if (typeof nethereumUnityInstance !== 'undefined') {
        nethereumUnityInstance.SendMessage('WalletConnector', 'OnConnectError', 'Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
      } else {
        console.error('nethereumUnityInstance is not defined');
      }
    }
 },
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
