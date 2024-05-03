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
 }
});
