mergeInto(LibraryManager.library, {
 ConnectWalletAndRetrieveDetails: function () {
    if (typeof window.ethereum !== 'undefined') {
      window.ethereum.request({ method: 'eth_requestAccounts' })
        .then(accounts => {
          if (accounts.length === 0) {
            console.log('Please connect to your wallet.');
            return;
          }
          const account = accounts[0];
          // Assuming you want to retrieve the account address
          unityInstance.SendMessage('WalletConnector', 'ReceiveWalletDetails', account);
        })
        .catch(err => {
          console.error(err);
          // Handle errors by sending an error message to Unity
          unityInstance.SendMessage('WalletConnector', 'OnConnectError', err.message);
        });
    } else {
      console.log('Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
      // Handle the case where the Ethereum object is not found
      unityInstance.SendMessage('WalletConnector', 'OnConnectError', 'Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
    }
 },
 });

 mergeInto(LibraryManager.library, {
 GetWalletBalance: function (address, rpcUrl) {
    if (typeof window.ethereum !== 'undefined') {
      // Use the provided RPC URL to create a new Web3 instance
      const web3 = new Web3(new Web3.providers.HttpProvider(rpcUrl));

      web3.eth.getBalance(address, 'latest')
        .then(balance => {
          // Convert the balance from wei to ether
          const balanceInEther = web3.utils.fromWei(balance, 'ether');
          // Send the balance back to Unity
          unityInstance.SendMessage('WalletConnector', 'ReceiveWalletBalance', balanceInEther);
        })
        .catch(err => {
          console.error(err);
          // Handle errors by sending an error message to Unity
          unityInstance.SendMessage('WalletConnector', 'OnConnectError', err.message);
        });
    } else {
      console.log('Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
      // Handle the case where the Ethereum object is not found
      unityInstance.SendMessage('WalletConnector', 'OnConnectError', 'Ethereum object not found. Please install MetaMask or another Ethereum wallet.');
    }
 },
});

mergeInto(LibraryManager.library, {
 LoadWeb3: function () {
    var script = document.createElement('script');
    script.src = 'Plugins/web3.min.js'; // Adjust the path to where you placed the Web3 library
    script.onload = function() {
        // Web3 is now loaded and can be used
        // You can call other functions that depend on Web3 here
    };
    document.head.appendChild(script);
 },
});
